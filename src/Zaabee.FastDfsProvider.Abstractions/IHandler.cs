namespace Zaabee.FastDfsProvider.Abstractions
{
    public interface IHandler
    {
        string UploadFile(byte[] fileBytes, string fileName);
        byte[] DownloadFile(string fileName);
        void RemoveFile(string fileName);
    }
}