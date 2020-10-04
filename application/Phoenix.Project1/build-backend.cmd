rmdir /q/s .\Assets\Project\Plugins\Phoenix\
dotnet build ..\..\backend\Regulus.Remote\Regulus.Remote.Client\Regulus.Remote.Client.csproj -o .\Assets\Project\Plugins\Phoenix
dotnet build ..\..\backend\Phoneix.Project1\Phoenix.Project1.Protocol\Phoenix.Project1.Protocol.csproj -o .\Assets\Project\Plugins\Phoenix
del .\Assets\Project\Plugins\Phoenix\*.nupkg

pause