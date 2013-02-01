# settings
$correctness = $false
$performance = $true

# load hron 
$root = Split-Path $MyInvocation.MyCommand.Definition 
. $root\hron.ps1

# setup
$base = Join-Path $root ..\..\reference-data | Resolve-Path 

$script:logfile = $null

function Write-Debug 
{ 
    param([Parameter(Mandatory=$true)][string]$Message)
    if ($script:logfile) {
        $message | Out-File $script:logfile -Append 
    }
}

function Test-HelloWorld
{    
    $x = Get-Content $base\helloworld.hron | ConvertFrom-HRON $text
    Write-Host "Common.LogPath: " $x.Common.LogPath 
    Write-Host "Common.Welcomemessage: " $x.Common.WelcomeMessage
    foreach($conn in $x.DataBaseConnection)
    {
        Write-Host "Databaseconnection.Name: " $conn.Name
        Write-Host "Databaseconnection.Timeout: " $conn.Timeout
        Write-Host "Databaseconnection.User.UserName: " $conn.User.UserName
        Write-Host "Databaseconnection.User.Password: " $conn.User.Password
    }
}

Test-HelloWorld

function Run-Test($hronFile, $hronRefLog)
{
    # prepare logging
    $script:logfile = "$root\log.txt"
    if (Test-Path $script:logfile) { Remove-Item -Force $script:logfile }

    # run
    $x = Get-Content $hronFile | ConvertFrom-HRON
    if (!$x) 
    { 
        throw "parse failed" 
    }

    # analyze logs
    $log = Get-Content $script:logfile
    $reflog = Get-Content $hronRefLog
    $result = Compare-Object $log $reflog 
    if ($result)
    {
        throw "logs are not equal"
    }

    # clean up
    if (Test-Path $script:logfile) { Remove-Item -Force $script:logfile }
}

# test correctness
if ($correctness)
{
    Run-Test $base\simple.hron $base\simple.hron.actionlog
    Run-Test $base\helloworld.hron $base\helloworld.hron.actionlog
    Run-Test $base\random.hron $base\random.hron.actionlog
    # only run this if you have plenty of time...
    #Run-Test $base\large.hron $base\reference-data\large.hron.actionlog
}

if ($performance)
{
    # test performance
    $script:logfile = $null
    Measure-Command {
        Get-Content $base\random.hron |  ConvertFrom-HRON
    }
}