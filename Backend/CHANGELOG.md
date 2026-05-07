# Changelog

## 2026-05-07

- Refactored the native API Dockerfile to build `API/API.csproj`, publish the `API` native AOT executable, and listen on port `60000` in the runtime container.
- Added a Native Docker ignore file to keep generated, local, and editor files out of that Docker target.
- Aligned the native API Dockerfile with `DevopsTools/Dev/scripts/init.sh` so it builds from the API directory context and tags image metadata with `BACKEND_TAG`.
