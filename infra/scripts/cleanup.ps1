Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Cleaning up Kubernetes Resources" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "Deleting application resources..." -ForegroundColor Gray
kubectl delete -f infra/core/ -o name 2>$null
kubectl delete -f infra/services/ -o name 2>$null
kubectl delete -f infra/observability/ -o name 2>$null
kubectl delete -f infra/networking/ -o name 2>$null

Write-Host "" 
Write-Host "✓ Resources deleted" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Cleanup complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Check remaining resources:" -ForegroundColor Gray
kubectl get all
kubectl get pvc
kubectl get secrets
kubectl get configmaps
