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
    }

    process
    {
        $line = $_
        $lineCount++
        do
        {
            $loop = $false
            switch($mode)
            {
                ############################### header mode #########################################
                $modeHeader 
                {
                    if ($line -match "^\s*!(?<directive>.*)")
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
                    if ($line -match "^\s*$")
                    {
                        Write-Debug "Empty:$line"
                    }      
                    elseif ($line -match "^(?<indent>\t*)#(?<comment>.*)")
                    {
                        Write-Debug "Comment:$($matches.indent.Length),$($matches.comment)"
                    }
                    elseif ($line -match "^(?<indent>^\t*)(?<controlchar>@|=)(?<membername>.*)")
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
                    elseif ($line -match "^(?<indent>\t*)#(?<comment>.*)")
                    {
                        Write-Debug "CommentLine:$($matches.indent.Length),$($matches.comment)"
                    }
                    elseif ($line -match "^\s*$")
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

        # return
        $object
    }
}

function Test-HelloWorld
{
    $x = Get-Content ..\..\reference-data\helloworld.hron | ConvertFrom-HRON $text
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
    $x = Get-Content ..\..\reference-data\random.hron | ConvertFrom-HRON $text
    write-host $x
}

function Test-Simple
{
    $x = Get-Content ..\..\reference-data\simple.hron | ConvertFrom-HRON $text
    write-host $x
}

function Test-Large
{
    $x = Get-Content ..\..\reference-data\large.hron | ConvertFrom-HRON $text
    write-host $x
}

#Test-HelloWorld
#Test-Random
#Test-Simple
Test-Large