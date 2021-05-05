using System;
using System.Collections.Generic;
using System.Text;

public static class StaticPdf
{
    public static byte[] BuildRequest(string guid, PDF_COMMAND cmd, PDF_STORE store, string file)
    {
        var ls = new List<byte>();
        ls.AddRange(Encoding.ASCII.GetBytes(guid));
        ls.Add((byte)cmd);
        ls.Add((byte)store);
        ls.AddRange(Encoding.UTF8.GetBytes(file));
        return ls.ToArray();
    }
}

public enum PDF_STORE
{
    MMF = 10,
    REDIS = 11,
    FILE = 12
}

public enum PDF_COMMAND
{
    GET_DOC_INFO = 0,

    SPLIT_ALL_PDF = 11,
    SPLIT_ALL_PNG = 12,
    SPLIT_ALL_JPG = 13,

    //SPLIT_PAGE_PDF = 30,
    //SPLIT_PAGE_PNG = 31,
    //SPLIT_PAGE_JPG = 32,

    //EXTRACT_IMAGE = 50,
    //EXTRACT_TEXT = 51,

    //CLEAR_MMF = 90,
}

