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
    /// Класс программного представления объекта управления.
    /// </summary>
    public class ControlObject
    {
        internal NodeDO self;
        internal LibraryManager libraryManager;
        internal CommandParams commandParams;
        internal string mmsReference;

        public string Originator
        {
            get
            {
                return commandParams.orIdent;
            }
            set
            {
                commandParams.orIdent = value;
            }
        }

        public OriginatorCategoryEnum OrCat
        {
            get
            {
                return commandParams.orCat;
            }
            set
            {
                commandParams.orCat = value;
            }
        }

        public bool SynchroCheck
        {
            get
            {
                return commandParams.synchroCheck;
            }
            set
            {
                commandParams.synchroCheck = value;
            }
        }
        
        public bool InterlockCheck
        {
            get
            {
                return commandParams.interlockCheck;
            }
            set
            {
                commandParams.interlockCheck = value;
            }
        }

        public bool Test
        {
            get
            {
                return commandParams.Test;
            }
            set
            {
                commandParams.Test = value;
            }
        }

        public ControlModelEnum ControlModel { get; internal set; }
        public string ObjectReference { get; internal set; }
        public AutoResetEvent CommandTerminationEvent 
        {
            internal get
            {
                return libraryManager.worker.iecs.mms.newCommandTerminationEvent;
            }
            set
            {
                libraryManager.worker.iecs.mms.newCommandTerminationEvent = value;
            }
        }
        public ConcurrentQueue<CommandTerminationReport> QueueOfComTerminationReports { get; internal set; }

        /// <summary>
        /// Обязательный конструктор объекта управления.
        /// </summary>
        /// <param name="objectReference">Ссылка (полное имя) узла управляемого объекта.</param>
        /// <param name="FC">Функциональная связь.</param>
        /// <param name="manager">Текущий экземпляр LibraryManager, в котором установлено соединение.</param>
        /// <param name="isControlObjectCorrect">Корректно ли создан объект управления.</param>
        public ControlObject(string objectReference, FunctionalConstraintEnum FC, LibraryManager manager, ref bool isControlObjectCorrect)
        {
            try
            {
                libraryManager = manager;
                string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(objectReference, FC);
                this.mmsReference = mmsReference;
                this.ObjectReference = objectReference;
                var node = manager.worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
                self = (NodeDO)node;
                //manager.worker.iecs.DataModel.addressNodesPairs.TryGetValue(objectReference, out var outNode);
                //self = (NodeDO)outNode;
                commandParams = libraryManager.worker.iecs.Controller.PrepareSendCommand(node.FindChildNode("Oper").FindChildNode("ctlVal"));
                ControlModel = commandParams.CommandFlowFlag;
                isControlObjectCorrect = true;
                manager.worker.iecs.mms.listOfControlObjects.Add(this);     
            }
            catch (Exception ex)
            {
                manager.UpdateLastExceptionInfo(new Exception("Ошибка при создании управляемого объекта: " + ex.Message), MethodBase.GetCurrentMethod().Name);
                isControlObjectCorrect = false;
            }
        }

        /// <summary>
        /// Синхронная отправка операции захвата Select.
        /// </summary>
        /// <param name="waitingTime">Время ожидания выполнения операции.</param>
        /// <returns>Состояние выполнения операции Select.</returns>
        public SelectResponse Select(int waitingTime = 1000)
        {
            try
            {
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                SelectResponse response = new SelectResponse();
                if (!SelectAsync(responseEvent, response))
                {
                    return null;
                }

                if (!responseEvent.WaitOne(waitingTime))
                {
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                libraryManager.UpdateLastExceptionInfo(new Exception("Ошибка отправки SELECT в " + ObjectReference + ": " + ex.Message), MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронная отправка операции Select.
        /// </summary>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении ответа на операцию.</param>
        /// <param name="selectResponse">Экземпляр SelectResponse, куда будет записан ответ.</param>
        /// <returns>Булева переменная, указывающая, успешно ли произошла отправка операции на устройство.</returns>
        public bool SelectAsync(AutoResetEvent responseEvent, SelectResponse selectResponse)
        {
            try
            {
                libraryManager.worker.iecs.Controller.ReadData(self.FindChildNode("SBO"), responseEvent, selectResponse);
                return true;
            }
            catch (Exception ex)
            {
                libraryManager.UpdateLastExceptionInfo(new Exception("Ошибка отправки SELECT в " + ObjectReference + ": " + ex.Message), MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Синхронная отправка операции SelectWithValue.
        /// </summary>
        /// <param name="ctlVal">Записываемое значение.</param>
        /// <param name="waitingTime">Время ожидания ответа на выполнение операции.</param>
        /// <returns>Ответ на запрос записи.</returns>
        public WriteResponse SelectWithValue(object ctlVal, int waitingTime = 1500)
        {
            try
            {
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                WriteResponse response = new WriteResponse();
                if (!SelectWithValueAsync(ctlVal, responseEvent, response))
                {
                    return null;
                }

                if (!responseEvent.WaitOne(waitingTime))
                {
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                libraryManager.UpdateLastExceptionInfo(new Exception("Ошибка отправки SELECTWITHVAL в " + ObjectReference + ": " + ex.Message),
                    MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронная отправка операции SelectWithValue.
        /// </summary>
        /// <param name="ctlVal">Записываемое значение.</param>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состоянии при получении ответа.</param>
        /// <param name="response">Экземпляр WhiteResponse, куда будет записан ответ.</param>
        /// <returns>Булева переменная, указывающая, успешно ли произошла отправка операции.</returns>
        public bool SelectWithValueAsync(object ctlVal, AutoResetEvent responseEvent, WriteResponse response)
        {
            try
            {
                var sendNode = (self.FindChildNode("SBOw").FindChildNode("ctlVal") as NodeData);
                commandParams.ctlVal = ctlVal;
                libraryManager.worker.iecs.Controller.SendCommandToIed(sendNode, commandParams, ActionRequested.WriteAsStructure, responseEvent, response);
                return true;
            }
            catch (Exception ex)
            {
                libraryManager.UpdateLastExceptionInfo(new Exception("Ошибка отправки SELECTWITHVAL в " + ObjectReference + ": " + ex.Message), 
                    MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Синхронное выполнение команды.
        /// </summary>
        /// <param name="ctlVal">Записываемое значение.</param>
        /// <param name="waitingTime">Время ожидания получения ответа.</param>
        /// <returns>Ответ на выполнение команды.</returns>
        public WriteResponse Operate(object ctlVal, int waitingTime = 2000)
        {
            try
            {
                AutoResetEvent responseEvent = new AutoResetEvent(false);
                WriteResponse response = new WriteResponse();
                if (!OperateAsync(ctlVal, responseEvent, response))
                {
                    return null;
                }

                if (!responseEvent.WaitOne(waitingTime))
                {
                    return null;
                }

                return response;
            }
            catch (Exception ex)
            {
                libraryManager.UpdateLastExceptionInfo(new Exception("Ошибка отправки OPERATE в " + ObjectReference + ": " + ex.Message),
                    MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// Асинхронное выполнение команды.
        /// </summary>
        /// <param name="ctlVal">Записываемое значение.</param>
        /// <param name="responseEvent">Пользовательское событие, которое перейдёт в сигнальное состояние при получении ответа.</param>
        /// <param name="response">Экземпляр WhiteResponse, куда будет записан ответ.</param>
        /// <returns>Булева переменная, указывающая, успешно ли произошла отправка команды.</returns>
        public bool OperateAsync(object ctlVal, AutoResetEvent responseEvent, WriteResponse response)
        {
            try
            {
                var sendNode = (self.FindChildNode("Oper").FindChildNode("ctlVal") as NodeData);
                commandParams.ctlVal = ctlVal;
                libraryManager.worker.iecs.Controller.SendCommandToIed(sendNode, commandParams, ActionRequested.WriteAsStructure, responseEvent, response);
                return true;
            }
            catch (Exception ex)
            {
                libraryManager.UpdateLastExceptionInfo(new Exception("Ошибка отправки OPERATE в " + ObjectReference + ": " + ex.Message),
                    MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
    }
}
