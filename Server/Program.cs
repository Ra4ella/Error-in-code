using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Server
{
    internal class Program
    {
        static int port_IT_developer = 7001;
        static int port_Web_designer = 7002;
        static int port_Cyber_security = 7003;
        static string ip_IT_developer = "239.0.0.101";
        static string ip_Web_designer = "239.0.0.102";
        static string ip_Cyber_security = "239.0.0.103";
        static Random rnd = new Random();
        static int id = 0;
        static void TcpSchedule(NetworkStream stream)
        {
            string schedule = "Schedule: 1. Math | 2. IT | 3. English";
            byte[] schedule_Byte = Encoding.UTF8.GetBytes(schedule);
            stream.Write(schedule_Byte, 0, schedule_Byte.Length);
        }
        static void BroadcastMessage()
        {
            UdpClient server = new UdpClient();
            server.EnableBroadcast = true;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Broadcast, 6001);

            while (true)
            {
                Thread.Sleep(1500);
                string announcement = "In building 1 there isn't water";
                byte[] announcement_Bytes = Encoding.UTF8.GetBytes(announcement);
                server.Send(announcement_Bytes, announcement_Bytes.Length, remoteEP);
                Console.WriteLine($"[Broadcast] Надіслано");
                Thread.Sleep(1500);
            }
        }
        static void MulticastMessage(string user2_choice)
        {
            UdpClient server = new UdpClient();
            if (user2_choice == "1")
            {
                Console.WriteLine("Work");
                string message = "In group IT developer there isn't third lesson";
                byte[] message_Bytes = Encoding.UTF8.GetBytes(message);
                server.Send(message_Bytes, message_Bytes.Length, ip_IT_developer, port_IT_developer);
                Console.WriteLine("[Multicast] Надіслано в групу IT developer");
            }
            else if (user2_choice == "2")
            {
                Console.WriteLine("Work");
                string message = "In group Web designer the class is moved to 4 PM";
                byte[] message_Bytes = Encoding.UTF8.GetBytes(message);
                server.Send(message_Bytes, message_Bytes.Length, ip_Web_designer, port_Web_designer);
                Console.WriteLine("[Multicast] Надіслано в групу Web designer");
            }
            else if (user2_choice == "3")
            {
                Console.WriteLine("Work");
                string message = "In group Cyber security there is a teacher replacement";
                byte[] message_Bytes = Encoding.UTF8.GetBytes(message);
                server.Send(message_Bytes, message_Bytes.Length, ip_Cyber_security, port_Cyber_security);
                Console.WriteLine("[Multicast] Надіслано в групу Cyber security");
            }
        }
        static void ServerWork(TcpClient client)
        {
            while (true)
            {
                NetworkStream stream = client.GetStream();

                byte[] user_choice_Byte = new byte[1024];
                int user_choice_Int = stream.Read(user_choice_Byte, 0, user_choice_Byte.Length);
                string user_choice = Encoding.UTF8.GetString(user_choice_Byte, 0, user_choice_Int);

                if (user_choice == "1")
                {
                    Thread thread1 = new Thread(() => TcpSchedule(stream));
                    thread1.Start();
                    Console.WriteLine("Schedule sent");
                }
                if (user_choice == "2")
                {
                    Thread thread2 = new Thread(() => BroadcastMessage());
                    thread2.Start();
                    Console.WriteLine("Broadcast acctived");
                }
                if (user_choice == "3")
                {
                    byte[] user_choice2_Byte = new byte[1024];
                    int user_choice2_Int = stream.Read(user_choice2_Byte, 0, user_choice2_Byte.Length);
                    string user2_choice = Encoding.UTF8.GetString(user_choice2_Byte, 0, user_choice2_Int);
                    Thread thread3 = new Thread(() => MulticastMessage(user2_choice));
                    thread3.Start();
                    Console.WriteLine("Multicast activated");
                }
            }
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server status: Online");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine($"Client {++id} online");

                Thread thread = new Thread(() => ServerWork(client));
                thread.Start();
            }
        }
    }
}
