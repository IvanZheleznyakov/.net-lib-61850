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
        protected LastExceptionInfo lastExceptionInfo = new LastExceptionInfo();
        internal List<ControlObject> listOfControlObjects = new List<ControlObject>();

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
        /// <param name="connectionShutDowned">Пользовательское событие, которое перейдет в сигнальное состоянии при обрыве соединения.</param>
        /// <param name="waitingTime">Время ожидания установки соединения и построения программной модели (в миллисекундах).</param>
        /// <returns>Булева переменная, указывающая, успешно ли установилось соединение за указанное время ожидания.</returns>
        public bool Start(string hostName, int port, AutoResetEvent connectionShutDowned, int waitingTime = 8000)
        {
            try
            {
                AutoResetEvent connectionStarted = new AutoResetEvent(false);
                if (!StartAsync(hostName, port, connectionShutDowned, connectionStarted))
                {
                    return false;
                }
                bool isReceive = connectionStarted.WaitOne(waitingTime);
                return isReceive;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Асинхронная установка соединения и построение программной модели.
        /// </summary>
        /// <param name="hostName">IP-адрес устройства.</param>
        /// <param name="port">Номер порта.</param>
        /// <param name="connectionShutDowned">Пользовательское событие, которое перейдет в сигнальное состоянии при обрыве соединения.</param>
        /// <param name="connectionStarted">Пользовательское событие, которое перейдет в сигнальное состояние при успешной установке соединения.</param>
        /// <returns>Булева переменная, указывающая, успешно ли установилось соединение за указанное время ожидания.</returns>
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
        /// Установка события для получения отчётов.
        /// </summary>
        /// <param name="reportReceivedEvent">Пользовательское событие, которое перейдёт в сигнальное состояние, когда придёт новый отчёт.</param>
        public void SetNewReportReceivedEvent(AutoResetEvent reportReceivedEvent)
        {
            worker.iecs.mms.newReportReceivedEvent = reportReceivedEvent;
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
                Console.WriteLine("writedata start");
                WriteResponse resultResponse = new WriteResponse();
                AutoResetEvent responseReceived = new AutoResetEvent(false);
                if (!WriteDataAsync(name, FC, value, responseReceived, resultResponse))
                {
                    return null;
                }
                responseReceived.WaitOne(waitingTime);

                Console.WriteLine("writedata got responseevent");

                return resultResponse;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронная запись данных в узел дерева объектов.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла в дереве объектов устройства.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="value">Записываемое значение.</param>
        /// <param name="responseReceived">Пользовательское событие, которое перейдёт в сигнальное состояние при успешной отправке запроса на запись.</param>
        /// <param name="response">Экземпляр WriteResponse, в который будет записан ответ на запрос записи.</param>
        /// <returns>Булева переменная, указывающая, успешно ли отправлен запрос на запись данных за указанное время.</returns>
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

        /// <summary>
        /// Синхронное чтение данных с устройства.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла дерева объектов в устройстве.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="waitingTime">Время ожидания чтения данных.</param>
        /// <returns>Результат чтения данных.</returns>
        public MmsValue ReadData(string name, FunctionalConstraintEnum FC, int waitingTime = 5000)
        {
            try
            {
                Console.WriteLine("readdata start");
                MmsValue resultMmsValue = new MmsValue();
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                if (!ReadDataAsync(name, FC, responseEvent, resultMmsValue))
                {
                    return null;
                }
                responseEvent.WaitOne(waitingTime);

                Console.WriteLine("readdata got event");

                return resultMmsValue;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронное чтение данных с устройства.
        /// </summary>
        /// <param name="name">Ссылка (полное имя) узла дерева объектов в устройстве.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении ответа.</param>
        /// <param name="value">Экземпляр MmsValue, в который будет записан ответ на чтение.</param>
        /// <returns>Булева переменная, указывающая, успешно ли отправлен запрос за чтение данных.</returns>
        public bool ReadDataAsync(string name, FunctionalConstraintEnum FC, AutoResetEvent responseEvent, object value)
        {
            try
            {
                Console.WriteLine("readdataasync start");
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(name, FC);
                var node = worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                worker.iecs.Controller.ReadData(node, responseEvent, value);
                Console.WriteLine("readdataasync end");
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
        /// <param name="isBuffered">Тип отчёта (true - буферизированный, false - небуферизированный).</param>
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

        /// <summary>
        /// Синхронное получение актуальных параметров отчёта.
        /// </summary>
        /// <param name="rcb">Экземпляр ReportControlBlock, соответствующий требуемому отчёту.</param>
        /// <param name="waitingTime">Время ожидания получения ответа.</param>
        /// <returns>Обновленные параметры отчёта.</returns>
        public ReportControlBlock UpdateReportControlBlock(ReportControlBlock rcb, int waitingTime = 2500)
        {
            try
            {
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                if (!UpdateReportControlBlockAsync(rcb, responseEvent))
                {
                    return null;
                }

                responseEvent.WaitOne(waitingTime);

                return rcb;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронное получение актуальных параметров отчёта.
        /// </summary>
        /// <param name="rcb">Экземпляр ReportControlBlock, соответствующий требуемому отчёту.</param>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении новых параметров отчёта.</param>
        /// <returns>Булева переменная, указывающая, успешно ли отпарвлен запрос.</returns>
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
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                WriteResponse resultResponse = new WriteResponse();
                if (!SetReportControlBlockAsync(rcbPar, responseEvent, resultResponse))
                {
                    return null;
                }

                responseEvent.WaitOne(waitingTime);

                return resultResponse;
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
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении ответа на запись.</param>
        /// <param name="response">Ответ на запись параметров.</param>
        /// <returns>Булева переменная, указывающая, успешно ли отправлен запрос на запись параметров.</returns>
        public bool SetReportControlBlockAsync(ReportControlBlock rcbPar, AutoResetEvent responseEvent, WriteResponse response)
        {
            try
            {
                worker.iecs.Controller.WriteRcb(rcbPar, true, responseEvent, response);
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }

            return true;
        }

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
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                FileDirectoryResponse response = new FileDirectoryResponse();
                if (!GetFileDirectoryAsync(name, responseEvent, response))
                {
                    return null;
                }

                responseEvent.WaitOne(waitingTime);

                return response;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронное получение директории файлов.
        /// </summary>
        /// <param name="name">Полное имя директории.</param>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении директории.</param>
        /// <param name="response">Экземпляр, в который будет записан ответ.</param>
        /// <returns>Булева переменная, указывающая, успешно ли отправлен запрос на получение директории.</returns>
        public bool GetFileDirectoryAsync(string name, AutoResetEvent responseEvent, FileDirectoryResponse response)
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

                worker.iecs.Controller.GetFileList(node, responseEvent, response);

                return true;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Синхронное получение файла.
        /// </summary>
        /// <param name="name">Полное имя файла.</param>
        /// <param name="waitingTime">Время ожидания получения файла.</param>
        /// <returns>Прочитанный файл с устройства.</returns>
        public FileBuffer GetFile(string name, int waitingTime = 5000)
        {
            try
            {
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                FileBuffer result = new FileBuffer();
                if (!GetFileAsync(name, responseEvent, result))
                {
                    return null;
                }

                responseEvent.WaitOne(waitingTime);

                return result;
            }
            catch (Exception ex)
            {
                UpdateLastExceptionInfo(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронное получение файла.
        /// </summary>
        /// <param name="name">Полное имя файла.</param>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении файла.</param>
        /// <param name="file">Экземпляр, куда будет записан файл.</param>
        /// <returns>Булева переменная, указывающая, успешно ли отправлен запрос на чтение файла.</returns>
        public bool GetFileAsync(string name, AutoResetEvent responseEvent, FileBuffer file)
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
                worker.iecs.Controller.GetFile((NodeFile)nodeFile, responseEvent, file);
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
        internal void UpdateLastExceptionInfo(Exception ex, string methodName)
        {
            lastExceptionInfo.LastException = ex;
            lastExceptionInfo.LastMethodWithException = methodName;
        }
    }
}