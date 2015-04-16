@echo off
msbuild AspNetMvcWebApiLoadTest.sln /p:DeployOnBuild=true /p:PublishProfile=apiloadtest.pubxml /p:Configuration=Release