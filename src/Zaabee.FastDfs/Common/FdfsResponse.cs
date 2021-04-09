using System.IO;

namespace Zaabee.FastDfs.Common
{

    public class FdfsResponse
    {
        public virtual void ReceiveResponse(Stream stream, long length)
        {
            var content = new byte[length];
            stream.Read(content, 0, (int) length);
            LoadContent(content);
        }

        protected virtual void LoadContent(byte[] content)
        {
        }
    }
}