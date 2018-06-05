using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SocketMessengerClient
{
    class Program
    {
        private static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            Client user = new Client();

            Authorize(user);

            Task.Run(() =>
            {
                while (true)
                {
                    GetResponce();
                }
            });

            try
            {
                while (true)
                {
                    SendMessage(user.Sender);
                }
            }
            catch (SocketException exception)
            {
                Console.WriteLine(exception.Message);
                socket.Close();
            }
        }

        private static void Authorize(Client user)
        {
            Console.WriteLine("Авторизация\n");
            Console.WriteLine("Введите имя:");

            user.Sender = Console.ReadLine();
            socket.Connect("10.3.6.62", 3535);

            Console.Clear();
            Console.WriteLine("Авторизация успешно выполнено\n");
        }

        private static void GetResponce()
        {
            try
            {
                int bytes = 0;
                Client receivedUser;

                byte[] buffer = new byte[1024];
                do
                {
                    bytes = socket.Receive(buffer, buffer.Length, 0);

                    byte[] data = new byte[bytes];
                    Array.Copy(buffer, data, bytes);

                    receivedUser = JsonConvert.DeserializeObject<Client>(Encoding.Default.GetString(data));
                }
                while (socket.Available > 0);

                Console.WriteLine(receivedUser.Sender + ":" + receivedUser.MessageText + "\n");
            }
            catch(SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendMessage(string username)
        {
            try
            {
                Client newMessage = new Client
                {
                    Sender = username
                };

                //Console.Write("Введите сообщение: ");
                newMessage.MessageText = Console.ReadLine();
                Console.WriteLine();

                string jsonObj = JsonConvert.SerializeObject(newMessage);
                byte[] buffer = Encoding.Default.GetBytes(jsonObj);
                
                socket.Send(buffer);
            }
            catch (SocketException exception)
            {
                Console.WriteLine(exception.Message);
                socket.Close();
            }
        }
    }
}
