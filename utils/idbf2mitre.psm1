<#

#>

function Get-MitreNavigatorLayer
{
    Param(
        [Parameter(Mandatory, Position=0)]
        [String] $Idbf
    )    
    
    Begin{
        $mitre_map_layer = '{"name":"layer","versions":{"attack":"11","navigator":"4.6.0","layer":"4.3"},
                            "domain":"enterprise-attack","description":"","filters":{"platforms":["Windows"]},
                            "sorting":0,
                            "layout":{"layout":"side","aggregateFunction":"average","showID":false,"showName":true,"showAggregateScores":false,"countUnscored":false},
                            "hideDisabled":false,"techniques":[],
                            "gradient":{"colors":["#8ec843ff","#ffe766ff","#ff6666ff"],"minValue":0,"maxValue":100},
                            "legendItems":[],"metadata":[],"links":[],"showTacticRowBackground":true,"tacticRowBackground":"#aaaaaa",
                            "selectTechniquesAcrossTactics":true,"selectSubtechniquesWithParent":true}'
    }

    Process{
        $des_idbf = Get-Content $Idbf | ConvertFrom-Yaml
        $des_layer = ConvertFrom-Json -InputObject $mitre_map_layer

        # Sets the name
        $des_layer.name = $des_idbf.information.guid

        # Sets the description
        $des_layer.description = $des_idbf.information.description

        $des_idbf.the_hive.procedures | % {
            $des_layer.techniques+=@{
                techniqueID= $_.pattern_id;
                tactic= $_.tactic;
                score= 0;
                color= "";
                comment= $_.description;
                enabled= $true;
                metadata= @();
                links= @();
                showSubtechniques= $true
            }
        }

        ConvertTo-Json -InputObject $des_layer | Out-file "$($des_idbf.information.guid)_layer.json"
    }  
}

Export-ModuleMember -Function Get-MitreNavigatorLayer