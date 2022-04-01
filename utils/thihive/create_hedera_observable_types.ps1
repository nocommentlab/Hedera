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
        $deserializedResponse = Invoke-RestMethod "http://${Ip}:$Port/api/v1/query?name=users" -Method 'POST' -Headers $headers -Body $body -Credential $Credential
        $adminUid = $($deserializedResponse | Where-Object {$_.Profile -eq "admin" -and $_._createdBy -eq "system@thehive.local"})._id

        Write-Host "[OK]  - Admin UID retrieved: ${adminUid}" -ForegroundColor Green
    }
    catch {
        Write-Host "[NOK] - Error during retrieve Admin UID: ${adminUid}" -ForegroundColor Green
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
        Invoke-RestMethod "http://${Ip}:$Port/api/v1/user/$Uid/password/set" -Method 'POST' -Headers $headers -Body $body -Credential $Credential | Out-Null
        Write-Host "[OK]  - Admin password changed" -ForegroundColor Green
    }catch{
        Write-Host "[NOK] - Admin password not changed" -ForegroundColor Red
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



$creds = Get-Credential -Message 'Enter default thehive credential'

$adminId = Get-DefaultAdminUid -IP 192.168.1.70 -Credential $creds

Set-DefaultAdminPassword -IP 192.168.1.70 -Credential $creds -Uid $adminId

#Add-HederaObservables -IP 192.168.1.64 -Credential $creds