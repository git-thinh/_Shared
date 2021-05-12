using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.MMF;

public enum MMF_TYPE
{
    NONE = 0,
    TEXT = 1,
    HTML = 2,
    JSON = 3,
    XML = 4,
    PNG = 5,
    JPG = 6
}

public class oMMF
{
    public long id { set; get; }
    public bool compress { set; get; }
    public MMF_TYPE type { set; get; }
    public Dictionary<string, string> infos { set; get; }
    public List<int> sizes { set; get; }
}

public static class StaticMMF
{
    public static void Write(string name, byte[] buf)
    {
        int size = buf.Length;
        MemoryMappedFile map = MemoryMappedFile.Create(MapProtection.PageReadWrite, size, name);
        using (Stream view = map.MapView(MapAccess.FileMapWrite, 0, size))
            view.Write(buf, 0, size);
        map.Close();
    }

    public static byte[] GetBuffer(string name, int size)
    {
        if (size < 0) size = 0;
        byte[] buf = new byte[size];
        if (size > 0)
        {
            MemoryMappedFile map = MemoryMappedFile.Create(MapProtection.PageReadOnly, size, name);
            using (Stream view = map.MapView(MapAccess.FileMapRead, 0, size))
                view.Read(buf, 0, size);
            map.Close();
        }
        return buf;
    }

    public static Bitmap GetBitmap(string name, int size)
    {
        byte[] buf = GetBuffer(name, size);
        if (buf.Length > 0)
        {
            using (var ms = new MemoryStream(buf))
                return new Bitmap(ms);
        }
        return null;
    }
}
