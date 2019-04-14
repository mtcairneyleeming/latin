dotnet publish ..\learning-gui.csproj -c Release -r win10-x64
$output = [Environment]::GetFolderPath("MyDocuments") + "/latin.exe"
.\warp-packer.exe --arch windows-x64 --input_dir ../bin/Release/netcoreapp2.2/win10-x64/publish/ --exec learning-gui.exe --output $output
