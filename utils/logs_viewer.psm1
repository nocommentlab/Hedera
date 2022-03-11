<#
.SYNOPSIS

    Shows the scan result using Out-GridView PowerShell cmdlet.

.DESCRIPTION

    This PowerShell module permitts to show the result of an Hedera Scan session using a simple UI.
    For each type of scan, Hedera generates different json file that could be ingest inside a SIEM or show locally using Out-GridView cmdlet.


.PARAMETER Name


.PARAMETER Extension
Specifies the extension. "Txt" is the default.


.EXAMPLE

PS> extension -name "File"
File.txt

.EXAMPLE

PS> extension -name "File" -extension "doc"
File.doc

.EXAMPLE

PS> extension "File" "doc"
File.doc

.LINK

http://www.fabrikam.com/extension.html

.LINK

Set-Item
#>


Add-Type -AssemblyName System.Web.Extensions
function Get-HederaLogUI
{
    Param(
        [Alias("Log")]
        [Parameter(Mandatory, Position=0)]
        [String] $LogPath,

        [Alias("Type")]
        [Parameter(Mandatory, Position=1)]
        [ValidateSet('registry','file','pipe')]
        [String] $LogType
    )
    
    Begin{

        $GridViewColumnConfig = @{ 

                registry = [array] @(
                    @{ Name = 'DateTime'; Expression = { $_.DATETIME_Datetime } }, 
                    @{ Name = 'Result'; Expression = { $_.Result } }, 
                    @{ Name = 'GUID'; Expression = { $_.RegistryIndicator.Guid } }, 
                    @{ Name = 'Type'; Expression = { $_.RegistryIndicator.Type } }, 
                    @{ Name = 'BaseKey'; Expression = { $_.RegistryIndicator.BaseKey } }, 
                    @{ Name = 'Key'; Expression = { $_.RegistryIndicator.Key } },  
                    @{ Name = 'ValueNameRegEx'; Expression = { $_.RegistryIndicator.ValueNameRegex } },
                    @{ Name = 'ValueDataRegEx'; Expression = { $_.RegistryIndicator.ValueDataRegex } },
                    @{ Name = 'Key Path'; Expression = { $_.RegistryItem.STRING_Name } },
                    @{ Name = 'ValueName'; Expression = { $_.RegistryItem.STRING_ValueName } }, 
                    @{ Name = 'ValueData'; Expression = { $_.RegistryItem.STRING_ValueData } }
                )

                file = [array] @(
                    @{ Name = 'DateTime'; Expression = { $_.DATETIME_Datetime } }, 
                    @{ Name = 'Result'; Expression = { $_.Result } }, 
                    @{ Name = 'GUID'; Expression = { $_.FileIndicator.Guid } }, 
                    @{ Name = 'Type'; Expression = { $_.FileIndicator.Type } },
                    @{ Name = 'Sha256'; Expression = { $_.FileIndicator.Sha256Hash } },
                    @{ Name = 'IMPHASH'; Expression = { $_.FileIndicator.Value } },
                    @{ Name = 'FileName'; Expression = { $_.FileIndicator.FileName } },
                    @{ Name = 'Yara Rule'; Expression = { $_.FileIndicator.Rule } },
                    @{ Name = 'Path'; Expression = { $_.FileItem.STRING_Path } }
                   
                )

                pipe = [array] @(
                    @{ Name = 'DateTime'; Expression = { $_.DATETIME_Datetime } }, 
                    @{ Name = 'Result'; Expression = { $_.Result } }, 
                    @{ Name = 'GUID'; Expression = { $_.PipeIndicator.Guid } }, 
                    @{ Name = 'Type'; Expression = { $_.PipeIndicator.Type } },
                    @{ Name = 'NameRegex'; Expression = { $_.PipeIndicator.Name } },
                    @{ Name = 'Name'; Expression = { $_.Name } }
                )
        }
    }

    Process{

        
        # Instantiates the JSON Deserialize Class
        $jsonDeserializer = New-Object -TypeName System.Web.Script.Serialization.JavaScriptSerializer
        # Sets the MAX Json Length Threshold to 100MB
        $jsonDeserializer.MaxJsonLength = 104857600 
        
        # Reads the JSON log file content
        $STRING_FileContent = [System.IO.File]::ReadAllText($LogPath)
        # Deserializes the JSON buffer
        $OBJECT_DeserializedJSON = $jsonDeserializer.Deserialize($STRING_FileContent, [System.Object])
        # Disposes the buffer
        $STRING_FileContent = $null
        # Disposes the JSON Deserialer object
        $jsonDeserializer = $null

        $OBJECT_DeserializedJSON | Select-Object -Property $GridViewColumnConfig.$LogType | Out-GridView  -Title "Scan result for ${LogType} indicators, File: ${LogPath} "
    }

    End{

    }
}

Export-ModuleMember -Function Get-HederaLogUI