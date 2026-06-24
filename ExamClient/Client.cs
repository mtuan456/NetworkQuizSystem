using System;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace ExamClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            client.Connect(
                "127.0.0.1",
                9999);

            Console.WriteLine(
                "Da ket noi server");

            Console.Write(
                "Nhap ten: ");

            string name =
                Console.ReadLine();

            client.Send(
                Encoding.UTF8.GetBytes(
                    name));

            byte[] data =
                new byte[1024];

            int size =
                client.Receive(data);

            Console.WriteLine(
                Encoding.UTF8.GetString(
                    data,
                    0,
                    size));
        }
    }
}