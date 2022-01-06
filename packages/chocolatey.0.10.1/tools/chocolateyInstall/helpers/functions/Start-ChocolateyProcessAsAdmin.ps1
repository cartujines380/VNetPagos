# Copyright 2011 - Present RealDimensions Software, LLC & original authors/contributors from https://github.com/chocolatey/chocolatey
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

function Start-ChocolateyProcessAsAdmin {
<#
.SYNOPSIS
**NOTE:** Administrative Access Required.

Runs a process with administrative privileges. If `-ExeToRun` is not
specified, it is run with PowerShell.

.NOTES
This command will assert UAC/Admin privileges on the machine.

Starting in 0.9.10, will automatically call Set-PowerShellExitCode to
set the package exit code in the following ways:

- 4 if the binary turns out to be a text file.
- The same exit code returned from the process that is run. If a 3010 is returned, it will set 3010 for the package.

.INPUTS
None

.OUTPUTS
None

.PARAMETER Statements
Arguments to pass to `ExeToRun` or the PowerShell script block to be
run.

.PARAMETER ExeToRun
The executable/application/installer to run. Defaults to `'powershell'`.

.PARAMETER Minimized
Switch indicating if a Windows pops up (if not called with a silent
argument) that it should be minimized.

.PARAMETER NoSleep
Used only when calling PowerShell - indicates the window that is opened
should return instantly when it is complete.

.PARAMETER ValidExitCodes
Array of exit codes indicating success. Defaults to `@(0)`.

.PARAMETER WorkingDirectory
The working directory for the running process. Defaults to 
`Get-Location`.

Available in 0.10.1+.

.PARAMETER SensitiveStatements
Arguments to pass to  `ExeToRun` that are not logged. 

Note that only licensed versions of Chocolatey provide a way to pass 
those values completely through without having them in the install 
script or on the system in some way.

Available in 0.10.1+.

.PARAMETER IgnoredArguments
Allows splatting with arguments that do not apply. Do not use directly.

.EXAMPLE
Start-ChocolateyProcessAsAdmin -Statements "$msiArgs" -ExeToRun 'msiexec'

.EXAMPLE
Start-ChocolateyProcessAsAdmin -Statements "$silentArgs" -ExeToRun $file

.EXAMPLE
Start-ChocolateyProcessAsAdmin -Statements "$silentArgs" -ExeToRun $file -ValidExitCodes @(0,21)

.EXAMPLE
>
# Run PowerShell statements
$psFile = Join-Path "$(Split-Path -parent $MyInvocation.MyCommand.Definition)" 'someInstall.ps1'
Start-ChocolateyProcessAsAdmin "& `'$psFile`'"

.LINK
Install-ChocolateyPackage

.LINK
Install-ChocolateyInstallPackage
#>
param(
  [parameter(Mandatory=$false, Position=0)][string[]] $statements,
  [parameter(Mandatory=$false, Position=1)][string] $exeToRun = 'powershell',
  [parameter(Mandatory=$false)][switch] $minimized,
  [parameter(Mandatory=$false)][switch] $noSleep,
  [parameter(Mandatory=$false)] $validExitCodes = @(0),
  [parameter(Mandatory=$false)][string] $workingDirectory = $(Get-Location),
  [parameter(Mandatory=$false)][string] $sensitiveStatements = '',
  [parameter(ValueFromRemainingArguments = $true)][Object[]] $ignoredArguments
)
  [string]$statements = $statements -join ' '
  
  Write-Debug "Running 'Start-ChocolateyProcessAsAdmin' with exeToRun:'$exeToRun', statements: '$statements', workingDirectory: '$workingDirectory' ";

  try{
    if ($exeToRun -ne $null) { $exeToRun = $exeToRun -replace "`0", "" }
    if ($statements -ne $null) { $statements = $statements -replace "`0", "" }
  } catch {
    Write-Debug "Removing null characters resulted in an error - $($_.Exception.Message)"
  }

  $wrappedStatements = $statements
  if ($wrappedStatements -eq $null) { $wrappedStatements = ''}

  if ($exeToRun -eq 'powershell') {
    $exeToRun = "$($env:SystemRoot)\System32\WindowsPowerShell\v1.0\powershell.exe"
    $importChocolateyHelpers = ""
    Get-ChildItem "$helpersPath" -Filter *.psm1 | ForEach-Object { $importChocolateyHelpers = "& import-module -name  `'$($_.FullName)`' | Out-Null;$importChocolateyHelpers" };
    $block = @"
      `$noSleep = `$$noSleep
      $importChocolateyHelpers
      try{
        `$progressPreference="SilentlyContinue"
        $statements
        if(!`$noSleep){start-sleep 6}
      }
      catch{
        if(!`$noSleep){start-sleep 8}
        throw
      }
"@
    $encoded = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($block))
    $wrappedStatements = "-NoProfile -ExecutionPolicy bypass -EncodedCommand $encoded"
    $dbgMessage = @"
Elevating Permissions and running powershell block:
$block
This may take a while, depending on the statements.
"@
  }
  else
  {
    $dbgMessage = @"
Elevating Permissions and running [`"$exeToRun`" $wrappedStatements]. This may take a while, depending on the statements.
"@
  }

  Write-Debug $dbgMessage

  try {
    $exeIsTextFile = [System.IO.Path]::GetFullPath($exeToRun) + ".istext"
    if (([System.IO.File]::Exists($exeIsTextFile))) {
      Set-PowerShellExitCode 4
      throw "The file was a text file but is attempting to be run as an executable - '$exeToRun'"
    }
  } catch {
    Write-Debug "Unable to detect whether the file is a text file or not - $($_.Exception.Message)"
  }

  if ($exeToRun -eq 'msiexec' -or $exeToRun -eq 'msiexec.exe') {
    $exeToRun = "$($env:SystemRoot)\System32\msiexec.exe"
  }

  if (!([System.IO.File]::Exists($exeToRun)) -and $exeToRun -notmatch 'msiexec') {
    Write-Warning "May not be able to find '$exeToRun'. Please use full path for executables."
    # until we have search paths enabled, let's just pass a warning
    #Set-PowerShellExitCode 2
    #throw "Could not find '$exeToRun'"
  }

  # Redirecting output slows things down a bit.
  $writeOutput = {
    if ($EventArgs.Data -ne $null) {
      Write-Verbose "$($EventArgs.Data)"
    }
  }

  $writeError = {
    if ($EventArgs.Data -ne $null) {
      Write-Error "$($EventArgs.Data)"
    }
  }

  $process = New-Object System.Diagnostics.Process
  $process.EnableRaisingEvents = $true
  Register-ObjectEvent -InputObject $process -SourceIdentifier "LogOutput_ChocolateyProc" -EventName OutputDataReceived -Action $writeOutput | Out-Null
  Register-ObjectEvent -InputObject $process -SourceIdentifier "LogErrors_ChocolateyProc" -EventName ErrorDataReceived -Action  $writeError | Out-Null

  #$process.StartInfo = New-Object System.Diagnostics.ProcessStartInfo($exeToRun, $wrappedStatements)
  # in case empty args makes a difference, try to be compatible with the older
  # version
  $psi = New-Object System.Diagnostics.ProcessStartInfo
  $psi.FileName = $exeToRun
  if ($wrappedStatements -ne '') {
    $psi.Arguments = "$wrappedStatements"
  }
  if ($sensitiveStatements -ne $null -and $sensitiveStatements -ne '') {
    Write-Host "Sensitive arguments have been passed. Adding to arguments."
    $psi.Arguments += " $sensitiveStatements"
  }
  $process.StartInfo =  $psi

  # process start info
  $process.StartInfo.RedirectStandardOutput = $true
  $process.StartInfo.RedirectStandardError = $true
  $process.StartInfo.UseShellExecute = $false
  $process.StartInfo.WorkingDirectory = $workingDirectory
  if ([Environment]::OSVersion.Version -ge (New-Object 'Version' 6,0)){
    Write-Debug "Setting RunAs for elevation"
    $process.StartInfo.Verb = "RunAs"
  }
  if ($minimized) {
    $process.StartInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Minimized
  }

  $process.Start() | Out-Null
  if ($process.StartInfo.RedirectStandardOutput) { $process.BeginOutputReadLine() }
  if ($process.StartInfo.RedirectStandardError) { $process.BeginErrorReadLine() }
  $process.WaitForExit()

  # For some reason this forces the jobs to finish and waits for
  # them to do so. Without this it never finishes.
  Unregister-Event -SourceIdentifier "LogOutput_ChocolateyProc"
  Unregister-Event -SourceIdentifier "LogErrors_ChocolateyProc"
  
  # sometimes the process hasn't fully exited yet.
  for ($loopCount=1; $loopCount -le 15; $loopCount++) { 
    if ($process.HasExited) { break; }
    Write-Debug "Waiting for process to exit - $loopCount/15 seconds"; 
    Start-Sleep 1; 
  }

  $exitCode = $process.ExitCode
  $process.Dispose()

  Write-Debug "Command [`"$exeToRun`" $wrappedStatements] exited with `'$exitCode`'."
  if ($validExitCodes -notcontains $exitCode) {
    Set-PowerShellExitCode $exitCode
    throw "Running [`"$exeToRun`" $wrappedStatements] was not successful. Exit code was '$exitCode'. See log for possible error messages."
  } else {
    $chocoSuccessCodes = @(0, 1605, 1614, 1641, 3010)
    if ($chocoSuccessCodes -notcontains $exitCode) {
      Write-Warning "Exit code '$exitCode' was considered valid by script, but not as a Chocolatey success code. Returning '0'."
      $exitCode = 0
    }
  }

  Write-Debug "Finishing 'Start-ChocolateyProcessAsAdmin'"

  return $exitCode
}

# SIG # Begin signature block
# MIIcrQYJKoZIhvcNAQcCoIIcnjCCHJoCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCC7Yy7eqydZAGiE
# 80uwaEhQmquWj2ta+dMZAWFctT2lDKCCF7cwggUwMIIEGKADAgECAhAECRgbX9W7
# ZnVTQ7VvlVAIMA0GCSqGSIb3DQEBCwUAMGUxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xJDAiBgNV
# BAMTG0RpZ2lDZXJ0IEFzc3VyZWQgSUQgUm9vdCBDQTAeFw0xMzEwMjIxMjAwMDBa
# Fw0yODEwMjIxMjAwMDBaMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2Vy
# dCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lD
# ZXJ0IFNIQTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0EwggEiMA0GCSqGSIb3
# DQEBAQUAA4IBDwAwggEKAoIBAQD407Mcfw4Rr2d3B9MLMUkZz9D7RZmxOttE9X/l
# qJ3bMtdx6nadBS63j/qSQ8Cl+YnUNxnXtqrwnIal2CWsDnkoOn7p0WfTxvspJ8fT
# eyOU5JEjlpB3gvmhhCNmElQzUHSxKCa7JGnCwlLyFGeKiUXULaGj6YgsIJWuHEqH
# CN8M9eJNYBi+qsSyrnAxZjNxPqxwoqvOf+l8y5Kh5TsxHM/q8grkV7tKtel05iv+
# bMt+dDk2DZDv5LVOpKnqagqrhPOsZ061xPeM0SAlI+sIZD5SlsHyDxL0xY4PwaLo
# LFH3c7y9hbFig3NBggfkOItqcyDQD2RzPJ6fpjOp/RnfJZPRAgMBAAGjggHNMIIB
# yTASBgNVHRMBAf8ECDAGAQH/AgEAMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUEDDAK
# BggrBgEFBQcDAzB5BggrBgEFBQcBAQRtMGswJAYIKwYBBQUHMAGGGGh0dHA6Ly9v
# Y3NwLmRpZ2ljZXJ0LmNvbTBDBggrBgEFBQcwAoY3aHR0cDovL2NhY2VydHMuZGln
# aWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNydDCBgQYDVR0fBHow
# eDA6oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJl
# ZElEUm9vdENBLmNybDA6oDigNoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0Rp
# Z2lDZXJ0QXNzdXJlZElEUm9vdENBLmNybDBPBgNVHSAESDBGMDgGCmCGSAGG/WwA
# AgQwKjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzAK
# BghghkgBhv1sAzAdBgNVHQ4EFgQUWsS5eyoKo6XqcQPAYPkt9mV1DlgwHwYDVR0j
# BBgwFoAUReuir/SSy4IxLVGLp6chnfNtyA8wDQYJKoZIhvcNAQELBQADggEBAD7s
# DVoks/Mi0RXILHwlKXaoHV0cLToaxO8wYdd+C2D9wz0PxK+L/e8q3yBVN7Dh9tGS
# dQ9RtG6ljlriXiSBThCk7j9xjmMOE0ut119EefM2FAaK95xGTlz/kLEbBw6RFfu6
# r7VRwo0kriTGxycqoSkoGjpxKAI8LpGjwCUR4pwUR6F6aGivm6dcIFzZcbEMj7uo
# +MUSaJ/PQMtARKUT8OZkDCUIQjKyNookAv4vcn4c10lFluhZHen6dGRrsutmQ9qz
# sIzV6Q3d9gEgzpkxYz0IGhizgZtPxpMQBvwHgfqL2vmCSfdibqFT+hKUGIUukpHq
# aGxEMrJmoecYpJpkUe8wggVAMIIEKKADAgECAhAHdGbtomdvOuySF9IwU3EQMA0G
# CSqGSIb3DQEBCwUAMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJ
# bmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lDZXJ0
# IFNIQTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0EwHhcNMTYwMzI0MDAwMDAw
# WhcNMTcwMzI4MTIwMDAwWjB9MQswCQYDVQQGEwJVUzEPMA0GA1UECBMGS2Fuc2Fz
# MQ8wDQYDVQQHEwZUb3Bla2ExJTAjBgNVBAoTHFJlYWxEaW1lbnNpb25zIFNvZnR3
# YXJlLCBMTEMxJTAjBgNVBAMTHFJlYWxEaW1lbnNpb25zIFNvZnR3YXJlLCBMTEMw
# ggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC52YS2WKUpBtdkDyJ3G0Qm
# 42v+W+yqr7DediVzIeCjHpKNkmmxO8+lS+nniFDjoFGO1JG/G3ZywVRFlM1LKHeP
# M1eON6wT0H1gvhxpMzyuC/SRW9wvMtTlvHnjdTLW06Oe9tvGkQkTM8rbzwRDIZ9o
# ddT8BxHSOmGelrAN9CwKf60ziw8jKLZnuAuZwSgkX5K7wvOs8viqydlnt7z3Wyim
# L+wjue85Mpa7jyjIfnUqssN1qz4nce+e89CxTD2AbWjGwnfTcTgmj3EUSJRQgDRk
# J+O/sVzS/V76xajLoPvI4ZlAsMpeK3ptLYqviU3ZaNUzLQWFjuWqc3fhjbWDFF51
# AgMBAAGjggHFMIIBwTAfBgNVHSMEGDAWgBRaxLl7KgqjpepxA8Bg+S32ZXUOWDAd
# BgNVHQ4EFgQUT0GgvxvwrOqMjXQ8yw7QDagIVJ4wDgYDVR0PAQH/BAQDAgeAMBMG
# A1UdJQQMMAoGCCsGAQUFBwMDMHcGA1UdHwRwMG4wNaAzoDGGL2h0dHA6Ly9jcmwz
# LmRpZ2ljZXJ0LmNvbS9zaGEyLWFzc3VyZWQtY3MtZzEuY3JsMDWgM6Axhi9odHRw
# Oi8vY3JsNC5kaWdpY2VydC5jb20vc2hhMi1hc3N1cmVkLWNzLWcxLmNybDBMBgNV
# HSAERTBDMDcGCWCGSAGG/WwDATAqMCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5k
# aWdpY2VydC5jb20vQ1BTMAgGBmeBDAEEATCBhAYIKwYBBQUHAQEEeDB2MCQGCCsG
# AQUFBzABhhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wTgYIKwYBBQUHMAKGQmh0
# dHA6Ly9jYWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFNIQTJBc3N1cmVkSURD
# b2RlU2lnbmluZ0NBLmNydDAMBgNVHRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IB
# AQAD6K2ldfDx7hOZxW6smYkiV4lxXY4bewxGv/gh2hjWgLiQ/sornz2fHDni/kOf
# qhn8/3KvYd4V33QqMqjm/qsRpwjj9NC/Q2BGuTg0ax3e/Z9ZIvcYB4xx8CRbGKse
# R9lixgMAJZMCWyGzAC/E3XX+BX3B6y8N5zBIKRY1M7xub+LM7zW9LGMhX3e56J7G
# UF7zIzQ7ZkaJzfxFbVvEz2/KNoNGiCmA7Y0biMXpX9730Dbg4Z+B4SUe4k4WPLS/
# 3goAq8lVMFtoqShvyvrmYtj2gFjQmH3BzSCSRZrAFbWYDCga57Fq7A4xrF2i67kG
# oljzeP+/35wuoOlrggn2EuJ1MIIGajCCBVKgAwIBAgIQAwGaAjr/WLFr1tXq5hfw
# ZjANBgkqhkiG9w0BAQUFADBiMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNl
# cnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMSEwHwYDVQQDExhEaWdp
# Q2VydCBBc3N1cmVkIElEIENBLTEwHhcNMTQxMDIyMDAwMDAwWhcNMjQxMDIyMDAw
# MDAwWjBHMQswCQYDVQQGEwJVUzERMA8GA1UEChMIRGlnaUNlcnQxJTAjBgNVBAMT
# HERpZ2lDZXJ0IFRpbWVzdGFtcCBSZXNwb25kZXIwggEiMA0GCSqGSIb3DQEBAQUA
# A4IBDwAwggEKAoIBAQCjZF38fLPggjXg4PbGKuZJdTvMbuBTqZ8fZFnmfGt/a4yd
# VfiS457VWmNbAklQ2YPOb2bu3cuF6V+l+dSHdIhEOxnJ5fWRn8YUOawk6qhLLJGJ
# zF4o9GS2ULf1ErNzlgpno75hn67z/RJ4dQ6mWxT9RSOOhkRVfRiGBYxVh3lIRvfK
# Do2n3k5f4qi2LVkCYYhhchhoubh87ubnNC8xd4EwH7s2AY3vJ+P3mvBMMWSN4+v6
# GYeofs/sjAw2W3rBerh4x8kGLkYQyI3oBGDbvHN0+k7Y/qpA8bLOcEaD6dpAoVk6
# 2RUJV5lWMJPzyWHM0AjMa+xiQpGsAsDvpPCJEY93AgMBAAGjggM1MIIDMTAOBgNV
# HQ8BAf8EBAMCB4AwDAYDVR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcD
# CDCCAb8GA1UdIASCAbYwggGyMIIBoQYJYIZIAYb9bAcBMIIBkjAoBggrBgEFBQcC
# ARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzCCAWQGCCsGAQUFBwICMIIB
# Vh6CAVIAQQBuAHkAIAB1AHMAZQAgAG8AZgAgAHQAaABpAHMAIABDAGUAcgB0AGkA
# ZgBpAGMAYQB0AGUAIABjAG8AbgBzAHQAaQB0AHUAdABlAHMAIABhAGMAYwBlAHAA
# dABhAG4AYwBlACAAbwBmACAAdABoAGUAIABEAGkAZwBpAEMAZQByAHQAIABDAFAA
# LwBDAFAAUwAgAGEAbgBkACAAdABoAGUAIABSAGUAbAB5AGkAbgBnACAAUABhAHIA
# dAB5ACAAQQBnAHIAZQBlAG0AZQBuAHQAIAB3AGgAaQBjAGgAIABsAGkAbQBpAHQA
# IABsAGkAYQBiAGkAbABpAHQAeQAgAGEAbgBkACAAYQByAGUAIABpAG4AYwBvAHIA
# cABvAHIAYQB0AGUAZAAgAGgAZQByAGUAaQBuACAAYgB5ACAAcgBlAGYAZQByAGUA
# bgBjAGUALjALBglghkgBhv1sAxUwHwYDVR0jBBgwFoAUFQASKxOYspkH7R7for5X
# DStnAs0wHQYDVR0OBBYEFGFaTSS2STKdSip5GoNL9B6Jwcp9MH0GA1UdHwR2MHQw
# OKA2oDSGMmh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJ
# RENBLTEuY3JsMDigNqA0hjJodHRwOi8vY3JsNC5kaWdpY2VydC5jb20vRGlnaUNl
# cnRBc3N1cmVkSURDQS0xLmNybDB3BggrBgEFBQcBAQRrMGkwJAYIKwYBBQUHMAGG
# GGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBBBggrBgEFBQcwAoY1aHR0cDovL2Nh
# Y2VydHMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEQ0EtMS5jcnQwDQYJ
# KoZIhvcNAQEFBQADggEBAJ0lfhszTbImgVybhs4jIA+Ah+WI//+x1GosMe06Fxlx
# F82pG7xaFjkAneNshORaQPveBgGMN/qbsZ0kfv4gpFetW7easGAm6mlXIV00Lx9x
# sIOUGQVrNZAQoHuXx/Y/5+IRQaa9YtnwJz04HShvOlIJ8OxwYtNiS7Dgc6aSwNOO
# Mdgv420XEwbu5AO2FKvzj0OncZ0h3RTKFV2SQdr5D4HRmXQNJsQOfxu19aDxxncG
# KBXp2JPlVRbwuwqrHNtcSCdmyKOLChzlldquxC5ZoGHd2vNtomHpigtt7BIYvfdV
# VEADkitrwlHCCkivsNRu4PQUCjob4489yq9qjXvc2EQwggbNMIIFtaADAgECAhAG
# /fkDlgOt6gAK6z8nu7obMA0GCSqGSIb3DQEBBQUAMGUxCzAJBgNVBAYTAlVTMRUw
# EwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20x
# JDAiBgNVBAMTG0RpZ2lDZXJ0IEFzc3VyZWQgSUQgUm9vdCBDQTAeFw0wNjExMTAw
# MDAwMDBaFw0yMTExMTAwMDAwMDBaMGIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxE
# aWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xITAfBgNVBAMT
# GERpZ2lDZXJ0IEFzc3VyZWQgSUQgQ0EtMTCCASIwDQYJKoZIhvcNAQEBBQADggEP
# ADCCAQoCggEBAOiCLZn5ysJClaWAc0Bw0p5WVFypxNJBBo/JM/xNRZFcgZ/tLJz4
# FlnfnrUkFcKYubR3SdyJxArar8tea+2tsHEx6886QAxGTZPsi3o2CAOrDDT+GEmC
# /sfHMUiAfB6iD5IOUMnGh+s2P9gww/+m9/uizW9zI/6sVgWQ8DIhFonGcIj5BZd9
# o8dD3QLoOz3tsUGj7T++25VIxO4es/K8DCuZ0MZdEkKB4YNugnM/JksUkK5ZZgrE
# jb7SzgaurYRvSISbT0C58Uzyr5j79s5AXVz2qPEvr+yJIvJrGGWxwXOt1/HYzx4K
# dFxCuGh+t9V3CidWfA9ipD8yFGCV/QcEogkCAwEAAaOCA3owggN2MA4GA1UdDwEB
# /wQEAwIBhjA7BgNVHSUENDAyBggrBgEFBQcDAQYIKwYBBQUHAwIGCCsGAQUFBwMD
# BggrBgEFBQcDBAYIKwYBBQUHAwgwggHSBgNVHSAEggHJMIIBxTCCAbQGCmCGSAGG
# /WwAAQQwggGkMDoGCCsGAQUFBwIBFi5odHRwOi8vd3d3LmRpZ2ljZXJ0LmNvbS9z
# c2wtY3BzLXJlcG9zaXRvcnkuaHRtMIIBZAYIKwYBBQUHAgIwggFWHoIBUgBBAG4A
# eQAgAHUAcwBlACAAbwBmACAAdABoAGkAcwAgAEMAZQByAHQAaQBmAGkAYwBhAHQA
# ZQAgAGMAbwBuAHMAdABpAHQAdQB0AGUAcwAgAGEAYwBjAGUAcAB0AGEAbgBjAGUA
# IABvAGYAIAB0AGgAZQAgAEQAaQBnAGkAQwBlAHIAdAAgAEMAUAAvAEMAUABTACAA
# YQBuAGQAIAB0AGgAZQAgAFIAZQBsAHkAaQBuAGcAIABQAGEAcgB0AHkAIABBAGcA
# cgBlAGUAbQBlAG4AdAAgAHcAaABpAGMAaAAgAGwAaQBtAGkAdAAgAGwAaQBhAGIA
# aQBsAGkAdAB5ACAAYQBuAGQAIABhAHIAZQAgAGkAbgBjAG8AcgBwAG8AcgBhAHQA
# ZQBkACAAaABlAHIAZQBpAG4AIABiAHkAIAByAGUAZgBlAHIAZQBuAGMAZQAuMAsG
# CWCGSAGG/WwDFTASBgNVHRMBAf8ECDAGAQH/AgEAMHkGCCsGAQUFBwEBBG0wazAk
# BggrBgEFBQcwAYYYaHR0cDovL29jc3AuZGlnaWNlcnQuY29tMEMGCCsGAQUFBzAC
# hjdodHRwOi8vY2FjZXJ0cy5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURS
# b290Q0EuY3J0MIGBBgNVHR8EejB4MDqgOKA2hjRodHRwOi8vY3JsMy5kaWdpY2Vy
# dC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3JsMDqgOKA2hjRodHRwOi8v
# Y3JsNC5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3JsMB0G
# A1UdDgQWBBQVABIrE5iymQftHt+ivlcNK2cCzTAfBgNVHSMEGDAWgBRF66Kv9JLL
# gjEtUYunpyGd823IDzANBgkqhkiG9w0BAQUFAAOCAQEARlA+ybcoJKc4HbZbKa9S
# z1LpMUerVlx71Q0LQbPv7HUfdDjyslxhopyVw1Dkgrkj0bo6hnKtOHisdV0XFzRy
# R4WUVtHruzaEd8wkpfMEGVWp5+Pnq2LN+4stkMLA0rWUvV5PsQXSDj0aqRRbpoYx
# YqioM+SbOafE9c4deHaUJXPkKqvPnHZL7V/CSxbkS3BMAIke/MV5vEwSV/5f4R68
# Al2o/vsHOE8Nxl2RuQ9nRc3Wg+3nkg2NsWmMT/tZ4CMP0qquAHzunEIOz5HXJ7cW
# 7g/DvXwKoO4sCFWFIrjrGBpN/CohrUkxg0eVd3HcsRtLSxwQnHcUwZ1PL1qVCCkQ
# JjGCBEwwggRIAgEBMIGGMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2Vy
# dCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lD
# ZXJ0IFNIQTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0ECEAd0Zu2iZ2867JIX
# 0jBTcRAwDQYJYIZIAWUDBAIBBQCggYQwGAYKKwYBBAGCNwIBDDEKMAigAoAAoQKA
# ADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4wDAYK
# KwYBBAGCNwIBFTAvBgkqhkiG9w0BCQQxIgQgxj7/I/QG74A+fVjsJ0TY2YQefUSG
# 7TwahaO7tQvFpPcwDQYJKoZIhvcNAQEBBQAEggEAewsDLf6yHvCM865xnbtr/E+N
# kWLhAJrhdnSrscDELxy38zGNzpUmYd0DU+3TBKgoi38NqJWSwXuT78+7+I083sak
# PU9Hx86HJV6px72LYDACnS8Lm4Fhx30/Agtft+P1/U99nmUYxPOVfyrSEA6sEVo8
# 3RY72tRaLhPce5myWldARYF0fqm2o5Bub3WcGEmLvTKJPKdXA5h0BqH2Xr7S5hnT
# K02hIacRdZhmlBg5lSGXHAFsRoBaNUiIZyIajRcBiVxPfl5hUcHqzTXFJ/ksyxeq
# CvQGo2iYANlG1+j2J5gWJ2/TvNge3LewPVyyGhs+QYp2yFFG4kwLWqT4nI6IL6GC
# Ag8wggILBgkqhkiG9w0BCQYxggH8MIIB+AIBATB2MGIxCzAJBgNVBAYTAlVTMRUw
# EwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20x
# ITAfBgNVBAMTGERpZ2lDZXJ0IEFzc3VyZWQgSUQgQ0EtMQIQAwGaAjr/WLFr1tXq
# 5hfwZjAJBgUrDgMCGgUAoF0wGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkq
# hkiG9w0BCQUxDxcNMTYwOTE5MTcyNzA1WjAjBgkqhkiG9w0BCQQxFgQUSEP8J10H
# yt5ukP2D/MG8tL7XUcQwDQYJKoZIhvcNAQEBBQAEggEAIVBGAS4dSJp25cDnLiaV
# z2ym05k3Uqd5vNMy3J103k1rcH/PanJrFWe67P4UM2OYwpHAW4ZajfXEoA4aaMdb
# hU2r0xkcHMTg/rEdqJKyZ8Cdj2r2GVDITZg7q2U5RFUxPqQy0+gMHo7xrNAOT0ee
# +Zv5/2hZfXIvIXm2bOaPxDJye1/y+DLx/IXgGwNcNmf4V8oT370DbDVTolhLp6Fc
# 4IoUjAAZpeXkKeYNfZVwLPIs/92ywQzM/7jnSMDI6X5OGik/robJZCW0h3Ch8rcH
# 5r6Ogg4ko5MWzkGS+yXHqeBeYVM4Q6g8snUg1bO/GpXZuXH6zsRic68wCE0KRtWB
# dg==
# SIG # End signature block
