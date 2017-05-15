using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ChatClient
{

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string NickName { get; set; }

        public MessageEventArgs(string nickName, string recieveMessage)
        {
            NickName = nickName;
            Message = recieveMessage;
        }
    }


    public class ParticipantEventArgs : EventArgs
    {
        public string NickNameOfParticiapant { get; set; }

        public ParticipantEventArgs(string nickNameOfParticiapant)
        {
            NickNameOfParticiapant = nickNameOfParticiapant;
        }
    }

    class Client
    {
        public static string UserName;
        //public static string Host = "127.0.0.1";
        //public static int Port = 8888;
        public static string Host;
        public static int Port;
        public static TcpClient ClientTcp;
        public static NetworkStream Stream;
        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public delegate void ParticipantEventHandler(object sender, ParticipantEventArgs e);
        public static event MessageEventHandler MessageReceived;
        public static event ParticipantEventHandler ParticipantConnected;
        public static event ParticipantEventHandler ParticipantDisconnected;

        public static void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length);

        }

        protected virtual void OnMessageReceive(MessageEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        protected virtual void OnParticipantConnected(ParticipantEventArgs e)
        {
            if (MessageReceived != null)
                ParticipantConnected(this, e);
        }

        protected virtual void OnParticipantDisonnected(ParticipantEventArgs e)
        {
            if (MessageReceived != null)
                ParticipantDisconnected(this, e);
        }

        public static void ReceiveMessage()
        {
            var client = new Client();
            //

            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = Stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);

                    string nickName = (builder.ToString()).Split(' ')[0];
                    string message = builder.ToString().Replace(nickName, "");

                    if(message == " ~Connect")
                    {
                        client.OnParticipantConnected(new ParticipantEventArgs(nickName.Split(':')[0]));
                    }
                    else if (message == " ~Disconnect")
                    {
                        Disconnect();
                    }
                    else
                    client.OnMessageReceive(new MessageEventArgs(nickName, message));
            }
                catch
            {
                //MessageBox.Show("Connect is lost!");
                Disconnect();
            }
        }
        }


        public static void Disconnect()
        {
            SendMessage(UserName + " Disconnected");
            if (Stream != null)
                Stream.Close();
            if (ClientTcp != null)
                ClientTcp.Close();
            Environment.Exit(0);
        }


    }
}