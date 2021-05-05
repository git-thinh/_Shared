using System.IO;

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