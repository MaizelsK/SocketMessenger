using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace SocketMessengerServer
{
    class Program
    {
        private const int defaultPort = 3535;
        private static Socket listenSocket;

        static void Main(string[] args)
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), defaultPort));

            try
            {
                listenSocket.Listen(3);

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    Task.Run(() => HandleClient(handler));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void HandleClient(Socket client)
        {
            ShowNewMember(client);

            while (true)
            {
                try
                {
                    Message response = GetResponse(client);

                    if (response != null)
                    {
                        string message = JsonConvert.SerializeObject(response);
                        listenSocket.Send(Encoding.Default.GetBytes(message));
                    }
                    //client.Send(Encoding.Default.GetBytes(message));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void ShowNewMember(Socket client)
        {
            try
            {
                Message response = GetResponse(client);

                if (response != null)
                {
                    string newMemberMessage = $"{response.Sender} подключился к серверу";
                    listenSocket.SendTo(Encoding.Default.GetBytes(newMemberMessage), listenSocket.LocalEndPoint);

                    string cleintMessage = JsonConvert.SerializeObject(response);
                    listenSocket.SendTo(Encoding.Default.GetBytes(cleintMessage), listenSocket.LocalEndPoint);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static Message GetResponse(Socket client)
        {
            Message response = null;

            try
            {
                int bytesRead = 0;
                byte[] allResponse = new byte[512];

                do
                {
                    bytesRead = client.Receive(allResponse);
                }
                while (client.Available > 0);

                byte[] data = new byte[bytesRead];
                Array.Copy(allResponse, data, bytesRead);

                response = JsonConvert.DeserializeObject<Message>(Encoding.Default.GetString(data));

                return response;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }
    }
}
