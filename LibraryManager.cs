using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    /// <summary>
    /// Основной пользовательский класс для работы с библиотекой.
    /// </summary>
    public class LibraryManager
    {
        /// <summary>
        /// Класс предоставляет информацию о последнем сработавшем исключении и методе, который в котором оно произошло.
        /// </summary>
        public class LastExceptionInfo
        {
            public Exception LastException { get; set; }
            public string LastMethodWithException { get; set; }
        }

        protected Scsm_MMS_Worker worker;
        public Dictionary<string, NodeBase> addressNodesPairs;
        protected LastExceptionInfo lastExceptionInfo;

        public delegate void connectionClosedEventHandler();
        public event connectionClosedEventHandler ConnectionClosed;
        public delegate void newReportReceivedEventhandler(string rptdVarQualityLog, string rptdVarTimestampLog, string rptdVarPathLogstring, string rptdVarDescriptionLog, string rptdVarValueLog);
        public event newReportReceivedEventhandler NewReportReceived;

        public LibraryManager()
        {
            worker = new Scsm_MMS_Worker();
            worker.ConnectShutDownedEvent += Worker_ConnectShutDownedEvent;
            worker.iecs.Controller.NewReportReceived += Controller_NewReportReceived;
        }

        /// <summary>
        /// Установка соединения с утройством.
        /// </summary>
        /// <param name="hostName">IP адрес устройства.</param>
        /// <param name="port">Номер порта.</param>
        /// <returns>Успешно ли создалось соединение.</returns>
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
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        /// <param name="reRead">True - прочитать данные в узле сразу после записи; False - иначе.</param>
        public bool WriteData(NodeData node, bool reRead)
        {
            try
            {
                worker.iecs.Controller.WriteData(node, reRead);
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
        public void ReadData(NodeBase node)
        {
            worker.iecs.Controller.ReadData(node);
        }

        /// <summary>
        /// Установка и запись параметров отчёта.
        /// </summary>
        /// <param name="rcbPar">Параметры отчёта.</param>
        /// <param name="reRead">True - прочитать данные в узле сразу после записи; False - иначе.</param>
        public void WriteRcb(RcbActivateParams rcbPar, bool reRead)
        {
            worker.iecs.Controller.WriteRcb(rcbPar, reRead);
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
        public void GetFileList(NodeBase node)
        {
            worker.iecs.Controller.GetFileList(node);
        }

        /// <summary>
        /// Получение файла.
        /// </summary>
        /// <param name="node">Узел программного дерева, соответствующий узлу в дереве объектов устройства.</param>
        public void GetFile(NodeFile node)
        {
            worker.iecs.Controller.GetFile(node);
        }

        /// <summary>
        /// Получение информации о последнем исключении.
        /// </summary>
        /// <returns>Объект, содержащий информацию о последнем исключении и методе, который его бросил.</returns>
        public LastExceptionInfo GetLastExceptionInfo()
        {
            return lastExceptionInfo;
        }

        protected void UpdateLastExceptionInfo(Exception ex, string methodName)
        {
            lastExceptionInfo.LastException = ex;
            lastExceptionInfo.LastMethodWithException = methodName;
        }

        protected void Worker_ConnectShutDownedEvent()
        {
            ConnectionClosed?.Invoke();
        }

        protected void Controller_NewReportReceived(string rptdVarQualityLog, string rptdVarTimestampLog, string rptdVarPathLogstring, string rptdVarDescriptionLog, string rptdVarValueLog)
        {
            NewReportReceived?.Invoke(null, null, null, null, null);
        }
    }
}