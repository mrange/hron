#Requires -Version 3

Push-Location "$PSScriptRoot\testdata"
& ".\copy-here.bat"
Pop-Location

Push-Location $PSScriptRoot
& ".\get-mongoose.ps1"
Get-Process mongoose-free-5.5 -ErrorAction SilentlyContinue | Stop-Process
& ".\mongoose-free-5.5.exe"
Pop-Location
