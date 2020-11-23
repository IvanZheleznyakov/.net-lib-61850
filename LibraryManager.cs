using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IEDExplorer
{
    /// <summary>
    /// Основной пользовательский класс для работы с библиотекой.
    /// </summary>
    public class LibraryManager
    {
        /// <summary>
        /// Класс предоставляет информацию о последнем сработавшем исключении и методе, в котором оно произошло.
        /// </summary>
        public class LastExceptionInfo
        {
            public Exception LastException { get; set; }
            public string LastMethodWithException { get; set; }
        }

        internal Scsm_MMS_Worker worker;
        protected LastExceptionInfo lastExceptionInfo = new LastExceptionInfo();

        public delegate void connectionClosedEventHandler();
        public event connectionClosedEventHandler ConnectionClosed;

        public delegate void newReportReceivedEventhandler(Report report);
        public event newReportReceivedEventhandler NewReportReceived;

        public delegate void modelHasBeenCreatedEventHandler(LibraryManager libraryManager);
        public event modelHasBeenCreatedEventHandler ModelHasBeenCreated;

        public delegate void reportControlBlockUpdatedEventHandler(LibraryManager libraryManager, ReportControlBlock reportControlBlock);
        public event reportControlBlockUpdatedEventHandler ReportControlBlockUpdated;

        public delegate void responseReceivedHandler(Response response, object param);

        /// <summary>
        /// Конструктор класса, инициализирующий необходимые поля.
        /// </summary>
        public LibraryManager()
        {
            worker = new Scsm_MMS_Worker();
            worker.ConnectShutDownedEvent += Worker_ConnectShutDownedEvent;
            worker.iecs.mms.NewReportReceived += Mms_NewReportReceived;
            worker.ModelHasBeenCreated += Worker_ModelHasBeenCreated;
        }

        /// <summary>
        /// Установка соединения с утройством.
        /// </summary>
        /// <param name="hostName">IP адрес устройства.</param>
        /// <param name="port">Номер порта.</param>
        /// <returns>Булева переменная, указывающая, успешно ли создалось соединение.</returns>
        public bool Start(string hostName, int port)
        {
            try
            {
                return worker.Start(hostName, port);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Закрытие соединения с устройством.
        /// </summary>
        public bool Stop()
        {
            try
            {
                worker.Stop();
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Запись данных.
        /// </summary>
        /// <param name="name">Имя узла в дереве объектов устройства.</param>
        /// <param name="value">Записываемое значение.</param>
        /// <param name="reRead">True - прочитать данные в узле сразу после записи; False - иначе.</param>
        /// <returns>Булева переменная, указывающая, успешно ли произошла запись.</returns>
        public bool WriteData(string name, object value, bool reRead)
        {
            try
            {
                var node = new NodeBase("");
                worker.iecs.DataModel.addressNodesPairs.TryGetValue(name, out node);
                (node as NodeData).DataValue = value;
                worker.iecs.Controller.WriteData((node as NodeData), reRead);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Чтение данных.
        /// </summary>
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        public bool ReadData(string name, responseReceivedHandler receivedHandler)
        {
            try
            {
                NodeBase outNode = new NodeBase("");
                worker.iecs.DataModel.addressNodesPairs.TryGetValue(name, out outNode);
                worker.iecs.Controller.ReadData(outNode, receivedHandler);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        public ReportControlBlock CreateReportControlBlock(string name)
        {
            ReportControlBlock resultRcb = new ReportControlBlock();
            NodeBase outNode = new NodeBase("");
            worker.iecs.DataModel.addressNodesPairs.TryGetValue(name, out outNode);
            resultRcb.self = (NodeRCB)outNode;

            return resultRcb;
        }

        public void UpdateReportControlBlock(ReportControlBlock rcb, responseReceivedHandler receivedHandler)
        {
            ReadData(rcb.self.IecAddress, receivedHandler);
        }

        /// <summary>
        /// Установка и запись параметров отчёта.
        /// </summary>
        /// <param name="rcbPar">Параметры отчёта.</param>
        /// <param name="reRead">True - прочитать данные в узле сразу после записи; False - иначе.</param>
        public bool SetReportControlBlock(ReportControlBlock rcbPar, bool reRead)
        {
            try
            {
                worker.iecs.Controller.WriteRcb(rcbPar, reRead);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Отправка команды.
        /// </summary>
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        /// <param name="commandParams">Параметры команды.</param>
        /// <param name="action"></param>
        public void SendCommand(NodeBase node, CommandParams commandParams, ActionRequested action = ActionRequested.WriteAsStructure)
        {
            worker.iecs.Controller.SendCommand(node, commandParams, action);
        }

        /// <summary>
        /// Получение списка файлов и директорий.
        /// </summary>
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        public void GetFileDirectory(string name, LibraryManager.responseReceivedHandler receivedHandler)
        {
            var node = new NodeBase("");
            if (name == "/")
            {
                node = worker.iecs.DataModel.files;
            }
            else
            {
                node = worker.iecs.DataModel.files.FindFileByName(name);
            }

            worker.iecs.Controller.GetFileList(node, receivedHandler);
        }

        /// <summary>
        /// Получение файла.
        /// </summary>
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        public bool GetFile(string name, LibraryManager.responseReceivedHandler receivedHandler)
        {
            Console.WriteLine("FILE STATE: " + worker.IsFileReadingNow + " ||| " + worker.iecs.fstate.ToString());
            if (worker.IsFileReadingNow || worker.iecs.fstate == FileTransferState.FILE_OPENED || worker.iecs.fstate == FileTransferState.FILE_READ)
            {
                Console.WriteLine("file is reading now");
                Exception exception = new Exception("В данный момент уже происходит чтение файла.");
                UpdateLastExceptionInfo(exception, MethodBase.GetCurrentMethod().Name);
                return false;
            }
            try
            {
                worker.IsFileReadingNow = true;
                var nodeFile = worker.iecs.DataModel.files.FindFileByName(name);
                worker.iecs.Controller.GetFile((NodeFile)nodeFile, receivedHandler);
            }
            catch (Exception ex)
            {
                worker.IsFileReadingNow = false;
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
            return true;
        }

        public void Select(ControlObject cntrlObj)
        {
            worker.iecs.Controller.ReadData(cntrlObj.self.FindChildNode("SBO"));
        }

        /// <summary>
        /// Получение информации о последнем исключении.
        /// </summary>
        /// <returns>Объект, содержащий информацию о последнем исключении и методе, в котором оно было выброшено.</returns>
        public LastExceptionInfo GetLastExceptionInfo()
        {
            return lastExceptionInfo;
        }

        /// <summary>
        /// Обновление информации о последнем исключении.
        /// </summary>
        /// <param name="ex">Последнее сработавшее исключение.</param>
        /// <param name="methodName">Имя метода, где сработало исключение.</param>
        protected void UpdateLastExceptionInfo(Exception ex, string methodName)
        {
            lastExceptionInfo.LastException = ex;
            lastExceptionInfo.LastMethodWithException = methodName;
        }

        protected void Worker_ConnectShutDownedEvent()
        {
            ConnectionClosed?.Invoke();
        }

        protected void Mms_NewReportReceived(Report report)
        {
            NewReportReceived?.Invoke(report);
        }

        protected void Worker_ModelHasBeenCreated()
        {
            ModelHasBeenCreated?.Invoke(this);
        }
    }
}