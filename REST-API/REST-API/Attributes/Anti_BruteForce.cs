using Microsoft.AspNetCore.Mvc.Filters;

namespace REST_API.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.All, AllowMultiple = true)]
    public class Anti_BruteForce : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var current = new RequestIP(context.HttpContext);
            if (current.RateLimited)
            {
                DateTime LastAccess = DateTime.Parse(current.LastAccesTime);
                int result = DateTime.Compare(DateTime.Now, LastAccess.AddSeconds(current.RateLimitTime));
                if (result == 0 || result == 1)
                {
                    current.RateLimited = false;
                    current.Reset();
                    current.UpadateLastAccess();
                    current.Save();
                    await next();
                    return;
                }
                else
                {
                    if (current.RateLimitTime < 268435456)
                    {
                        current.RateLimitTime = (current.RateLimitTime * 2);
                    }
                    context.Result = context.HttpContext.HttpResponse(403, $"You are RateLimited for {current.RateLimitTime} seconds");
                    current.UpadateLastAccess();
                    current.Save();
                    return; 
                }
            }
            else
            {
                if (current.AccessCount > 5)
                {
                    current.RateLimited = true;
                    current.AccessCount = 0;
                    current.RateLimitTime = ((10* current.RateLimitedCount)+5);
                    context.Result = context.HttpContext.HttpResponse(403, $"You got RateLimited for {current.RateLimitTime} seconds");
                    current.RateLimitedCount++;
                    current.UpadateLastAccess();
                    current.Save();
                    return;
                }
                else
                {
                    current.AccessCount++;
                    current.UpadateLastAccess();
                    current.Save();
                    await next();
                    return;
                }
            }
        }
    }
    public class RequestIP
    {
        static Dictionary<string, RequestIP> Requests = new Dictionary<string, RequestIP>();
        public RequestIP(HttpContext context)
        {
            if (Requests.TryGetValue(context.GetRemoteIPAddress(), out RequestIP value))
            {
                this.IP = value.IP;
                this.AccessCount = value.AccessCount;
                this.AccessCount = value.AccessCount + 1;
                this.RateLimited = value.RateLimited;
                this.RateLimitedCount = value.RateLimitedCount;
                this.RateLimitTime = value.RateLimitTime;
                this.LastAccesTime = value.LastAccesTime;
            }
            else
            {
                this.IP = context.GetRemoteIPAddress();
                this.AccessCount = 1;
                this.RateLimited = false;
                this.RateLimitTime = 0;
                this.RateLimitedCount = 0;
                UpadateLastAccess();
                Requests.Add(IP, this);
            }
        }
        public void Reset()
        {
            AccessCount = 0;
        }
        public void SuccessFullyRequest()
        {
            AccessCount = 0;
            RateLimitedCount = 0;
            UpadateLastAccess();
            Save();
        }
        public void Save()
        {
            Requests.Remove(IP);
            Requests.Add(IP, this);
        }
        public void UpadateLastAccess()
        {
            LastAccesTime = DateTime.Now.ToString();
        }
        public string IP { get; set; }
        public int AccessCount { get; set; } 
        public bool RateLimited { get; set; }
        public int RateLimitTime { get; set; }
        public int RateLimitedCount { get; set; }
        public string LastAccesTime { get; set; }
    }
}
