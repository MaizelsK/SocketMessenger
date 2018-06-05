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
        private static Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> clients = new List<Socket>();

        static void Main(string[] args)
        {
            listenSocket.Bind(new IPEndPoint(IPAddress.Parse("10.3.6.62"), defaultPort));

            try
            {
                listenSocket.Listen(10);

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    clients.Add(handler);

                    Task.Run(() => HandleClient(handler));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            listenSocket.Close();
        }

        // Обработка клиента
        static void HandleClient(Socket client)
        {
            ShowNewMember(client);

            try
            {
                while (true)
                {
                    Message response = GetResponse(client);

                    if (response != null)
                    {
                        string message = JsonConvert.SerializeObject(response);
                        SendToAllClients(message);
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            CloseClient(client);
        }

        // Уведомление о новом подключении
        private static void ShowNewMember(Socket client)
        {
            try
            {
                Message response = GetResponse(client);

                if (response != null)
                {
                    Message newConnectionMessage = new Message
                    {
                        Sender = "Server",
                        MessageText = $"{response.Sender} подключился к серверу"
                    };

                    Console.WriteLine(newConnectionMessage.MessageText);

                    string newClientMessage = JsonConvert.SerializeObject(newConnectionMessage);
                    SendToAllClients(newClientMessage);

                    string clientMessage = JsonConvert.SerializeObject(response);
                    SendToAllClients(clientMessage);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        // Получения ответа от клиента
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
                throw ex;
            }
        }

        // Рассылка сообщения всем клиентам
        private static void SendToAllClients(string message)
        {
            try
            {
                foreach (Socket client in clients)
                {
                    client.Send(Encoding.Default.GetBytes(message));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Закрытие клиента
        private static void CloseClient(Socket client)
        {
            clients.Remove(client);
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}

