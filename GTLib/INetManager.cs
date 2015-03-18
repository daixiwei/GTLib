using com.gt.events;
using com.gt.mpnet;
using System;

namespace com.gt
{

    /// <summary>
    /// The net manager interface
    /// </summary>
    public interface INetManager : IService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetTransmitter<T>(string key) where T : MessageTransmitter;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetMessageHandler<T>(string key) where T : MessageHandler;

        /// <summary>
        /// The add event listener
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        void AddEventListener(string eventType, EventListenerDelegate listener);

        /// <summary>
        /// The remove listener delegate where event type
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        void RemoveEventListener(string eventType, EventListenerDelegate listener);

        /// <summary>
        /// The remove all listeners
        /// </summary>
        void RemoveAllEventListeners();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        void AddMPNetClient(MPNetClient mpnet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="transmitter"></param>
        void AddTransmitter(string key, MessageTransmitter transmitter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        void AddMessageHandler(string key, MessageHandler handler);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        void DispatchEvent(BaseEvent evt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void KillConnection(int id);

        /// <summary>
        /// Kill all connection
        /// </summary>
        void KillAllConnection();

        /// <summary>
        /// 
        /// </summary>
        bool CacheQueueProcess { get; set; }
    }
}
