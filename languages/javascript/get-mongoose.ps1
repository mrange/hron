#Requires –Version 3
param($targetFolder)

$downloadUrl = "http://cesanta.com/cgi-bin/api.cgi?act=dl&id=7NBB80IL1YZZ9ZXN&os=win" 
if (!$targetFolder) {
    $targetFolder = $PSScriptRoot # Join-Path $PSScriptRoot ".." | Resolve-Path
}
$target = Join-Path $targetFolder mongoose-free-5.5.exe

if (Test-Path $target) {
    Write-Host "Mongoose already exists. Nothing done"
    return
}

Write-Host "Downloading Mongoose..." -NoNewline
Invoke-WebRequest -Uri $downloadUrl -OutFile $target
Write-Host "Done."

Write-Host "Unblock file to allow execution"
Unblock-File $target

Write-Host "Completed successfully"

