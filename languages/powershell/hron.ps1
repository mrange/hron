$script:linecount = 1

$logfile = ".\log.txt"

Remove-Item $logfile

function Write-Debug 
{ 
    param([Parameter(Mandatory=$true)][string]$Message)
    $Message | Out-File $logfile -Append 
#    $d = Get-Command Write-Debug -CommandType cmdlet; & $d $Message; 
#    if ($global:__DebugInfo.Enabled) {
#        $global:__DebugInfo.Messages.Add($Message) > $null
#   }
}
function Eat-Line([ref]$lines)
{ 
    $script:linecount++;
    $line = $lines.Value[0]
#    Write-Verbose "$($script:linecount)>>> $line"
    $lines.Value = $lines.Value[1..($lines.Value.Length)] 
}

function Parse-String([ref]$lines, $indent)
{
    $sb = New-Object System.Text.StringBuilder
    while($lines.Value.Count -gt 0)
    {
        $line = $lines.Value[0]
        if ($line -match "^\s*$")
        {
            Write-Debug "EmptyLine:$line"
            Eat-Line $lines
            $sb.AppendLine() | Out-Null
        }        
        else
        {
            $pattern = "^(?<indent>^\t{$indent})(?<value>.*)"
            if ($line -match $pattern)
            {
                if (($matches.indent.Length -eq $indent) -or [string]::IsNullOrEmpty($matches.value.Trim()))
                {
                    Write-Debug "ContentLine:$($matches.value)"
                    Eat-Line $lines
                    $sb.AppendLine($matches.value) | Out-Null
                }
                else
                {
                    break;
                }
            }
            elseif ($line -match "^(?<indent>\t*)#(?<comment>.*)")
            {
                Write-Debug "CommentLine:$($matches.indent.Length),$($matches.comment)"
                Eat-Line $lines
            }
            else
            {
                break;
            }
        }
    }
    return $sb.ToString()
}

function AddOrExtend-Member($object, $member, $value)
{
    if ($object | Get-Member $member)
    {
        if ($object -is [array])
        {
            $object.$member += $value
        }
        else
        {
            $object.$member = $object.$member, $value
        }
    }
    else
    {
        $object | Add-Member NoteProperty $member $value
    }
}

function Parse-Members([ref]$lines, $object, $indent)
{
    $margin = New-Object string($indent)
    while($lines.Value.Count -gt 0)
    {
        $line = $lines.Value[0]
        if ($line -match "^\s*$")
        {
            Eat-Line $lines
            Write-Debug "Empty:$line"
        }      
        elseif ($line -match "^(?<indent>\t*)#(?<comment>.*)")
        {
            Eat-Line $lines
            Write-Debug "Comment:$($matches.indent.Length),$($matches.comment)"
        }
        elseif ($line -match "^(?<indent>^\t*)(?<controlchar>@|=)(?<membername>.*)")
        {                       
            if ($indent -eq $matches.indent.Length)
            {
                $memberName = $matches.membername
                Eat-Line $lines
                $value = $null
                switch($matches.controlchar)
                {
                    "@" 
                    {
                        Write-Debug "Object_Begin:$memberName"
                        $value = New-Object PSObject
                        Parse-Members $lines $value ($indent+1)
                        Write-Debug "Object_End:$memberName"
                    }
                    "="
                    {
                        Write-Debug "Value_Begin:$memberName"
                        $value = Parse-String $lines ($indent+1)
                        Write-Debug "Value_End:$memberName"
                    }
                }

                AddOrExtend-Member $object $memberName $value
            }
            else
            {
                break;
            }
        }
        else
        {
            throw "Parse error on line $($script:linecount); expecting empty line, comment or member declaration"
        }
    }
}

function Parse-Header([ref]$lines)
{
    while($lines.Value.Count -gt 0)
    {
        $line = $lines.Value[0]
        if ($line -match "^\s*!(?<directive>.*)")
        {
            Eat-Line $lines
            Write-Debug "PreProcessor:$($matches.directive)"            
        }
        else
        {
            break;
        }
    }
}

function ConvertFrom-HRON($lines)
{
    $result = New-Object psobject
    $linesRef = [ref]$lines
    Parse-Header $linesRef
    Parse-Members $linesRef $result 0
    return $result
}

function Test-HelloWorld
{
    $text = Get-Content ..\..\reference-data\helloworld.hron
    $x = ConvertFrom-HRON $text
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

function Test-Random
{
    $text = Get-Content ..\..\reference-data\random.hron
    $x = ConvertFrom-HRON $text
    write-host $x
}

function Test-Simple
{
    $text = Get-Content ..\..\reference-data\simple.hron
    $x = ConvertFrom-HRON $text
    write-host $x
}

function Test-Large
{
    $text = Get-Content ..\..\reference-data\large.hron
    $x = ConvertFrom-HRON $text
    write-host $x
}

#Test-HelloWorld
Test-Random
#Test-Simple
#Test-Large