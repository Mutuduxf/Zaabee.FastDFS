using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Zaabee.FastDfsClient.Config;

namespace Zaabee.FastDfsClient.Common
{
    /// <summary>
    /// 链接管理池
    /// </summary>
    internal static class ConnectionManager
    {
        #region 私有字段

        private static List<IPEndPoint> _listTrackers = new List<IPEndPoint>();

        #endregion

        #region 公共静态字段

        private static readonly ConcurrentDictionary<IPEndPoint, Pool> TrackerPools =
            new ConcurrentDictionary<IPEndPoint, Pool>();

        private static readonly ConcurrentDictionary<IPEndPoint, Pool> StorePools =
            new ConcurrentDictionary<IPEndPoint, Pool>();

        #endregion

        #region 公共静态方法

        public static bool Initialize(List<IPEndPoint> trackers)
        {
            foreach (var point in trackers)
            {
                if (!TrackerPools.ContainsKey(point))
                    TrackerPools.TryAdd(point, new Pool(point, FdfsConfig.TrackerMaxConnection));
            }

            _listTrackers = trackers;

            return true;
        }

        public static bool InitializeForConfigSection(FastDfsConfig config)
        {
            if (config == null) return false;
            var trackers = new List<IPEndPoint>();

            foreach (var ipInfo in config.FastDfsServer)
            {
                trackers.Add(new IPEndPoint(IPAddress.Parse(ipInfo.IpAddress), ipInfo.Port));
            }

            return Initialize(trackers);

        }

        public static Connection GetTrackerConnection()
        {
            var index = new Random().Next(TrackerPools.Count);

            var pool = TrackerPools[_listTrackers[index]];

            return pool.GetConnection();
        }

        public static Connection GetStorageConnection(IPEndPoint endPoint)
        {
            if (!StorePools.ContainsKey(endPoint))
                StorePools.TryAdd(endPoint, new Pool(endPoint, FdfsConfig.StorageMaxConnection));

            return StorePools[endPoint].GetConnection();
        }

        #endregion
    }
}