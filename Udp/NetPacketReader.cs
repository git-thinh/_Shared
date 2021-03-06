using System;
using System.Collections.Generic;
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

    public string ReadRequestId()
    {
        int length = 36;

        //var string_bytes = Packet.Buffer.Skip(CurrentIndex).Take(length).ToArray();                
        byte[] buf = new byte[length];
        for (int i = CurrentIndex; i < length + CurrentIndex; i++)
            buf[i - CurrentIndex] = Packet.Buffer[i];

        string strValue = Encoding.ASCII.GetString(buf);
        CurrentIndex += length;

        return strValue;
    }

    public T Read<T>()
    {
        Type type = typeof(T);
        int length = 0;
        switch (type.Name)
        {
            //////case "Int16": //short
            //////    short shortValue = BitConverter.ToInt16(Packet.Buffer.ToArray(), CurrentIndex);
            //////    CurrentIndex += 2;
            //////    return (T)Convert.ChangeType(shortValue, typeof(T));
            //////case "Byte": //byte
            //////    byte byteValue = Packet.Buffer[CurrentIndex];
            //////    CurrentIndex += 1;
            //////    return (T)Convert.ChangeType(byteValue, typeof(T));
            //////case "Single": //float
            //////    float fValue = BitConverter.ToSingle(Packet.Buffer.ToArray(), CurrentIndex);
            //////    CurrentIndex += 4;
            //////    return (T)Convert.ChangeType(fValue, typeof(T));
            //////case "Double":
            //////    double dValue = BitConverter.ToDouble(Packet.Buffer.ToArray(), CurrentIndex);
            //////    CurrentIndex += 8;
            //////    return (T)Convert.ChangeType(dValue, typeof(T));
            //////case "Int64": //long
            //////    long lValue = BitConverter.ToInt64(Packet.Buffer.ToArray(), CurrentIndex);
            //////    CurrentIndex += 8;
            //////    return (T)Convert.ChangeType(lValue, typeof(T));
            case "Int32":
                int intValue = BitConverter.ToInt32(Packet.Buffer.ToArray(), CurrentIndex);
                CurrentIndex += 4;
                return (T)Convert.ChangeType(intValue, typeof(T));
            case "COMMANDS":
                COMMANDS cValue = (COMMANDS)(int)Packet.Buffer[CurrentIndex];
                CurrentIndex += 1;
                return (T)Convert.ChangeType(cValue, typeof(T));
            case "String":
                length = Read<int>();

                //var string_bytes = Packet.Buffer.Skip(CurrentIndex).Take(length).ToArray();                
                byte[] strBuffer = new byte[length];
                for (int i = CurrentIndex; i < length + CurrentIndex; i++)
                    strBuffer[i - CurrentIndex] = Packet.Buffer[i];

                string strValue = Encoding.UTF8.GetString(strBuffer);
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
                    string strJson = Encoding.UTF8.GetString(dicBuffer);
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

}

