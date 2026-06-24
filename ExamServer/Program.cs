
using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 5000);

server.Start();

Console.WriteLine("===== EXAM SERVER =====");
Console.WriteLine("Server started...");
Console.WriteLine("Waiting for clients...");

while (true)
{
    TcpClient client = server.AcceptTcpClient();

    Console.WriteLine("A client connected!");

    Thread clientThread = new Thread(() =>
    {
        try
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Received: {message}");

            string response = "Welcome to Exam Server";

            byte[] responseData = Encoding.UTF8.GetBytes(response);

            stream.Write(responseData, 0, responseData.Length);

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    });

    clientThread.Start();
}
