using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class StaticImage {
    static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
        foreach (ImageCodecInfo codec in codecs)
            if (codec.FormatID == format.Guid)
                return codec;
        return null;
    }

    // using the highest possible quality level when saving the Jpeg.
    public static byte[] saveJpg(Bitmap bitmap)
    {
        // Get a bitmap.
        //var bitmap = new Bitmap(@"c:\TestPhoto.jpg");
        ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

        // Create an Encoder object based on the GUID 
        // for the Quality parameter category.
        var myEncoder = System.Drawing.Imaging.Encoder.Quality;

        // Create an EncoderParameters object. 
        // An EncoderParameters object has an array of EncoderParameter 
        // objects. In this case, there is only one 
        // EncoderParameter object in the array.
        EncoderParameters myEncoderParameters = new EncoderParameters(1);

        // Save the bitmap as a JPG file with zero quality level compression.
        var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        //bitmap.Save(@"c:\TestPhotoQualityHundred.jpg", jgpEncoder, myEncoderParameters);
        using (var ms = new MemoryStream())
        {
            bitmap.Save(ms, jgpEncoder, myEncoderParameters);
            return ms.ToArray();
        }
    }

}