namespace com.gt.units
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class FSMState
    {
        /// <summary>
        /// 
        /// </summary>
        private int stateName;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, int> transitions = new Dictionary<int, int>();

        /// <summary>
        ///  The add transiton method
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="outputState"></param>
        public void AddTransition(int transition, int outputState)
        {
            this.transitions[transition] = outputState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public int ApplyTransition(int transition)
        {
            int stateName = this.stateName;
            if (this.transitions.ContainsKey(transition))
            {
                stateName = this.transitions[transition];
            }
            return stateName;
        }

        /// <summary>
        /// The Get State Name Method
        /// </summary>
        /// <returns>Return curren state</returns>
        public int GetStateName()
        {
            return stateName;
        }

        /// <summary>
        /// The Set State Name Method
        /// </summary>
        /// <param name="newStateName">New state name</param>
        public void SetStateName(int newStateName)
        {
            this.stateName = newStateName;
        }
    }
}

