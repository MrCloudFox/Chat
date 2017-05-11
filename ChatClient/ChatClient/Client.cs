using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public MessageEventArgs(String recieveMessage)
        {
            Message = recieveMessage;
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
        public static event MessageEventHandler MessageReceived;

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

        public static void ReceiveMessage()
        {
            var client = new Client();

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

                    string message = builder.ToString();
                    Console.WriteLine(message);
                    client.OnMessageReceive(new MessageEventArgs(message));
                }
                catch
                {
                    Console.WriteLine("Connect is lost!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }


        public static void Disconnect()
        {
            if (Stream != null)
                Stream.Close();
            if (ClientTcp != null)
                ClientTcp.Close();
            Environment.Exit(0);
        }


    }
}