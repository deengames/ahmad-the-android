$windowsZipFile = 'windows-release.zip'

if (Test-Path("AliTheAndroid\out_windows")) {
    Remove-Item "AliTheAndroid\out_windows" -Recurse
}

# Source: https://stackoverflow.com/questions/44074121/build-net-core-console-application-to-output-an-exe
# Publish to an exe + dependencies. 40MB baseline.
dotnet publish -c Release -r win10-x64 -o out_windows

# Copy all sound-effects over since we're not using the MonoGame content pipeline
Copy-Item -Recurse "AliTheAndroid\Content" "AliTheAndroid\out_windows\Content"

# Zip it up. ~17MB baseline.
if (Test-Path($windowsZipFile)) {
    Remove-Item $windowsZipFile
}

Add-Type -A 'System.IO.Compression.FileSystem'
[IO.Compression.ZipFile]::CreateFromDirectory('AliTheAndroid\out_windows', $windowsZipFile);
Write-Host DONE! Zipped to $windowsZipFile

# for Linux/MacOS builds!
# dotnet publish -c Release -r ubuntu.16.10-x64 -o out_linux
# dotnet publish -c Release -r osx.10.11-x64 -o out_macos