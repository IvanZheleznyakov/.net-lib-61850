using System;
using System.Net;

namespace lib61850net
{
    /// <summary>
    /// TPKT Header parsing according to RFC1006 / OSI (COTP) mapping to TCP/IP
    /// </summary>
    class IsoTpkt
    {
        internal const byte TPKT_START = 0x03;
        internal const byte TPKT_RES = 0x00;
        internal const int TPKT_MAXLEN = 2048;

        internal const int TPKT_IDX_START = 0;
        internal const int TPKT_IDX_RES = 1;
        internal const int TPKT_IDX_LEN = 2;

        internal const int TPKT_SIZEOF = 4;

        /// <summary>
        /// Parsing of data from socket into TPKT datagrams
        /// </summary>
        /// <param name="iecs">Global protocol state structure</param>
        internal static void Parse(TcpState tcps)
        {
            Iec61850State iecs = (Iec61850State)tcps;

            for (int i = 0; i < iecs.recvBytes; i++)
            {
                if (iecs.kstate == IsoTpktState.TPKT_RECEIVE_ERROR)
                {
                    iecs.kstate = IsoTpktState.TPKT_RECEIVE_START;
                    tcps.logger.LogError("iec61850tpktState.IEC61850_RECEIVE_ERROR\n");
                    tcps.sourceLogger?.SendError("lib61850net: iec61850tpktState.IEC61850_RECEIVE_ERROR\n");
                    break;
                }
                switch (iecs.kstate)
                {
                    case IsoTpktState.TPKT_RECEIVE_START:
                        if (iecs.recvBuffer[i] == TPKT_START)
                        {
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_RES;
                            iecs.dataBufferIndex = 0;
                        }
                        else
                        {
                            tcps.sourceLogger?.SendError("lib61850net: Synchronization lost: TPKT START / VERSION!\n");
                            tcps.logger.LogError("Synchronization lost: TPKT START / VERSION!\n");
                            // давайте-ка разорвем соединение от греха подальше, если уже произошла ошибка синхронизации по TPKT
                            iecs.tstate = TcpProtocolState.TCP_STATE_SHUTDOWN;
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_ERROR;
                        }
                        break;
                    case IsoTpktState.TPKT_RECEIVE_RES:
                        if (iecs.recvBuffer[i] == TPKT_RES)
                        {
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_LEN1;
                        }
                        else
                        {
                            tcps.sourceLogger?.SendError("lib61850net: Synchronization lost: TPKT RES!\n");
                            tcps.logger.LogError("Synchronization lost: TPKT RES!\n");
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_ERROR;
                        }
                        break;
                    case IsoTpktState.TPKT_RECEIVE_LEN1:
                        iecs.TpktLen = iecs.recvBuffer[i] << 8;
                        iecs.kstate = IsoTpktState.TPKT_RECEIVE_LEN2;
                        break;
                    case IsoTpktState.TPKT_RECEIVE_LEN2:
                        iecs.TpktLen |= iecs.recvBuffer[i];
                        if (iecs.TpktLen <= TPKT_MAXLEN)
                        {
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_DATA_COPY;
                        }
                        else
                        {
                            tcps.sourceLogger?.SendError("lib61850net: Synchronization lost: TPKT TPDU too long!\n");
                            tcps.logger.LogError("Synchronization lost: TPKT TPDU too long!\n");
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_ERROR;
                        }
                        break;
                    case IsoTpktState.TPKT_RECEIVE_DATA_COPY:
                        // OPTIMIZE!!!
                        //iecs.dataBuffer[iecs.dataBufferIndex++] = iecs.recvBuffer[i];
                        int copylen = Math.Min(/*available*/iecs.recvBytes - i, /*wanted*/iecs.TpktLen - TPKT_SIZEOF - iecs.dataBufferIndex);
                        Array.Copy(iecs.recvBuffer, i, iecs.dataBuffer, iecs.dataBufferIndex, copylen);
                        i += copylen - 1; // i will be incremented in 'for' cycle, so we must decrement here
                        iecs.dataBufferIndex += copylen;

                        if (iecs.dataBufferIndex == iecs.TpktLen - TPKT_SIZEOF)
                        {
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_START;
                            // Call OSI Layer
                            tcps.logger.LogDebug("TPKT sent to OSI");
                            iecs.iso.Receive(iecs);
                        }
                        break;
                    default:
                        tcps.sourceLogger?.SendError("lib61850net: iecs.tstate: unknown state!\n");
                        tcps.logger.LogError("iecs.tstate: unknown state!\n");
                        break;
                }	// switch
            }	// for
        }

        internal static void Send(TcpState tcps)
        {
            // TPKT
            tcps.sendBuffer[IsoTpkt.TPKT_IDX_START] = IsoTpkt.TPKT_START;
            tcps.sendBuffer[IsoTpkt.TPKT_IDX_RES] = IsoTpkt.TPKT_RES;
            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(tcps.sendBytes))), 0, tcps.sendBuffer, IsoTpkt.TPKT_IDX_LEN, 2);

            tcps.logger.LogDebugBuffer("Send Tpkt", tcps.sendBuffer, 0, tcps.sendBytes);
            TcpRw.Send(tcps);
        }
    }
}
