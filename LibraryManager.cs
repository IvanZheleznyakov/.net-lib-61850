using System;
using System.Collections.Concurrent;
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
        public LibraryManager()
        {
            worker = new Scsm_MMS_Worker();
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
                return connectedTask.Wait(waitingTime);
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
        /// <param name="ldName">Имя логического устройства.</param>
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
        /// <returns></returns>
        public MmsVariableSpecification GetVariableSpecification(string variableReference, FunctionalConstraintEnum FC)
        {
            try
            {
                var result = new List<MmsVariableSpecification>();
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(variableReference, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                return new MmsVariableSpecification((NodeData)node);
        //        var childs = node.GetChildNodes();
                //foreach (var ch in childs)
                //{
                //    result.Add(new MmsVariableSpecification((NodeData)ch));
                //}

                //return result;
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
                NodeBase node = worker.iecs.DataModel.datasets.FindNodeByAddress(datasetName);
                return node.GetDataSetChildsWithFC();
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


        private WriteResponse lastWriteRespone;

        /// <summary>
        /// Синхронная запись данных в узел дерева объектов.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла в дереве объектов устройства.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="value">Записываемое значение.</param>
        /// <param name="waitingTime">Время ожидания записи данных (в миллисекундах).</param>
        /// <returns>Ответ на запрос записи данных.</returns>
        public WriteResponse WriteData(string name, FunctionalConstraintEnum FC, object value, int waitingTime = 2000)
        {
            try
            {
                lastWriteRespone = null;
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
        public Task WriteDataAsync(string name, FunctionalConstraintEnum FC, object value, writeResponseReceivedHandler responseHandler)
        {
            try
            {
                //Action<Response, object> responseAction = (response, param) =>
                //{
                //    responseHandler(response.Item1, response.Item2);
                //};
                WriteResponse response = new WriteResponse();
                Task responseTask = new Task(() => responseHandler(response));

                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                (node as NodeData).DataValue = value;
                worker.iecs.Controller.WriteData((node as NodeData), true, responseTask, response);
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
                ReadDataSetResponse response = new ReadDataSetResponse();
                Task responseTask = new Task(() => responseHandler(response));

                var node = worker.iecs.DataModel.datasets.FindNodeByAddress(name);
                worker.iecs.Controller.ReadData((NodeVL)node, responseTask, response);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        private ReadDataSetResponse lastReadDataSetResponse;

        public ReadDataSetResponse ReadDataSetValues(string name, int waitingTime = int.MaxValue)
        {
            try
            {
                lastReadDataSetResponse = null;
                Task responseTask = ReadDataSetValuesAsync(name, ReadDSPrivateHandler);
                responseTask.Wait(waitingTime);
                return lastReadDataSetResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
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
        public ReadResponse ReadData(string name, FunctionalConstraintEnum FC, int waitingTime = 5000)
        {
            try
            {
                lastReadResponse = null;
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
                ReadResponse response = new ReadResponse();
                Task responseTask = new Task(() => responseHandler(response));
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                worker.iecs.Controller.ReadData(node, responseTask, response);
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
                resultRcb.self = (NodeRCB)repNode;

                return resultRcb;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
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
        public RCBResponse UpdateReportControlBlock(ReportControlBlock rcb, int waitingTime = int.MaxValue)
        {
            try
            {
                lastRCBResponse = null;
                Task responseTask = UpdateReportControlBlockAsync(rcb, UpdateRCBHandler);
                responseTask.Wait(waitingTime);
                return lastRCBResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
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
                RCBResponse response = new RCBResponse();
                Task responseTask = new Task(() => responseHandler(response));
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(rcb.self.IecAddress, rcb.IsBuffered ? FunctionalConstraintEnum.BR : FunctionalConstraintEnum.RP);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                worker.iecs.Controller.ReadData(node, responseTask, response);
                return responseTask;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Синхронная запись параметров отчёта.
        /// </summary>
        /// <param name="rcbPar">Пользовательские параметры отчёта.</param>
        /// <param name="waitingTime">Время ожидания получения ответа.</param>
        /// <returns>Ответ на запрос записи параметров отчёта.</returns>
        public WriteResponse SetReportControlBlock(ReportControlBlock rcbPar, int waitingTime = 2000)
        {
            try
            {
                lastWriteRespone = null;
                Task responseTask = SetReportControlBlockAsync(rcbPar, WriteDataPrivateHandler);
                responseTask.Wait(waitingTime);
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
                WriteResponse response = new WriteResponse();
                Task responseTask = new Task(() => responseHandler(response));
                worker.iecs.Controller.WriteRcb(rcbPar, true, responseTask, response);
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
                var node = new NodeBase("");
                if (name == "/")
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
        public FileResponse GetFile(string name, int waitingTime = 5000)
        {
            try
            {
                lastFileResponse = null;
                Task responseTask = GetFileAsync(name, FilePrivateHandler);
                responseTask.Wait(waitingTime);
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
            if (worker.IsFileReadingNow || worker.iecs.fstate == FileTransferState.FILE_OPENED || worker.iecs.fstate == FileTransferState.FILE_READ)
            {
                Console.WriteLine("file is reading now");
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