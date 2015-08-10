using Microsoft.ServiceFabric.Services;

namespace HelloWorld.WebService
{
    public class Service : StatelessService
    {
        public const string ServiceTypeName = "HelloWorldWebServiceType";

        protected override ICommunicationListener CreateCommunicationListener()
        {
            return new OwinCommunicationListener("HelloWorld", new Startup());
        }
    }
}
