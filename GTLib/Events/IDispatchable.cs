namespace com.gt.events
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IDispatchable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        void AddEventListener(string eventType, EventListenerDelegate listener);

        /// <summary>
        /// 
        /// </summary>
        EventDispatcher Dispatcher { get; }
    }
}

