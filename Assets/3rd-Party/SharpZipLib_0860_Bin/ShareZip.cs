using System.IO;

public class ShareZip {

    public static byte[] CompressByteToByte(byte[] inputBytes)
    {
        MemoryStream ms = new MemoryStream();
        Stream stream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(ms);
        try
        {
            stream.Write(inputBytes, 0, inputBytes.Length);
        }
        finally
        {
            stream.Close();
            ms.Close();
        }
        return ms.ToArray();
    }

    public static MemoryStream DecompressByteToMS(byte[] inputBytes)
    {
        MemoryStream ms = new MemoryStream(inputBytes);
        Stream sm = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(ms);
        byte[] data = new byte[sm.Length];
        int count = 0;
        MemoryStream re = new MemoryStream();
        while ((count = sm.Read(data, 0, data.Length)) != 0)
        {
            re.Write(data, 0, count);
        }
        re.Seek(0, SeekOrigin.Begin);

        sm.Close();
        ms.Close();
        return re;
    }

    public static byte[] DecompressByteToByte(byte[] inputBytes)
    {
        var ms = DecompressByteToMS(inputBytes);
        var ret = ms.ToArray();
        ms.Close();
        return ret;
    }
}
