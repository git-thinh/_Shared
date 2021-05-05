using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class RedisBase : IDisposable
{
    #region [ Ctor ]

    const string MESSAGE_SPLIT_END = "}>\r\n$";
    const string MESSAGE_SPLIT_BEGIN = "\r\n<{";
    const int BUFFER_HEADER_MAX_SIZE = 1000;
    public Tuple<string,byte[]> __getBodyPublish(byte[] buf, string channel = null)
    {
        if (string.IsNullOrEmpty(channel) || buf == null || buf.Length == 0) return null;

        var val = new Tuple<string, byte[]>(string.Empty, null);

        int len = 0;
        int pos = 0;

        len = buf.Length;
        if (buf.Length > BUFFER_HEADER_MAX_SIZE) len = BUFFER_HEADER_MAX_SIZE;

        string s = Encoding.ASCII.GetString(buf, 0, len);
        var a = s.Split(new string[] { MESSAGE_SPLIT_END }, StringSplitOptions.None);
        if (a.Length > 2)
        {

            for (int i = 0; i < a.Length - 1; i++) pos += a[i].Length + MESSAGE_SPLIT_END.Length;
            pos += a[a.Length - 1].Split('\r')[0].Length + 2;

            if (pos <= buf.Length - 2)
            {
                len = buf.Length - pos - 2;
                byte[] bs = new byte[len];
                for (int i = pos; i < buf.Length - 2; i++) bs[i - pos] = buf[i];

                a = a[a.Length - 2].Split(new string[] { MESSAGE_SPLIT_BEGIN }, StringSplitOptions.None);
                string _channel = a[a.Length - 1].Trim();

                if (string.IsNullOrEmpty(channel)
                    || (!string.IsNullOrEmpty(channel) && channel == _channel))
                {
                    val = new Tuple<string, byte[]>(_channel, bs);
                }
            }
        }
        return val;
    }


    public readonly string __MONITOR_CHANNEL = "<{__MONITOR__}>";

    internal static readonly byte[] _END_DATA = new byte[] { 13, 10 }; //= \r\n
    internal static byte[] __combine(int size, params byte[][] arrays)
    {
        byte[] rv = new byte[size];
        int offset = 0;
        foreach (byte[] array in arrays)
        {
            System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
            offset += array.Length;
        }
        return rv;
    }

    private Socket socket = null;
    private BufferedStream bstream = null;
    private NetworkStream networkStream = null;
    internal NetworkStream m_stream { get { return networkStream; } }
    internal RedisSetting m_setting { get; }

    internal RedisBase(RedisSetting setting)
    {
        if (setting != null)
        {
            m_setting = setting;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.ReceiveTimeout = m_setting.ReceiveTimeout;
            socket.ReceiveBufferSize = m_setting.ReceiveBufferSize;

            Connect();
        }
    }

    void Connect()
    {
        socket.Connect(m_setting.Host, m_setting.Port);
        if (!this._connected)
        {
            socket.Close();
            socket = null;
            return;
        }
        networkStream = new NetworkStream(socket);
        bstream = new BufferedStream(networkStream, m_setting.BufferedStreamSize);
    }

    internal bool _connected
    {
        get
        {
            return socket != null && socket.Connected;
        }
    }

    public bool SelectDb(int indexDb)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*2\r\n");
            sb.Append("$6\r\nSELECT\r\n");
            sb.AppendFormat("${0}\r\n{1}\r\n", indexDb.ToString().Length, indexDb);
            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());
            bool ok = SendBuffer(buf);
            string line = ReadLine();
            return ok && !string.IsNullOrEmpty(line) && line[0] == '+';
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    #endregion

    #region [ PUBLISH - SUBCRIBE ]

    public bool PSUBSCRIBE(string channel)
    {
        if (string.IsNullOrEmpty(channel)) return false;
        if (channel != __MONITOR_CHANNEL)
            channel = "<{" + channel + "}>";

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*2\r\n");
            sb.Append("$10\r\nPSUBSCRIBE\r\n");
            sb.AppendFormat("${0}\r\n{1}\r\n", channel.Length, channel);

            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());
            var ok = SendBuffer(buf);
            var lines = ReadMultiString();
            //Console.WriteLine("\r\n\r\n{0}\r\n\r\n", string.Join(Environment.NewLine, lines));
            return ok;
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    internal bool PUBLISH(string channel, long value)
        => PUBLISH(channel, value.ToString());
    internal bool PUBLISH(string channel, byte[] vals)
    {
        if (!this._connected) return false;
        if (string.IsNullOrEmpty(channel)) return false;

        if (channel != __MONITOR_CHANNEL) channel = "<{" + channel + "}>";
        channel = channel.ToUpper();

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*3\r\n");
            sb.Append("$7\r\nPUBLISH\r\n");
            sb.AppendFormat("${0}\r\n{1}\r\n", channel.Length, channel);
            //sb.AppendFormat("${0}\r\n{1}\r\n", value.Length, value);

            sb.AppendFormat("${0}\r\n", vals.Length);
            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());

            var arr = __combine(buf.Length + vals.Length + 2, buf, vals, _END_DATA);

            var ok = SendBuffer(arr);
            var line = ReadLine();
            //Console.WriteLine("->" + line);
            return ok;
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    internal bool PUBLISH(string channel, string value)
        => PUBLISH(channel, Encoding.UTF8.GetBytes(value));

    #endregion

    bool SendBuffer(byte[] buf)
    {
        if (socket == null) Connect();
        if (socket == null) return false;
        try { socket.Send(buf); }
        catch (SocketException ex)
        {
            // timeout;
            socket.Close();
            socket = null;
            return false;
        }
        return true;
    }

    #region [ READ ]

    internal string ReadLine()
    {
        StringBuilder sb = new StringBuilder();
        int c;
        while ((c = bstream.ReadByte()) != -1)
        {
            if (c == '\r')
                continue;
            if (c == '\n')
                break;
            sb.Append((char)c);
        }
        return sb.ToString();
    }

    internal string ReadString()
    {
        var result = ReadBuffer();
        if (result != null)
            return Encoding.UTF8.GetString(result);
        return null;
    }

    internal string[] ReadMultiString()
    {
        string r = ReadLine();
        //Log(string.Format("R: {0}", r));
        if (r.Length == 0)
            throw new Exception("Zero length respose");

        char c = r[0];
        if (c == '-')
            throw new Exception(r.StartsWith("-ERR") ? r.Substring(5) : r.Substring(1));

        List<string> result = new List<string>();

        if (c == '*')
        {
            int n;
            if (Int32.TryParse(r.Substring(1), out n))
                for (int i = 0; i < n; i++)
                {
                    string str = ReadString();
                    result.Add(str);
                }
        }
        return result.ToArray();
    }

    internal byte[] ReadBuffer()
    {
        string s = ReadLine();
        //Log("S", s);
        if (s.Length == 0)
            throw new ResponseException("Zero length respose");

        char c = s[0];
        if (c == '-')
            throw new ResponseException(s.StartsWith("-ERR ") ? s.Substring(5) : s.Substring(1));

        if (c == '$')
        {
            if (s == "$-1")
                return null;
            int n;

            if (Int32.TryParse(s.Substring(1), out n))
            {
                byte[] retbuf = new byte[n];

                int bytesRead = 0;
                do
                {
                    int read = bstream.Read(retbuf, bytesRead, n - bytesRead);
                    if (read < 1)
                        throw new ResponseException("Invalid termination mid stream");
                    bytesRead += read;
                }
                while (bytesRead < n);
                if (bstream.ReadByte() != '\r' || bstream.ReadByte() != '\n')
                    throw new ResponseException("Invalid termination");
                return retbuf;
            }
            throw new ResponseException("Invalid length");
        }

        /* don't treat arrays here because only one element works -- use DataArray!
		//returns the number of matches
		if (c == '*') {
			int n;
			if (Int32.TryParse(s.Substring(1), out n)) 
				return n <= 0 ? new byte [0] : ReadData();			
			throw new ResponseException ("Unexpected length parameter" + r);
		}
		*/

        if (c == ':')
            return Encoding.ASCII.GetBytes(s);

        throw new ResponseException("Unexpected reply: " + s);
    }

    #endregion

    #region [ GET ]

    public string[] KEYS(string pattern = "*")
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("*2\r\n");
        sb.Append("$4\r\nKEYS\r\n");
        sb.AppendFormat("${0}\r\n{1}\r\n", pattern.Length, pattern);
        byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());

        bool ok = SendBuffer(buf);
        var keys = ReadMultiString();

        return keys;
    }


    public string GET(string key)
    {
        if (!this._connected) return null;

        var buf = GET_BUFFER(key);
        if (buf == null) return string.Empty;
        else return Encoding.UTF8.GetString(buf);
    }

    public Bitmap GET_BITMAP(string key)
    {
        if (!this._connected) return null;

        var buf = GET_BUFFER(key);
        if (buf == null) return null;
        else
        {
            var ms = new MemoryStream(buf);
            return new Bitmap(ms);
        }
    }

    public Stream GET_STREAM(string key)
    {
        if (!this._connected) return null;

        var buf = GET_BUFFER(key);
        if (buf == null) return null;
        else return new MemoryStream(buf);
    }

    public byte[] GET_BUFFER(string key)
    {
        if (!this._connected) return null;

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*2\r\n");
            sb.Append("$3\r\nGET\r\n");
            sb.AppendFormat("${0}\r\n{1}\r\n", key.Length, key);
            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());

            bool ok = SendBuffer(buf);
            var rs = ReadBuffer();

            return rs;
        }
        catch (Exception ex)
        {
        }
        return null;
    }

    public Bitmap HGET_BITMAP(long key, int field)
        => HGET_BITMAP(key.ToString(), field.ToString());
    public Bitmap HGET_BITMAP(string key, string field)
    {
        if (!this._connected) return null;

        var bs = HGET_BUFFER(key, field);
        if (bs != null && bs.Length > 0)
            return new Bitmap(new MemoryStream(bs));
        return null;
    }

    public string HGET(string key, string field)
    {
        if (!this._connected) return null;

        var buf = HGET_BUFFER(key, field);
        if (buf == null) return string.Empty;
        else return Encoding.UTF8.GetString(buf);
    }

    public byte[] HGET_BUFFER(string key, string field)
    {
        if (!this._connected) return null;

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*3\r\n");
            sb.Append("$4\r\nHGET\r\n");
            sb.AppendFormat("${0}\r\n{1}\r\n", key.Length, key);
            sb.AppendFormat("${0}\r\n{1}\r\n", field.Length, field);
            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());

            bool ok = SendBuffer(buf);
            var rs = ReadBuffer();

            return rs;
        }
        catch (Exception ex)
        {
        }
        return null;
    }

    public int[] HKEYS(long key)
    {
        if (!this._connected) return null;

        var keys = HKEYS(key.ToString());
        int[] vs = new int[keys.Length];
        for (int i = 0; i < keys.Length; i++)
            int.TryParse(keys[i], out vs[i]);
        return vs;
    }

    public string[] HKEYS(string key)
    {
        if (!this._connected) return null;

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*2\r\n");
            sb.Append("$5\r\nHKEYS\r\n");
            sb.AppendFormat("${0}\r\n{1}\r\n", key.Length, key);
            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());

            bool ok = SendBuffer(buf);
            var keys = ReadMultiString();
            return keys;
        }
        catch (Exception ex)
        {
        }
        return null;
    }



    #endregion

    #region [ SET ]

    public bool HSET(long key, int field, byte[] value)
        => HMSET(key.ToString(), new Dictionary<string, byte[]>() { { field.ToString(), value } });
    public bool HSET(string key, string field, byte[] value)
        => HMSET(key.ToString(), new Dictionary<string, byte[]>() { { field.ToString(), value } });
    public bool HSET(string key, string field, string value)
        => HMSET(key.ToString(), new Dictionary<string, string>() { { field.ToString(), value.ToString() } });

    public bool HMSET(string key, IDictionary<string, string> fields)
    {
        if (!this._connected) return false;

        var dic = new Dictionary<string, byte[]>();
        foreach (var kv in fields)
            dic.Add(kv.Key, Encoding.UTF8.GetBytes(kv.Value));
        return HMSET(key, dic);
    }

    public bool HMSET(string key, IDictionary<string, byte[]> fields)
    {
        if (!this._connected) return false;

        if (fields == null || fields.Count == 0) return false;
        try
        {
            StringBuilder bi = new StringBuilder();
            bi.AppendFormat("*{0}\r\n", 2 + fields.Count * 2);
            bi.Append("$5\r\nHMSET\r\n");
            bi.AppendFormat("${0}\r\n{1}\r\n", key.Length, key);

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buf = Encoding.UTF8.GetBytes(bi.ToString());
                ms.Write(buf, 0, buf.Length);

                string keys_ = key;
                if (fields != null && fields.Count > 0)
                {
                    foreach (var data in fields)
                    {
                        buf = Encoding.UTF8.GetBytes(string.Format("${0}\r\n{1}\r\n", data.Key.Length, data.Key));
                        ms.Write(buf, 0, buf.Length);
                        buf = Encoding.UTF8.GetBytes(string.Format("${0}\r\n", data.Value.Length));
                        ms.Write(buf, 0, buf.Length);
                        ms.Write(data.Value, 0, data.Value.Length);
                        ms.Write(_END_DATA, 0, 2);
                        keys_ += "|" + data.Key;
                    }
                }
                var ok = SendBuffer(ms.ToArray());
                string line = ReadLine();
                if (ok && !string.IsNullOrEmpty(line) && (line[0] == '+' || line[0] == ':'))
                    return true;
                return false;
            }
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    #endregion

    #region [ SEND TO COMMAND ]

    public string SendToCommand(string channel, DOC_CMD cmd, string data)
    {
        string sendId = Guid.NewGuid().ToString();
        var ls = new List<byte>();
        ls.AddRange(Encoding.ASCII.GetBytes(sendId));
        ls.Add((byte)cmd);
        ls.AddRange(Encoding.UTF8.GetBytes(data));
        bool ok = PUBLISH(channel, ls.ToArray());
        return sendId;
    }

    #endregion

    #region [ REPLY DOCUMENT STATUS ]

    public bool ReplyStatus(string channel, string requestId, string cmd, int ok = 1, long docId = 0, int page = 0, string file = "", string err = "")
        => PUBLISH(channel, _replyStatus(requestId, cmd, ok, docId, page, file, err));

    public bool ReplyStatus(string channel, string requestId, string cmd, int ok, long docId, string err)
        => PUBLISH(channel, _replyStatus(requestId, cmd, ok, docId, 0, string.Empty, err));

    string _replyStatus(string requestId, string cmd, int ok = 1, long docId = 0, int page = 0, string file = "", string err = "")
        => string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}", requestId, cmd, ok, docId, page, file, err);

    #endregion

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (socket != null)
        {
            socket.Close();
            socket = null;
        }
    }
}
