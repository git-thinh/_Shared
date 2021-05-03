using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

    public Dictionary<string, string> infos { set; get; }
    public string metadata { set; get; }

    public int page_current { set; get; }
    public int page_total { set; get; }

    public byte[] page_image { set; get; }

    public string GetPageTitle()
        => StaticDocument.GetPageTitle(this.page_current + 1, this.page_total, Path.GetFileNameWithoutExtension(this.file_path), this.id);
}
