param(
    [string]$ClusterName = "kind",
    [string]$ImageVersion = "latest"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building and Loading Images to Kind" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Cluster: $ClusterName" -ForegroundColor Gray
Write-Host "Version: $ImageVersion" -ForegroundColor Gray
Write-Host ""
$kindClusters = kind get clusters 2>&1
if (-not ($kindClusters -match $ClusterName))
{
    Write-Host "Warning: Kind cluster '$ClusterName' not found" -ForegroundColor Yellow
    Write-Host "Creating cluster '$ClusterName'..." -ForegroundColor Yellow
    kind create cluster --name $ClusterName
} else
{
    Write-Host "Kind cluster '$ClusterName' found" -ForegroundColor Green
}
Write-Host ""
Write-Host "Building Docker images..." -ForegroundColor Cyan
Write-Host "" 
Write-Host "Building Gateway image..." -ForegroundColor Yellow
docker build `
    -t "localhost/gateway:$ImageVersion" `
    -f src/Gateway/Dockerfile .
if ($LASTEXITCODE -ne 0)
{ 
    Write-Host "Error: Failed to build gateway image" -ForegroundColor Red exit 1 
}
Write-Host "✓ Gateway image built" -ForegroundColor Green
Write-Host "" 
Write-Host "Building Expenses image..." -ForegroundColor Yellow
docker build `
    -t "localhost/expenses:$ImageVersion" `
    -f src/Services/Expenses/Dockerfile .
if ($LASTEXITCODE -ne 0)
{
    Write-Host "Error: Failed to build expenses image" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Expenses image built" -ForegroundColor Green
Write-Host ""
Write-Host "Building Notifications image..." -ForegroundColor Yellow
docker build `
    -t "localhost/notifications:$ImageVersion" `
    -f src/Services/Notifications/Dockerfile .
if ($LASTEXITCODE -ne 0)
{
    Write-Host "Error: Failed to build notifications image" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Notifications image built" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Loading Images to Kind Cluster" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "" 
Write-Host "Loading gateway image..." -ForegroundColor Yellow
kind load docker-image "localhost/gateway:$ImageVersion" --name $ClusterName
Write-Host "✓ Gateway image loaded" -ForegroundColor Green
Write-Host "" 
Write-Host "Loading expenses image..." -ForegroundColor Yellow
kind load docker-image "localhost/expenses:$ImageVersion" --name $ClusterName
Write-Host "✓ Expenses image loaded" -ForegroundColor Green
Write-Host "" 
Write-Host "Loading notifications image..." -ForegroundColor Yellow
kind load docker-image "localhost/notifications:$ImageVersion" --name $ClusterName
Write-Host "✓ Notifications image loaded" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Images built and loaded successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Deploy to cluster with: kubectl apply -k .\infra\" -ForegroundColor Gray
