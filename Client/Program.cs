using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    internal class Program
    {
        static int port_IT_developer = 7001;
        static int port_Web_designer = 7002;
        static int port_Cyber_security = 7003;
        static string ip_IT_developer = "239.0.0.101";
        static string ip_Web_designer = "239.0.0.102";
        static string ip_Cyber_security = "239.0.0.103";
        static void GetMulticastGroup(string choice)
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

            if (choice == "1")
            {
                client.Client.Bind(new IPEndPoint(IPAddress.Any, port_IT_developer));
                client.JoinMulticastGroup(IPAddress.Parse(ip_IT_developer));
                Console.WriteLine("Підключено до групи: IT Developer");
            }
            else if (choice == "2")
            {
                client.Client.Bind(new IPEndPoint(IPAddress.Any, port_Web_designer));
                client.JoinMulticastGroup(IPAddress.Parse(ip_Web_designer));
                Console.WriteLine("Підключено до групи: Web Designer");
            }
            else if (choice == "3")
            {
                client.Client.Bind(new IPEndPoint(IPAddress.Any, port_Cyber_security));
                client.JoinMulticastGroup(IPAddress.Parse(ip_Cyber_security));
                Console.WriteLine("Підключено до групи: Cyber Security");
            }

            while (true)
            {
                Console.Write("-: ");
                byte[] data = client.Receive(ref clientEP);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine($"[Multicast] {message}");
                Console.Write("-: ");
            }
        }
        static void GetBroadcastMessange()
        {
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 6001));
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Broadcast підключен...");

            while (true)
            {
                byte[] message_Bytes = client.Receive(ref clientEP);
                string message = Encoding.UTF8.GetString(message_Bytes);
                Console.WriteLine($"[Broadcast] {message}");
                Console.Write($"-: ");
            }
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            TcpClient client = new TcpClient("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();

            while (true)
            {
                Console.WriteLine($"1. Запитати свій розклад (TCP)");
                Console.WriteLine($"2. Слухати Broadcast");
                Console.WriteLine($"3. Приєднатись до Multicast-групи");
                Console.WriteLine($"4. Вийти");
                Console.Write($"-: ");
                string choice = Console.ReadLine();
                byte[] choice_Bytes = Encoding.UTF8.GetBytes(choice);
                stream.Write(choice_Bytes, 0, choice_Bytes.Length);

                if (choice == "1")
                {
                    byte[] schedule_Bytes = new byte[1024];
                    int schedule_Int = stream.Read(schedule_Bytes, 0, schedule_Bytes.Length);
                    string schedule = Encoding.UTF8.GetString(schedule_Bytes);
                    Console.WriteLine(schedule);
                }
                if (choice == "2")
                {
                    Thread thread1 = new Thread(() => GetBroadcastMessange());
                    thread1.Start();
                }
                if (choice == "3")
                {
                    Console.WriteLine($"1. IT developer");
                    Console.WriteLine($"2. Web designer");
                    Console.WriteLine($"3. Cyber security");
                    Console.Write($"-: ");
                    string choice2 = Console.ReadLine();
                    byte[] choice2_Bytes = Encoding.UTF8.GetBytes(choice2);
                    stream.Write(choice2_Bytes, 0, choice2_Bytes.Length);

                    Thread thread2 = new Thread(() => GetMulticastGroup(choice2));
                    thread2.Start();
                }
                if (choice == "4")
                {
                    client.Close();
                }
            }
        }
    }
}
