using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;

namespace HelloWorld.ActorService
{
    public class ServiceHost
    {
        public static void Main(string[] args)
        {
            try
            {
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    ActorRegistration.RegisterActor(fabricRuntime, typeof(HelloWorldActor));

                    ServiceEventSource.Current.ActorTypeRegistered(Process.GetCurrentProcess().Id, typeof(HelloWorldActor).ToString());

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ActorHostInitializationFailed(e);
                throw;
            }
        }
    }
}