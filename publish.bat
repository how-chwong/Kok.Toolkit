chcp 65001
set PUBLISH_PATH=.\publish


dotnet pack .\src\Kok.Toolkit.Core\Kok.Toolkit.Core.csproj -c Release -o %PUBLISH_PATH%
dotnet pack .\src\Kok.Toolkit.Wpf\Kok.Toolkit.Wpf.csproj -c Release -o %PUBLISH_PATH%



pause
