using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerSocketApp
{
    public partial class Form1 : Form
    {
        private static bool mbStartListening = false;
        private TcpListener server = null;
        private TcpClient client = null;
        private NetworkStream stream = null;
        private ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private bool mbListenAndConnectToClient = false;
        private static Thread mThreadReClaim;
        private static ArrayList marrlstClientSocket = new ArrayList();
        private ArrayList marrlstCreateUnSentFolder = new ArrayList();

        public Form1()
        {
            InitializeComponent();
        }

        private void timerStartListening_Tick(object sender, EventArgs e)
        {
            //if (mbStartListening)
            //    return;

            //mbStartListening = true;
            if (server == null)
            {
                mThreadReClaim = new Thread(new ThreadStart(Reclaim));
                mThreadReClaim.IsBackground = true;
                mThreadReClaim.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;      
                mThreadReClaim.Start();

                try
                {
                    IPAddress localAddr = IPAddress.Parse("192.168.2.20");
                    server = new TcpListener(localAddr, 65000);
                    server.Start();
                }
                catch (Exception ex)
                {
                    Application.Exit();
                    return;
                }
            }

            
            Console.WriteLine("tmrStartListening_Tick: Waiting for a connection... " + ", at: " + DateTime.Now.ToString());

            tcpClientConnected.Reset();             // Set the event to nonsignaled state.
            server.BeginAcceptTcpClient(new AsyncCallback(StartListening), server);
            //tcpClientConnected.WaitOne(); // Signal the calling thread to continue.

        }

        private void StartListening(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;      // Get the listener that handles the client request.
                client = listener.EndAcceptTcpClient(ar);               // End the operation and display the received data on the console.

                if (client != null)
                {
                    Console.WriteLine("Client accepted!");

                    lock (marrlstClientSocket.SyncRoot)                      // An incoming connection needs to be processed.
                    {
                        ClassClientSocket clsClientSocket = new ClassClientSocket(client);
                        clsClientSocket.ReceivedEvent += new ClassClientSocket.ReceivedEventHandler(Received_Msg);
                        clsClientSocket.SentEvent += new ClassClientSocket.SentEventHandler(Sent_Msg);
                        clsClientSocket.DisconnectedEvent += new ClassClientSocket.DisconnectedHandler(Disconnected_FromSever);

                        int i = marrlstClientSocket.Add(clsClientSocket);
                        clsClientSocket.Start();
                    }


                    lock (marrlstCreateUnSentFolder.SyncRoot)
                    {
                        System.Net.IPEndPoint ipend = (System.Net.IPEndPoint)client.Client.RemoteEndPoint;
                        string sIPAddress = ipend.Address.ToString().Trim();    //Client IP Address, eg. 192.168.0.246"
                        marrlstCreateUnSentFolder.Add(sIPAddress);              //we need to create "\\UnSent\\192.168.0.246"
                    }
  
                }

               }
            catch (Exception ex)
            {
                Console.WriteLine("StartListening Error: " + ex.Message + ", at: " + DateTime.Now.ToString());
            }

            Console.WriteLine("StartListening: Client connected completed" + ", at: " + DateTime.Now.ToString());        // Process the connection here. (Add the client to a server table, read data, etc.)
            tcpClientConnected.Set();                               // Signal the calling thread to continue.

            //mbStartListening = false;
            //==============================================
        }

        private static void Reclaim()
        {
            lock (marrlstClientSocket.SyncRoot)
            {
                for (int i = marrlstClientSocket.Count - 1; i >= 0; i--)
                {
                    Object Client = marrlstClientSocket[i];
                    if (!((ClassClientSocket)Client).Alive)
                    {
                        ClassClientSocket clsClient = (ClassClientSocket)marrlstClientSocket[i];
                        Console.WriteLine("A client left: " + clsClient.IPAddress);

                        clsClient.Dispose();
                        marrlstClientSocket.Remove(Client);
                    }

                }
            }
            Application.DoEvents();
            Thread.Sleep(200);

        }

        private void Received_Msg(object src, ClassMsgSendRecv clsMsg)
        {   //this event is triggered, when a message is received

            Console.WriteLine("Received_Msg, from IP Address: " + clsMsg.IPAddress);
            //Console.WriteLine("Received_Msg, data: " + clsMsg.MsgData);
            //MessageBox.Show("Received_Msg" + mGlobal.NEW_LINE + clsMsg.MsgData);

            if (clsMsg.MsgData.Trim() != "")
            {
                ClassClientSocket clsClientSocket = (ClassClientSocket)src;
                MessageBox.Show(clsMsg.MsgData);
                //SaveReceivedPacket(clsMsg.MsgData, clsClientSocket);
            }
        }

        private void Sent_Msg(object src, ClassMsgSendRecv clsMsg)
        {   //this event is triggered, when a message has been sent out

            Console.WriteLine("Sent_Msg, from IP Address: " + clsMsg.IPAddress);
            //Console.WriteLine("Sent_Msg, data: " + clsMsg.MsgData);

            if (clsMsg.MsgData.Trim() != "")
            {
                ClassClientSocket clsClientSocket = (ClassClientSocket)src;
                //SendReplyMsg(clsMsg.MsgData, clsClientSocket);
                //AddSentMsgToList(clsMsg.MsgData, clsMsg.Remark, clsClientSocket);          //Hao, 21/07/2011
            }
        }

        private void Disconnected_FromSever(object src, string sIPAddress)
        {   //this event is triggered, when a Client Socket has been disconnected

            Console.WriteLine("Disconnected_FromSever, from IP Address: " + sIPAddress);
        }
    }
}
