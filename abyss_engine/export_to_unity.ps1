Write-Output "Deleting ..\abyss_unity\AbyssCLI"
Remove-Item ..\abyss_unity\AbyssCLI -Recurse

Write-Output "Copying \Debug to \AbyssUI\AbyssCLI"
Copy-Item -Path .\bin\Debug\net8.0\* -Destination ..\abyss_unity\AbyssCLI -Recurse

Write-Output "Copying \ABI to \AbyssUI\Assets\Host\ABI"
Copy-Item -Path .\ABI\* -Destination ..\abyss_unity\Assets\Host\ABI -Recurse