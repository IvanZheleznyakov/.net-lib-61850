using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lib61850net
{
    internal class Iec61850Controller
    {
        private Iec61850State iecs;
        private Iec61850Model model;
        private long m_ctlNum = 0;
        private System.Threading.Timer delayTimer;

        internal delegate void newReportReceivedEventhandler(string rptdVarQualityLog, string rptdVarTimestampLog, string rptdVarPathLogstring, string rptdVarDescriptionLog, string rptdVarValueLog);
        internal event newReportReceivedEventhandler NewReportReceived;

        internal Iec61850Controller(Iec61850State iecs, Iec61850Model model)
        {
            this.iecs = iecs;
            this.model = model;
        }

        internal void FireNewReport(string rptdVarQualityLog, string rptdVarTimestampLog, string rptdVarPathLog, string rptdVarDescriptionLog, string rptdVarValueLog)
        {
            NewReportReceived?.Invoke(rptdVarQualityLog, rptdVarTimestampLog, rptdVarPathLog, rptdVarDescriptionLog, rptdVarValueLog);
        }

        internal void DeleteNVL(NodeVL nvl)
        {
            NodeBase[] ndarr = new NodeBase[1];
            ndarr[0] = nvl;
            iecs.Send(ndarr, nvl.CommAddress, ActionRequested.DeleteNVL);
        }

        internal void GetFileList(NodeBase nfi, AutoResetEvent responseEvent = null, FileDirectoryResponse response = null)
        {
            CommAddress ad = new CommAddress();
            ad.Variable = "/";  // for the case of reading root
            NodeBase[] ndarr = new NodeBase[1];
            ndarr[0] = nfi;
            /* if (!(nfi is NodeFile))
            {
                NodeData nd = new NodeData("x");
                nd.DataType = scsm_MMS_TypeEnum.visible_string;
                nd.DataValue = "/";
                EditValue ev = new EditValue(nd);
                DialogResult r = ev.ShowDialog();
                if (r == DialogResult.OK)
                {
                    ad.Variable = nd.StringValue;
                }
            } */
            iecs.Send(ndarr, ad, ActionRequested.GetDirectory, responseEvent, response);
        }

        internal void GetFile(NodeFile nfi, AutoResetEvent responseEvent = null, byte[] file = null)
        {
            CommAddress ad = new CommAddress();
            NodeBase[] ndarr = new NodeBase[1];
            ndarr[0] = nfi;

            if ((nfi is NodeFile))
            {
                NodeData nd = new NodeData("x");
                nd.DataType = MmsTypeEnum.VISIBLE_STRING;
                nd.DataValue = nfi.Name;
                //    EditValue ev = new EditValue(nd);
                //      System.Windows.Forms.DialogResult r = ev.ShowDialog();
                //if (r == System.Windows.Forms.DialogResult.OK)
                //{
                //    ad.Variable = nd.StringValue;
                //    nfi.NameSet4Test(ad.Variable);
                //}
                ad.Variable = nfi.Name;
                nfi.NameSet4Test(ad.Variable);
            }
            nfi.Reset();
            iecs.Send(ndarr, ad, ActionRequested.OpenFile, responseEvent, file);
        }

        internal void FileDelete(NodeFile nfi)
        {
             CommAddress ad = new CommAddress();
             NodeBase[] ndarr = new NodeBase[1];
             ndarr[0] = nfi;
             //nfi.NameSet4Test("anyfile.icd");
             nfi.Reset();
             iecs.Send(ndarr, ad, ActionRequested.FileDelete);
        }

        internal void DefineNVL(NodeVL nvl)
        {
            List<NodeBase> ndar = new List<NodeBase>();
            foreach (NodeBase n in nvl.GetChildNodes())
            {
                ndar.Add(n);
            }
            iecs.Send(ndar.ToArray(), nvl.CommAddress, ActionRequested.DefineNVL);
        }

        internal CommandParams PrepareSendCommand(NodeBase data)
        {
            if (data != null)
            {
                NodeData d = (NodeData)data.Parent;
                if (d != null)
                {
                    NodeBase b;//, c;
                    CommandParams cPar = new CommandParams();
                    cPar.CommType = CommandType.SingleCommand;
                    if ((b = d.FindChildNode("ctlVal")) != null)
                    {
                        cPar.DataType = ((NodeData)b).DataType;
                        cPar.Address = b.IecAddress;
                        cPar.ctlVal = ((NodeData)b).DataValue;
                    }
                    cPar.T = DateTime.MinValue;
                    cPar.interlockCheck = false;
                    cPar.synchroCheck = false;
                    cPar.orCat = OriginatorCategoryEnum.STATION_CONTROL;
                    cPar.orIdent = "mtra";
                    cPar.CommandFlowFlag = ControlModelEnum.Unknown;
                    b = data;
                    List<string> path = new List<string>();
                    do
                    {
                        b = b.Parent;
                        path.Add(b.Name);
                    } while (!(b is NodeFC));
                    path[0] = "ctlModel";
                    path[path.Count - 1] = "CF";
                    b = b.Parent;
                    for (int i = path.Count - 1; i >= 0; i--)
                    {
                        if ((b = b.FindChildNode(path[i])) == null)
                            break;
                    }
                    if (b != null)
                    {
                        if (b is NodeData && !(b is NodeDO))
                        {
                            cPar.CommandFlowFlag = (ControlModelEnum)((long)((b as NodeData).DataValue));
                        }
                    }

                    if (cPar.CommandFlowFlag == ControlModelEnum.Select_Before_Operate_With_Enhanced_Security || cPar.CommandFlowFlag == ControlModelEnum.Select_Before_Operate_With_Normal_Security)
                    {
                        cPar.SBOrun = true;
                    }
                    else 
                    {
                        cPar.SBOrun = false;
                    }
                    cPar.SBOdiffTime = false;
                    cPar.SBOtimeout = 100;
                    return cPar;
                }
                else
                    Logger.getLogger().LogError("Basic structure for a command not found at " + data.IecAddress + "!");
            }
            return null;
        }

        private async Task PutTaskDelay(int millis)
        {
            await Task.Delay(millis);
        }

        internal async void SendCommand(NodeBase data, CommandParams cPar, ActionRequested how)
        {
            if (cPar.SBOrun)
            {
                string sName = (cPar.CommandFlowFlag == ControlModelEnum.Select_Before_Operate_With_Enhanced_Security) ? "SBOw" : "SBO";
                NodeData d = (NodeData)data.Parent;
                NodeData op = null, sel = null;
                if (d != null)
                {
                    if (d.Name == "SBOw" || d.Name == "SBO")
                    {
                        sName = "Oper";
                        sel = (NodeData)data;
                    }
                    else
                        op = (NodeData)data;
                    NodeBase dd = d.Parent;
                    if (dd != null)
                    {
                        NodeData d2 = (NodeData)dd.FindChildNode(sName);
                        if (d2 != null)
                        {
                            NodeData d3 = (NodeData)d2.FindChildNode("ctlVal");
                            if (d3 != null)
                            {
                                if (op == null)
                                    op = d3;
                                else
                                    sel = d3;
                                SendCommandToIed(sel, cPar, how);
                                await PutTaskDelay(cPar.SBOtimeout);
                                SendCommandToIed(op, cPar, how);
                            }
                            else
                                Logger.getLogger().LogWarning("Cannot send SBO command sequence, ctlVal not found in " + d2.IecAddress);
                        }
                        else
                            Logger.getLogger().LogWarning("Cannot send SBO command sequence, " + sName + " not found in " + dd.IecAddress);
                    }
                    else
                        Logger.getLogger().LogWarning("Cannot send SBO command sequence, null parent of " + d.IecAddress);
                }
                else
                    Logger.getLogger().LogWarning("Cannot send SBO command sequence, null parent of " + data.IecAddress);
            }
            else
                SendCommandToIed(data, cPar, how);
        }

        internal void SendCommandToIed(NodeBase data, CommandParams cPar, ActionRequested how)
        {
            if (data != null)
            {
                Logger.getLogger().LogInfo("Sending command " + data.IecAddress);
                NodeData d = (NodeData)data.Parent;
                if (d != null)
                {
                    NodeBase b, c;

                    List<NodeData> ndar = new List<NodeData>();
                    //char *nameo[] = {"$Oper$ctlVal", "$Oper$origin$orCat", "$Oper$origin$orIdent", "$Oper$ctlNum", "$Oper$T", "$Oper$Test", "$Oper$Check"};
                    if ((b = d.FindChildNode("ctlVal")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = cPar.ctlVal;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("origin")) != null)
                    {
                        if (how == ActionRequested.WriteAsStructure)
                        {
                            NodeData n = new NodeData(b.Name);
                            n.DataType = MmsTypeEnum.STRUCTURE;
                            n.DataValue = 2;
                            ndar.Add(n);
                            if ((c = b.FindChildNode("orCat")) != null)
                            {
                                NodeData n2 = new NodeData(b.Name + "$" + c.Name);
                                n2.DataType = ((NodeData)c).DataType;
                                n2.DataValue = (long)cPar.orCat;
                                n.AddChildNode(n2);
                            }
                            if ((c = b.FindChildNode("orIdent")) != null)
                            {
                                NodeData n2 = new NodeData(b.Name + "$" + c.Name);
                                n2.DataType = ((NodeData)c).DataType;
                                byte[] bytes = new byte[cPar.orIdent.Length];
                                int tmp1, tmp2; bool tmp3;
                                Encoder ascii = (new ASCIIEncoding()).GetEncoder();
                                ascii.Convert(cPar.orIdent.ToCharArray(), 0, cPar.orIdent.Length, bytes, 0, cPar.orIdent.Length, true, out tmp1, out tmp2, out tmp3);
                                n2.DataValue = bytes;
                                n.AddChildNode(n2);
                            }
                        }
                        else
                        {
                            if ((c = b.FindChildNode("orCat")) != null)
                            {
                                NodeData n = new NodeData(b.Name + "$" + c.Name);
                                n.DataType = ((NodeData)c).DataType;
                                n.DataValue = (long)cPar.orCat;
                                ndar.Add(n);
                            }
                            if ((c = b.FindChildNode("orIdent")) != null)
                            {
                                NodeData n = new NodeData(b.Name + "$" + c.Name);
                                n.DataType = ((NodeData)c).DataType;
                                byte[] bytes = new byte[cPar.orIdent.Length];
                                int tmp1, tmp2; bool tmp3;
                                Encoder ascii = (new ASCIIEncoding()).GetEncoder();
                                ascii.Convert(cPar.orIdent.ToCharArray(), 0, cPar.orIdent.Length, bytes, 0, cPar.orIdent.Length, true, out tmp1, out tmp2, out tmp3);
                                n.DataValue = bytes;
                                ndar.Add(n);
                            }
                        }
                    }
                    if ((b = d.FindChildNode("ctlNum")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        if (d.Name == "SBO" || d.Name == "SBOw")
                            n.DataValue = m_ctlNum;
                        else
                            n.DataValue = m_ctlNum++;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("T")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        byte[] btm = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                        n.DataValue = btm;

                        if (cPar.T != DateTime.MinValue)
                        {
                            if (d.Name == "Oper" && cPar.SBOdiffTime && cPar.SBOrun)
                                cPar.T.AddMilliseconds(cPar.SBOtimeout);
                            Scsm_MMS.ConvertToUtcTime(cPar.T, btm);
                        }
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("Test")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = cPar.Test;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("Check")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        byte sync = 0x80;
                        byte intl = 0x40;
                        byte check = 0;
                        if (cPar.synchroCheck)
                            check |= sync;
                        if (cPar.interlockCheck)
                            check |= intl;
                        n.DataValue = new byte[] { check };
                        n.DataParam = ((NodeData)b).DataParam;
                        ndar.Add(n);
                    }
                    iecs.Send(ndar.ToArray(), d.CommAddress, how);
                }
                else
                    Logger.getLogger().LogError("Basic structure for a command not found at " + data.IecAddress + "!");
            }
        }

        internal static NodeData PrepareWriteData(NodeData data)
        {
            NodeData nd = new NodeData(data.Name)
            {
                DataType = data.DataType,
                DataValue = data.DataValue,
                DataParam = data.DataParam,
                Parent = data.Parent
            };
            return nd;
        }

        internal void WriteData(NodeData data, bool reRead, AutoResetEvent responseReceived = null, WriteResponse response = null)
        {
            if (data != null && data.DataValue != null)
            {
                NodeData[] ndarr = new NodeData[1];
                ndarr[0] = data;
                iecs.Send(ndarr, data.Parent.CommAddress, ActionRequested.Write, responseReceived, response);

                if (reRead)
                {
                    delayTimer = new System.Threading.Timer(obj =>
                    {
                        ReadData(data);
                    }, null, 1000, System.Threading.Timeout.Infinite);
                }
            }
            else
                Logger.getLogger().LogError("Iec61850Controller.WriteData: null data (-Value), cannot send");
        }

        internal void WriteRcb(ReportControlBlock rpar, bool reRead, AutoResetEvent responseEvent = null, WriteResponse response = null)
        {
            iecs.Send(rpar.GetWriteArray(), rpar.self.CommAddress, ActionRequested.Write, responseEvent, response);
            rpar.ResetFlags();

            if (reRead)
            {
                delayTimer = new System.Threading.Timer(obj =>
                {
                    ReadData(rpar.self);
                }, null, 1000, System.Threading.Timeout.Infinite);
            }
        }

        internal void ReadData(NodeBase data, AutoResetEvent responseEvent = null, object param = null)
        {
            NodeBase[] ndarr = new NodeBase[1];
            ndarr[0] = data;
            iecs.Send(ndarr, data.CommAddress, ActionRequested.Read, responseEvent, param);
        }

        internal void ActivateNVL(NodeVL vl)
        {
            //Logger.getLogger().LogError("Function not active, try to configure an RCB!");
            //return;

            NodeBase ur = null;
            Iec61850State iecs = vl.GetIecs();
            bool retry;
            if (iecs != null)
            {
                do
                {
                    ur = (NodeData)iecs.DataModel.ied.FindNodeByValue(MmsTypeEnum.VISIBLE_STRING, vl.IecAddress, ref ur);
                    if (ur == null || ur.Parent == null)
                    {
                        Logger.getLogger().LogError("Suitable URCB not found, list cannot be activated!");
                        return;
                    }
                    retry = !ur.Parent.Name.ToLower().Contains("rcb");
                    vl.urcb = (NodeData)ur;
                    NodeData d = (NodeData)vl.urcb.Parent;
                    NodeData b;
                    if ((b = (NodeData)d.FindChildNode("Resv")) != null)
                    {
                        // Resv is always a boolean
                        // If true then the rcb is occupied and we need to find another one
                        if ((bool)b.DataValue) retry = true;
                    }
                } while (retry);

                if (vl.urcb != null)
                {
                    NodeData d = (NodeData)vl.urcb.Parent;
                    List<NodeData> ndar = new List<NodeData>();
                    NodeBase b;
                    if ((b = d.FindChildNode("Resv")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = true;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("DatSet")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = ((NodeData)b).DataValue;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("OptFlds")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = new byte[] { 0x7c, 0x00 };
                        n.DataParam = 6;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("TrgOps")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = new byte[] { 0x74 };
                        n.DataParam = 2;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("RptEna")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = true;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("GI")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = true;
                        ndar.Add(n);
                    }
                    iecs.Send(ndar.ToArray(), d.CommAddress, ActionRequested.Write);
                    vl.Activated = true;
                }
            }
            else
                Logger.getLogger().LogError("Basic structure not found!");
        }

        internal void DeactivateNVL(NodeVL vl)
        {
            Iec61850State iecs = vl.GetIecs();
            if (iecs != null)
            {
                if (vl.urcb != null)
                {
                    NodeData d = (NodeData)vl.urcb.Parent;
                    List<NodeData> ndar = new List<NodeData>();
                    NodeBase b;
                    if ((b = d.FindChildNode("RptEna")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = false;
                        ndar.Add(n);
                    }
                    if ((b = d.FindChildNode("GI")) != null)
                    {
                        NodeData n = new NodeData(b.Name);
                        n.DataType = ((NodeData)b).DataType;
                        n.DataValue = false;
                        ndar.Add(n);
                    }
                    iecs.Send(ndar.ToArray(), d.CommAddress, ActionRequested.Write);
                    vl.Activated = false;
                    vl.urcb = null;
                }
            }
            else
                Logger.getLogger().LogError("Basic structure not found!");
        }

    }
}
