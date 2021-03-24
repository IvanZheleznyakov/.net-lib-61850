using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DLL_Log;

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
        internal LastExceptionInfo lastExceptionInfo = new LastExceptionInfo();
        internal List<ControlObject> listOfControlObjects = new List<ControlObject>();

        public delegate void connectionClosedEventHandler();
        public delegate void connectionStartedHandler();

        public delegate void newReportReceivedEventHandler(Report report);

        public delegate void responseReceivedHandler(IResponse response);
        public delegate void writeResponseReceivedHandler(WriteResponse response);
        public delegate void readResponseReceivedHandler(ReadResponse response);
        public delegate void rcbResponseReceivedHandler(RCBResponse response);
        public delegate void selectResponseReceivedHandler(SelectResponse response);
        public delegate void fileDirectoryResponseReceivedHandler(FileDirectoryResponse response);
        public delegate void fileResponseReceivedHandler(FileResponse response);
        public delegate void readDataSetResponseReceivedHandler(ReadDataSetResponse response);
        public delegate void getVariableSpecificationReceivedHandler(MmsVariableSpecResponse response);

        /// <summary>
        /// Очередь из поступивших отчётов.
        /// </summary>
        public ConcurrentQueue<Report> QueueOfReports
        {
            get
            {
                return worker.iecs.mms.queueOfReports;
            }
        }

        /// <summary>
        /// Конструктор класса, инициализирующий необходимые внутренние поля.
        /// </summary>
        public LibraryManager(SourceMsg_t logger = null, int? vtu = null)
        {
            worker = new Scsm_MMS_Worker(logger, vtu);
        }

        public TcpProtocolState TcpState
        {
            get
            {
                if (worker == null || worker.iecs == null)
                {
                    return TcpProtocolState.TCP_STATE_CLOSED;
                }
                return worker.iecs.tstate;
            }
            internal set
            {

            }
        }

        /// <summary>
        /// Синхронная установка соединения и построение программной модели.
        /// </summary>
        /// <param name="hostName">IP-адрес устройства.</param>
        /// <param name="port">Номер порта.</param>
        /// <param name="closedHandler">Пользовательский обработчик закрытия соединения.</param>
        /// <param name="waitingTime">Время ожидания установки соединения и построения программной модели (в миллисекундах).</param>
        /// <returns>Булева переменная, указывающая, успешно ли установилось соединение за указанное время ожидания.</returns>
        public bool Start(string hostName, int port, connectionClosedEventHandler closedHandler, int waitingTime = 8000)
        {
            try
            {
                Task connectedTask = StartAsync(hostName, port, closedHandler, ConnectionStartedPrivateHandler);
                worker.iecs.sourceLogger?.SendInfo("Ожидание соединения с устройством по IP: " + hostName);
                bool isConnected = connectedTask.Wait(60000);
                worker.iecs.sourceLogger?.SendInfo("Соединение с " + hostName + " вернуло: " + isConnected);
                if (!isConnected)
                {
                    worker.iecs.sourceLogger?.SendError("lib61850: слишком долгое ожидание. Самостоятельно закрываем соединение с " + hostName);
                    worker.Stop();
                }
                return isConnected;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        private void ConnectionStartedPrivateHandler()
        {

        }

        /// <summary>
        /// Асинхронная установка соединения и построение программной модели.
        /// </summary>
        /// <param name="hostName">IP-адрес устройства.</param>
        /// <param name="port">Номер порта.</param>
        /// <param name="closedHandler">Пользовательский обработчик закрытия соединения.</param>
        /// <param name="startedHandler">Пользовательский обработчик успешной установки соединения.</param>
        /// <returns>Задача, выполняющая обработчик успешной установки соединения.</returns>
        public Task StartAsync(string hostName, int port, connectionClosedEventHandler closedHandler, connectionStartedHandler startedHandler)
        {
            try
            {
                worker.iecs.sourceLogger?.SendInfo("lib61850net: start opening connection on " + hostName);
                Task connectedTask = new Task(() => startedHandler());
                Task closedTask = new Task(() => closedHandler());
                if (worker.Start(hostName, port, closedTask, connectedTask))
                {
                    return connectedTask;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Закрытие текущего соединения.
        /// </summary>
        /// <returns>Булева переменная, указывающая, успешно ли закрылось соединение.</returns>
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
        /// Установка обработчика для получения отчётов.
        /// </summary>
        /// <param name="eventHandler">Пользовательский обработчик получения отчётов.</param>
        /// <returns>Булева переменная, указывающая, успешно ли произошла установка обработчика.</returns>
        public bool SetReportReceivedHandler(newReportReceivedEventHandler eventHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return false;
                }
                worker.iecs.mms.reportReceivedEventHandler = eventHandler;
                return true;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
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
                    string errorMsg = "Программная модель дерева объектов построена с ошибкой";
                    worker?.iecs?.sourceLogger?.SendError(errorMsg);
                    throw new Exception(errorMsg);
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
        /// <param name="ldName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями всех логических узлов устройства.</returns>
        public List<string> GetLogicalDeviceDirectory(string ldName)
        {
            try
            {
                var node = worker.iecs.DataModel.ied.FindChildNode(ldName);
                if (node == null)
                {
                    string errorMsg = "GetLogicalDeviceDirectory: ошибка в нахождении логического устройства " + ldName;
               //     worker?.iecs?.sourceLogger?.SendInfo(errorMsg);
                    throw new Exception(errorMsg);
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
                    string errorMsg = "GetLogicalNodeDirectory: ошибка в нахождении логического узла " + lnReference + " " + FC;
            //        worker?.iecs?.sourceLogger?.SendInfo(errorMsg);
                    throw new Exception(errorMsg);
                }

                return node.GetChildNodeNames(false, false);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public async Task<DeviceDirectoryResponse> GetDatasetsAsync_NoModel(string ldName)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                DeviceDirectoryResponse response = new DeviceDirectoryResponse()
                {
                    TypeOfError = DataAccessErrorEnum.timeoutError
                };
                Task responseTask = new Task(() => privateDirResponseHandler(response));
                worker.iecs.mms.SendGetNameListNamedVariableList(worker.iecs, ldName, responseTask, response);
                return await Task.Run(() => privateDirResponseHandler(response));
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

    //    public 

        private DeviceDirectoryResponse privateDirResponseHandler(DeviceDirectoryResponse response)
        {
            return response;
        }

        private DeviceDirectoryResponse lastDirectoryResponse = null;

        /// <summary>
        /// Получение списка датасетов.
        /// </summary>
        /// <param name="ldName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями датасетов на указанном устройстве.</returns>
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

        public List<string> GetDatasetNameValues(string datasetName)
        {
            try
            {
          //      Console.WriteLine("start getting datasetnamevalues on " + datasetName);
                NodeBase node = worker.iecs.DataModel.datasets.FindNodeByAddress(datasetName);
           //     Console.WriteLine("node in datasetname got, name : " + node.Name);
                return node.GetDataSetChildsWithFC();
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                throw new IedException("lib61850net: Dataset " + datasetName + " is not found with ex: " + ex.Message);
            }
        }

        /// <summary>
        /// Получение списка буферизированных отчётов.
        /// </summary>
        /// <param name="ldName">Имя логического устройства.</param>
        /// <returns>Список строк с названиями буферизированных отчётов указанного устройства.</returns>
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
        /// <returns>Список строк с названиями небуферизированных отчётов указанного устройства.</returns>
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

        //public async Task<WriteResponse> WriteDataAsync_NoModel(string name, FunctionalConstraintEnum FC, MmsValue value, int waitingTime = 2000)
        //{

        //}

        private WriteResponse lastWriteRespone;

        /// <summary>
        /// Синхронная запись данных в узел дерева объектов.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла в дереве объектов устройства.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="value">Записываемое значение.</param>
        /// <param name="waitingTime">Время ожидания записи данных (в миллисекундах).</param>
        /// <returns>Ответ на запрос записи данных.</returns>
        public WriteResponse WriteData(string name, FunctionalConstraintEnum FC, MmsValue value, int waitingTime = 2000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastWriteRespone = new WriteResponse()
                {
                    TypeOfErrors = new List<DataAccessErrorEnum>() { DataAccessErrorEnum.timeoutError },
                    Names = null,
                };
                Task responseTask = WriteDataAsync(name, FC, value, WriteDataPrivateHandler);
                responseTask.Wait(waitingTime);
                return lastWriteRespone;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private void WriteDataPrivateHandler(WriteResponse response)
        {
            lastWriteRespone = response;
        }

        /// <summary>
        /// Асинхронная запись данных в узел дерева объектов.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла в дереве объектов устройства.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="value">Записываемое значение.</param>
        /// <param name="responseHandler">Пользовательский обработчик получения ответа на запись.</param>
        /// <returns>Задача, выполняющая обработчик.</returns>
        public Task WriteDataAsync(string name, FunctionalConstraintEnum FC, MmsValue value, writeResponseReceivedHandler responseHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                //Action<Response, object> responseAction = (response, param) =>
                //{
                //    responseHandler(response.Item1, response.Item2);
                //};
                WriteResponse response = new WriteResponse();
                Task responseTask = new Task(() => responseHandler(response));

                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                MmsValue[] sendindMmsValue = new MmsValue[1] { value };
                worker.iecs.Controller.WriteData((node as NodeData), true, responseTask, response, sendindMmsValue);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private MmsVariableSpecResponse lastMVSRespone;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="FC"></param>
        /// <param name="waitingTime"></param>
        /// <returns></returns>
        public MmsVariableSpecResponse GetVariableSpecification(string name, FunctionalConstraintEnum FC, int waitingTime = 15000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastMVSRespone = new MmsVariableSpecResponse()
                {
                    TypeOfError = DataAccessErrorEnum.timeoutError,
                    MmsVariableSpecification = null,
                };
                Task responseTask = GetVariableSpecificationAsync(name, FC, GetVarSpecPrivateHandler);
                if (responseTask == null)
                {
                    lastMVSRespone.TypeOfError = DataAccessErrorEnum.invalidAddress;
                    return lastMVSRespone;
                }
                if (responseTask.Wait(waitingTime))
                {
                    return lastMVSRespone;
                }
                else
                {
                    return new MmsVariableSpecResponse()
                    {
                        MmsVariableSpecification = null,
                        TypeOfError = DataAccessErrorEnum.timeoutError
                    };
                }
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private void GetVarSpecPrivateHandler(MmsVariableSpecResponse response)
        {
            lastMVSRespone = response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="FC"></param>
        /// <param name="responseHandler"></param>
        /// <returns></returns>
        public Task GetVariableSpecificationAsync(string name, FunctionalConstraintEnum FC, getVariableSpecificationReceivedHandler responseHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                MmsVariableSpecResponse response = new MmsVariableSpecResponse();
                Task responseTask = new Task(() => responseHandler(response));
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                if (node == null)
                {
                    worker.iecs.sourceLogger?.SendInfo("LibraryManager.GetVariableSpecification(): Не найдена переменная с именем " + name + " для получения спецификации");
                    return null;
                }
                worker.iecs.mms.SendGetVariableAccessAttributes(worker.iecs, node, responseTask, response);
                //  worker.iecs.Controller.WriteData((node as NodeData), true, responseTask, response);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public Task ReadDataSetValuesAsync(string name, readDataSetResponseReceivedHandler responseHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                ReadDataSetResponse response = new ReadDataSetResponse();
                Task responseTask = new Task(() => responseHandler(response));

                var node = worker.iecs.DataModel.datasets.FindNodeByAddress(name);
                worker.iecs.Controller.ReadData((NodeVL)node, responseTask, response);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                throw new IedException("lib61850net: cant send ReadDataSetValues with ex: " + ex.Message);
            }
        }

        private ReadDataSetResponse lastReadDataSetResponse;

        public ReadDataSetResponse ReadDataSetValues(string name, int waitingTime = 30000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastReadDataSetResponse = new ReadDataSetResponse()
                {
                    MmsValues = null,
                    TypeOfErrors = new List<DataAccessErrorEnum>() { DataAccessErrorEnum.timeoutError }
                };
                Task responseTask = ReadDataSetValuesAsync(name, ReadDSPrivateHandler);
                responseTask.Wait(waitingTime);
                return lastReadDataSetResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                throw new IedException("lib61850net: cant send ReadDataSetValues with ex: " + ex.Message);
            }
        }

        private void ReadDSPrivateHandler(ReadDataSetResponse response)
        {
            lastReadDataSetResponse = response;
        }

        private ReadResponse lastReadResponse;

        /// <summary>
        /// Синхронное чтение данных с устройства.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла дерева объектов в устройстве.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="waitingTime">Время ожидания чтения данных.</param>
        /// <returns>Результат чтения данных.</returns>
        public ReadResponse ReadData(string name, FunctionalConstraintEnum FC, int waitingTime = 10000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastReadResponse = new ReadResponse()
                {
                    TypeOfError = DataAccessErrorEnum.timeoutError,
                    MmsValue = null,
                };
                Task responseTask = ReadDataAsync(name, FC, ReadDataPrivateHandler);
                responseTask.Wait(waitingTime);
                return lastReadResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private void ReadDataPrivateHandler(ReadResponse response)
        {
            lastReadResponse = response;
        }

        /// <summary>
        /// Асинхронное чтение данных с устройства.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла дерева объектов в устройстве.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="responseHandler">Пользовательский обработчик получения ответа на чтение.</param>
        /// <returns>Задача, выполняющая обработчик.</returns>
        public Task ReadDataAsync(string name, FunctionalConstraintEnum FC, readResponseReceivedHandler responseHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                ReadResponse response = new ReadResponse();
                Task responseTask = new Task(() => responseHandler(response));
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                if (node == null)
                {
                    response.TypeOfError = DataAccessErrorEnum.invalidAddress;
                    responseTask.Start();
                }
                else
                {
                    worker.iecs.Controller.ReadData(node, responseTask, response);
                }
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Создание экземпляра ReportControlBlock для настраивания параметров отчёта.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) отчёта.</param>
        /// <param name="isBuffered">Тип отчёта (true - буферизированный, false - небуферизированный).</param>
        /// <returns>Экземпляр ReportControlBlock с текущими параметрами данного отчёта.</returns>
        public ReportControlBlock CreateReportControlBlock(string name, bool isBuffered)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                //     Console.WriteLine("Creating reportcontrolblock with name" + name);
                FunctionalConstraintEnum repFC = isBuffered ? FunctionalConstraintEnum.BR : FunctionalConstraintEnum.RP;
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, repFC);
                ReportControlBlock resultRcb = new ReportControlBlock();
                resultRcb.ObjectReference = name;
                var repNode = new NodeBase("");
                if (isBuffered)
                {
                    repNode = worker.iecs.DataModel.brcbs.FindNodeByAddress(mmsReference);
                }
                else
                {
                    repNode = worker.iecs.DataModel.urcbs.FindNodeByAddress(mmsReference);
                }

                if (repNode == null)
                {
                    throw new Exception("Параметры отчёта с заданным именем " + name + " не найдены в дереве объектов");
                }

                resultRcb.self = (NodeRCB)repNode;

           //     Console.WriteLine("rcb created with " + name);

                return resultRcb;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                worker?.iecs?.sourceLogger?.SendError("CreateReportControlBlock exception " + name + ": " + ex.Message);
                return null;
            }
        }

        private RCBResponse lastRCBResponse;

        /// <summary>
        /// Синхронное получение актуальных параметров отчёта.
        /// </summary>
        /// <param name="rcb">Экземпляр ReportControlBlock, соответствующий требуемому отчёту.</param>
        /// <param name="waitingTime">Время ожидания получения ответа.</param>
        /// <returns>Ответ на обновление ReportControlBlock.</returns>
        public RCBResponse UpdateReportControlBlock(ReportControlBlock rcb, int waitingTime = 60000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    worker.iecs.sourceLogger.SendInfo("in update rcb tcp not connected(???) " + rcb.Name + " " + TcpState);
                    return null;
                }
                lastRCBResponse = null;
                Task responseTask = UpdateReportControlBlockAsync(rcb, UpdateRCBHandler);
                worker.iecs.sourceLogger.SendInfo("now task waiting in updatercb with name: " + rcb.Name);
                responseTask.Wait(waitingTime);
                worker.iecs.sourceLogger.SendInfo("task in updatercb finished with name " + rcb.Name);
                return lastRCBResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                worker.iecs.sourceLogger.SendInfo(GetLastExceptionInfo().LastException.Message + " " + GetLastExceptionInfo().LastException.StackTrace + " " + GetLastExceptionInfo().LastMethodWithException);
                return null;
            }
        }

        private void UpdateRCBHandler(RCBResponse response)
        {
            lastRCBResponse = response;
        }

        /// <summary>
        /// Асинхронное получение актуальных параметров отчёта.
        /// </summary>
        /// <param name="rcb">Экземпляр ReportControlBlock, соответствующий требуемому отчёту.</param>
        /// <param name="responseHandler">Пользовательский обработчик получения ответа на обновление RCB.</param>
        /// <returns>Задача, выполняющая обработчик.</returns>
        public Task UpdateReportControlBlockAsync(ReportControlBlock rcb, rcbResponseReceivedHandler responseHandler)
        {
            try
            {
                worker.iecs.sourceLogger.SendInfo((rcb == null).ToString() + " " + (rcb?.Name == null).ToString() + " " + (rcb?.self == null).ToString() + " ");
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    worker.iecs.sourceLogger.SendInfo("in update rcb tcp not connected(???) " + rcb.Name + " " + TcpState);
                    return null;
                }
                worker.iecs.sourceLogger.SendInfo("start update rcb with name " + rcb.Name);
                RCBResponse response = new RCBResponse();
                Task responseTask = new Task(() => responseHandler(response));
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(rcb.self.IecAddress, rcb.IsBuffered ? FunctionalConstraintEnum.BR : FunctionalConstraintEnum.RP);
                worker.iecs.sourceLogger.SendInfo("updatercb mmsref: " + mmsReference);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                worker.iecs.sourceLogger.SendInfo("node==null " + (node == null).ToString());
                worker.iecs.Controller.ReadData(node, responseTask, response);
                worker.iecs.sourceLogger.SendInfo("updatercb task created on " + rcb.Name);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                worker.iecs.sourceLogger.SendInfo(GetLastExceptionInfo().LastException.Message + " " + GetLastExceptionInfo().LastException.StackTrace + " " + GetLastExceptionInfo().LastMethodWithException);
                return null;
            }
        }

        /// <summary>
        /// Синхронная запись параметров отчёта.
        /// </summary>
        /// <param name="rcbPar">Пользовательские параметры отчёта.</param>
        /// <param name="waitingTime">Время ожидания получения ответа.</param>
        /// <returns>Ответ на запрос записи параметров отчёта.</returns>
        public WriteResponse SetReportControlBlock(ReportControlBlock rcbPar, int waitingTime = 15000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastWriteRespone = null;
                Task responseTask = SetReportControlBlockAsync(rcbPar, WriteDataPrivateHandler);
                responseTask.Wait(waitingTime);
                if (lastWriteRespone != null)
                {
                    rcbPar.ResetFlags();
                }
                return lastWriteRespone;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронная запись параметров отчёта.
        /// </summary>
        /// <param name="rcbPar">Пользовательские параметры отчёта.</param>
        /// <param name="responseHandler">Пользовательский обработчик получения ответа на запись параметров отчёта.</param>
        /// <returns>Задача, выполняющая обработчик.</returns>
        public Task SetReportControlBlockAsync(ReportControlBlock rcbPar, writeResponseReceivedHandler responseHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                WriteResponse response = new WriteResponse();
                Task responseTask = new Task(() => responseHandler(response));
                worker.iecs.Controller.WriteRcb(rcbPar, false, responseTask, response);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private FileDirectoryResponse lastFileDirectoryResponse;

        /// <summary>
        /// Синхронное получение директории файлов.
        /// </summary>
        /// <param name="name">Полное имя директории.</param>
        /// <param name="waitingTime">Время ожидания получения директории.</param>
        /// <returns>Ответ на запрос.</returns>
        public FileDirectoryResponse GetFileDirectory(string name, int waitingTime = 3000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastFileDirectoryResponse = null;
                Task responseTask = GetFileDirectoryAsync(name, FileDirectoryPrivateHandler);
                responseTask.Wait(waitingTime);
                return lastFileDirectoryResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public void FileDirectoryPrivateHandler(FileDirectoryResponse response)
        {
            lastFileDirectoryResponse = response;
        }

        /// <summary>
        /// Асинхронное получение директории файлов.
        /// </summary>
        /// <param name="name">Полное имя директории.</param>
        /// <param name="responseHandler">Пользовательский обработчик получения ответа на запрос.</param>
        /// <returns>Задача, выполняющая обработчик.</returns>
        public Task GetFileDirectoryAsync(string name, fileDirectoryResponseReceivedHandler responseHandler)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                var node = new NodeBase("");
                if (name == "")
                {
                    node = worker.iecs.DataModel.files;
                }
                else
                {
                    node = worker.iecs.DataModel.files.FindFileByName(name);
                }

                FileDirectoryResponse response = new FileDirectoryResponse();
                Task responseTask = new Task(() => responseHandler(response));

                worker.iecs.Controller.GetFileList(node, responseTask, response);

                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        FileResponse lastFileResponse;

        /// <summary>
        /// Синхронное получение файла.
        /// </summary>
        /// <param name="name">Полное имя файла.</param>
        /// <param name="waitingTime">Время ожидания получения файла.</param>
        /// <returns>Прочитанный файл с устройства.</returns>
        public FileResponse GetFile(string name, int waitingTime = 100000)
        {
            try
            {
                if (TcpState != TcpProtocolState.TCP_CONNECTED)
                {
                    return null;
                }
                lastFileResponse = null;
                Task responseTask = GetFileAsync(name, FilePrivateHandler);
                responseTask?.Wait(waitingTime);
                return lastFileResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private void FilePrivateHandler(FileResponse response)
        {
            lastFileResponse = response;
        }

        /// <summary>
        /// Асинхронное получение файла.
        /// </summary>
        /// <param name="name">Полное имя файла.</param>
        /// <param name="responseHandler">Пользовательский обработчик получения ответа на запрос.</param>
        /// <returns>Задача, выполняющая обработчик.</returns>
        public Task GetFileAsync(string name, fileResponseReceivedHandler responseHandler)
        {
            if (TcpState != TcpProtocolState.TCP_CONNECTED)
            {
                return null;
            }
            if (worker.IsFileReadingNow || worker.iecs.fstate == FileTransferState.FILE_OPENED || worker.iecs.fstate == FileTransferState.FILE_READ)
            {
           //     Console.WriteLine("file is reading now");
                Exception exception = new Exception("В данный момент уже происходит чтение файла.");
                UpdateLastExceptionInfo(exception, MethodBase.GetCurrentMethod().Name);
                return null;
            }
            try
            {
                FileResponse response = new FileResponse();
                Task responseTask = new Task(() => responseHandler(response));
                worker.IsFileReadingNow = true;
                var nodeFile = worker.iecs.DataModel.files.FindFileByName(name);
                worker.iecs.Controller.GetFile((NodeFile)nodeFile, responseTask, response);
                return responseTask;
            }
            catch (Exception ex)
            {
                worker.IsFileReadingNow = false;
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
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
        internal void UpdateLastExceptionInfo(Exception ex, string methodName)
        {
            lastExceptionInfo.LastException = ex;
            lastExceptionInfo.LastMethodWithException = methodName;
        }
    }
}