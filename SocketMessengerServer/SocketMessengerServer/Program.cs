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

        private static List<int> membersIds = new List<int>();
        private static int newMemberId = 1;

        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(new IPEndPoint(IPAddress.Parse("10.3.6.62"), defaultPort));

            try
            {
                listenSocket.Listen(3);

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    int bytesRead = 0;
                    byte[] allResponse = new byte[512];

                    do
                    {
                        bytesRead = handler.Receive(allResponse);
                    }
                    while (handler.Available > 0);

                    byte[] data = new byte[bytesRead];
                    Array.Copy(allResponse, data, bytesRead);

                    Message response = JsonConvert.DeserializeObject<Message>(Encoding.Default.GetString(data));

                    Console.WriteLine($"{response.Sender} подключился к серверу");

                    string message = JsonConvert.SerializeObject(response);
                    handler.Send(Encoding.Default.GetBytes(message));

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
