using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace lib61850net
{
    internal class TcpRw
    {
        internal static void StartClient(TcpState tcps)
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo;
                IPAddress ipAddress = null;

                if (!IPAddress.TryParse(tcps.hostname, out ipAddress))
                {
                    ipHostInfo = Dns.GetHostEntry(tcps.hostname);
                    for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                    {
                        if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = ipHostInfo.AddressList[i];
                            break;
                        }
                    }
                }
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, tcps.port);

                // Create a TCP/IP socket.
                tcps.workSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Events reset
                tcps.connectDone.Reset();
                tcps.receiveDone.Reset();
                tcps.sendDone.Reset();

                // Connect to the remote endpoint.
                tcps.workSocket.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), tcps);
                tcps.tstate = TcpProtocolState.TCP_CONNECT_WAIT;
            }
            catch (Exception e)
            {
                StopClient(tcps);
                tcps.sourceLogger?.SendError("lib61850net: ошибка при запуске клиента: " + e.ToString());
                tcps.logger.LogError(e.ToString());
            }
        }

        internal static void StopClient(TcpState tcps)
        {
            // Connect to a remote device.
            tcps?.logger.LogInfo("StopClient: Socket shutdowned.");
            try
            {
                if (tcps.tstate != TcpProtocolState.TCP_STATE_CLOSING)
                {
                    if (tcps != null)
                    {
                        tcps.tstate = TcpProtocolState.TCP_STATE_CLOSING;
                    }
                    // Release the socket.
                    if (tcps?.workSocket != null)
                    {
                        if (tcps.workSocket.Connected)
                        {
                            tcps.workSocket.Shutdown(SocketShutdown.Both);
                            tcps.receiveDone.WaitOne(1000);
                        }
                        if (tcps.workSocket != null)
                        {

                            tcps.workSocket.Close();
                            tcps.workSocket.Dispose();
                            tcps.workSocket = null;
                            tcps.logger.LogInfo("Socket closed and disposed.");
                        }
                    }
                }
            } 
            catch (Exception e)
            {
                tcps.sourceLogger?.SendError("lib61850net: ошибка при закрытии соединения: " + e.ToString());
                tcps.logger.LogError("Closing: " + e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            TcpState tcps = (TcpState)ar.AsyncState;
            try
            {
                if (tcps.workSocket == null)
                {
                    throw new Exception("Double closing TCP connection.");
                }
                // Complete the connection.
                tcps.workSocket.EndConnect(ar);

                tcps.logger.LogInfo(String.Format("ConnectCallback: Socket connected to {0}",
                    tcps.workSocket.RemoteEndPoint.ToString()));

                // Signal that the connection has been made.
                tcps.tstate = TcpProtocolState.TCP_CONNECTED;
                tcps.connectDone.Set();
            }
            catch (Exception e)
            {
                tcps.tstate = TcpProtocolState.TCP_STATE_SHUTDOWN;
                tcps.sourceLogger?.SendError("lib61850net: connectcallback error " + e.ToString());
                tcps.logger.LogError(e.ToString());
            }
        }

        internal static void Receive(TcpState tcps)
        {
            try
            {
                // Begin receiving the data from the remote device.
                //
                tcps?.workSocket?.BeginReceive(tcps.recvBuffer, 0, Iec61850State.recvBufferSize, 0,
                    new AsyncCallback(ReceiveCallback), tcps);
            }
            catch (Exception e)
            {
                StopClient(tcps);
                tcps.sourceLogger?.SendError("lib61850net: receive data error " + e.ToString());
                tcps.logger.LogError(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            TcpState tcps = (TcpState)ar.AsyncState;
            try
            {
                // Complete the connection.
                if (tcps.workSocket != null)
                {
                    if (tcps.workSocket.Poll(5000, SelectMode.SelectRead) && tcps.workSocket.Available == 0)
                    {
                        //socket not connected, close it if it's still running
                        tcps.workSocket.Close();
                        tcps.workSocket = null;
                        tcps.sourceLogger?.SendError("lib61850net: Socket disconnected (detected in ReceiveCallback)");
                        tcps.logger.LogError("Socket disconnected (detected in ReceiveCallback)");
                        tcps.tstate = TcpProtocolState.TCP_STATE_SHUTDOWN;
                    }
                    else
                    {
                        try
                        {
                            tcps.recvBytes = tcps.workSocket.EndReceive(ar);
                            //Console.WriteLine("ReceiveCallback: Data received {0}",
                            //    tcps.recvBytes.ToString());
                        } catch (Exception e) {
                            tcps.sourceLogger?.SendError("lib61850net: endReceive error " + e.Message);
                            tcps.logger.LogError(e.Message);
                        }

                        try {
                            IsoTpkt.Parse(tcps);
                        }
                        catch (Exception e)
                        {
                            tcps.sourceLogger?.SendError("lib61850net: isotpkt.parse error " + e.Message);
                            tcps.logger.LogError(e.Message);
                        }
                        // Signal that the data has been received.
                        tcps.receiveDone.Set();
                    }
                }
            }
            catch (Exception e)
            {
            //   StopClient(tcps);
            //    tcps.logger.LogInfo(e.ToString());
            }
        }

        public static void Send(TcpState tcps) //Socket client, byte[] data)
        {
            try
            {
                // Begin sending the data to the remote device.
                if (tcps.workSocket != null) tcps.workSocket.BeginSend(tcps.sendBuffer, 0, tcps.sendBytes, 0,
                    new AsyncCallback(SendCallback), tcps);
            }
            catch (Exception e)
            {
                tcps.sourceLogger?.SendError("lib61850net: send data error " + e.Message);
                tcps.logger.LogError(e.ToString());
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            TcpState tcps = (TcpState)ar.AsyncState;
            try
            {
                // Retrieve the socket from the state object.

                // Complete sending the data to the remote device.
                int bytesSent = tcps.workSocket.EndSend(ar);
                tcps.logger.LogDebug(String.Format("Sent {0} bytes to server.", bytesSent));

                // Signal that all bytes have been sent.
                tcps.sendDone.Set();
            }
            catch (Exception e)
            {
                tcps.logger.LogError(e.ToString());
                tcps.sourceLogger?.SendError("lib61850net: sendcallback error " + e.Message);
                StopClient(tcps);
            }
        }
    }
}
