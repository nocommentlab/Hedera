# Manual Tasks

- Restore the VM
- Configure Remote Desktop Connection
- Disable **Require computers to use Network Level Authentication** to connect
- Configure WinRM with Powershell:
```
$url = "https://raw.githubusercontent.com/ansible/ansible/devel/examples/scripts/ConfigureRemotingForAnsible.ps1"
$file = "$env:temp\ConfigureRemotingForAnsible.ps1"
(New-Object -TypeName System.Net.WebClient).DownloadFile($url, $file)
powershell.exe -ExecutionPolicy ByPass -File $file
```
- Check WinRm Listners with: winrm enumerate winrm/config/Listener
- Execute this playbook with: `ansible-playbook atomic-redteam.yml -i hosts -u Admin -k`
- Import the AtomicRedTeam with: `Import-Module "C:\AtomicRedTeam\invoke-atomicredteam\Invoke-AtomicRedTeam.psd1" -Force`
- Use it :)