using Microsoft.AspNetCore.Mvc;
using REST_API.Attributes;

namespace REST_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Download : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFile(string FileName)
        {
            try
            {
                var file = DataBase.GetFilebyFilename(FileName);
                if (file == null)
                {
                    return HttpContext.HttpResponse(404, "Coudnt find Specific File!");
                }
                file.Dispose();
            }
            catch
            {
                return HttpContext.HttpResponse(500, "File is currently in Upload");
            }
#pragma warning disable CS8604
            return File(DataBase.GetFilebyFilename(FileName), "application/x-msdownload", FileName);
#pragma warning restore CS8604
        }
    }
    [ApiController]
    [Route("[controller]")]
    [Anti_BruteForce]
    public class Files : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFile(string Username, string Password)
        {
            if (!DataBase.TryLogin(Username, Password))
            {
                return HttpContext.HttpResponse(403, "Login Failed!");
            }
            new RequestIP(HttpContext).SuccessFullyRequest();
            string result = string.Empty;
            foreach (string a in DataBase.GetAllFiles())
            {
                result += a + "\n"; 
            }
            return HttpContext.HttpResponse(200, result);
        }
    }
    [ApiController]
    [Route("[controller]")]
    [Anti_BruteForce]
    public class Delete : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFile(string Username, string Password, string FileName)
        {
            if (!DataBase.TryLogin(Username, Password))
            {
                return HttpContext.HttpResponse(403, "Login Failed!");
            }
            new RequestIP(HttpContext).SuccessFullyRequest();
            DataBase.DeleteFile(FileName);
            return HttpContext.HttpResponse(200, "Successfully Deleted File!");
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class Upload : ControllerBase
    {
        [HttpPost]
        public ContentResult PostFile(string FileName, [FromBody] string Base64)
        {
            if (DataBase.FileExist(FileName))
            {
                return HttpContext.HttpResponse(403, "The File already Exist use a other FileName!");
            }
            try
            {
                DataBase.UploadFile(FileName, Base64);
                return HttpContext.HttpResponse(200, "SuccessFully Uploaded!");
            }
            catch (Exception e)
            {
                return HttpContext.HttpResponse(403, "Failed to Upload Reason: "+e);
            }
        }
    }
}