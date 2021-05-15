public class __CONFIG
{
    public const string UDP_HOST = "127.0.0.1";
    public const int UDP_PORT = 12300;

    public const int REDIS_DB = 15;
    public const string REDIS_HOST = "127.0.0.1";
    public const int REDIS_PORT_WRITE = 1000;
    public const int REDIS_PORT_READ = 1001;

    public const string CHANNEL_NAME = "TT2";

    public static string PATH_TT_RAW = System.Configuration.ConfigurationManager.AppSettings["PATH_TT_RAW"];
    public static string PATH_TT_ZIP = System.Configuration.ConfigurationManager.AppSettings["PATH_TT_ZIP"];

    // Redis receive from subcriber
    public const string MESSAGE_SPLIT_END = "}>\r\n$";
    public const string MESSAGE_SPLIT_BEGIN = "\r\n<{";
    public const int BUFFER_HEADER_MAX_SIZE = 1000;
}
