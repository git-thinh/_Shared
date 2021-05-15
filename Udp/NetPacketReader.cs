using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
//using System.Linq;
using System.Text;

public class NetPacketReader
{
    NetPacket Packet;

    int CurrentIndex;

    public NetPacketReader(NetPacket packet)
    {
        Packet = packet;
        CurrentIndex = 0;
    }

    public void ResetRead()
    {
        CurrentIndex = 0;
    }

    public T Read<T>()
    {
        Type type = typeof(T);
        int length = 0;
        switch (type.Name)
        {
            case "Int32":
                int intValue = BitConverter.ToInt32(Packet.Buffer.ToArray(), CurrentIndex);
                CurrentIndex += 4;
                return (T)Convert.ChangeType(intValue, typeof(T));
            case "Int16": //short
                short shortValue = BitConverter.ToInt16(Packet.Buffer.ToArray(), CurrentIndex);
                CurrentIndex += 2;
                return (T)Convert.ChangeType(shortValue, typeof(T));
            case "Byte": //byte
                byte byteValue = Packet.Buffer[CurrentIndex];
                CurrentIndex += 1;
                return (T)Convert.ChangeType(byteValue, typeof(T));
            case "Single": //float
                float fValue = BitConverter.ToSingle(Packet.Buffer.ToArray(), CurrentIndex);
                CurrentIndex += 4;
                return (T)Convert.ChangeType(fValue, typeof(T));
            case "Double":
                double dValue = BitConverter.ToDouble(Packet.Buffer.ToArray(), CurrentIndex);
                CurrentIndex += 8;
                return (T)Convert.ChangeType(dValue, typeof(T));
            case "Int64": //long
                long lValue = BitConverter.ToInt64(Packet.Buffer.ToArray(), CurrentIndex);
                CurrentIndex += 8;
                return (T)Convert.ChangeType(lValue, typeof(T));
            case "COMMANDS":
                COMMANDS cValue = (COMMANDS)(int)Packet.Buffer[CurrentIndex];
                CurrentIndex += 1;
                return (T)Convert.ChangeType(cValue, typeof(T));
            case "String":
                length = Read<int>();

                //var string_bytes = Packet.Buffer.Skip(CurrentIndex).Take(length).ToArray();                
                byte[] string_bytes = new byte[length];
                for (int i = CurrentIndex; i < length + CurrentIndex; i++)
                    string_bytes[i - CurrentIndex] = Packet.Buffer[i];

                string strValue = Decompress(string_bytes);
                CurrentIndex += length;
                return (T)Convert.ChangeType(strValue, typeof(T));
            case "Dictionary`2": //Dictionary<string, object>
                try
                {
                    length = Read<int>();
                    //var string_bytes = Packet.Buffer.Skip(CurrentIndex).Take(length).ToArray();                
                    byte[] dicBuffer = new byte[length];
                    for (int i = CurrentIndex; i < length + CurrentIndex; i++)
                        dicBuffer[i - CurrentIndex] = Packet.Buffer[i];
                    string strJson = Decompress(dicBuffer);
                    var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(strJson);
                    CurrentIndex += length;
                    return (T)Convert.ChangeType(dic, typeof(T));
                }
                catch { 
                }
                return default(T);
        }

        return (T)Convert.ChangeType(null, typeof(T));
    }

    public static string Decompress(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
        //using (var msi = new MemoryStream(bytes))
        //using (var mso = new MemoryStream())
        //{
        //    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        //    {
        //        gs.CopyTo(mso);
        //    }
        //    return Encoding.Unicode.GetString(mso.ToArray());
        //}
    }
}

