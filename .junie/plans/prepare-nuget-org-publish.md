---
sessionId: session-261018-130718-h30n
---

# Requirements

### Overview & Goals
Prepare DesharpCore for publishing on nuget.org under owner **rsobrepena** (rsobrepena@gmail.com, https://askroland.com), and provide a step-by-step publishing guide.

### Scope
- In: update package metadata (`DesharpCore.nuspec`, `DesharpCore.csproj`), add a `PUBLISHING.md` guide.
- Out: renaming namespaces/assembly, actually pushing the package (requires the user's API key), editing `_CreateNewNuGetPackage` tooling.

### Functional Requirements
- Metadata: authors/owners `rsobrepena`, projectUrl `https://askroland.com`, copyright with owner name, repository/readme info.
- Package must build with `dotnet pack` and be pushable with `dotnet nuget push`.
- Guide covers: account creation, API key, versioning, pack, validate, push, verify, updating.

# Technical Design

### Current Implementation
- `DesharpCore.nuspec`: id `DesharpCore`, authors/owners `Tom Flidr`, projectUrl on github.com/rsobrepena/desharpcore, deprecated `iconUrl` (ico), license expression BSD-3-Clause, `$version$` placeholder, content files (config example + xdt transforms).
- `DesharpCore.csproj`: SDK-style net10.0, no package properties; `packages.config` references `CreateNewNuGetPackageFromProjectAfterEachBuild` 1.8.16 (legacy tooling in `_CreateNewNuGetPackage/`).
- `LICENSE.md`: BSD 3-Clause, Roland Sobrepena.
- `gfx/desharp-icon.png` exists (the nuspec wrongly points to `desharpcore-icon.ico`).

### Key Decisions
- Keep the nuspec as the source of metadata, but pack through `dotnet pack -p:NuspecFile=DesharpCore.nuspec -p:NuspecProperties=version=1.0.0`, or alternatively move metadata into csproj. Chosen: add metadata to csproj (`PackageId`, `Version`, `Authors`, `PackageProjectUrl`, `PackageIcon`, `PackageReadmeFile`, `PackageLicenseExpression`) and set `NuspecFile` so content/xdt files from the nuspec are retained.
- Replace deprecated `iconUrl` with embedded `<icon>` pointing to `gfx/desharp-icon.png`; embed `README.md`.

### Proposed Changes
- `DesharpCore.nuspec`: authors/owners -> `rsobrepena`; `projectUrl` -> `https://askroland.com`; remove `iconUrl`; add `<icon>desharp-icon.png</icon>`, `<readme>README.md</readme>`, `<repository type="git" url="https://github.com/rsobrepena/desharpcore"/>`; copyright `Copyright © 2026 rsobrepena`; add the icon and README to `<files>`; keep `$version$`.
- `DesharpCore.csproj`: add `Version` 1.0.0, `PackageId`, `Authors`, `Company`, `PackageProjectUrl`, `NuspecFile`, `NuspecProperties`.
- `PUBLISHING.md` (new): the step-by-step guide.

### Risks
- Both csproj metadata and `NuspecFile` can conflict; the nuspec wins when set. Mitigate by verifying with `dotnet pack` and inspecting the .nupkg.
- The id `DesharpCore` may already be taken on nuget.org; the guide tells how to check/choose a prefix.
- The GitHub URL in metadata is retained as repository; confirm it exists.

# Testing

### Validation Approach
- `dotnet build DesharpCore.csproj` succeeds.
- `dotnet pack DesharpCore.csproj -c Release` produces `DesharpCore.1.0.0.nupkg`; unzip and inspect the nuspec for correct authors, projectUrl, icon, readme, content files.
- No push is performed.

# Delivery Steps

### ✓ Step 1: Update package metadata for nuget.org
Package metadata reflects rsobrepena / rsobrepena@gmail.com / askroland.com and packs successfully.

- Edit `DesharpCore.nuspec`: authors, owners, projectUrl, copyright, icon, readme, repository; remove `iconUrl`; add icon/README to files.
- Edit `DesharpCore.csproj`: add Version, PackageId, Authors, PackageProjectUrl, NuspecFile/NuspecProperties.
- Run `dotnet pack -c Release` and inspect the resulting .nupkg.

### ✓ Step 2: Write the nuget.org publishing guide
A `PUBLISHING.md` explains the full publish flow and the README links to it.

- Steps: create nuget.org account (rsobrepena@gmail.com) and enable 2FA, check the package id, reserve an ID prefix (optional), create an API key scoped to `DesharpCore`.
- Steps: set version, `dotnet pack`, `dotnet nuget push ... --api-key <KEY> --source https://api.nuget.org/v3/index.json`, wait for validation/indexing, verify the package page.
- Cover releasing updates, unlisting and key safety (never commit the key).
- Add a short link in `README.md`; also summarize the guide in the final reply.