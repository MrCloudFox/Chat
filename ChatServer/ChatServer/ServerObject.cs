using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace ChatServer
{
    class ServerObject
    {
        internal static TcpListener tcpListener;
        internal static List<ConnectedClient> clients = new List<ConnectedClient>();
        public static List<string> listOfParticipants = new List<string>();


        protected internal void AddConnection(ConnectedClient connectedClient)
        {
            clients.Add(connectedClient);
        }


        protected internal void RemoveConnection(string id)
        {
            ConnectedClient client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null) clients.Remove(client);

            listOfParticipants.Remove(client.userName);

        }


        protected internal void Listen()
        {
            tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();
            Console.WriteLine("Server is started. Wait to connecting...");

            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            ConnectedClient connectedClient = new ConnectedClient(tcpClient, this);
            Thread clientThread = new Thread(new ThreadStart(connectedClient.Process));
            clientThread.Start();
        }


        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            try
            {
                Parallel.For(0, clients.Count, i =>
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                });
            }
            catch(ArgumentException e)
            {
                Console.WriteLine(e);
            }

        }


        protected internal void Disconnect()
        {
            tcpListener.Stop();

            for(int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
