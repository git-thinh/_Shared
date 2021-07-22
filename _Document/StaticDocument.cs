using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;



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

public static class DictionaryExt
{
    public static T Get<T>(this Dictionary<string, object> dic, string key, T def = default(T))
    {
        if (dic.ContainsKey(key))
        {
            object v = dic[key];
            T o = (T)Convert.ChangeType(v, typeof(T));
            return o;
        }
        return def;
    }
}

/*--------------------------------------*/

public enum EngineMode : int
{
    /// <summary>
    /// Only the legacy tesseract OCR engine is used.
    /// </summary>
    TesseractOnly = 0,

    /// <summary>
    /// Only the new LSTM-based OCR engine is used.
    /// </summary>
    LstmOnly,

    /// <summary>
    /// Both the legacy and new LSTM based OCR engine is used.
    /// </summary>
    TesseractAndLstm,

    /// <summary>
    /// The default OCR engine is used (currently LSTM-ased OCR engine).
    /// </summary>
    Default
}