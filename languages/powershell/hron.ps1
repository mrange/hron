function Eat-Line([ref]$lines)
{ 
    $line = $lines.Value[0]
    Write-Debug ">>> $line"
    $lines.Value = $lines.Value[1..($lines.Value.Length)] 
}

function Parse-String([ref]$lines, $indent)
{
    $sb = New-Object System.Text.StringBuilder
    while($lines.Value.Count -gt 0)
    {
        $line = $lines.Value[0]
        if ($line -match "^(?<indent>^\t*)(?<value>.*)")
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
        else
        {
            throw "Parse error; expecting empty line or text"
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
            Write-Debug "$margin Empty line"
            Eat-Line $lines
        }      
        elseif ($line -match "^\s*#")
        {
            Write-Debug "$margin Comment"
            Eat-Line $lines
        }
        elseif ($line -match "^(?<indent>^\t*)(?<controlchar>@|=)(?<membername>.*)")
        {                       
            if ($indent -eq $matches.indent.Length)
            {
                Write-Debug "$margin Member $($matches.membername)"
                Eat-Line $lines
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
            throw "Parse error; expecting empty line, comment or member declaration"
        }
    }
}

function ConvertFrom-HRON($lines)
{
    $result = New-Object psobject
    Parse-Members ([ref]$lines) $result 0
    return $result
}

$text = Get-Content ..\..\reference-data\helloworld.hron
$x = ConvertFrom-HRON $text
$x