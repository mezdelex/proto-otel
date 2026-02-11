# Kubernetes Infrastructure for Kind Cluster

Kubernetes infrastructure for deploying `proto-otel` application to a local `kind` cluster.

## Directory Structure

```
├───kind-config.yaml
├───kustomization.yaml
├───README.md
│
├───core
│   ├───configmaps.yaml
│   ├───kustomization.yaml
│   └───volumes.yaml
│
├───networking
│   ├───ingress.yaml
│   └───kustomization.yaml
│
├───observability
│   ├───data-prepper.yaml
│   ├───kustomization.yaml
│   ├───opensearch-dashboards.yaml
│   ├───opensearch.yaml
│   └───otel-collector.yaml
│
├───scripts
│   ├───BuildAndLoad.ps1
│   └───CleanUp.ps1
│
└───services
    ├───expenses-deployment.yaml
    ├───gateway-deployment.yaml
    ├───kustomization.yaml
    ├───notifications-deployment.yaml
    ├───rabbitmq-deployment.yaml
    ├───redis-deployment.yaml
    └───sqlserver-deployment.yaml
```

## Deployment

```powershell
# 1. Create kind cluster and expose needed node ports
kind create cluster --name kind --config .\infra\kind-config.yaml

# 2. Build and load Docker images to kind
.\infra\scripts\BuildAndLoad.ps1

# 3. Generate the secrets in kubernetes
kubectl create secret generic app-certs --from-file=localhost.pfx=$HOME/.certs/localhost.pfx && kubectl create secret generic app-secrets --from-env-file=.env

# 4. Deploy with kustomize
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

## Current WIP Status

<img width="1919" height="1026" alt="image" src="https://github.com/user-attachments/assets/e9622a98-432c-4a5d-b88d-3183eb6f1e8f" />
<img width="1919" height="1022" alt="image" src="https://github.com/user-attachments/assets/e83011c3-474d-428c-aa70-e0bc84ee95be" />

> [!IMPORTANT]
> Requests are being redirected properly, but the services are not accepting them; pending further investigation
