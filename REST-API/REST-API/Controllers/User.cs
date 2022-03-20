using Microsoft.AspNetCore.Mvc;
using REST_API.Attributes;

namespace REST_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Anti_BruteForce]
    public class Register : ControllerBase
    {
        [HttpPost]
        public string RegisterUser(string Username, string Password, string Key)
        {
            string result = DataBase.Register(Username, Password, Key);
            if (result.StartsWith("Succesfully"))
            {
                new RequestIP(HttpContext).SuccessFullyRequest();
            }
            return result;
        }
    }
    [ApiController]
    [Route("[controller]")]
    [Attributes.Anti_BruteForce]
    public class CreateRegisterKey : ControllerBase
    {
        [HttpPost]
        public string RegisterUser(string Username, string Password)
        {
            if (!DataBase.TryLogin(Username, Password)) return "Login Failed!";
            new RequestIP(HttpContext).SuccessFullyRequest();
            return DataBase.CreateRegisterKey();
        }
    }
    public class User
    {
#pragma warning disable CS8618
        public string username { get; set; }
        public string passwordhash { get; set; }
#pragma warning restore CS8618
    }
}
