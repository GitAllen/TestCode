using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Microsoft.ServiceFabric.Actors;

namespace HelloWorld.ActorService
{
    public class HelloWorldActor : Actor, IHelloWorld
    {
        public Task<string> SayHello(string greeting)
        {
            return Task.FromResult("You said: '" + greeting + "'");
        }
    }
}