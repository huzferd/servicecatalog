param(
    [string] [Parameter(Mandatory = $true)]$subscriptionId,
    [string] [Parameter(Mandatory = $true)]$tenantId,
    [string] [Parameter(Mandatory = $true)]$siteName,
    [string] [Parameter(Mandatory = $true)]$password
)

Import-Module AzureRM.Resources

$url = "https://$siteName.azurewebsites.net/"
$replyUrl = "$url.auth/login/aad/callback"
$replyUrls = @($url, $replyUrl)
$endDate = (Get-Date).AddYears(4)

# Login to your Azure Subscription
Login-AzureRMAccount
Set-AzureRMContext -SubscriptionId $subscriptionId -TenantId $tenantId

# Create an Octopus Deploy Application in Active Directory1.1
Write-Output "Creating AAD application..."
$securePassword = ConvertTo-SecureString $password -AsPlainText -Force
$azureAdApplication = New-AzureRmADApplication -DisplayName $siteName -HomePage $url -IdentifierUris $url -Password $securePassword -ReplyUrls $replyUrls -EndDate $endDate

# Create the Service Principal
Write-Output "Creating AAD service principal..."
New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId

# Sleep, to Ensure the Service Principal is Actually Created
Write-Output "Sleeping for 10s to give the service principal a chance to finish creating..."
Start-Sleep -s 10

# Assign the Service Principal the Contributor Role to the Subscription.
Write-Output "Assigning the Contributor role to the service principal..."
Connect-AzureAD
New-AzureRmRoleAssignment -RoleDefinitionName Contributor -ServicePrincipalName $azureAdApplication.ApplicationId

# Assign delegated permissions
Write-Output "Assigning the delegated permissions..."
$requiredResources = [System.Collections.Generic.List[Microsoft.Open.AzureAD.Model.RequiredResourceAccess]]::New()

# Section 1 | Windows Azure Active Directory
$requiredResourceAccess1 = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]::New()
$requiredResourceAccess1.ResourceAppId = "00000002-0000-0000-c000-000000000000"

$resourceAccess1_1 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess1_1.Id = "cba73afc-7f69-4d86-8450-4978e04ecd1a"
$resourceAccess1_1.Type = "Scope"

$resourceAccess1_2 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess1_2.Id = "311a71cc-e848-46a1-bdf8-97ff7156d8e6"
$resourceAccess1_2.Type = "Scope"

$resourceAccess1_3 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess1_3.Id = "5778995a-e1bf-45b8-affa-663a9f3f4d04"
$resourceAccess1_3.Type = "Scope"

$resourceAccess1_4 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess1_4.Id = "5778995a-e1bf-45b8-affa-663a9f3f4d04"
$resourceAccess1_4.Type = "Role"

$resourceAccess1_5 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess1_5.Id = "6234d376-f627-4f0f-90e0-dff25c5211a3"
$resourceAccess1_5.Type = "Scope"

$resourceAccess1_6 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess1_6.Id = "c582532d-9d9e-43bd-a97c-2667a28ce295"
$resourceAccess1_6.Type = "Scope"

$requiredResourceAccess1.ResourceAccess = $resourceAccess1_1, $resourceAccess1_2, $resourceAccess1_3, $resourceAccess1_4, $resourceAccess1_5, $resourceAccess1_6
$requiredResources.Add($requiredResourceAccess1)

# Section 2 | Windows Azure Service Management API
$requiredResourceAccess2 = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]::New()
$requiredResourceAccess2.ResourceAppId = "797f4846-ba00-4fd7-ba43-dac1f8f63013"

$resourceAccess2_1 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess2_1.Id = "41094075-9dad-400e-a0bd-54e686782033"
$resourceAccess2_1.Type = "Scope"

$requiredResourceAccess2.ResourceAccess = $resourceAccess2_1
$requiredResources.Add($requiredResourceAccess2);

# Section 3 | Microsoft Graph
$requiredResourceAccess3 = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]::New()
$requiredResourceAccess3.ResourceAppId = "00000003-0000-0000-c000-000000000000"

$resourceAccess3_1 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess3_1.Id = "7ab1d382-f21e-4acd-a863-ba3e13f7da61"
$resourceAccess3_1.Type = "Role"

$resourceAccess3_2 = [Microsoft.Open.AzureAD.Model.ResourceAccess]::New()
$resourceAccess3_2.Id = "06da0dbc-49e2-44d2-8312-53f166ab848a"
$resourceAccess3_2.Type = "Scope"

$requiredResourceAccess3.ResourceAccess = $resourceAccess3_1, $resourceAccess3_2;
$requiredResources.Add($requiredResourceAccess3);

# Update Azure AD Application
Set-AzureADApplication -ObjectId $azureAdApplication.ObjectId -RequiredResourceAccess $requiredResources

# Generate a client secret
$passwordCredential = New-AzureADApplicationPasswordCredential -ObjectId $azureAdApplication.ObjectId -StartDate $now -EndDate $endDate

# Get Azure Tenant Domain Name
$tenant = Get-AzureRmTenant

# Display Output Parameters
Write-Output ""
Write-Output "====== Application Configs ======"
Write-Output "Client ID     : $($azureAdApplication.ApplicationId)"
Write-Output "Client Secret : $($passwordCredential.Value)"
Write-Output "Tenant Name   : $($tenant.Directory)"
Write-Output "Site Name     : $($siteName)"
