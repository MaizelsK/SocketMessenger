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
            Console.WriteLine("Авторизация\n");
            Console.WriteLine("Введите имя:");
            user.Sender = Console.ReadLine();
            SendMessage(user);


            Client receivedUser;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder(); 
            int bytes = 0;
            do
            {
                bytes = socket.Receive(buffer, buffer.Length, 0);


                builder.Append(Encoding.Default.GetString(buffer,0,bytes));
                receivedUser = JsonConvert.DeserializeObject<Client>(Encoding.Default.GetString(buffer));


            }
            while (socket.Available > 0);
           
            Console.WriteLine("Пользователь:"+ receivedUser.Sender);
            Console.WriteLine("Сообщение:" + receivedUser.MessageText);


            Console.ReadLine();
        }

        private static void SendMessage(Client client)
        {
            socket.Connect("10.3.6.62", 3535);
            Console.Clear();
            Console.WriteLine("Авторизация успешно выполнено\n");
            Console.WriteLine("Введите сообщение:");
            client.MessageText= Console.ReadLine();
            Console.Clear();

                
            string jsonObj= JsonConvert.SerializeObject(client);
            byte[] buffer = Encoding.Default.GetBytes(jsonObj);

           
            socket.Send(buffer);
           


            
        }
    }
}
