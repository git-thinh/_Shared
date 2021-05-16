using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

public class NetPacket
{
    public List<byte> Buffer { get; set; }

    public NetPacket() => Buffer = new List<byte>();
    public NetPacket(byte[] data) => Buffer = new List<byte>(data);
    public NetPacket(COMMANDS cmd, string requestId, string input, Dictionary<string, object> data)
    {
        Buffer = new List<byte>();
        SetCommand(cmd);
        SetRequestId(requestId);
        Write(input);
        Write(data);
    }

    ////public void Write(byte n) => Buffer.Add(n); // len = 1
    ////public void Write(short n) => Buffer.AddRange(BitConverter.GetBytes(n)); // len = 2
    ////public void Write(bool n) => Buffer.Add(BitConverter.GetBytes(n)[0]); // len = 1
    ////public void Write(int n)
    ////{
    ////    var bytes = BitConverter.GetBytes(n); // len = 4
    ////    Buffer.AddRange(bytes);
    ////}
    ////public void Write(float n)
    ////{
    ////    var bytes = BitConverter.GetBytes(n); // len = 4
    ////    Buffer.AddRange(bytes);
    ////}
    ////public void Write(double n)
    ////{
    ////    var bytes = BitConverter.GetBytes(n); // len = 8
    ////    Buffer.AddRange(bytes);
    ////}
    ////public void Write(long n)
    ////{
    ////    var bytes = BitConverter.GetBytes(n); // len = 8
    ////    Buffer.AddRange(bytes);
    ////}

    public void Write(string str)
    {
        if (string.IsNullOrEmpty(str))
            Buffer.AddRange(new byte[] { 0, 0, 0, 0 });
        else
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            // Write size before appending str
            var bytes = BitConverter.GetBytes(buffer.Length); // len = 4
            Buffer.AddRange(bytes);
            Buffer.AddRange(buffer);
        }
    }

    public void SetRequestId(string requestId)
    {
        if (!string.IsNullOrEmpty(requestId))
        {
            byte[] buffer = Encoding.ASCII.GetBytes(requestId);
            Buffer.AddRange(buffer);
        }
    }

    public void SetCommand(COMMANDS command)
    {
        byte v = (byte)command;
        Buffer.Add(v);
    }

    public void Write(Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            Buffer.AddRange(new byte[] { 0, 0, 0, 0 });
        else
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            // Write size before appending str
            var bytes = BitConverter.GetBytes(buffer.Length); // len = 4
            Buffer.AddRange(bytes);
            Buffer.AddRange(buffer);
        }
    }

}

