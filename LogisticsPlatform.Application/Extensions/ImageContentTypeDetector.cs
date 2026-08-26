namespace LogisticsPlatform.Application.Extensions;

public static class ImageContentTypeDetector
{
    public static bool TryDetect(ReadOnlySpan<byte> content, out string contentType)
    {
        contentType = null!;

        if (content.Length >= 3
            && content[0] == 0xFF
            && content[1] == 0xD8
            && content[2] == 0xFF)
        {
            contentType = "image/jpeg";
            return true;
        }

        if (content.Length >= 8
            && content[0] == 0x89
            && content[1] == 0x50
            && content[2] == 0x4E
            && content[3] == 0x47
            && content[4] == 0x0D
            && content[5] == 0x0A
            && content[6] == 0x1A
            && content[7] == 0x0A)
        {
            contentType = "image/png";
            return true;
        }

        if (content.Length >= 6
            && content[0] == (byte)'G'
            && content[1] == (byte)'I'
            && content[2] == (byte)'F'
            && content[3] == (byte)'8'
            && (content[4] == (byte)'7' || content[4] == (byte)'9')
            && content[5] == (byte)'a')
        {
            contentType = "image/gif";
            return true;
        }

        if (content.Length >= 12
            && content[0] == (byte)'R'
            && content[1] == (byte)'I'
            && content[2] == (byte)'F'
            && content[3] == (byte)'F'
            && content[8] == (byte)'W'
            && content[9] == (byte)'E'
            && content[10] == (byte)'B'
            && content[11] == (byte)'P')
        {
            contentType = "image/webp";
            return true;
        }

        return false;
    }
}
