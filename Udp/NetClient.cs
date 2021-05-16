﻿using System.Net.Sockets;
using System.Net;
using System;

public class NetClient: IDisposable
{
    private UdpClient ClientUDP;

    public delegate void Recieve(NetPacket packet);
    public event Recieve OnRecieve;
    private IPAddress Address;
    private int Port;

    private bool StopListening = false;

    public UdpClient GetUDPClient() => ClientUDP;

    public NetClient(string address, int port)
    {
        ClientUDP = new UdpClient();
        ClientUDP.Connect(address, port);
        Address = IPAddress.Parse(address);
        Port = port;
    }

    /// <summary>
    /// Start recieving packets
    /// </summary>
    public void Listen()
    {
        while (!StopListening)
        {
            IPEndPoint endpoint = new IPEndPoint(Address, Port);
            var data = ClientUDP.Receive(ref endpoint);
            OnRecieve(new NetPacket(data));
        }
    }

    /// <summary>
    /// Send a packet to the specified endpoint
    /// </summary>
    public void Send(NetPacket packet)
    {
        var data = packet.Buffer.ToArray();
        ClientUDP.Send(data, data.Length);
    }

    public void Subcribe()
    {
        ClientUDP.Send(new byte[] { (byte)COMMANDS.NODE_SUBCRIBER }, 1);
    }

    public void Stop()
    {
        StopListening = true;
        ClientUDP.Close();
    }

    public void Dispose()
    {
        Stop();
    }
}

