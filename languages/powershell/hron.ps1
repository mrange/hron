function ConvertFrom-HRON
{
<#
.Synopsis
	Converts HRON (line based) data from input pipe to a powershell object
.Example
	Get-Content .\myfile.hron | ConvertFrom-HRON
    
    This example will load the data from myfile.hron, send it down
    the pipe and parse it using this command. The parsed result
    will be a powershell custom object.
#>

    [CmdLetBinding()]
    param([Parameter(ValueFromPipeLine=$true)][string]$Line)

    begin
    {        
        $createMap = if ($host.Version.Major -lt 3) { { @{} } } else { { [ordered]@{} } }        
        $convertToObject = if ($host.Version.Major -lt 3) { { New-Object PsObject -Property $args[0] } } else { { [pscustomobject]$args[0] } }     
        function AddOrExtend-Member($object, $member, $value)
        {
            if ($object.Contains($member)) {
                if ($object.$member -is [array]) { $object.$member += $value } else { $object.$member = $object.$member, $value }
            } 
            else {
                $object[$member] = $value
            }
        }
        
        $object = & $createMap
        $stack = New-Object System.Collections.Stack
        $mode = 0
        $lineCount = 0
        $loop = $false
        $regexDirective = [regex]"^\s*!(?<directive>.*)"
        $regexEmptyLine = [regex]"^\s*$"
        $regexComment = [regex]"^(?<indent>\t*)#(?<comment>.*)"
        $regexMember = [regex]"^(?<indent>^\t*)(?<controlchar>@|=)(?<membername>.*)"
    }

    process
    {
        $lineCount++
        if ($lineCount % 100 -eq 0)
        {
            Write-Progress -Activity "Parsing HRON" -Status "$lineCount lines processed"
        }

        do
        {
            $loop = $false
            switch($mode)
            {
                ############################### header mode #########################################
                0 
                {
                    if ($line -match $regexDirective)
                    {                        
                        Write-Debug "PreProcessor:$($matches.directive)"            
                    }
                    else
                    {
                        $mode = 1
                        $loop = $true
                    }
                }

                ############################### object mode #########################################
                1
                {
                    if ($line -match $regexEmptyLine)
                    {
                        Write-Debug "Empty:$line"
                    }                    
                    elseif ($line -match $regexComment)
                    {
                        Write-Debug "Comment:$($matches.indent.Length),$($matches.comment)"
                    }
                    elseif ($line -match $regexMember)
                    {             
                        if ($stack.Count -eq $matches.indent.Length)
                        {
                            $memberName = $matches.membername
                            $stack.Push(@($object, $memberName))
                            switch($matches.controlchar)
                            {
                                "@" 
                                {
                                    Write-Debug "Object_Begin:$memberName"
                                    $object = & $createMap
                                }
                                "="
                                {
                                    Write-Debug "Value_Begin:$memberName"
                                    $mode = 2
                                    $object = New-Object System.Text.StringBuilder
                                }
                            }
                        }
                        else
                        {                           
                            $value = & $convertToObject $object
                            $object, $memberName = $stack.Pop()
                            Write-Debug "Object_End:$memberName"
                            AddOrExtend-Member $object $memberName $value                                                        
                            $mode = 1
                            $loop = $true
                        }
                    }
                    else
                    {
                        throw "Parse error on line $linecount; expecting empty line, comment or member declaration"
                    }
                }

                ############################### value mode #########################################
                2
                {
                    if ($line -match "^(?<indent>^\t{$($stack.Count)})(?<value>.*)")
                    {
                        Write-Debug "ContentLine:$($matches.value)"
                        $object.AppendLine($matches.value) | Out-Null
                    }
                    elseif ($line -match $regexComment)
                    {
                        Write-Debug "CommentLine:$($matches.indent.Length),$($matches.comment)"
                    }
                    elseif ($line -match $regexEmptyLine)
                    {
                        Write-Debug "EmptyLine:$line"
                        $object.AppendLine() | Out-Null
                    }        
                    else
                    {                        
                        $value = $object.ToString()
                        $object, $memberName = $stack.Pop()
                        Write-Debug "Value_End:$memberName"
                        AddOrExtend-Member $object $memberName $value                            
                        $mode = 1
                        $loop = $true
                    }
                }
            }
        } while($loop);
    }

    end
    {
        # unwind stack
        if ($mode -eq 2)
        {
            $value = $object.ToString()
            $object, $memberName = $stack.Pop()
            Write-Debug "Value_End:$memberName"
            AddOrExtend-Member $object $memberName $value                            
        }

        while($stack.Count -gt 0)
        {
            $value = & $convertToObject $object
            $object, $memberName = $stack.Pop()
            Write-Debug "Object_End:$memberName"
            AddOrExtend-Member $object $memberName $value                            
        }

        # report progress
        Write-Progress -Activity "Parsing HRON" -Completed

        # return
        & $convertToObject $object
    }
}

function ConvertTo-HRON
{
<#
    .SYNOPSIS
    Convert a powershell obect to HRON

    .DESCRIPTION
    Convert a powershell obect to HRON.

    .EXAMPLE
    $x = New-Object PsObject -Property @{A=1; B="Hello"}
    ConvertTo-HRON $x

    This example creates an object that are serialized to HRON using this command

    .PARAMETER Object
    The object to convert

    .PARAMETER Indent
    A string that will be prepended to all output lines. The string should
    consist of zero or more tab characters

#>
    [CmdLetBinding()]
    param(
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        $Object,
        [Parameter(Position=1)]
        [ValidatePattern({^\t*$})]
        [string]
        $Indent=""
        )

    if (!$object) { return }

    $memberNames =
        if ($object -is [pscustomobject]) {
            # trick for enumerating properties in order, 
            # http://stackoverflow.com/questions/5831623/powershell-import-csv-get-member-sort-column-names-property-names-based-on
            $object.psobject.properties | Select -ExpandProperty Name 
        }
        else {
            Get-Member -InputObject $object -MemberType Properties | Select -ExpandProperty Name 
        }

    $memberNames | ForEach-Object {
        $memberName = $_
        $object.$memberName | ForEach-Object {
            $memberTypeName = $_.GetType().Name        
            if ($memberTypeName -eq "PSCustomObject")
            {
                "$indent@$memberName"
                ConvertTo-HRON $_ ($indent+"`t")
            }
            else
            {
                "$indent=$memberName"
                $trimmedStringVal = $_.ToString() -replace "`r`n$", "" 
                $values = $trimmedStringVal -split "\r\n" 
                $values | ForEach-Object {
                    "$indent`t$_"
                }
            }
        }
    }     
}
