using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{

    //public class NotImplementedActionResult : ActionResult
    //{
    //    public NotImplementedActionResult(string message)
    //    {
    //        Message = message;
    //    }

    //    public string Message { get; private set; }

    //    public async override Task ExecuteResultAsync(ActionContext context)
    //    {
    //        var response = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(Message),
    //            RequestMessage = _request
    //        };            

    //        await response.ExecuteResultAsync(context);
    //    }
    //}
}
