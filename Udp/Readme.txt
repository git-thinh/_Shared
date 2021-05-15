
class Server
{
    static NetServer server;

    static void Main(string[] args)
    {
        Console.WriteLine("Server started");
        server = new NetServer(8706);
        server.OnRecieve += Server_OnRecieve;

        Thread ListenThread = new Thread(server.Listen);
        ListenThread.Start();

        Console.ReadLine();
    }

    private static void Server_OnRecieve(System.Net.IPEndPoint address, NetPacket packet)
    {
        var reader = new NetPacketReader(packet);
        var type = reader.Read<byte>();
        if (type == 1)
        {
            string result = reader.Read<string>();
            Console.WriteLine(result);

            NetPacket send_packet = new NetPacket();
            send_packet.Write((byte)1);
            send_packet.Write("Nguyễn Cẩm Tú");
            server.Send(address, send_packet);
        }
    }
}


class Client
{
    static void Main(string[] args)
    {
        Console.WriteLine("Client started");
        Thread.Sleep(100);
        NetClient client = new NetClient("127.0.0.1", 8706);
        client.OnRecieve += Client_OnRecieve;

        Thread ClientThread = new Thread(client.Listen);
        ClientThread.Start();

        NetPacket send_packet = new NetPacket();
        send_packet.Write((byte)1);
        send_packet.Write("Nguyễn Văn Thịnh");
        client.Send(send_packet);

        Console.ReadLine();
    }

    private static void Client_OnRecieve(NetPacket packet)
    {
        var reader = new NetPacketReader(packet);
        var type = reader.Read<byte>();
        if (type == 1)
        {
            string result = reader.Read<string>();
            Console.WriteLine("Recieved data: {0}", result);
        }
    }
}
