public enum TESSERACT_DATA
{
    TEXT,
    JSON,
    IMAGE
}

public enum ENGINE_MODE
{
    TESSERACT_ONLY,
    LSTM_ONLY,
    TESSERACT_AND_LSTM,
    DEFAULT
}

public enum TESSERACT_COMMAND
{
    GET_TEXT,
    GET_SEGMENTED_REGION_BLOCK,
    GET_SEGMENTED_REGION_PARA,
    GET_SEGMENTED_REGION_TEXTLINE,
    GET_SEGMENTED_REGION_WORD,
    GET_SEGMENTED_REGION_SYMBOL
}

public class oTesseractRequest
{
    public int ok { get; set; }
    public long time_created { get; set; }

    public ENGINE_MODE mode { get; set; }
    public TESSERACT_COMMAND command { get; set; }

    public string redis_key { get; set; }
    public string redis_field { get; set; }

    public string lang { get; set; }
    public string data_path { get; set; }

    public string output_channel { get; set; }
    public TESSERACT_DATA output_type { get; set; }

    public int output_count { get; set; }
    public bool output_image { get; set; }
    public string output_format { get; set; }
    public string output_text { get; set; }

    public oTesseractRequest(TESSERACT_COMMAND command_, bool outputImage = false)
    {
        output_image = outputImage;
        time_created = long.Parse(System.DateTime.Now.ToString("yyMMddHHmmssfff"));
        command = command_;
        mode = ENGINE_MODE.DEFAULT;
        lang = "eng";
        data_path = @"tessdata";
    }

    [Newtonsoft.Json.JsonIgnore]
    public string requestId {
        get {
            return string.Format("{0}_{1}|{2}_{3}|{4}", redis_key, redis_field, (int)command, (int)mode, time_created);
        }
    }
}
