function ConvertFrom-HRON
{
    begin
    {        
        function AddOrExtend-Member($object, $member, $value)
        {
            if ($object | Get-Member $member)
            {
                if ($object -is [array]) { $object.$member += $value } else { $object.$member = $object.$member, $value }
            }
            else
            {
                $object | Add-Member NoteProperty $member $value
            }
        }

        $object = New-Object psobject
        $objectStack = New-Object System.Collections.ArrayList
        $memberStack = New-Object System.Collections.ArrayList
        $indent = 0
        $modeHeader = 0; $modeObject = 1; $modeValue = 2;
        $mode =  $modeHeader
        $lineCount = 0
        $loop = $false
        $sb = $null
        $memberName = $null
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
                $modeHeader 
                {
                    if ($line -match $regexDirective)
                    {                        
                        Write-Debug "PreProcessor:$($matches.directive)"            
                    }
                    else
                    {
                        $mode = $modeObject;
                        $loop = $true;
                    }
                }

                ############################### object mode #########################################
                $modeObject
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
                        if ($indent -eq $matches.indent.Length)
                        {
                            $memberStack.Add($memberName) | Out-Null
                            $memberName = $matches.membername
                            $value = $null
                            switch($matches.controlchar)
                            {
                                "@" 
                                {
                                    Write-Debug "Object_Begin:$memberName"
                                    $objectStack.Add($object) | Out-Null
                                    $object = New-Object PSObject
                                    $indent++
                                    $mode = $modeObject                                    
                                }
                                "="
                                {
                                    Write-Debug "Value_Begin:$memberName"
                                    $indent++
                                    $mode = $modeValue
                                    $sb = New-Object System.Text.StringBuilder
                                    #$value = Parse-String $lines ($indent+1)
                                    #Write-Debug "Value_End:$memberName"
                                }
                            }
                        }
                        else
                        {                           
                            Write-Debug "Object_End:$memberName"
                            $parent = $objectStack[$objectStack.Count-1]
                            $objectStack.RemoveAt($objectStack.Count-1) | Out-Null
                            AddOrExtend-Member $parent $memberName $object
                            $memberName = $memberStack[$memberStack.Count-1]
                            $memberStack.RemoveAt($memberStack.Count-1) | Out-Null
                            $object = $parent
                            $indent--
                            $mode = $modeObject
                            $loop = $true
                        }
                    }
                    else
                    {
                        throw "Parse error on line $linecount; expecting empty line, comment or member declaration"
                    }
                }

                ############################### value mode #########################################
                $modeValue
                {
                    if ($line -match "^(?<indent>^\t{$indent})(?<value>.*)")
                    {
                        Write-Debug "ContentLine:$($matches.value)"
                        $sb.AppendLine($matches.value) | Out-Null
                    }
                    elseif ($line -match $regexComment)
                    {
                        Write-Debug "CommentLine:$($matches.indent.Length),$($matches.comment)"
                    }
                    elseif ($line -match $regexEmptyLine)
                    {
                        Write-Debug "EmptyLine:$line"
                        $sb.AppendLine() | Out-Null
                    }        
                    else
                    {                        
                        Write-Debug "Value_End:$memberName"
                        AddOrExtend-Member $object $memberName $sb.ToString()
                        $memberName = $memberStack[$memberStack.Count-1]
                        $memberStack.RemoveAt($memberStack.Count-1) | Out-Null
                        $sb = $null
                        $indent--
                        $mode = $modeObject
                        $loop = $true
                    }
                }
            }
        } while($loop);
    }

    end
    {
        # unwind value
        if ($sb)
        {
            Write-Debug "Value_End:$memberName"
            AddOrExtend-Member $object $memberName $sb.ToString()
            $memberName = $memberStack[$memberStack.Count-1]
            $memberStack.RemoveAt($memberStack.Count-1) | Out-Null
        }

        # unwind object stack
        while($objectStack.Count -gt 0)
        {
            Write-Debug "Object_End:$memberName"
            $parent = $objectStack[$objectStack.Count-1]
            $objectStack.RemoveAt($objectStack.Count-1) | Out-Null
            AddOrExtend-Member $parent $memberName $object
            $memberName = $memberStack[$memberStack.Count-1]
            $memberStack.RemoveAt($memberStack.Count-1) | Out-Null
            $object = $parent
        }

        # report progress
        Write-Progress -Activity "Parsing HRON" -Completed

        # return
        $object
    }
}

