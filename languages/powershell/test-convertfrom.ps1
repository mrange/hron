param([switch]$LongRunning)

# load hron 
$root = Split-Path $MyInvocation.MyCommand.Definition 
. $root\hron.ps1

# initialize
$base = Join-Path $root ..\..\reference-data | Resolve-Path 
$script:log = $null

function Write-Debug 
{ 
    param([Parameter(Mandatory=$true)][string]$Message)
    $script:log.Add($message) | Out-Null
}

function Run-Test($hronFile, $hronRefLog)
{
    # create a new log
    $script:log = New-Object System.Collections.ArrayList

    # run
    Write-Host "---------------------------------------------------------------------------"
    Write-Host "Parsing $hronFile."
    $time = Measure-Command {
        Get-Content $hronFile | ConvertFrom-HRON
    }

    # analyze logs
    $reflog = Get-Content $hronRefLog
    $result = Compare-Object $script:log $reflog 
    if ($result)
    {
        Write-Host -ForegroundColor Red "Test failed. Action log produced when parsning $hronFile did not match reference $hronRefLog."
        $fileName = Join-Path $root ((Split-Path -Leaf $hronFile) + ".errorlog")
        $script:log | Out-File -Encoding utf8 $fileName
        Write-Host "Temporary action log was saved in $fileName"
    }
    else
    {
        Write-Host -ForegroundColor Green "Ok. (Took $($time.TotalSeconds))"
    }

    # clean up
    $script:log = $null
}

Clear-Host
Run-Test $base\simple.hron $base\simple.hron.actionlog
Run-Test $base\helloworld.hron $base\helloworld.hron.actionlog
Run-Test $base\random.hron $base\random.hron.actionlog
if ($longRunning) {
    Run-Test $base\large.hron $base\large.hron.actionlog
}
