
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public static class ActionExecutingContext_Extensions
{
    public static void Response(this ActionExecutingContext context,int code, string Message)
    {
        context.Result = new ContentResult()
        {
            Content = Message,
            StatusCode = code
        };
    }
}

