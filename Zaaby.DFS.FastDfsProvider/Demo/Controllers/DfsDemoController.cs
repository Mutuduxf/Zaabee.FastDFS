using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
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
}