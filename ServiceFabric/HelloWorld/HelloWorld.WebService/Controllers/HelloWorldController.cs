using System;
using System.Web.Http;
using HelloWorld.Interfaces;
using Microsoft.ServiceFabric.Actors;

namespace HelloWorld.WebService.Controllers
{
    public class HelloWorldController : ApiController
    {
        private const string ApplicationName = "fabric:/HelloWorldApplication";

        // GET api/helloworld
        public string Get(string greeting)
        {
            try
            {
                IHelloWorld friend = ActorProxy.Create<IHelloWorld>(ActorId.NewId(), ApplicationName);
                return string.Format("From Actor {1}: {0}", friend.SayHello(greeting).Result, friend.GetActorId());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}