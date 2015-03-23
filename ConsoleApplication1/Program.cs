using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.platform.windows;
using com.gt;
using com.gt.mpnet;
using com.gt.mpnet.core;
using com.gt.events;
using System.Collections;
using com.gt.entities;
using System.Threading;
using com.gt.mpnet.requests;

namespace ConsoleApplication1
{
    class Program
    {
        public const int CONNECTNUM = 4999;

        public const string EVENT_CONNECT = "conn";
        public const string EVENT_CONNECT_LOST = "lost";
        public const string EVENT_LOGIN = "login";
        public const string EVENT_LOGIN_ERROR = "le";
        public const string EVENT_TEST = "test";
        static void Main(string[] args)
        {
            TestGameManager game = new TestGameManager();
            for (int i = 1; i <= CONNECTNUM; ++i)
            {
                int temid = i;
                TestTransmitter testTransmitter = GTLib.NetManager.GetTransmitter<TestTransmitter>("test"+i);
                testTransmitter.ConnectTest("192.168.0.235", 9934);

                GTLib.NetManager.AddEventListener(EVENT_CONNECT + i, delegate(BaseEvent evt)
                {
                    GTLib.GameManager.Log.Info("连接成功！");
                    testTransmitter.LoginTest("ggg" + temid, "ggg");
                });
                GTLib.NetManager.AddEventListener(EVENT_CONNECT_LOST + i, delegate(BaseEvent evt)
                {
                    GTLib.GameManager.Log.Info("连接失败！");
                });
                GTLib.NetManager.AddEventListener(EVENT_LOGIN + i, delegate(BaseEvent evt)
                {
                    GTLib.GameManager.Log.Info("登陆成功！");
                    IMPObject data = new MPObject();
                    data.PutUtfString("s", "hello server");
                    testTransmitter.ExtensionTest(data);
                });
                GTLib.NetManager.AddEventListener(EVENT_LOGIN_ERROR + i, delegate(BaseEvent evt)
                {
                    GTLib.GameManager.Log.Info("登陆失败！");
                });
                GTLib.NetManager.AddEventListener(EVENT_TEST + i, delegate(BaseEvent evt)
                {
                    GTLib.GameManager.Log.Info("test:" + evt.Params["c"] + "temid:" + temid);
                    //GTLib.NetManager.KillConnection(temid);
                });
                Thread.Sleep(10);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TestGameManager : WindowsGameManager
    {
        protected override void Init()
        {
            for (int i = 1; i <= Program.CONNECTNUM; ++i)
            {
                GTLib.NetManager.AddTransmitter("test"+i, new TestTransmitter(i));

                GTLib.NetManager.AddMessageHandler("test" + i, new TestMessageHandler(i));
            }
        }

    }

    
    /// <summary>
    /// 
    /// </summary>
    public class TestTransmitter : MessageTransmitter
    {
        /// <summary>
        /// 
        /// </summary>
        public TestTransmitter(int id)
            : base(id)
        {

        }
        public void ConnectTest(string ip, int port)
        {
            MPNetClient mpnet = new MPNetClient(m_PrefabConnecterId);
            mpnet.Connect(ip, port);
            
            mpnet.AddEventListener(MPEvent.CONNECTION, delegate(BaseEvent evt)
            {
                Hashtable par = evt.Params;
                if ((bool)par["success"])
                {
                    DispatchEvent(new MPEvent(Program.EVENT_CONNECT + m_PrefabConnecterId));
                }
                else
                {
                    DispatchEvent(new MPEvent(Program.EVENT_CONNECT_LOST + m_PrefabConnecterId));
                }
            });
            mpnet.AddEventListener(MPEvent.CONNECTION_LOST, delegate(BaseEvent evt)
            {
                GTLib.NetManager.KillConnection(m_PrefabConnecterId);
                DispatchEvent(new MPEvent(Program.EVENT_CONNECT_LOST + m_PrefabConnecterId));
            });
            
            GTLib.NetManager.AddMPNetClient(mpnet);
        }

        public void LoginTest(string userName, string passwd)
        {
            Send(new LoginRequest(userName, passwd));
            mpnet.AddEventListener(MPEvent.LOGIN, delegate(BaseEvent evt)
            {
                mpnet.InitUDP(mpnet.CurrentIp, mpnet.CurrentPort);
                DispatchEvent(new MPEvent(Program.EVENT_LOGIN + m_PrefabConnecterId));
            });
            mpnet.AddEventListener(MPEvent.LOGIN_ERROR, delegate(BaseEvent evt)
            {
                DispatchEvent(new MPEvent(Program.EVENT_LOGIN_ERROR + m_PrefabConnecterId));
            });
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void ExtensionTest(IMPObject parameters)
        {
            SendExtensionRequest("test", parameters,true);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class TestMessageHandler : MessageHandler
    {
        public TestMessageHandler(int id)
            : base(id)
        {

        }

        private void Test(IMPObject parameters)
        {
            Hashtable rdata = new Hashtable();
            rdata["c"] = parameters.GetUtfString("c");
            DispatchEvent(new MPEvent(Program.EVENT_TEST + m_PrefabConnecterId, rdata));
        }

        protected override void Init()
        {
            RegisterMessageHandler("test", Test);
        }
    }


}
