using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ExamServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket server = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            server.Bind(
                new IPEndPoint(
                    IPAddress.Any,
                    9999));

            server.Listen(10);

            Console.WriteLine(
                "Exam Server dang cho ket noi...");

            while (true)
            {
                Socket client = server.Accept();

                Console.WriteLine(
                    "Co mot hoc sinh vua ket noi");

                byte[] data = new byte[1024];

                int size =
                    client.Receive(data);

                string studentName =
                    Encoding.UTF8.GetString(
                        data,
                        0,
                        size);

                Console.WriteLine(
                    "Hoc sinh: " +
                    studentName);

                client.Send(
                    Encoding.UTF8.GetBytes(
                        "Ket noi thanh cong"));
            }
        }
    }
}