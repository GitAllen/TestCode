A HelloWorld code sample for how to crate a Service Fabric Actor and wrapper it with self-host WebApi controller project.

To play, first build and deploy to local cluster, then visit http://localhost:10281/HelloWorld/api/helloworld?greeting=hello%20world

Trick parts:
- WebApi project must be target x64
- Standard WebApi project won't work, has to make it self-host by OWIN
- Actor Service must be declared as Stateful