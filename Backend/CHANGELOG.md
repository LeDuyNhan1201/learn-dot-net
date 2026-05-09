# Changelog

## 2026-05-09

- Kept `appsettings.Development.json` in the native backend image and set the compose runtime environment to Development for both ASP.NET Core and .NET host configuration.
- Fixed the native backend image build to use the Backend directory as Docker context so API, Application, Domain, and Infrastructure projects are available during restore and publish.
- Added a Native Dockerfile-specific ignore file so `build-image.sh` excludes generated, local, and secret development files from the Backend build context.

## 2026-05-07

- Refactored the native API Dockerfile to build `API/API.csproj`, publish the `API` native AOT executable, and listen on port `60000` in the runtime container.
- Added a Native Docker ignore file to keep generated, local, and editor files out of that Docker target.
- Aligned the native API Dockerfile with `DevopsTools/Dev/scripts/init.sh` so it builds from the API directory context and tags image metadata with `BACKEND_TAG`.
