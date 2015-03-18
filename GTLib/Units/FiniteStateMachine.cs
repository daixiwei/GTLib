namespace com.gt.units
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    public class FiniteStateMachine
    {
        private int currentStateName;
        private object locker = new object();
        public OnStateChangeDelegate onStateChange;
        private List<FSMState> states = new List<FSMState>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statesEnumType"></param>
        public void AddAllStates(Type statesEnumType)
        {
            IEnumerator enumerator = Enum.GetValues(statesEnumType).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    this.AddState(current);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="st"></param>
        public void AddState(object st)
        {
            int newStateName = (int) st;
            FSMState item = new FSMState();
            item.SetStateName(newStateName);
            states.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tr"></param>
        public void AddStateTransition(object from, object to, object tr)
        {
            int st = (int) from;
            int outputState = (int) to;
            int transition = (int) tr;
            this.FindStateObjByName(st).AddTransition(transition, outputState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        public int ApplyTransition(object tr)
        {
            lock (locker)
            {
                int transition = (int) tr;
                int temStateName = currentStateName;
                currentStateName = FindStateObjByName(currentStateName).ApplyTransition(transition);
                if ((temStateName != currentStateName) && onStateChange != null)
                {
                    onStateChange(temStateName, currentStateName);
                }
                return currentStateName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private FSMState FindStateObjByName(object st)
        {
            int num = (int) st;
            foreach (FSMState state in this.states)
            {
                if (num.Equals(state.GetStateName()))
                {
                    return state;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentState()
        {
            lock (locker)
            {
                return currentStateName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetCurrentState(object state)
        {
            int toStateName = (int) state;
            if (onStateChange != null)
            {
                onStateChange(currentStateName, toStateName);
            }
            currentStateName = toStateName;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromStateName"></param>
        /// <param name="toStateName"></param>
        public delegate void OnStateChangeDelegate(int fromStateName, int toStateName);
    }
}

