# Multi-Environment Deployment Guide

## Environments

| Env | Branch | Trigger | URL |
|-----|--------|---------|-----|
| UAT | main | push | (disabled - runner issue) |
| Staging | staging | push | ghcr.io/louisphamdev/it-platform-portal:staging-* |
| Production | release/v* | tag | https://it-platform.internal |

## CI/CD Pipeline

```
feature/* → PR → review → merge to main → CI Build → deploy staging
                                              ↓
                                    test staging
                                              ↓
                              create tag release/v* → production deploy
```

## Deploy Status

- ✅ CI Build: Pass on all commits
- ✅ Deploy Staging: Pass (needs portal Dockerfile fix in progress)
- ⚠️ Deploy UAT: Disabled (GitHub runner infrastructure issue)
- ⏳ Deploy Production: Ready (triggers on `release/v*` tags)

## Secrets Management

Never commit secrets. Use `.env` files:
- `.env` is in `.gitignore`
- `.env.example` is committed (no real values)
- Production secrets via CI/CD secrets or vault
