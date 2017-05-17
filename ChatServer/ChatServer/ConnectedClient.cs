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
        internal string userName;
        internal TcpClient client;
        internal ServerObject server;

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

            ServerObject.listOfParticipants.Add(userName);

            message = "Notification: ================ " + userName + " enter in the chat.================";
            server.BroadcastMessage(message, Id);
            Console.WriteLine(message);

            message = userName + " ~Connect";
            server.BroadcastMessage(message, Id);

            UpdateParticipantsList(Id, userName);

            while (true)
            {
                try
                {
                    message = GetMessage();
                    message = String.Format("{0}: {1}", userName, message);
                    Console.WriteLine(message);
                    server.BroadcastMessage(message, Id);
                }
                catch
                {
                    message = String.Format(userName, "{0}: close from chat");
                    Console.WriteLine(message);
                    server.BroadcastMessage(message, Id);
                    break;
                }
            }
        }


        private void UpdateParticipantsList(string id, string usersName)
        {
            byte[] dataOfNickNameOfParticipant;

            for (int i = 0; i < ServerObject.listOfParticipants.Count; i++)
                {
                    if (ServerObject.clients[i].Id == Id && ServerObject.clients[i].userName != usersName)
                    {
                        for (int j = 0; j < ServerObject.listOfParticipants.Count; j++)
                        {
                            dataOfNickNameOfParticipant = Encoding.Unicode.GetBytes((ServerObject.listOfParticipants[j] + ": ~Connect"));
                            ServerObject.clients[i].Stream.Write(dataOfNickNameOfParticipant, 0, dataOfNickNameOfParticipant.Length);
                        }
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
            ServerObject.listOfParticipants.Remove(userName);

            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }


    }
}