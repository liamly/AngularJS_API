using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ServerSocketApp
{
    class ClassClientSocket : IDisposable       //declare as "public" --> error ???
    {

        private TcpClient client;
        private NetworkStream stream = null;
        public string msClientIPAddress;
        private string msClientPort;            //not used
        private DateTime mdtmConnectedTime;

        private bool mbContinueProcess = false;
        private Thread mClientThread;

        public delegate void ReceivedEventHandler(Object sender, ClassMsgSendRecv clsMsg);
        public event ReceivedEventHandler ReceivedEvent;                                        //event is raised, when a message is received from client

        public delegate void SentEventHandler(Object sender, ClassMsgSendRecv clsMsg);
        public event SentEventHandler SentEvent;

        public delegate void DisconnectedHandler(Object sender, string sIPAddress);
        public event DisconnectedHandler DisconnectedEvent;

        public ClassClientSocket(TcpClient tcpclient)
        {
            client = tcpclient;
            stream = client.GetStream();
            Console.WriteLine("ClassClientSocket, stream.ReadTimeout: " + stream.ReadTimeout);
            Console.WriteLine("ClassClientSocket, stream.WriteTimeout: " + stream.WriteTimeout);

            System.Net.IPEndPoint ipend = (System.Net.IPEndPoint)client.Client.RemoteEndPoint;

            msClientIPAddress = ipend.Address.ToString().Trim();
            msClientPort = ipend.Port.ToString();
            mdtmConnectedTime = DateTime.Now;

            Console.WriteLine("ClassClientSocket, " + msClientIPAddress + " connected");
        }

      

        public void Start()
        {
            mbContinueProcess = true;
            mClientThread = new Thread(new ThreadStart(Process));
            mClientThread.IsBackground = true;
            mClientThread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
            mClientThread.Start();
        }

        private static bool mbProcessRecvMsg = false;
        private static bool mbProcessSentMsg = false;

        private void Process()
        {   //note: while we are doing this funciton, "mClientThread.IsAlive" is true.
            //if we set "mbContinueProcess= false", or in "while" loop, we call "break"
            //then we will exit from this funciton -->  "mClientThread.IsAlive" is false

            try
            {
                while (mbContinueProcess)
                {
                    Application.DoEvents();

                    if (IsClientConnected())
                    {
                        if (!mbProcessRecvMsg)
                        {
                            mbProcessRecvMsg = true;
                            string sRecMsg = ReceiveMsg();          //receive XML messge from Socket Client
                            //if (sRecMsg.Trim() != "")
                            while (sRecMsg.Trim() != "")
                            {
                                Application.DoEvents();
                                //SaveReceivedPacket(sRecMsg);

                                if (ReceivedEvent != null)           //this should happen
                                {
                                    ClassMsgSendRecv clsMsg = new ClassMsgSendRecv();
                                    clsMsg.IPAddress = msClientIPAddress;
                                    clsMsg.MsgData = sRecMsg.Trim();  //Remove STX character at begining, EDX character at end of XML message
                                    ReceivedEvent(this, clsMsg);         //raise Receive event to client, client will catch this event, process/save data, refer to frmCAMCO_FG.Received_Msg()
                                    clsMsg = null;                       //Hao, 25/03/2013, TIL_FOS0001055
                                }
                                else                                    //this will never happen
                                {
                                    //ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.Process(), ReceivedEvent is null, cannot save received message: " + mGlobal.NEW_LINE + sRecMsg.Trim() + mGlobal.NEW_LINE, true);
                                }

                                sRecMsg = ReceiveMsg();             //read Socket again, receive any XML message
                            }
                            mbProcessRecvMsg = false;
                        }


                        if (!mbProcessSentMsg)
                        {
                            mbProcessSentMsg = true;

                            if ((stream != null) && (stream.CanWrite == true))
                            {
                                string sFileName = "";
                                string sRemark = "";
                                string sRepliedMsg = "";
                                bool bCanSend = true;

                                if (sRepliedMsg.Trim() != "")
                                {
                                    bool bSent = SendMsg(sRepliedMsg, sFileName);

                                    if (bSent)
                                    {
                                        if (SentEvent != null)             //this should happen
                                        {
                                            ClassMsgSendRecv clsMsg = new ClassMsgSendRecv();
                                            clsMsg.IPAddress = msClientIPAddress;
                                            clsMsg.MsgData = sRepliedMsg;
                                            clsMsg.Remark = sRemark;
                                            SentEvent(this, clsMsg);        //send XML messge To Socket Client. Raise Receive event to client, client will catch this event, and show message on Reply list, refer to frmCAMCO_FG.Sent_Msg()
                                            clsMsg = null;                  //Hao, 25/03/2013, TIL_FOS0001055
                                        }
                                        else                                //this will never happen
                                        {
                                            //ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.Process(), fail to show message on Reply list, SentEvent is null: " + mGlobal.NEW_LINE + sRepliedMsg.Trim() + mGlobal.NEW_LINE, true);
                                        }

                                    }
                                    else
                                    {
                                     //   ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.Process(), fail to send message: " + mGlobal.NEW_LINE + sRepliedMsg.Trim() + mGlobal.NEW_LINE, true);
                                        //MoveFileToUnSentFolder(sFileName);     //move the file from "MsgSent" folder into "UnSent\192.168.0.246", this file will be sent later
                                        bCanSend = false;
                                    }
                                }

                                if (bCanSend)
                                    if (SentEvent != null)             //this should happen
                                    {
                                        ClassMsgSendRecv clsMsg = new ClassMsgSendRecv();
                                        clsMsg.IPAddress = msClientIPAddress;
                                        clsMsg.MsgData = sRepliedMsg;
                                        SentEvent(this, clsMsg);        //send XML messge To Socket Client. Raise Receive event to client, client will catch this event, and show message on Reply list
                                                                      
                                    }

                            }
                             
                            mbProcessSentMsg = false;
                        }

                    }
                    else
                    {
                        if (DisconnectedEvent != null)
                        {
                            DisconnectedEvent(this, msClientIPAddress);         //raise Receive event to client, refer to Disconnected_FromSever
                        }

                        break;
                    }

                    Thread.Sleep(200);              //pause for 0.2 second
                }   // while (mbContinueProcess)
            }
            catch (Exception ex)
            {
                //ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.Process(), error: " + mGlobal.NEW_LINE + ex.Message.ToString() + mGlobal.NEW_LINE, true);
            }

            mbProcessRecvMsg = false;
            mbProcessSentMsg = false;
        }

        

        public void Stop()
        {
            mbContinueProcess = false;

            if (mClientThread != null && mClientThread.IsAlive)
            {
                mClientThread.Join();         //Blocks the calling thread until a thread terminates, Hao, 14/06/2011
                //mClientThread.Abort();      //Terminate thread

                mClientThread = null;                   //Hao, 25/03/2013, TIL_FOS0001055
                GC.Collect();                           //Hao, 25/03/2013, TIL_FOS0001055
                GC.WaitForPendingFinalizers();          //Hao, 25/03/2013, TIL_FOS0001055
            }
        }

        public bool Alive
        {
            get
            {
                return (mClientThread!=null && mClientThread.IsAlive);
            }
        }

        public string IPAddress
        {
            get
            {
                return msClientIPAddress;
            }
        }

        public DateTime ConnectedTime
        {
            get
            {
                return mdtmConnectedTime;
            }
        }


        

        private string ReceiveMsg()
        {
            try
            {
                int i;
                Byte[] bytes = new Byte[2048];
                String data = null;
                string sRecMsg = "";

                if (stream != null)
                {
                    if (stream.CanRead)
                    {
                        while (stream.DataAvailable)
                        {
                            if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);       // Translate data bytes to a ASCII string.

                                sRecMsg += data;
                            }
                        }
                        stream.Flush();

                        bytes = null;           //Hao, 25/03/2013, TIL_FOS0001055
                        return sRecMsg;
                    }

                }
                bytes = null;                   //Hao, 25/03/2013, TIL_FOS0001055
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("ClassClientSocket.ReceiveMsg(), error: " + ex.Message.ToString());
                return "";
            }
        }

        public bool SendMsg(string psMsg, string psFileName)
        {   //send a reply message to Client, psMsg is NOT blank
            try
            {
                if (stream != null)
                {
                    if (stream.CanWrite == true)
                    {
                        if (IsClientConnected())                                        //this should happen
                        {
                            string sMsg = psMsg.Trim();      //Hao, 08/06/2011, Add STX character to begining, EDX character to end of XML message
                            //MessageBox.Show("ClassClientSocket.SendMsg" + mGlobal.NEW_LINE + psMsg);

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(sMsg);
                            stream.Write(msg, 0, msg.Length);
                            stream.Flush();

                            msg = null;                     //Hao, 25/03/2013, TIL_FOS0001055
                            return true;
                        }
                    }
                    else
                    {
                        //ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.SendMsg(), cannot send reply message, as stream.CanWrite=false:" + mGlobal.NEW_LINE + psMsg.Trim() + mGlobal.NEW_LINE, true);
                    }
                }
                else
                {
                    //ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.SendMsg(), cannot send reply message, as stream=null:" + mGlobal.NEW_LINE + psMsg.Trim() + mGlobal.NEW_LINE, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendMsg Error: " + ex.Message.ToString());
                //ModuleMisc.LogError(mGlobal.FILE_LOG_ERROR, "ClassClientSocket.SendMsg(), SendMsg Error:" + mGlobal.NEW_LINE + ex.ToString() + mGlobal.NEW_LINE, true);
            }
            return false;
        }

      

        public bool IsClientConnected()
        {
            try
            {
                if (client == null)
                    return false;           //disconnected from Client

                //if (stream != null)
                //{
                //    if (stream.DataAvailable)
                //    {
                //        return true;            //still connected from Client
                //    }
                //}

                bool bln = client.Client.Poll(1000, SelectMode.SelectRead);
                if (bln)
                {
                    if (stream != null)
                    {
                        if (stream.DataAvailable)
                        {
                            return true;            //still connected from Client
                        }
                    }

                    return false;           //disconnected from Client
                }
                else
                {
                    return true;            //still connected from Client
                }
            }
            catch
            {
                return false;
            }
        }

       
        private bool mDispose = false;

        public void Dispose()
        {   //search on VS2010 MSDN: "IDisposable Interface"

            if (mDispose)
                return;

            mDispose = true;

            try
            {
                Stop();

                if (client != null)
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream = null;
                    }

                    client.Close();
                    client = null;
                    GC.Collect();
                }
            }
            catch (Exception)
            {
                //ignore any error
            }

            //GC.SuppressFinalize(this);
        }

        ~ClassClientSocket()                //override Finalize
        {
            Dispose();
        }
    }
}

public class clsCompareFileInfo : IComparer
{
    public int Compare(object x, object y)
    {
        FileInfo File1 = default(FileInfo);
        FileInfo File2 = default(FileInfo);

        File1 = (FileInfo)x;
        File2 = (FileInfo)y;

        return DateTime.Compare(File1.LastWriteTime, File2.LastWriteTime);
    }
}
