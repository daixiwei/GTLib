namespace com.gt.events
{
    using System;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    public class EventDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        private Hashtable listeners = new Hashtable();
        /// <summary>
        /// 
        /// </summary>
        private object target;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public EventDispatcher(object target)
        {
            this.target = target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public void AddEventListener(string eventType, EventListenerDelegate listener)
        {
            EventListenerDelegate a = listeners[eventType] as EventListenerDelegate;
            a = (EventListenerDelegate) Delegate.Combine(a, listener);
            listeners[eventType] = a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public void DispatchEvent(BaseEvent evt)
        {
            EventListenerDelegate delegate2 = listeners[evt.Type] as EventListenerDelegate;
            if (delegate2 != null)
            {
                evt.Target = this.target;
                try
                {
                    delegate2(evt);
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Concat(" Error dispatching event ", evt.Type, ": ", exception.Message, " ", exception.StackTrace), exception);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveAll()
        {
            listeners.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public void RemoveEventListener(string eventType, EventListenerDelegate listener)
        {
            EventListenerDelegate source = listeners[eventType] as EventListenerDelegate;
            if (source != null)
            {
                source = (EventListenerDelegate) Delegate.Remove(source, listener);
            }
            listeners[eventType] = source;
        }
    }
}

