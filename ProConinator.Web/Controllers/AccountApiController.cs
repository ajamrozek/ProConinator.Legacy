using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProConinator.Web.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProConinator.Web.Controllers
{
    public class AccountApiController : ApiController
    {

        public HttpResponseMessage Login(LoginViewModel model)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            var user = userManager.Find(model.UserName, model.Password);
            var response = Request.CreateResponse(user == null ? HttpStatusCode.Unauthorized : HttpStatusCode.OK);
            
            return response;
        }
    }
}
