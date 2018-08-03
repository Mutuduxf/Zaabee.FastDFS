namespace Zaaby.DFS.Core
{
    public interface IRepository
    {
        void Add(FileDfsInfo fileDfsInfo);
        void DeleteByDfsFileName(string dfsFileName);
    }
}