# Zaabee.FastDfs

FastDFS client(.net standard 2.0).

[FastDFS](https://github.com/happyfish100/fastdfs) is an open source high performance distributed file system (DFS). It's major functions include: file storing, file syncing and file accessing, and design for high capacity and load balance.

InitClient

```csharp
//The IP 192.168.5.100 is tracker server
var client = new FastDfsClient(new List<IPEndPoint> {new IPEndPoint(IPAddress.Parse("192.168.5.222"), 22122)});
```

GetStorageNode

```csharp
var storageNode = _client.GetStorageNode("group1");
var endPoint = storageNode.EndPoint;
var nodeGroupName = storageNode.GroupName;
Assert.Contains(endPoint.Address.ToString(), new List<string> {"192.168.78.153", "192.168.78.154", "192.168.78.155"});
Assert.Equal(nodeGroupName, "group1");
FastDfsOperation

var storageNode = _client.GetStorageNode(groupName);
var uploadFile = File.ReadAllBytes(filePath);

var fileName = _client.UploadFile(storageNode, uploadFile, fileExt);

var downloadFile = _client.DownloadFile(storageNode, fileName);

_client.RemoveFile(groupName, fileName);

Assert.Equal(Md5Helper.Get32Md5(uploadFile), Md5Helper.Get32Md5(downloadFile));
```