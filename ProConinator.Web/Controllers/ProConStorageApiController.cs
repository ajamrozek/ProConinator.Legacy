using ProConinator.Web.Data;
using ProConinator.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProConinator.Web.Controllers
{
    public class ProConStorageApiController : ApiController
    {
        private readonly IProConinatorRepository _proConRepo;

        public ProConStorageApiController(IProConinatorRepository proConRepo){
            _proConRepo = proConRepo;
        }

        // GET api/<controller>
        public IEnumerable<ProConGroupModel> GetList(string userId)
        {
            var result = _proConRepo.List(userId)
                .OrderByDescending(group => group.ModifiedDate);
            
            return result;
        }

        // POST api/<controller>
        [HttpPost]
        public HttpResponseMessage SaveGroup(ProConGroupModel value)
        {
            value.ModifiedDate = DateTime.UtcNow;

            var result = _proConRepo.Save(value);
            return Request.CreateResponse<ProConGroupModel>(HttpStatusCode.OK, result);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpPost]
        public void DeleteGroup(string userId, string groupId)
        {
            //_proConRepo.D
            _proConRepo.DeleteDocument(userId, groupId);
        }
    }
}