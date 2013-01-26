function Parse-String([ref]$lines, $indent)
{
    $sb = New-Object System.Text.StringBuilder
    while($lines.Value[0] -match "(?<indent>^\t*)(?<value>.*)" -and $matches.indent.Length -eq $indent)
    {
        $lines.Value = $lines.Value[1..($lines.Value.Length)]
        $sb.AppendLine($matches.value) | Out-Null
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
    while
        (
            ($lines.Value.Count -gt 0) -and
            ($lines.Value[0] -match "(?<indent>^\t*)(?<controlchar>@|=)(?<membername>.*)") -and 
            ($indent -eq $matches.indent.Length)
        )
    {
        $lines.Value = $lines.Value[1..($lines.Value.Length)]
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
}

function ConvertFrom-HRON($text_and_comments)
{
    $lines = $text_and_comments | ? { !$_.StartsWith("#") } | ? { ![string]::IsNullOrEmpty($_.Trim()) }
    $result = New-Object psobject
    Parse-Members ([ref]$lines) $result 0
    return $result
}

$text = Get-Content .\sample.hron
$x = ConvertFrom-HRON $text
$x