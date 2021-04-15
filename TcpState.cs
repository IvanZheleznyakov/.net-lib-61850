using DLL_Log;
using System;
using System.Net.Sockets;
using System.Threading;

namespace lib61850net
{
    internal class TcpState
    {
        internal SourceMsg_t sourceLogger;

        /// <summary>
        ///  logger.
        /// </summary>
        internal Logger logger = null;
        /// <summary>
        ///  Client socket.
        /// </summary>
        internal Socket workSocket = null;
        /// <summary>
        /// Size of receive buffer.
        /// </summary>
        internal const int recvBufferSize = 65535;
        /// <summary>
        /// Bytes received to buffer.
        /// </summary>
        internal int recvBytes = 0;
        /// <summary>
        /// Receive buffer.
        /// </summary>
        internal byte[] recvBuffer = new byte[recvBufferSize];
        /// <summary>
        /// Size of send buffer.
        /// </summary>
        internal const int sendBufferSize = 65535;
        /// <summary>
        /// Bytes to be sent from send buffer.
        /// </summary>
        internal int sendBytes = 0;
        /// <summary>
        /// Send buffer.
        /// </summary>
        internal byte[] sendBuffer = new byte[recvBufferSize];
        /// <summary>
        /// Upper level protocol state
        /// </summary>
        internal TcpProtocolState tstate = TcpProtocolState.TCP_STATE_START;
        /// <summary>
        /// Hostname
        /// </summary>
        internal String hostname;
        /// <summary>
        /// tcp Port
        /// </summary>
        internal int port = 102;
        /// <summary>
        /// Keepalive time
        /// </summary>
        internal ulong keepalive_time = 10000;
        /// <summary>
        /// Keepalive interval
        /// </summary>
        internal ulong keepalive_interval = 15000;
        /// <summary>
        /// Connect Timeout.
        /// </summary>
        internal int ConnectTimeoutms = 15000;
        /// <summary>
        /// ManualResetEvent instances signal completion.
        /// </summary>
        internal ManualResetEvent connectDone =
            new ManualResetEvent(false);
        internal ManualResetEvent sendDone =
            new ManualResetEvent(false);
        internal ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        /// <summary>
        /// SafeHandle 
        /// </summary>
    }
}
