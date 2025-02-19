REM edit BruTile.Common.props to set the wanted version
REM Put the sln in Release mode and rebuild all. The version is retrieved from the Release version of BruTile.dll
REM call this script without arguments from the sln folder.
call dotnet build Tools\GetVersionFromAssembly\GetVersionFromAssembly.csproj /property:Configuration=Release
REM todo: Call run on the csproj. That is possible nowadays.
for /f "delims=" %%i in ('dotnet Tools\GetVersionFromAssembly\bin\Release\netcoreapp3.1\GetVersionFromAssembly.dll .\Release\netstandard2.0\BruTile.dll') do set VERSION=%%i
ECHO VERSION is: %VERSION%

msbuild brutile.sln /property:Configuration=Release

nuget push .\Release\brutile.%VERSION%.nupkg -source nuget.org
nuget push .\Release\brutile.mbtiles.%VERSION%.nupkg -source nuget.org
git commit -m %VERSION% -a
git tag %VERSION%
git push origin %VERSION%
git push

