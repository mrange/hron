$script:linecount = 1

function Eat-Line([ref]$lines)
{ 
    $script:linecount++;
    $line = $lines.Value[0]
    Write-Debug "$($script:linecount)>>> $line"
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
                    Eat-Line $lines
                    $sb.AppendLine($matches.value) | Out-Null
                }
                else
                {
                    break;
                }
            }
            elseif ($line -match "^\s*#")
            {
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
    Write-Debug "$margin Beginning of object"
    while($lines.Value.Count -gt 0)
    {
        $line = $lines.Value[0]
        if ($line -match "^\s*$")
        {
            Eat-Line $lines
            Write-Debug "$margin Empty line"
        }      
        elseif ($line -match "^\s*#")
        {
            Eat-Line $lines
            Write-Debug "$margin Comment"
        }
        elseif ($line -match "^(?<indent>^\t*)(?<controlchar>@|=)(?<membername>.*)")
        {                       
            if ($indent -eq $matches.indent.Length)
            {
                Eat-Line $lines
                Write-Debug "$margin Member $($matches.membername)"
                $value = $null
                switch($matches.controlchar)
                {
                    "@" 
                    {
                        $value = New-Object PSObject
                        Parse-Members $lines $value ($indent+1)
                    }
                    "="
                    {
                        $value = Parse-String $lines ($indent+1)
                    }
                }

                AddOrExtend-Member $object $matches.membername $value
            }
            else
            {
                Write-Debug "$margin End of object reached"
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
        if ($line -match "^\s*!")
        {
            Eat-Line $lines
            Write-Debug "Directive"            
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

#Test-HelloWorld
Test-Random