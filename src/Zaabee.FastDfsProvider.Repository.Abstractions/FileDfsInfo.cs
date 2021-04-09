using System;

namespace Zaabee.FastDfsProvider.Repository.Abstractions
{
    public class FileDfsInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string DfsFileName { get; set; }
        public DateTime UtcCreateTime { get; set; }
    }
}