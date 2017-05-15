using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ChatServer
{
    class ConnectedClient
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server;

        public ConnectedClient(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }


        public void Process()
        {
            Stream = client.GetStream();
            string message = GetMessage();
            userName = message;

            message = "Notification: ================ " + userName + " enter in the chat.================";
            server.BroadcastMessage(message, this.Id);
            Console.WriteLine(message);


            message = userName + ": ~Connect";
            server.BroadcastMessage(message, this.Id);

            while (true)
            {
                try
                {
                    message = GetMessage();
                    message = String.Format("{0}: {1}", userName, message);
                    Console.WriteLine(message);
                    server.BroadcastMessage(message, this.Id);
                }
                catch
                {
                    message = String.Format(userName, "{0}: close from chat");
                    Console.WriteLine(message);
                    server.BroadcastMessage(message, this.Id);
                    break;
                }
            }
        }


        private string GetMessage()
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

            return builder.ToString();
        }


        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }


    }
}
