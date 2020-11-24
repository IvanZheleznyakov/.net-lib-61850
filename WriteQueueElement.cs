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
using System.Linq;
using System.Text;

namespace IEDExplorer
{
    internal class WriteQueueElement
    {
        internal WriteQueueElement(NodeBase[] Data, CommAddress Address, ActionRequested Action, LibraryManager.responseReceivedHandler handler = null)
        {
            this.Data = Data;
            this.Address = Address;
            this.Action = Action;
            this.Handler = handler;
        }

        internal NodeBase[] Data { get; private set; }
        internal CommAddress Address { get; private set; }
        internal ActionRequested Action { get; private set; }
        internal LibraryManager.responseReceivedHandler Handler { get; private set; }
    }
}
