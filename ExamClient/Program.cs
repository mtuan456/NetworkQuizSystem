
using System.Net.Sockets;
using System.Text;

TcpClient client = new TcpClient();

try
{
    client.Connect("127.0.0.1", 5000);

    Console.WriteLine("Connected to server!");

    NetworkStream stream = client.GetStream();

    Console.Write("Enter your name: ");
    string name = Console.ReadLine() ?? "";

    byte[] data = Encoding.UTF8.GetBytes(name);

    stream.Write(data, 0, data.Length);

    byte[] buffer = new byte[1024];

    int bytesRead = stream.Read(buffer, 0, buffer.Length);

    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

    Console.WriteLine(response);

    Console.ReadLine();

    client.Close();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

