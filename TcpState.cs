/*
 *  Copyright (C) 2013 Pavel Charvat
 * 
 *  This file is part of IEDExplorer.
 *
 *  IEDExplorer is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General internal License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IEDExplorer is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General internal License for more details.
 *
 *  You should have received a copy of the GNU General internal License
 *  along with IEDExplorer.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace lib61850net
{
    internal class TcpState
    {
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
