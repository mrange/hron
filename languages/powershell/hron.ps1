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

    begin
    {        
        function AddOrExtend-Member($object, $member, $value)
        {
            if ($object | Get-Member $member)
            {
                if ($object.$member -is [array]) { $object.$member += $value } else { $object.$member = $object.$member, $value }
            }
            else
            {
                $object | Add-Member NoteProperty $member $value
            }
        }

        $object = New-Object psobject
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
        $line = $_
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
                                    $object = New-Object PSObject
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
                            $value = $object
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
            $value = $object
            $object, $memberName = $stack.Pop()
            Write-Debug "Object_End:$memberName"
            AddOrExtend-Member $object $memberName $value                            
        }

        # report progress
        Write-Progress -Activity "Parsing HRON" -Completed

        # return
        $object
    }
}

