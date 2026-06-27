
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ExamServer
{
    class Program
    {
        static List<Socket> clients =
            new List<Socket>();

        static Dictionary<Socket, string> studentNames =
            new Dictionary<Socket, string>();

        static Dictionary<string, int> scores =
            new Dictionary<string, int>();

        static List<string> questions =
            new List<string>();

        static List<string> correctAnswers =
            new List<string>();

        static int currentQuestion = 0;

        static void Main(string[] args)
        {
            LoadQuestionsFromJson();

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
                "===== EXAM SERVER =====");

            Console.WriteLine(
                "Server dang chay...");

            Console.WriteLine(
                "Da doc " +
                questions.Count +
                " cau hoi.");

            Console.WriteLine(
                "Lenh:");
            Console.WriteLine(
                "START - Gui cau hoi");
            Console.WriteLine(
                "NEXT  - Cau tiep theo");
            Console.WriteLine(
                "RANK  - Bang xep hang");

            Thread acceptThread =
                new Thread(() =>
                {
                    while (true)
                    {
                        Socket client =
                            server.Accept();

                        Thread clientThread =
                            new Thread(() =>
                            {
                                HandleClient(
                                    client);
                            });

                        clientThread.Start();
                    }
                });

            acceptThread.Start();

            while (true)
            {
                string command =
                    Console.ReadLine()
                    ?? "";

                command =
                    command.ToUpper();

                if (command == "START")
                {
                    SendCurrentQuestion();
                }
                else if (command == "NEXT")
                {
                    currentQuestion++;

                    if (currentQuestion >=
                        questions.Count)
                    {
                        Console.WriteLine(
                            "Da het cau hoi.");

                        foreach (
                            Socket student
                            in clients)
                        {
                            try
                            {
                                student.Send(
                                    Encoding.UTF8
                                    .GetBytes(
                                        "END"));
                            }
                            catch
                            {
                            }
                        }

                        currentQuestion =
                            questions.Count - 1;
                    }
                    else
                    {
                        SendCurrentQuestion();
                    }
                }
                else if (command == "RANK")
                {
                    SendRanking();
                }
            }
        }

        static void SendCurrentQuestion()
        {
            if (questions.Count == 0)
            {
                Console.WriteLine(
                    "Khong co cau hoi.");

                return;
            }

            string question =
                questions[currentQuestion];

            foreach (
                Socket student
                in clients)
            {
                try
                {
                    student.Send(
                        Encoding.UTF8
                        .GetBytes(
                            question));
                }
                catch
                {
                }
            }

            Console.WriteLine(
                "Da gui cau hoi " +
                (currentQuestion + 1));
        }

        static void SendRanking()
        {
            string ranking =
                "RANKING";

            foreach (
                var item
                in scores)
            {
                ranking +=
                    "|" +
                    item.Key +
                    ": " +
                    item.Value +
                    " diem";
            }

            foreach (
                Socket student
                in clients)
            {
                try
                {
                    student.Send(
                        Encoding.UTF8
                        .GetBytes(
                            ranking));
                }
                catch
                {
                }
            }

            Console.WriteLine(
                "Da gui bang xep hang.");
        }

        static void LoadQuestionsFromJson()
        {
            try
            {
                string json =
                    File.ReadAllText(
                        "questions.json");

                JsonDocument doc =
                    JsonDocument.Parse(
                        json);

                foreach (
                    JsonElement q
                    in doc.RootElement
                    .EnumerateArray())
                {
                    string packet =
                        "QUESTION|" +
                        q.GetProperty("id")
                            .GetInt32() +
                        "|" +
                        q.GetProperty(
                            "question")
                            .GetString() +
                        "|" +
                        "A." +
                        q.GetProperty("A")
                            .GetString() +
                        "|" +
                        "B." +
                        q.GetProperty("B")
                            .GetString() +
                        "|" +
                        "C." +
                        q.GetProperty("C")
                            .GetString() +
                        "|" +
                        "D." +
                        q.GetProperty("D")
                            .GetString();

                    questions.Add(
                        packet);

                    correctAnswers.Add(
                        q.GetProperty(
                            "correct")
                        .GetString()
                        ?? "A");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Loi doc JSON: " +
                    ex.Message);
            }
        }

        static void HandleClient(
            Socket client)
        {
            try
            {
                byte[] data =
                    new byte[1024];

                int size =
                    client.Receive(
                        data);

                string msg =
                    Encoding.UTF8
                    .GetString(
                        data,
                        0,
                        size);

                string[] parts =
                    msg.Split('|');

                if (parts[0] == "JOIN")
                {
                    string name =
                        parts[1];

                    clients.Add(
                        client);

                    studentNames[
                        client] = name;

                    if (!scores
                        .ContainsKey(
                            name))
                    {
                        scores[name]
                            = 0;
                    }

                    Console.WriteLine(
                        name +
                        " da tham gia.");

                    Console.WriteLine(
                        "Tong so hoc sinh: "
                        +
                        clients.Count);

                    client.Send(
                        Encoding.UTF8
                        .GetBytes(
                            "JOIN_SUCCESS"));
                }

                while (true)
                {
                    data =
                        new byte[1024];

                    size =
                        client.Receive(
                            data);

                    if (size == 0)
                    {
                        break;
                    }

                    string receive =
                        Encoding.UTF8
                        .GetString(
                            data,
                            0,
                            size);

                    string[] answer =
                        receive
                        .Split('|');

                    if (answer[0]
                        == "ANSWER")
                    {
                        string student =
                            studentNames[
                                client];

                        string choice =
                            answer[2];

                        Console.WriteLine(
                            student +
                            " tra loi: " +
                            choice);

                        if (choice
                            .ToUpper() ==
                            correctAnswers[
                                currentQuestion]
                            .ToUpper())
                        {
                            scores[
                                student] += 10;

                            client.Send(
                                Encoding.UTF8
                                .GetBytes(
                                    "RESULT|Dung (+10 diem)"));
                        }
                        else
                        {
                            client.Send(
                                Encoding.UTF8
                                .GetBytes(
                                    "RESULT|Sai (0 diem)"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    ex.Message);
            }

            try
            {
                clients.Remove(
                    client);

                client.Close();
            }
            catch
            {
            }
        }
    }
}

