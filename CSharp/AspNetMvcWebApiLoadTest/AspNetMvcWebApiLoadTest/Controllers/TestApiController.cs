using AspNetMvcWebApiLoadTest.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetMvcWebApiLoadTest.Controllers
{
    [RoutePrefix("api")]
    public class TestApiController : ApiController
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> Get(int num)
        {
            _logger.Info("number = {0}", num);

            return Ok();
        }

        [HttpPost]
        [Route("post")]
        public async Task<IHttpActionResult> PostWithRequestBody(PostDataViewModel model)
        {
            if (model == null)
            {
                return BadRequest("do you forget something");
            }

            _logger.Info("Id = {0}", model.Id);

            return Ok();
        }
    }
}
