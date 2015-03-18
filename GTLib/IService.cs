using System;

namespace com.gt
{
    /// <summary>
    /// The service interface
    /// </summary>
    public interface IService
    {

        /// <summary>
        /// The init service method
        /// </summary>
        /// <param name="gameManager"></param>
        void Init(IGameManager gameManager);

        /// <summary>
        /// 
        /// </summary>
        void ProcessEvents();

        /// <summary>
        /// The destroy service method
        /// </summary>
        void Destroy();
    }
}
