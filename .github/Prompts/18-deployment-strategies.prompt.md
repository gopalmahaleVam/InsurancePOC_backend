---
description: 'Create a multi-stage Dockerfile and GitHub Actions CI/CD pipeline for build, test, and deploy.'
mode: 'agent'
tools: ['edit/editFiles', 'execute/runInTerminal']
---

# Configure Deployment Strategy

## Mission
Create a multi-stage `Dockerfile` and a GitHub Actions CI/CD pipeline for `${input:projectName:YourProject}` that builds, tests, and deploys to `${input:deployTarget:Azure App Service}`.

## Scope & Preconditions
- Steps 01–17 must be completed and `dotnet test` must pass
- Namespace root: `${input:namespace:YourCompany.YourProject}`
- Deploy target: `${input:deployTarget:Azure App Service}`
- Container registry: `${input:containerRegistry:ghcr.io}`
- If `deployTarget` is not provided, default to `Azure App Service`

## Inputs
| Input | Description | Default |
|---|---|---|
| `projectName` | Solution name | `YourProject` |
| `namespace` | Root namespace | `YourCompany.YourProject` |
| `deployTarget` | Deployment target | `Azure App Service` |
| `containerRegistry` | Container registry URL | `ghcr.io` |

## Workflow

### Step 1 — Create Multi-Stage Dockerfile
Create `Dockerfile` at solution root:
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/${input:namespace}.API/${input:namespace}.API.csproj", "src/${input:namespace}.API/"]
COPY ["src/${input:namespace}.Application/${input:namespace}.Application.csproj", "src/${input:namespace}.Application/"]
COPY ["src/${input:namespace}.Domain/${input:namespace}.Domain.csproj", "src/${input:namespace}.Domain/"]
COPY ["src/${input:namespace}.Infrastructure/${input:namespace}.Infrastructure.csproj", "src/${input:namespace}.Infrastructure/"]
RUN dotnet restore "src/${input:namespace}.API/${input:namespace}.API.csproj"
COPY . .
RUN dotnet build "src/${input:namespace}.API/${input:namespace}.API.csproj" -c Release -o /app/build

# Test stage
FROM build AS test
RUN dotnet test --no-build -c Release

# Publish stage
FROM build AS publish
RUN dotnet publish "src/${input:namespace}.API/${input:namespace}.API.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "${input:namespace}.API.dll"]
```

### Step 2 — Create .dockerignore
Create `.dockerignore` at solution root:
```
**/bin/
**/obj/
**/.git/
**/node_modules/
**/*.user
.vs/
```

### Step 3 — Create GitHub Actions CI/CD Pipeline
Create `.github/workflows/ci-cd.yml`:
```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  REGISTRY: ${input:containerRegistry}
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore -c Release

      - name: Test
        run: dotnet test --no-build -c Release --collect:"XPlat Code Coverage"

  build-and-push-image:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    permissions:
      contents: read
      packages: write
    steps:
      - uses: actions/checkout@v4

      - name: Log in to registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:latest
```

### Step 4 — Build Docker Image Locally
```bash
docker build -t ${input:projectName}:local .
```

### Step 5 — Verify Docker Image Runs
```bash
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="<your-connection-string>" ${input:projectName}:local
```

Open `http://localhost:8080/health` — should return `Healthy`.

## Output Expectations
- `Dockerfile` at solution root with 4 stages (build, test, publish, final)
- `.dockerignore` at solution root
- `.github/workflows/ci-cd.yml` with build, test, and push jobs
- `docker build` succeeds locally
- `http://localhost:8080/health` returns `Healthy`

## Quality Assurance

### Validation Checklist
- [ ] `docker build` succeeds with no errors
- [ ] Tests run inside Docker build (test stage)
- [ ] Final image uses `aspnet:8.0` runtime (not SDK)
- [ ] No secrets or connection strings in `Dockerfile`
- [ ] `.dockerignore` excludes `bin/`, `obj/`, `.git/`
- [ ] GitHub Actions pipeline triggers on `main` and `develop` branches
- [ ] Image push only on `main` branch

### Failure Triggers
- Stop if `docker build` fails — check project file paths in `COPY` commands
- Stop if health check fails in container — check environment variable for connection string
- Stop if tests fail in CI — fix before merging to main

### Next Step
Proceed to [19-data-migration-tools.prompt.md](./19-data-migration-tools.prompt.md) once Docker image builds and health check passes.
