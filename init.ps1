Function Test-Administrator  
{  
    [OutputType([bool])]
    param()
    process {
        [Security.Principal.WindowsPrincipal]$user = [Security.Principal.WindowsIdentity]::GetCurrent();
        return $user.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator);
    }
}

Function Confirm-Administrator
{
    if(-not (Test-Administrator))
    {
        Write-Error "You must run the PowerShell as administrator to setup build and test environment.";
        exit 1;
    }
}

Function Get-TimeStamp {
    return "[{0:MM/dd/yy} {0:HH:mm:ss}]" -f (Get-Date)
}

Function Install-CosmosEmulator {
    Write-Output "Checking whether Azure Cosmos Emulator installed on this machine."
    Write-Output ""

    $exeFolder = "C:\Program Files\Azure Cosmos DB Emulator"
    $exePath = $exeFolder + "\Microsoft.Azure.Cosmos.Emulator.exe"

    # Install the Cosmos DB emulator if not installed
    if (-not (Test-Path "$exePath")) {

        $downloadPath = $ENV:TEMP + "\CoyoteWorkshopDev\"
        $msi = $downloadPath + "azure-cosmosdb-emulator.msi"
        $downloadLink = "https://aka.ms/cosmosdb-emulator"

        New-Item -ItemType Directory -Force -Path $downloadPath

        if (-not (Test-Path "$msi")) {
            Write-Host "$(Get-TimeStamp) Downloading $downloadLink in folder $msi."
            Invoke-WebRequest $downloadLink -OutFile $msi;
        }

        Write-Host "$(Get-TimeStamp) Installing cosmos DB Emulator..."
        Start-Process "msiexec.exe" -Wait -ArgumentList "/i $msi /quiet /qn /norestart"

        if (-not (Test-Path $exePath)) {
            throw "Cosmos Emulator installation failed"
        }

        Write-Host "$(Get-TimeStamp) Deleting the installer package"
        Remove-Item -Path $downloadPath -Recurse
    }
    else {
        Write-Output "Found Azure Cosmos Emulator Installed in $exeFolder"
        Write-Output ""
    }
}

Function Install-AzureStorageEmulator {
    Write-Output "Checking whether Azure Storage Emulator installed on this machine."
    Write-Output ""

    $exeFolder = "C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator"
    $exePath = $exeFolder + "\AzureStorageEmulator.exe"

    # Install the Azure Storage emulator if not installed
    if (-not (Test-Path "$exePath")) {

        $downloadPath = $ENV:TEMP + "\CoyoteWorkshopDev\"
        $msi = $downloadPath + "microsoftazurestorageemulator.msi"
        $downloadLink = "https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409"

        New-Item -ItemType Directory -Force -Path $downloadPath

        if (-not (Test-Path "$msi")) {
            Write-Host "$(Get-TimeStamp) Downloading $downloadLink in folder $msi."
            Invoke-WebRequest $downloadLink -OutFile $msi;
        }

        Write-Host "$(Get-TimeStamp) Installing Azure Storage Emulator..."
        Start-Process "msiexec.exe" -Wait -ArgumentList "/i $msi /quiet /qn /norestart"

        if (-not (Test-Path $exePath)) {
            throw "Azure Storage Emulator installation failed"
        }

        Write-Host "$(Get-TimeStamp) Deleting the installer package"
        Remove-Item -Path $downloadPath -Recurse
    }
    else {
        Write-Output "Found Azure Storage Emulator Installed in $exeFolder"
        Write-Output ""
    }
}

Confirm-Administrator

Install-CosmosEmulator

Install-AzureStorageEmulator

Write-Host "Initialization completed."
Write-Host "Optionally run `Install-AzureStorageExplorer` to install Azure Storage Explorer."

# Utility Functions

Function global:Install-AzureStorageExplorer {
    Write-Output "Checking whether Azure Storage Explorer installed on this machine."
    Write-Output ""

    $exeFolder = $env:LOCALAPPDATA + "\Programs\Microsoft Azure Storage Explorer"
    $exePath = $exeFolder + "\StorageExplorer.exe"

    # Install the Azure Storage explorer if not installed
    if (-not (Test-Path "$exePath")) {

        $downloadPath = $ENV:TEMP + "\CoyoteWorkshopDev\"
        $exe = $downloadPath + "StorageExplorer.exe"
        $downloadLink = "https://go.microsoft.com/fwlink/?LinkId=708343&clcid=0x409"

        New-Item -ItemType Directory -Force -Path $downloadPath

        if (-not (Test-Path "$exe")) {
            Write-Host "$(Get-TimeStamp) Downloading $downloadLink in folder $exe."
            Invoke-WebRequest $downloadLink -OutFile $exe;
        }

        Write-Host "$(Get-TimeStamp) Installing Azure Storage Explorer..."
        Start-Process $exe -Wait -ArgumentList "/verysilent /norestart /currentuser"

        if (-not (Test-Path $exePath)) {
            throw "Azure Storage Explorer installation failed"
        }

        Write-Host "$(Get-TimeStamp) Deleting the installer package"
        Remove-Item -Path $downloadPath -Recurse
    }
    else {
        Write-Output "Found Azure Storage Explorer Installed in $exeFolder"
        Write-Output ""
    }
}