# load hron 
$root = Split-Path $MyInvocation.MyCommand.Definition 
. $root\hron.ps1

function BasicTest
{
    $y = New-Object PSObject -Property @{ Q=1; W=2 }
    $z = New-Object PSObject -Property @{ Q=3; W=4 }

    $x = New-Object PSObject -Property @{
        A=10
        B=20.3
        C="Daniel"
        D=New-Object PSObject -Property @{
            AA=100
            BB=300.343
            CC="jdslkjlsk"
        }    
        E=1,2,3,"dds"
        F=$y,$z
    }

    ConvertTo-HRON $x
}

# setup
$base = Join-Path $root ..\..\reference-data | Resolve-Path 

# test correctness
$original = Get-Content $base\simple.hron 
$intermediate = $original | ConvertFrom-HRON
$converted = $intermediate | ConvertTo-HRON
$result = Compare-Object $original $converted
if ($result)
{
    throw "test failed"
}

