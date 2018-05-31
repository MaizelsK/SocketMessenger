using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SocketMessengerServer
{
    class Program
    {
        private static int defaultPort = 3535;
        private static List<int> membersIds = new List<int>();
        private static int newMemberId = 1;

        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), defaultPort));

            while (true)
            {
                socket.Listen(3);

                Console.WriteLine($"Пользователь №{newMemberId++} подключился");
            }
        }
    }
}
