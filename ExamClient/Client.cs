
using System;
using System.Net.Sockets;
using System.Text;

namespace ExamClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Socket client = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                Console.Write("Nhap IP Server: ");
                string ip =
                    Console.ReadLine() ?? "";

                client.Connect(
                    ip,
                    9999);

                Console.WriteLine(
                    "Da ket noi den Server.");

                Console.Write(
                    "Nhap ten hoc sinh: ");

                string name =
                    Console.ReadLine() ?? "";

                Console.Write(
                    "Nhap ma phong: ");

                string room =
                    Console.ReadLine() ?? "";

                string joinMessage =
                    "JOIN|" +
                    name +
                    "|" +
                    room;

                client.Send(
                    Encoding.UTF8.GetBytes(
                        joinMessage));

                byte[] data =
                    new byte[1024];

                int size =
                    client.Receive(data);

                string response =
                    Encoding.UTF8.GetString(
                        data,
                        0,
                        size);

                Console.WriteLine(
                    "Server: " +
                    response);

                while (true)
                {
                    byte[] buffer =
                        new byte[1024];

                    int receiveSize =
                        client.Receive(buffer);

                    if (receiveSize == 0)
                    {
                        break;
                    }

                    string message =
                        Encoding.UTF8.GetString(
                            buffer,
                            0,
                            receiveSize);

                    string[] parts =
                        message.Split('|');

                    if (parts[0] == "QUESTION")
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "====================");

                        Console.WriteLine(
                            "CAU HOI SO " +
                            parts[1]);

                        Console.WriteLine(
                            "====================");

                        Console.WriteLine(
                            parts[2]);

                        Console.WriteLine(
                            parts[3]);

                        Console.WriteLine(
                            parts[4]);

                        Console.WriteLine(
                            parts[5]);

                        Console.WriteLine(
                            parts[6]);

                        Console.WriteLine();

                        Console.Write(
                            "Nhap dap an (A/B/C/D): ");

                        string answer =
                            Console.ReadLine()
                            ?? "";

                        answer =
                            answer.ToUpper();

                        string answerPacket =
                            "ANSWER|" +
                            parts[1] +
                            "|" +
                            answer;

                        client.Send(
                            Encoding.UTF8.GetBytes(
                                answerPacket));

                        Console.WriteLine(
                            "Da gui dap an.");
                    }

                    else if (parts[0] == "RESULT")
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "Ket qua: " +
                            parts[1]);
                    }

                    else if (parts[0] == "RANKING")
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "===== BANG XEP HANG =====");

                        for (
                            int i = 1;
                            i < parts.Length;
                            i++)
                        {
                            Console.WriteLine(
                                parts[i]);
                        }
                    }

                    else if (parts[0] == "END")
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "Ky thi da ket thuc.");

                        break;
                    }
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Loi: " +
                    ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine(
                "Nhan Enter de thoat.");

            Console.ReadLine();
        }
    }
}

