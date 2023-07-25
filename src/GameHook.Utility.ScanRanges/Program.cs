using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text;

using IHost host = Host.CreateDefaultBuilder(args).Build();

const int RETROARCH_PORT = 55355;

var receiver = new UdpClient(RETROARCH_PORT - 1);
receiver.BeginReceive(DataReceived, receiver);

await ConsolePrompt(host.Services);

string? responseString = string.Empty;

async Task ConsolePrompt(IServiceProvider services)
{
    while (true)
    {
        Console.WriteLine("Enter command [READ_CORE_MEMORY] >");

        var cmd = string.Empty;
        while (string.IsNullOrEmpty(cmd))
        {
            cmd = Console.ReadLine();
        }

        var response = await WaitForPacket(cmd);

        if (response == null)
        {
            Console.WriteLine("That brought back nothing.");
        }
        else
        {
            Console.WriteLine($"That brought back {response.Count()} bytes.");
        }

        Console.WriteLine();
    }
}

async Task<int[]?> WaitForPacket(string cmd)
{
    if (receiver == null) throw new Exception("UdpClient is null.");

    responseString = null;

    var msg = Encoding.ASCII.GetBytes(cmd);
    receiver.Send(msg, msg.Length, "localhost", RETROARCH_PORT);

    while (responseString == null)
    {
        await Task.Delay(1);
    }

    if (responseString.Contains("-1")) { return null; }

    var bytesString = responseString.Split().Skip(2);
    var bytes = bytesString.Select(x => Convert.ToInt32(x, 16)).ToArray();

    return bytes;
}

void DataReceived(IAsyncResult ar)
{
    var c = ar.AsyncState as UdpClient;
    if (c == null) throw new Exception("ar.AsyncState is null.");

    var receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
    var receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

    // Convert data to ASCII and print in console
    string receivedText = Encoding.ASCII.GetString(receivedBytes).Replace("\n", string.Empty);
    Console.WriteLine(receivedText);

    responseString = receivedText;

    // Restart listening for udp data packages
    c.BeginReceive(DataReceived, ar.AsyncState);
}