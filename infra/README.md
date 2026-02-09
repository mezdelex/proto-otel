# Kubernetes Infrastructure for Kind Cluster

Kubernetes infrastructure for deploying `proto-otel` application to a local `kind` cluster.

## Directory Structure

```
infra/
├── core/
│   ├── configmaps.yaml
│   ├── kustomization.yaml
│   ├── secrets.yaml
│   └── volumes.yaml
│
├── services/
│   ├── expenses-deployment.yaml
│   ├── gateway-deployment.yaml
│   ├── kustomization.yaml
│   ├── notifications-deployment.yaml
│   ├── rabbitmq-deployment.yaml
│   ├── redis-deployment.yaml
│   └── sqlserver-deployment.yaml
│
├── observability/
│   ├── data-prepper.yaml
│   ├── kustomization.yaml
│   ├── opensearch-dashboards.yaml
│   ├── opensearch.yaml
│   └── otel-collector.yaml
│
├── networking/
│   ├── ingress.yaml
│   └── kustomization.yaml
│
├── scripts/
│   ├── BuildAndLoad.ps1
│   └── CleanUp.ps1
│
└── kustomization.yaml
```

## Deployment

```powershell
# 1. Create kind cluster
kind create cluster --name kind

# 2. Build and load Docker images
.\infra\scripts\BuildAndLoad.ps1

# 3. Deploy with kustomize
kubectl apply -k .\infra\
```

## Scripts

| Script             | Purpose                                        |
| ------------------ | ---------------------------------------------- |
| `BuildAndLoad.ps1` | Builds and loads Docker images to kind cluster |
| `CleanUp.ps1`      | Removes all deployed resources                 |

## Ports

> [!NOTE]
> Ports are exposed by Ingress, no need to use `type: LoadBalancer` like in cloud services

| Service               | Port | Type      |
| --------------------- | ---- | --------- |
| Gateway HTTP          | 8000 | ClusterIP |
| Gateway HTTPS         | 8100 | ClusterIP |
| Expenses HTTP         | 8001 | ClusterIP |
| Expenses HTTPS        | 8101 | ClusterIP |
| Notifications HTTP    | 8002 | ClusterIP |
| Notifications HTTPS   | 8102 | ClusterIP |
| OpenSearch            | 9200 | ClusterIP |
| OpenSearch Dashboards | 5601 | ClusterIP |

## Requirements

- Docker Desktop (or Docker Engine)
- kind v0.20.0+
- kubectl v1.29.0+
- kustomize v5.8.0+
- PowerShell 7+
