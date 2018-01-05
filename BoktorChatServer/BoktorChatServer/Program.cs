using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BoktorChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var MY_IP = IPAddress.Parse("127.0.0.1");
            TcpListener serversocket = new TcpListener(MY_IP, 8888);
            TcpClient clientsocket = default(TcpClient);
            int counter = 0;

            serversocket.Start();
            Console.WriteLine("Server: The server is started");
            counter = 0;

            while (true)
            {
                try
                {
                    counter += 1;
                    clientsocket = serversocket.AcceptTcpClient();
                    Console.WriteLine("Server:" + counter.ToString() + "is joined");
                    var client = new ClientHandler();
                    client.StartClient(clientsocket, counter.ToString());

                }
                catch
                {
                    clientsocket.Close();
                    serversocket.Stop();
                    Console.WriteLine("Server: the server is stopping......");
                    Console.ReadLine();
                }

            }
        }
    }

    public class ClientHandler
    {
        TcpClient clientsocket;
        string clnumber;

        public void StartClient(TcpClient client, string clientnumber)
        {
            clientsocket = client;
            clnumber = clientnumber;
            Thread clientthread = new Thread(doChat);
            clientthread.Start();
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientsocket.GetStream();
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> " + "From client-" + clnumber + dataFromClient);

                    rCount = Convert.ToString(requestCount);
                    serverResponse = "Server to client(" + clnumber + ") " + rCount;
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }
    }
}

