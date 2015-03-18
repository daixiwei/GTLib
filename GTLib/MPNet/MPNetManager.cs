using System;
using System.Collections.Generic;
using com.gt.mpnet;
using com.gt.mpnet.core;
using com.gt.mpnet.requests;
using System.Collections;
using com.gt.units;
using System.Timers;
using com.gt.mpnet.bitswarm;
using System.Security.Cryptography;
using com.gt.events;
using com.gt;
using com.gt.entities;

namespace com.gt.mpnet
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    public delegate void ExtensionMessageDelegate(IMPObject parameters);

    /// <summary>
    /// The net manager class
    /// </summary>
    public class MPNetManager : INetManager, IDispatchable
    {
        /// <summary>
        /// 
        /// </summary>
        private EventDispatcher m_Dispatcher;
        /// <summary>
        /// 
        /// </summary>
        private bool isInit;
        /// <summary>
        /// 
        /// </summary>
        private IGameManager gameManager;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, MPNetClient> connecters = new Dictionary<int, MPNetClient>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, MessageTransmitter> transmitters = new Dictionary<string, MessageTransmitter>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, MessageHandler> handlers = new Dictionary<string, MessageHandler>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, Hashtable> extensionMessages = new Dictionary<int, Hashtable>();
        /// <summary>
        /// 
        /// </summary>
        private Logger m_Log;
        /// <summary>
        /// 
        /// </summary>
        private bool m_CacheQueueProcess = true;
        /// <summary>
        /// 
        /// </summary>
        private object eventsLocker = new object();
        /// <summary>
        /// 
        /// </summary>
        private object cachesLocker = new object();
        /// <summary>
        /// 
        /// </summary>
        private Queue<Runnable> m_CacheQueue = new Queue<Runnable>();
        /// <summary>
        /// 
        /// </summary>
        private Queue<BaseEvent> m_EventsQueue = new Queue<BaseEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameManager"></param>
        public void Init(IGameManager gameManager)
        {
            if (isInit) return;
            this.gameManager = gameManager;
            m_Log = new Logger(typeof(MPNetManager));
            m_Dispatcher = new EventDispatcher(this);
            isInit = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        public void AddMPNetClient(MPNetClient mpnet)
        {
            if (mpnet == null)
            {
                throw new ArgumentException("Controller is null, it can't be added.");
            }
            if (connecters.ContainsKey(mpnet.Id))
            {
                throw new ArgumentException(string.Concat("A controller with id: ", mpnet.Id, " already exists! Controller can't be added: ", mpnet));
            }
            lock (connecters)
            {
                connecters.Add(mpnet.Id, mpnet);
            }
            
            ICollection<MessageTransmitter> tms = transmitters.Values;
            foreach (MessageTransmitter tm in tms)
            {

                if (tm.PrefabConnecterId == mpnet.Id)
                {
                    tm.SetMPNetClient(mpnet);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="transmitter"></param>
        public void AddTransmitter(string key, MessageTransmitter transmitter)
        {
            if (transmitter == null)
            {
                throw new ArgumentException("Transmitter is null, it can't be added.");
            }
            if (transmitters.ContainsKey(key))
            {
                throw new ArgumentException(string.Concat("A transmitter with type: ", transmitter.GetType(), " already exists! Transmitter can't be added: ", transmitter));
            }
            transmitters.Add(key, transmitter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddMessageHandler(string key, MessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentException("message handler is null, it can't be added.");
            }
            if (handlers.ContainsKey(key))
            {
                throw new ArgumentException(string.Concat("A message handler with type: ", handler.GetType(), " already exists! message handler can't be added: ", handler));
            }
            handlers.Add(key, handler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd"></param>
        /// <param name="messageHandler"></param>
        internal void AddMessageHandler(int id, string cmd, ExtensionMessageDelegate messageHandler)
        {
            if (!extensionMessages.ContainsKey(id))
            {
                extensionMessages.Add(id, new Hashtable());
            }
            Hashtable messages = extensionMessages[id];
            ExtensionMessageDelegate a = messages[cmd] as ExtensionMessageDelegate;
            a = (ExtensionMessageDelegate)Delegate.Combine(a, messageHandler);
            messages[cmd] = a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        internal void RemoveAllMessageHandlers(int id)
        {
            if (extensionMessages.ContainsKey(id))
            {
                extensionMessages.Remove(id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd"></param>
        /// <param name="messageHandler"></param>
        internal void RemoveMessageHandler(int id, string cmd, ExtensionMessageDelegate messageHandler)
        {
            if (!extensionMessages.ContainsKey(id))
            {
                return;
            }
            Hashtable messages = extensionMessages[id];
            ExtensionMessageDelegate source = messages[cmd] as ExtensionMessageDelegate;
            if (source != null)
            {
                source = (ExtensionMessageDelegate)Delegate.Remove(source, messageHandler);
            }
            messages[cmd] = source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        internal void HandleExtension(MPNetClient mpnet, string cmd, IMPObject parameters)
        {
            if (!extensionMessages.ContainsKey(mpnet.Id))
            {
                return;
            }
            Hashtable messages = extensionMessages[mpnet.Id];
            ExtensionMessageDelegate delegate2 = messages[cmd] as ExtensionMessageDelegate;
            if (delegate2 != null)
            {
                try
                {
                    delegate2(parameters);
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Concat("Error extension handler [", cmd, "]: ", exception.Message, " ", exception.StackTrace), exception);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetTransmitter<T>(string key) where T : MessageTransmitter
        {
            return (T)transmitters[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetMessageHandler<T>(string key) where T : MessageHandler
        {
            return (T)handlers[key];
        }

        /// <summary>
        /// The process events
        /// </summary>
        public void ProcessEvents()
        {
            if (!isInit)
            {
                throw new Exception("not init net manager!");
            }
            if (CacheQueueProcess && m_CacheQueue.Count > 0)
            {
                Runnable[] runnableArray;
                lock (cachesLocker)
                {
                    runnableArray = m_CacheQueue.ToArray();
                    m_CacheQueue.Clear();
                }
                foreach (Runnable runnable in runnableArray)
                {
                    runnable();
                }
            }
            if (m_EventsQueue.Count > 0)
            {
                BaseEvent[] eventArray;
                lock (eventsLocker)
                {
                    eventArray = m_EventsQueue.ToArray();
                    m_EventsQueue.Clear();
                }
                foreach (BaseEvent event2 in eventArray)
                {
                    Dispatcher.DispatchEvent(event2);
                }
            }

            if (connecters.Count > 0)
            {
                MPNetClient[] tem = null;
                lock (connecters)
                {
                    tem = new MPNetClient[connecters.Count];
                    connecters.Values.CopyTo(tem, 0);
                }
                for (int i = 0; i < tem.Length; ++i)
                {
                    MPNetClient nc = tem[i];
                    nc.ProcessEvents();
                }
            }

        }

        /// <summary>
        /// The add event listener
        /// </summary>
        /// <param name="eventType">The event type</param>
        /// <param name="listener">The listener delegate</param>
        public void AddEventListener(string eventType, EventListenerDelegate listener)
        {
            if (!isInit)
            {
                throw new Exception("not init net manager!");
            }
            Dispatcher.AddEventListener(eventType, listener);
        }

        /// <summary>
        /// The remove all listeners
        /// </summary>
        public void RemoveAllEventListeners()
        {
            if (!isInit)
            {
                throw new Exception("not init net manager!");
            }
            Dispatcher.RemoveAll();
        }

        /// <summary>
        /// The remove listener delegate where event type
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public void RemoveEventListener(string eventType, EventListenerDelegate listener)
        {
            if (!isInit)
            {
                throw new Exception("not init net manager!");
            }
            Dispatcher.RemoveEventListener(eventType, listener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public void DispatchEvent(BaseEvent evt)
        {
            if (!isInit)
            {
                throw new Exception("not init net manager!");
            }
            lock (eventsLocker)
            {
                m_EventsQueue.Enqueue(evt);
            }
        }

        /// <summary>
        /// 断开指定连接
        /// </summary>
        /// <param name="id"></param>
        public void KillConnection(int id)
        {
            MPNetClient nc = connecters[id];
            if (nc != null)
            {
                nc.RemoveAllEventListeners();
                RemoveAllMessageHandlers(id);
                nc.Disconnect();
                lock (connecters)
                {
                    connecters.Remove(id);
                }
                ICollection<MessageTransmitter> tms = transmitters.Values;
                foreach (MessageTransmitter tm in tms)
                {

                    if (tm.PrefabConnecterId == id)
                    {
                        tm.SetMPNetClient(null);
                    }
                }
            }
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        public void KillAllConnection()
        {
            if (!isInit)
            {
                throw new Exception("not init net manager!");
            }
            foreach (MPNetClient nc in connecters.Values)
            {
                nc.RemoveAllEventListeners();
                extensionMessages.Clear();
                nc.KillConnection();
            }
            lock (connecters)
            {
                connecters.Clear();
            }

            ICollection<MessageTransmitter> tms = transmitters.Values;
            foreach (MessageTransmitter tm in tms)
            {
                tm.SetMPNetClient(null);
            }
        }

        /// <summary>
        /// Adds the cache task.
        /// </summary>
        internal virtual void PostRunnable(Runnable runnable)
        {
            lock (cachesLocker)
            {
                m_CacheQueue.Enqueue(runnable);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            KillAllConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        public IGameManager GameManager
        {
            get
            {
                return gameManager;
            }
        }

        /// <summary>
        /// The log
        /// </summary>
        public Logger Log
        {
            get
            {
                return m_Log;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EventDispatcher Dispatcher
        {
            get
            {
                return m_Dispatcher;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CacheQueueProcess
        {
            get
            {
                return m_CacheQueueProcess;
            }
            set
            {
                m_CacheQueueProcess = value;
            }
        }
    }
}
