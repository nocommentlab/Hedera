function Add-HederaObservables {

    param(
        [Parameter(Mandatory, Position=0)]
        [string] $APIKey
    )

    $HederaObservables = @("registry","file","process","pipe")

    $HederaObservables | ForEach-Object {
        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("Authorization", "Bearer lx9RWX4B1+CrsHkZqlqDMVUkIcspfEJE")
        $headers.Add("Content-Type", "application/json")

        $body = "{`n    `"title`": `"My first case`",`n    `"description`": `"This case has been created by my custom script`"`n    `"tasks`": [{`n        `"title`": `"mytask`",`n        `"description`": `"description of my task`"`n    }],`n    `"customFields`": {`n        `"cvss`": {`n            `"number`": 9,`n        },`n        `"businessImpact`": {`n            `"string`": `"HIGH`"`n        }`n    }`n}"

        $response = Invoke-RestMethod 'http://192.168.1.64:9000/api/case' -Method 'POST' -Headers $headers -Body $body
        $response | ConvertTo-Json
    }
}