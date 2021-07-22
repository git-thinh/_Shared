public class __CONFIG
{
    public const string UDP_HOST = "127.0.0.1";
    public const int UDP_PORT = 12311;

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

public enum COMMANDS
{
    NONE = 0,
    DOC_INFO = 10,

    PDF_MMF_TT = 11,
    PDF_SPLIT_ALL_PDF = 21,
    PDF_SPLIT_ALL_PNG = 22,
    PDF_SPLIT_ALL_JPG = 23,

    OCR_TEXT_PAGE = 50,
    OCR_TEXT_ALL_PAGE = 51,
    OCR_BOX_PAGE = 62,
    OCR_BOX_ALL_PAGE = 61,

    TRANSLATE_TEXT_GOOGLE_01 = 70,

    CURL_GET_HEADER = 80,
    CURL_GET_HTML = 81,
    CURL_GET_HTML_COOKIE = 82,
    CURL_POST = 84,
    CURL_POST_COOKIE = 85,
    CURL_POST_UPLOAD_FILE_COOKIE = 86,
    CURL_POST_UPLOAD_FILE = 87,
    CURL_FTP_UPLOAD_FILE = 89,

    NODE_SUBCRIBER = 200
}