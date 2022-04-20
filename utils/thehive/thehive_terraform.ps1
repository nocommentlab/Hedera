function Get-DefaultAdminUid {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,


        [int] $Port=9000

    )

    $adminUid = $null

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")

    $body = "{`"query`": [{`"_name`": `"listOrganisation`"},{`"_name`": `"users`"}]}"

    try {
        Write-Host "[I] - Retrieving default admin UID....."

        $deserializedResponse = Invoke-RestMethod "http://${Ip}:$Port/api/v1/query?name=users" -Method 'POST' -Headers $headers -Body $body -Credential $Credential
        $adminUid = $($deserializedResponse | Where-Object {$_.Profile -eq "admin" -and $_._createdBy -eq "system@thehive.local"})._id

        Write-Host "[OK]  - Admin UID retrieved: ${adminUid}" -ForegroundColor Green
    }
    catch {
        Write-Host "[NOK] - Error during retrieve Admin UID: ${_}" -ForegroundColor Green
    }
    
    return $adminUid
    
    
}

function Set-DefaultAdminPassword {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,

        [Parameter(Mandatory)]
        [string] $Uid,

        [int] $Port=9000

    )

    $newPasswordSecureString = Read-Host "Insert the new password" -AsSecureString

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")

    $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($newPasswordSecureString)
    $newPasswordString = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)
    $body = "{`"password`": `"${newPasswordString}`"}"

    try {
        Write-Host "[I] - Setting default admin password....."
        Invoke-RestMethod "http://${Ip}:$Port/api/v1/user/$Uid/password/set" -Method 'POST' -Headers $headers -Body $body -Credential $Credential | Out-Null
        Write-Host "[OK]  - Admin password changed" -ForegroundColor Green
    }catch{
        Write-Host "[NOK] - Admin password not changed" -ForegroundColor Red
    }
    
}

function Add-HederaOrganisation {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,


        [int] $Port=9000

    )

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")

    $body = "{`"description`": `"The Hedera Organisation`",`"name`": `"Hedera`"}"

    try
    {
        
        Write-Host "[I] - Adding hedera organisation....."
        Invoke-RestMethod "http://${Ip}:$Port/api/v1/organisation" -Method 'POST' -Headers $headers -Body $body -Credential $Credential | Out-Null
        Write-Host "[OK]  - Hedera organisation created!" -ForegroundColor Green

    }catch{
        Write-Host "[NOK] - Hedera organisation not created!: ${_}" -ForegroundColor Red
    }
    
}

function Add-AdminHederaOrganisation {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,

        [int] $Port=9000

    )

    $hederaScannerPassword = Read-Host "Insert the hedera-scanner account password" -AsSecureString

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")

    $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($hederaScannerPassword)
    $hederaScannerPasswordString = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)

    $body = "{
    `n    `"login`": `"hedera-scanner`",
    `n    `"name`": `"Hedera Scanner`",
    `n    `"organisation`": `"Hedera`",
    `n    `"profile`": `"org-admin`",
    `n    `"password`": `"${hederaScannerPasswordString}`"
    `n}"

    try {
        Write-Host "[I] - Adding hedera-scanner user organisation....."

        $deserializedResponse = Invoke-RestMethod "http://${Ip}:$Port/api/v1/user" -Method 'POST' -Headers $headers -Body $body -Credential $Credential
        Write-Host "[OK]  - hedera-scanner user created for the Hedera organisation" -ForegroundColor Green

        return $($deserializedResponse | Where-Object {$_.login -eq "hedera-scanner@thehive.local"})._id
    }
    catch {
        $deserializedResponse = ConvertFrom-Json -InputObject $_
        Write-Host "[NOK] - $($deserializedResponse.message)" -ForegroundColor Red
    }

}

function Add-AnalystHederaOrganisation {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,

        [int] $Port=9000

    )

    $hederaAnalystPassword = Read-Host "Insert the hedera-analyst account password" -AsSecureString

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")

    $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($hederaAnalystPassword)
    $hederaAnalystPasswordString = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)

    $body = "{
        `n    `"login`": `"hedera-analyst`",
        `n    `"name`": `"Hedera Analyst`",
        `n    `"organisation`": `"Hedera`",
        `n    `"profile`": `"analyst`",
        `n    `"password`": `"${hederaAnalystPasswordString}`"
        `n}"

    try {

        Write-Host "[I] - Adding hedera-analyst user organisation....."
        $deserializedResponse = Invoke-RestMethod "http://${Ip}:$Port/api/v1/user" -Method 'POST' -Headers $headers -Body $body -Credential $Credential
        Write-Host "[OK]  - hedera-analyst user created for the Hedera organisation" -ForegroundColor Green

        return $($deserializedResponse | Where-Object {$_.login -eq "hedera-scanner@thehive.local"})._id
    }
    catch {
        $deserializedResponse = ConvertFrom-Json -InputObject $_
        Write-Host "[NOK] - $($deserializedResponse.message)" -ForegroundColor Red
    }

}

function Get-AdminHederaOrganisationApiKey {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,

        [Parameter(Mandatory)]
        [string] $Uid,

        [int] $Port=9000

    )

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")


    try {
        Write-Host "[I] - Retrieving hedera-scanner ApiKey....."
        $deserializedResponse = Invoke-RestMethod "http://${Ip}:$Port/api/v1/user/$Uid/key/renew" -Method 'POST' -Headers $headers -Credential $Credential
        Write-Host "[OK]  - Hedera Scanner User ApiKey retrieved: ${deserializedResponse}" -ForegroundColor Green

    }
    catch {
        $deserializedResponse = ConvertFrom-Json -InputObject $_
        Write-Host "[NOK] - $($deserializedResponse.message)" -ForegroundColor Red
    }

}
function Add-HederaObservables {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,

        [int] $Port=9000

    )

    $HederaObservables = @("registry", "file", "process", "pipe")



    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", "application/json")

    Write-Host "[I] - Adding hedera observables....."

    $HederaObservables | ForEach-Object {
        
        try {
            
            $body = "{`"name`":`"$_`"}"
            Invoke-RestMethod "http://${Ip}:$Port/api/observable/type" -Method 'POST' -Headers $headers -Body $body -Credential $Credential | Out-Null
            Write-Host "[OK]  - Observable type $_ created" -ForegroundColor Green
            
        }
        catch {            
            $deserializedResponse = ConvertFrom-Json -InputObject $_
            Write-Host "[NOK] - $($deserializedResponse.message)" -ForegroundColor Red
        }
        
    }
}

function Add-MitreAttackPatterns {

    param(
        [Parameter(Mandatory)]
        [string] $Ip,
        
        [Parameter(Mandatory)]
        [System.Management.Automation.PSCredential]$Credential,

        [int] $Port=9000

    )

    Write-Host "[I] - Downloading MITRE ATT&CK Enterprise database....."
    # Downloads the Enterprise Attack Matrix
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/mitre/cti/master/enterprise-attack/enterprise-attack.json?version=TheHive-4.1.18-1" -OutFile "$PSScriptRoot/enterprise-attack.txt"

    Write-Host "[OK]  - MITRE ATT&CK Enterprise database downloaded" -ForegroundColor Green

    # PowerShell doesn't support multipart/form-data....
    # Long and verbose method..
    # Thanks PowerShell!

    $fileBytes = [System.IO.File]::ReadAllBytes("${PSScriptRoot}/enterprise-attack.txt");
    $fileEnc = [System.Text.Encoding]::GetEncoding('UTF-8').GetString($fileBytes);
    $boundary = [System.Guid]::NewGuid().ToString(); 
    $LF = "`r`n";

    $bodyLines = ( 
        "--$boundary",
        "Content-Disposition: form-data; name=`"file`"; filename=`"enterprise-attack.txt`"",
        "Content-Type: t$LF",
        $fileEnc,
        "--$boundary--$LF" 
    ) -join $LF

    try
    {
        Invoke-RestMethod -Uri "http://${Ip}:${Port}/api/v1/pattern/import/attack" -Method 'POST' -ContentType "multipart/form-data; boundary=`"$boundary`"" -Body $bodyLines -Credential $creds | Out-Null
        Write-Host "[OK]  - MITRE ATT&CK Enterprise database uploaded to TheHive" -ForegroundColor Green
    }catch{
        Write-Host "[NOK] - $($deserializedResponse.message)" -ForegroundColor Red
    }
    
}


$theHiveRemoteIpServer = "127.0.0.1"

Write-Host "[I] - Retrieving default admin credential....."

$creds = Get-Credential -Message "Enter the default TheHive credential. Typically is: admin/secret"

$adminId = Get-DefaultAdminUid -IP $theHiveRemoteIpServer -Credential $creds

Set-DefaultAdminPassword -IP $theHiveRemoteIpServer -Credential $creds -Uid $adminId

Add-HederaOrganisation -IP $theHiveRemoteIpServer -Credential $creds

$hederaScannerUid = Add-AdminHederaOrganisation -IP $theHiveRemoteIpServer -Credential $creds

Get-AdminHederaOrganisationApiKey -IP $theHiveRemoteIpServer -Credential $creds -Uid $hederaScannerUid

Add-AnalystHederaOrganisation -IP $theHiveRemoteIpServer -Credential $creds

Add-HederaObservables -IP $theHiveRemoteIpServer -Credential $creds

Add-MitreAttackPatterns -IP $theHiveRemoteIpServer -Credential $creds