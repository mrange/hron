param([switch]$LongRunning)

# load hron 
$root = Split-Path $MyInvocation.MyCommand.Definition 
. $root\hron.ps1

# initialize
$base = Join-Path $root ..\..\reference-data | Resolve-Path 
$script:log = $null
$delimiterSize = if ($Host.UI.RawUI.BufferSize.Width) { $Host.UI.RawUI.BufferSize.Width-2 } else { 78 }
$delimiter = '-' * $delimiterSize

function Write-Debug 
{ 
    param([Parameter(Mandatory=$true)][string]$Message)
    if ($script:log.Add) {
        $script:log.Add($message) | Out-Null
    }
}

function Test-ConvertFrom($hronFile, $hronRefLog)
{
    # create a new log
    $script:log = New-Object System.Collections.ArrayList

    # run
    Write-Host "$delimiter"
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

# this test will not work in ps 2 since the hron serializer does not guarantee property order
# this test does not work if the reference file contains comments
function Test-ConvertTo($refHronFile)
{
    Write-Host "$delimiter"
    
    $script:log = $null
    $refSerialized = Get-Content $base\simple.hron 
    $hronObject = $refSerialized | ConvertFrom-HRON

    Write-Host "Serializing hron object (reference file: $refHronFile)"

    if ($host.Version.Major -lt 3) {
        Write-Host -ForegroundColor Red "This test might not succeed in powershell 2 or lower since property order is not guaranteed by the hron serializer."
    }

    $serialized = [ref]$null
    $time = Measure-Command {
        $serialized.Value = $hronObject | ConvertTo-HRON
    }

    $result = Compare-Object $serialized.Value $refSerialized 
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
}


Clear-Host
Test-ConvertFrom $base\simple.hron $base\simple.hron.actionlog
Test-ConvertFrom $base\helloworld.hron $base\helloworld.hron.actionlog
Test-ConvertFrom $base\random.hron $base\random.hron.actionlog
if ($longRunning) {
    Test-ConvertFrom $base\large.hron $base\large.hron.actionlog
}
Test-ConvertTo $base\simple.hron 