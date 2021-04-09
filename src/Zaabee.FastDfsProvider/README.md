# Zaabee.FastDfsProvider

[FastDFS](https://github.com/happyfish100/fastdfs) is an open source high performance distributed file system (DFS). It's major functions include: file storing, file syncing and file accessing, and design for high capacity and load balance.

## QuickStart

### NuGet

```bash
Install-Package Zaaby.DFS.FastDfsProvider
```

If you need a repository we have a mongo implementation.

```bash
Install-Package Zaaby.DFS.FastDfsProvider.Mongo
```

### Build Project

Create an asp.net core project and import reference in startup.cs

```csharp
using Zaaby.DFS.Core;
using Zaaby.DFS.FastDfsProvider;
using Zaaby.DFS.FastDfsProvider.Mongo;
```

```csharp
//Register the FastDfsClient
services.AddSingleton<IHandler, ZaabyFastDfsClient>(p =>
new ZaabyFastDfsClient(new List<IPEndPoint> {new IPEndPoint(IPAddress.Parse("192.168.78.152"), 22122)},
"group1", services.BuildServiceProvider().GetService<IRepository>()));
Register repository,the repository IS NOT NECCESSARY and you can implementing it what you like.In this example we use mongo,you can get it on Nuget or [here](https://github.com/Mutuduxf/Zaaby.DFS.FastDfsProvider.Mongo)

//Register the FastDfsClient repostory
services.AddSingleton<IRepository, Repository>(p =>
new Repository(new MongoDbConfiger(new List<string> {"192.168.5.61:27017"}, "FlytOaData", "FlytOaDev",
"2016")));
```

Create a controller named DfsDemoController like this(add a gif file in the project named 3.gif) and run the web application

```charp
[Route("api/[controller]/[action]")]
public class DfsDemoController : Controller
{
    private readonly Zaaby.DFS.Core.IHandler _dfsHandler;

    public DfsDemoController(Zaaby.DFS.Core.IHandler dfsHandler)
    {
        _dfsHandler = dfsHandler;
    }

    [HttpGet]
    [HttpPost]
    public string UploadFile()
    {
        var fileName = "3.gif";

        var uploadFile = System.IO.File.ReadAllBytes(fileName);

        return _dfsHandler.UploadFile(uploadFile, fileName);
    }

    [HttpGet]
    [HttpPost]
    public FileResult DownloadFile(string dfsFileName)
    {
        return File(_dfsHandler.DownloadFile(dfsFileName),"text/plain","test.gif");
    }

    [HttpGet]
    [HttpPost]
    public void RemoveFile(string dfsFileName)
    {
        _dfsHandler.RemoveFile(dfsFileName);
    }
}
```

### Request the webapi

#### Upload file

Now you can access [http://localhost:5000/api/DfsDemo/UploadFile](http://localhost:5000/api/DfsDemo/UploadFile) in a browser and it will return a DFS filename in the response like "M00/00/00/wKhOmVpHXsyAZbMZAAJ7jVznybQ162.gif".You can access the storage server like [http://192.168.78.155:8080/group1/M00/00/00/wKhOmVpHW6CANytuAAJ7jVznybQ240.gif](http://192.168.78.155:8080/group1/M00/00/00/wKhOmVpHW6CANytuAAJ7jVznybQ240.gif)(not the tracker server) to check the file whether is uploaded

#### Download file

Access [http://localhost:5000/api/DfsDemo/DownloadFile?dfsFileName=M00/00/00/wKhOmVpHW6CANytuAAJ7jVznybQ240.gif](http://localhost:5000/api/DfsDemo/DownloadFile?dfsFileName=M00/00/00/wKhOmVpHW6CANytuAAJ7jVznybQ240.gif) in the browser and download the file

#### Remove file

Request [http://localhost:5000/api/DfsDemo/RemoveFile?dfsFileName=M00/00/00/wKhOmVpHkBiAK2AdAAJ7jVznybQ650.gif](http://localhost:5000/api/DfsDemo/RemoveFile?dfsFileName=M00/00/00/wKhOmVpHkBiAK2AdAAJ7jVznybQ650.gif) to remove the file in FastDFS and the file data in resposity(in this example is mongo)
