param (
    [Parameter(Mandatory = $true)]
    [string]
    $billingPeriod
)

$connectionName = "AzureRunAsConnection"
try {
    $servicePrincipalConnection = Get-AutomationConnection -Name $connectionName

    "Logging in to Azure..."
    Add-AzureRmAccount `
        -ServicePrincipal `
        -TenantId $servicePrincipalConnection.TenantId `
        -ApplicationId $servicePrincipalConnection.ApplicationId `
        -CertificateThumbprint $servicePrincipalConnection.CertificateThumbprint 
}
catch {
    if (!$servicePrincipalConnection) {
        $ErrorMessage = "Connection $connectionName not found."
        throw $ErrorMessage
    }
    else {
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}
$invoices = @{}
switch ($billingPeriod) {
    "Latest" {
        $invoices = Get-AzureRmBillingInvoice -Latest
    }
    "All" {
        $invoices = Get-AzureRmBillingInvoice -GenerateDownloadUrl
    }
    Default {
        Write-Error "$billingPeriod - incorrect period"
    }
}

foreach ($invoice in $invoices) {
    Write-Host $invoice.Name | $invoice.DownloadUrl 
}