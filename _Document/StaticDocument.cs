using System.IO;

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

public class oDocumentReply
{
    public string request_id { get; set; }
    public string command { get; set; }
    public string tag { get; set; }
    public int ok { get; set; }
    public long doc_id { get; set; }
    public int page { get; set; }
    public string file { get; set; }
    public string error { get; set; }

    public string input { get; set; }
    public string output { get; set; }
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
    public static oDocumentReply ToDocumentReply(byte[] buf)
    {
        try
        {
            string replyMessage = System.Text.Encoding.UTF8.GetString(buf);
            var a = replyMessage.Split('^');
            if (a.Length == 10)
            {
                var o = new oDocumentReply();
                o.request_id = a[0];
                o.command = a[1];
                o.tag = a[2];
                o.ok = int.Parse(a[3]);
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
