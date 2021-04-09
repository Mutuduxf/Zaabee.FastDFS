namespace Zaabee.FastDfsProvider.Repository.Abstractions
{
    public interface IRepository
    {
        void Add(FileDfsInfo fileDfsInfo);
        void DeleteByDfsFileName(string dfsFileName);
    }
}