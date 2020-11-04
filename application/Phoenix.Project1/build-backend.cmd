rmdir /q/s .\Assets\Project\Plugins\Phoenix\
dotnet publish  ..\..\backend\Regulus.Remote\Regulus.Remote.Client\Regulus.Remote.Client.csproj -o .\Assets\Project\Plugins\Phoenix
dotnet publish ..\..\backend\Regulus.Remote\Regulus.Remote.Standalone\Regulus.Remote.Standalone.csproj -o .\Assets\Project\Plugins\Phoenix
dotnet publish ..\..\backend\Phoneix.Project1\Phoenix.Project1.Protocol\Phoenix.Project1.Protocol.csproj -o .\Assets\Project\Plugins\Phoenix
dotnet publish ..\..\backend\Phoneix.Project1\Phoenix.Project1.Configs\Phoenix.Project1.Configs.csproj -o .\Assets\Project\Plugins\Phoenix
dotnet publish ..\..\backend\Phoneix.Project1\Phoenix.Project1.Client\Phoenix.Project1.Client.csproj -o .\Assets\Project\Plugins\Phoenix
rem dotnet publish ..\..\backend\Phoneix.Project1\Phoenix.Project1.Game\Phoenix.Project1.Game.csproj -o .\Assets\Project\Plugins\Phoenix
del .\Assets\Project\Plugins\Phoenix\*.nupkg
mkdir .\Assets\Project\Plugins\Phoenix\Source
xcopy ..\..\backend\Phoneix.Project1\Phoenix.Project1.Game .\Assets\Project\Plugins\Phoenix\Source /E

rmdir /S /Q .\Assets\Project\Plugins\Phoenix\Source\bin 
rmdir /S /Q .\Assets\Project\Plugins\Phoenix\Source\obj 

pause