using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Zaabee.FastDfs.Config;

namespace Zaabee.FastDfs.Common
{
    /// <summary>
    /// 链接管理池
    /// </summary>
    internal static class ConnectionManager
    {
        private static List<IPEndPoint> _listTrackers = new();

        private static readonly ConcurrentDictionary<IPEndPoint, Pool> TrackerPools = new();

        private static readonly ConcurrentDictionary<IPEndPoint, Pool> StorePools = new();

        private static int _connectionTimeout;

        #region 公共静态方法

        public static bool Initialize(List<IPEndPoint> trackers, int connectionTimeout = 30)
        {
            foreach (var point in trackers.Where(point => !TrackerPools.ContainsKey(point)))
            {
                TrackerPools.TryAdd(point, new Pool(point, FdfsConfig.TrackerMaxConnection));
            }

            _listTrackers = trackers;
            _connectionTimeout = connectionTimeout;
            return true;
        }

        public static bool InitializeForConfigSection(FastDfsConfig config, int connectionTimeout = 30)
        {
            if (config is null) return false;
            var trackers = config.FastDfsServer
                .Select(ipInfo => new IPEndPoint(IPAddress.Parse(ipInfo.IpAddress), ipInfo.Port))
                .ToList();

            return Initialize(trackers, connectionTimeout);
        }

        public static Connection GetTrackerConnection()
        {
            var index = new Random().Next(TrackerPools.Count);

            var pool = TrackerPools[_listTrackers[index]];

            return pool.GetConnection(_connectionTimeout);
        }

        public static Connection GetStorageConnection(IPEndPoint endPoint)
        {
            if (!StorePools.ContainsKey(endPoint))
                StorePools.TryAdd(endPoint, new Pool(endPoint, FdfsConfig.StorageMaxConnection));

            return StorePools[endPoint].GetConnection(_connectionTimeout);
        }

        #endregion
    }
}