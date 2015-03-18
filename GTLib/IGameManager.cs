using com.gt.assets;
using com.gt.events;
using com.gt.units;
using System.Collections;
using System.Collections.Generic;

namespace com.gt
{
    /// <summary>
    /// Task.
    /// </summary>
    public delegate void Runnable();

    /// <summary>
    /// The game manager interface
    /// </summary>
    public interface IGameManager
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>() where T : IService;

        /// <summary>
        /// Post the cache task.
        /// </summary>
        void PostRunnable(Runnable runnable);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="typeName"></param>
        /// <param name="message"></param>
        void Print(LogLevel level, string typeName, string message);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="needs"></param>
        void StartGTCoroutine(IEnumerator routine);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="endCallBack"></param>
        void StartGTCoroutine(IEnumerator routine, Runnable endCallBack);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        void StopGTCoroutine(IEnumerator routine);

        /// <summary>
        /// 
        /// </summary>
        void StopAllGTCoroutine();

        /// <summary>
        /// The logger
        /// </summary>
        Logger Log { get; }

        /// <summary>
        /// The delta time
        /// </summary>
        float DeltaTime { get; }
    }
}
