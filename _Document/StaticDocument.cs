using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public enum COMMANDS
{
    NONE = 0,
    DOC_INFO = 10,

    PDF_MMF_TT = 11,
    PDF_SPLIT_ALL_PDF = 21,
    PDF_SPLIT_ALL_PNG = 22,
    PDF_SPLIT_ALL_JPG = 23,

    TRANSLATE_TEXT_GOOGLE_01 = 70,

    CURL_GET_HEADER = 80,
    CURL_GET_HTML = 81,
    CURL_GET_HTML_COOKIE = 82,
    CURL_POST = 84,
    CURL_POST_COOKIE = 85,
    CURL_POST_UPLOAD_FILE_COOKIE = 86,
    CURL_POST_UPLOAD_FILE = 87,
    CURL_FTP_UPLOAD_FILE = 89
}



public enum DOC_TYPE
{
    TT_FILE = 10,

    INFO_OGRINAL = 11,
    INFO_PROTOBUF = 19,

    PDF_OGRINAL = 30,
    PDF_COMPRESS = 31,

    JPG_OGRINAL = 50,
    JPG_OGRINAL_SIZE = 51,
    JPG_NO_BORDER_PAGE = 60,
    JPG_LINES = 61,

    PNG_OGRINAL = 70,
    PNG_OGRINAL_SIZE = 71,
    PNG_NO_BORDER_PAGE = 78,
    PNG_LINES = 79,

    TEXT_OGRINAL = 80,
    TEXT_COMPRESS = 81,

    HTML_OGRINAL = 90,
    HTML_COMPRESS = 91,
}

public class oRequestService
{
    public string id { set; get; }
    public COMMANDS command { set; get; }
    public string input { set; get; }
    public Dictionary<string, object> para { set; get; }

    public oRequestService() { }
    public oRequestService(COMMANDS cmd, string input_ = "", Dictionary<string, object> para_ = null)
    {
        id = System.Guid.NewGuid().ToString();
        command = cmd;
        input = input_;
        if (para_ == null) para_ = new Dictionary<string, object>();
        para = para_;
    }


    public static oRequestService Load(byte[] buf)
    {
        if (buf == null || buf.Length == 0) return null;
        string json;
        try
        {
            var r = StaticDocument.__getBodyOfPUBLISH(buf);
            //var lz = LZ4.LZ4Codec.Unwrap(r.Item2);
            json = Encoding.UTF8.GetString(r.Item2);
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<oRequestService>(json);
            return o;
        }
        catch
        {
        }
        return null;
    }

    public byte[] ToBytes()
    {
        try
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            var buf = Encoding.UTF8.GetBytes(json);
            //var lz = LZ4.LZ4Codec.Wrap(buf);
            return buf;
        }
        catch { }
        return new byte[] { };
    }
}

public class oRequestReply
{
    public string request_id { get; set; }
    public string command { get; set; }
    public string tag { get; set; }
    public bool ok { get; set; }
    public long doc_id { get; set; }
    public int page { get; set; }
    public string file { get; set; }
    public string error { get; set; }

    public string input { get; set; }
    public string output { get; set; }

    public override string ToString()
    {
        return string.Format("{0}-{1}: {2} - {3}", request_id, ok ? "OK" : "??", tag, input);
    }

    public static oRequestReply Load(byte[] buf)
    {
        if (buf == null || buf.Length == 0) return null;
        string json;
        try
        {
            var r = StaticDocument.__getBodyOfPUBLISH(buf);
            if (r.Item1 == "*")
            {
                //var lz = LZ4.LZ4Codec.Unwrap(r.Item2);
                json = Encoding.UTF8.GetString(r.Item2);
                var o = Newtonsoft.Json.JsonConvert.DeserializeObject<oRequestReply>(json);
                return o;
            }
        }
        catch
        {
        }
        return null;
    }

    public byte[] ToBytes()
    {
        try
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            var buf = Encoding.UTF8.GetBytes(json);
            //var lz = LZ4.LZ4Codec.Wrap(buf);
            return buf;
        }
        catch { }
        return new byte[] { };
    }
}

public class oDocument
{
    public long id { set; get; }
    public int file_page { set; get; }
    public long file_length { set; get; }
    public DOC_TYPE file_type { set; get; }
    public int year_created { set; get; }

    public string file_path { set; get; }
    public string file_name_ogrinal { set; get; }
    public string file_name_ascii { set; get; }

    public string name_author { set; get; }
    public string time_created { set; get; }

    public System.Collections.Generic.Dictionary<string, string> infos { set; get; }
    public string metadata { set; get; }

    public int page_current { set; get; }
    public int page_total { set; get; }

    public byte[] page_image { set; get; }

    public string GetPageTitle()
        => StaticDocument.GetPageTitle(this.page_current + 1, this.page_total, Path.GetFileNameWithoutExtension(this.file_path), this.id);
}

public static class StaticDocument
{
    public static oRequestReply ToDocumentReply(Tuple<string, byte[]> data)
    {
        if (data.Item2 == null) return null;

        try
        {
            string replyMessage = System.Text.Encoding.UTF8.GetString(data.Item2);
            var a = replyMessage.Split('^');
            if (a.Length == 10)
            {
                var o = new oRequestReply();
                o.request_id = a[0];
                o.command = a[1];
                o.tag = a[2];
                o.ok = a[3] == "1";
                o.doc_id = long.Parse(a[4]);
                o.page = int.Parse(a[5]);
                o.file = a[6];
                o.error = a[7];

                o.input = a[8];
                o.output = a[9];
                return o;
            }
        }
        catch { }
        return null;
    }


    public static Tuple<string, byte[]> __getBodyOfPUBLISH(byte[] buf, string channel = null)
    {
        if (buf == null || buf.Length == 0) return null;

        var val = new Tuple<string, byte[]>(string.Empty, null);

        int len = 0;
        int pos = 0;

        len = buf.Length;
        if (buf.Length > __CONFIG.BUFFER_HEADER_MAX_SIZE) len = __CONFIG.BUFFER_HEADER_MAX_SIZE;

        string s = Encoding.ASCII.GetString(buf, 0, len);
        var a = s.Split(new string[] { __CONFIG.MESSAGE_SPLIT_END }, StringSplitOptions.None);
        if (a.Length > 2)
        {

            for (int i = 0; i < a.Length - 1; i++) pos += a[i].Length + __CONFIG.MESSAGE_SPLIT_END.Length;
            pos += a[a.Length - 1].Split('\r')[0].Length + 2;

            if (pos <= buf.Length - 2)
            {
                len = buf.Length - pos - 2;
                byte[] bs = new byte[len];
                for (int i = pos; i < buf.Length - 2; i++) bs[i - pos] = buf[i];

                a = a[a.Length - 2].Split(new string[] { __CONFIG.MESSAGE_SPLIT_BEGIN }, StringSplitOptions.None);
                string _channel = a[a.Length - 1].Trim();

                if (string.IsNullOrEmpty(channel)
                    || _channel == "*"
                    || (!string.IsNullOrEmpty(channel) && channel == _channel))
                    val = new Tuple<string, byte[]>(_channel, bs);
            }
        }
        return val;
    }

    static RedisBase __commandSender = null;
    public static string Send(COMMANDS cmd, string input_ = "", Dictionary<string, object> para_ = null)
    {
        var r = new oRequestService(cmd, input_, para_);
        var buf = r.ToBytes();

        if (__commandSender == null)
            __commandSender = new RedisBase(new RedisSetting(REDIS_TYPE.ONLY_WRITE, __CONFIG.REDIS_PORT_WRITE));
        __commandSender.PUBLISH(__CONFIG.CHANNEL_NAME, buf);

        return r.id;
    }

    public static string GetPageTitle(int pageCurrent, int pageTotal, string file, long key = 0)
        => string.Format("[{0}.{1}] {2} - {3}", pageCurrent + 1, pageTotal, Path.GetFileNameWithoutExtension(file), key);

    public static long BuildId(DOC_TYPE type, int pageTotal, long fileSize)
    {
        string s = string.Empty;
        switch (pageTotal.ToString().Length)
        {
            default: s = "00000"; break;
            case 1: s = "0000" + s; break;
            case 2: s = "000" + s; break;
            case 3: s = "00" + s; break;
            case 4: s = "0" + s; break;
            case 5: s = "" + s; break;
        }
        string key = string.Format("{0}{1}{2}", (int)type, s, fileSize);
        return long.Parse(key);
    }
}

public class KeySubscriber
{
    readonly bool isRequest = false;
    readonly bool isReply = false;
    readonly string m_channel;
    readonly Action<oRequestService> m_request;
    public KeySubscriber(Action<oRequestService> request)
    {
        isRequest = true;
        m_channel = __CONFIG.CHANNEL_NAME;
        m_request = request;
    }

    readonly Action<byte[]> m_reply;
    public KeySubscriber(Action<byte[]> reply)
    {
        isReply = true;
        m_channel = "*";
        m_reply = reply;
    }

    bool __running = true;
    public void Start()
    {
        var redis = new RedisBase(new RedisSetting(REDIS_TYPE.ONLY_SUBCRIBE, __CONFIG.REDIS_PORT_WRITE));
        redis.PSUBSCRIBE(m_channel);
        var bs = new List<byte>();
        while (__running)
        {
            if (!redis.m_stream.DataAvailable)
            {
                if (bs.Count > 0)
                {
                    if (isReply)
                        m_reply(bs.ToArray());
                    else if (isRequest)
                    {
                        oRequestService rs = oRequestService.Load(bs.ToArray());
                        if (rs != null)
                            m_request(rs);
                    }
                    bs.Clear();
                }
                //Thread.Sleep(100);
                continue;
            }
            byte b = (byte)redis.m_stream.ReadByte();
            bs.Add((byte)b);
        }
    }

    public void Stop()
    {
        __running = false;
    }

}
