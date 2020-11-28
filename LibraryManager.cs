using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lib61850net
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

        public delegate void connectionOpenedEventHandler();
        public event connectionOpenedEventHandler ConnectionOpened;

        public delegate void connectionClosedEventHandler();
        public event connectionClosedEventHandler ConnectionClosed;

        public delegate void newReportReceivedEventhandler(Report report);
        public event newReportReceivedEventhandler NewReportReceived;

        public delegate void modelHasBeenCreatedEventHandler();
        public event modelHasBeenCreatedEventHandler ModelHasBeenCreated;

        public delegate void responseReceivedHandler(ReadResponse response, object param);

        /// <summary>
        /// Конструктор класса, инициализирующий необходимые поля.
        /// </summary>
        public LibraryManager()
        {
            worker = new Scsm_MMS_Worker();
            worker.ConnectShutDownedEvent += Worker_ConnectShutDownedEvent;
            worker.iecs.mms.NewReportReceived += Mms_NewReportReceived;
            worker.ModelHasBeenCreated += Worker_ModelHasBeenCreated;
            worker.ConnectionOpened += Worker_ConnectionOpened;
        }

        /// <summary>
        /// Установка соединения с утройством.
        /// </summary>
        /// <param name="hostName">IP адрес устройства.</param>
        /// <param name="port">Номер порта.</param>
        /// <returns>Булева переменная, указывающая, успешно ли создалось соединение.</returns>
        public bool Start(string hostName, int port, AutoResetEvent connectionShutDowned)
        {
            try
            {

                AutoResetEvent connectionStarted = new AutoResetEvent(false);
                if (!StartAsync(hostName, port, connectionShutDowned, connectionStarted))
                {
                    return false;
                }
                bool isReceive = connectionStarted.WaitOne(10000);
                return isReceive;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public bool StartAsync(string hostName, int port, AutoResetEvent connectionShutDowned, AutoResetEvent connectionStarted)
        {
            try
            {
                return worker.Start(hostName, port, connectionShutDowned, connectionStarted);
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
        /// Получение списка логических устройств.
        /// </summary>
        /// <returns>Список строк с названиями всех логических устройств.</returns>
        public List<string> GetLogicalDevicesList()
        {
            try
            {
                if (worker.iecs.DataModel.ied == null || worker.iecs.DataModel.ied.GetChildCount() == 0)
                {
                    return null;
                }

                return worker.iecs.DataModel.ied.GetChildNodeNames(false, false);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Получение списка логических узлов устройства.
        /// </summary>
        /// <param name="serverName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями всех логических узлов устройства.</returns>
        public List<string> GetLogicalDeviceDirectory(string ldName)
        {
            try
            {
                var node = worker.iecs.DataModel.ied.FindChildNode(ldName);
                if (node == null)
                {
                    return null;
                }

                return node.GetChildNodeNames(false, false);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Получение данных логического узла.
        /// </summary>
        /// <param name="lnReference">Ссылка (полное имя) логического узла.</param>
        /// <returns>Список строк с названиями данных логического узла.</returns>
        public List<string> GetLogicalNodeDirectory(string lnReference, FunctionalConstraintEnum FC)
        {
            try
            {
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(lnReference, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                if (node == null)
                {
                    return null;
                }

                return node.GetChildNodeNames(false, false);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Получение данных из переменной.
        /// </summary>
        /// <param name="variableReference">Ссылка (полное имя) переменной.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <returns>Список экземпляров MmsVariableSpecification, которые описывают данные в переменной.</returns>
        public List<MmsVariableSpecification> GetDataValues(string variableReference, FunctionalConstraintEnum FC)
        {
            try
            {
                var result = new List<MmsVariableSpecification>();
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(variableReference, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                var childs = node.GetChildNodes();
                foreach (var ch in childs)
                {
                    result.Add(new MmsVariableSpecification((NodeData)ch));
                }

                return result;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Получение списка датасетов.
        /// </summary>
        /// <param name="ldName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями датасетов.</returns>
        public List<string> GetDatasets(string ldName)
        {
            try
            {
                NodeBase ldDir = worker.iecs.DataModel.datasets.FindChildNode(ldName);
                return ldDir.GetChildNodeNames(false, true);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Получение списка буферизированных отчётов.
        /// </summary>
        /// <param name="ldName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями буферизированных отчётов.</returns>
        public List<string> GetBufferedReports(string ldName)
        {
            try
            {
                NodeBase brDir = worker.iecs.DataModel.brcbs.FindChildNode(ldName);
                return brDir.GetChildNodeNames(true, false);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Получение списка небуферизированных отчётов.
        /// </summary>
        /// <param name="ldName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями небуферизированных отчётов.</returns>
        public List<string> GetUnbufferedReports(string ldName)
        {
            try
            {
                NodeBase urDir = worker.iecs.DataModel.urcbs.FindChildNode(ldName);
                return urDir.GetChildNodeNames(true, false);
            }
            catch (Exception ex) 
            { 
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name); 
                return null; 
            }
        }

        public WriteResponse WriteData(string name, FunctionalConstraintEnum FC, object value)
        {
            try
            {
                WriteResponse resultResponse = null;
                AutoResetEvent responseReceived = new AutoResetEvent(false);
                if (!WriteDataAsync(name, FC, value, responseReceived, resultResponse))
                {
                    return null;
                }
                responseReceived.WaitOne(10000);

                return resultResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Запись данных.
        /// </summary>
        /// <param name="name">Имя узла в дереве объектов устройства.</param>
        /// <param name="value">Записываемое значение.</param>
        /// <returns>Булева переменная, указывающая, успешно ли произошла запись.</returns>
        public bool WriteDataAsync(string name, FunctionalConstraintEnum FC, object value, AutoResetEvent responseReceived, WriteResponse response)
        {
            try
            {
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                (node as NodeData).DataValue = value;
                worker.iecs.Controller.WriteData((node as NodeData), true, responseReceived, response);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        public MmsValue ReadData(string name, FunctionalConstraintEnum FC)
        {
            try
            {
                MmsValue resultMmsValue = null;
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                if (!ReadDataAsync(name, FC, responseEvent, resultMmsValue))
                {
                    return null;
                }
                responseEvent.WaitOne(10000);

                return resultMmsValue;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Чтение данных.
        /// </summary>
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        public bool ReadDataAsync(string name, FunctionalConstraintEnum FC, AutoResetEvent responseEvent, object value)
        {
            try
            {
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                worker.iecs.Controller.ReadData(node, responseEvent, value);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Создание экземпляра ReportControlBlock для настраивания параметров отчёта.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) отчёта.</param>
        /// <returns>Экземпляр ReportControlBlock с текущими параметрами данного отчёта.</returns>
        public ReportControlBlock CreateReportControlBlock(string name, bool isBuffered)
        {
            try
            {
                FunctionalConstraintEnum repFC = isBuffered ? FunctionalConstraintEnum.BR : FunctionalConstraintEnum.RP;
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, repFC);
                ReportControlBlock resultRcb = new ReportControlBlock();
                var repNode = new NodeBase("");
                if (isBuffered)
                {
                    repNode = worker.iecs.DataModel.brcbs.FindNodeByAddress(mmsReference);
                }
                else
                {
                    repNode = worker.iecs.DataModel.urcbs.FindNodeByAddress(mmsReference);
                }
                resultRcb.self = (NodeRCB)repNode;

                return resultRcb;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public ReportControlBlock UpdateReportControlBlock(ReportControlBlock rcb)
        {
            try
            {
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                if (!UpdateReportControlBlockAsync(rcb, responseEvent))
                {
                    return null;
                }

                responseEvent.WaitOne(10000);

                return rcb;
            }
        }

        /// <summary>
        /// Получить актуальные параметры отчёта с IED.
        /// </summary>
        /// <param name="rcb">Экземпляр ReportControlBlock, для которого хотим обновить параметры.</param>
        /// <param name="receivedHandler">Обработчик получения ответа с IED.</param>
        public bool UpdateReportControlBlockAsync(ReportControlBlock rcb, AutoResetEvent responseEvent)
        {
            try
            {
                ReadDataAsync(rcb.self.IecAddress, rcb.IsBuffered ? FunctionalConstraintEnum.BR : FunctionalConstraintEnum.RP, responseEvent, rcb);
                return true;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        private AutoResetEvent waitHandler = new AutoResetEvent(false);

        private void ReceiveUpdatedRCBHandler(ReadResponse response, object param)
        {
            waitHandler.Set();
        }

        /// <summary>
        /// Установка и запись параметров отчёта.
        /// </summary>
        /// <param name="rcbPar">Параметры отчёта.</param>
        public bool SetReportControlBlock(ReportControlBlock rcbPar)
        {
            try
            {
                worker.iecs.Controller.WriteRcb(rcbPar, true);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

        public bool GetFileDirectory(string name, responseReceivedHandler receivedHandler)
        {
            try
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

                return true;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        public bool GetFile(string name, responseReceivedHandler receivedHandler)
        {
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
            ModelHasBeenCreated?.Invoke();
        }

        private void Worker_ConnectionOpened()
        {
            ConnectionOpened?.Invoke();
        }
    }
}