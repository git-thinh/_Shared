using System.IO;

public enum DOC_CMD
{
    DOC_INFO = 0,

    PDF_SPLIT_ALL_PDF = 11,
    PDF_SPLIT_ALL_PNG = 12,
    PDF_SPLIT_ALL_JPG = 13,
}

public enum DOC_TYPE
{
    INFO_OGRINAL = 10,
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

public class oDocumentStatus
{
    public string request_id { get; }
    public string cmd { get; }
    public int ok { get; }
    public long doc_id { get; }
    public int page { get; }
    public string file { get; }
    public string error { get; }

    public oDocumentStatus(string replyMessage) {
        var a = replyMessage.Split('^');
        if (a.Length == 7)
        {
            this.request_id = a[0];
            this.cmd = a[1];
            this.ok = int.Parse(a[2]);
            this.doc_id = long.Parse(a[3]);
            this.page = int.Parse(a[4]);
            this.file = a[5];
            this.error = a[6];
        }
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
