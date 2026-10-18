# Publishing DesharpCore to nuget.org

Owner: **rsobrepena** (rsobrepena@gmail.com) - https://askroland.com

## 1. Create the nuget.org account
1. Go to https://www.nuget.org and click **Sign in** -> sign in with a Microsoft account (use rsobrepena@gmail.com).
2. Set your username to `rsobrepena` in **Account settings**.
3. Confirm your email address and enable two-factor authentication (Microsoft account security settings).

## 2. Check the package id
Open https://www.nuget.org/packages/DesharpCore. If it exists and isn't yours, pick another id, e.g. `rsobrepena.DesharpCore`, and change `<id>` in `DesharpCore.nuspec` and `<PackageId>` in `DesharpCore.csproj`.

Optional: reserve an ID prefix (e.g. `DesharpCore.`) at https://www.nuget.org/account/reserved (request via https://www.nuget.org/policies/Contact).

## 3. Create an API key
1. https://www.nuget.org/account/apikeys -> **Create**.
2. Key name: `DesharpCore publish`; scope: **Push new packages and package versions**.
3. Glob pattern: `DesharpCore*`; set an expiry.
4. Copy the key immediately (it is shown once). **Never commit it** to git; keep it in an environment variable or password manager.

## 4. Set the version
Edit `<Version>` in `DesharpCore.csproj` (Semantic Versioning, e.g. `1.0.0`; pre-releases like `1.1.0-beta1`). Versions can't be reused once pushed.

## 5. Pack
```
dotnet pack DesharpCore.csproj -c Release
```
Output: `bin/Release/DesharpCore.<version>.nupkg`. The metadata comes from `DesharpCore.nuspec`; the version is injected from the csproj.

## 6. Validate locally
- Inspect contents: `unzip -l bin/Release/DesharpCore.1.0.0.nupkg` (expect `lib/net10.0/DesharpCore.dll`, `content/*`, `README.md`, `desharp-icon.png`).
- Optionally test with a local feed: `dotnet nuget add source $(pwd)/bin/Release --name local` and install from a test project.

## 7. Push
```
dotnet nuget push bin/Release/DesharpCore.1.0.0.nupkg --api-key <YOUR_KEY> --source https://api.nuget.org/v3/index.json
```

## 8. Wait for validation and verify
The package is scanned (usually a few minutes, indexing can take up to an hour). Check https://www.nuget.org/packages/DesharpCore and the email sent to rsobrepena@gmail.com. Verify the icon, README, license, project URL (https://askroland.com) and authors on the page, then test with `dotnet add package DesharpCore`.

## 9. Releasing updates
Bump `<Version>`, run `dotnet pack -c Release`, then push the new `.nupkg` as in step 7. Add `--skip-duplicate` to make re-runs harmless.

## 10. Unlisting / mistakes
Published versions can't be deleted. On the package's **Manage** page use **Listed** -> uncheck to unlist a bad version, and publish a fixed version. Deprecate old versions from the same page.

## Key safety
- Never commit the API key; revoke it at https://www.nuget.org/account/apikeys if leaked.
- Use a scoped key with an expiry and only the `DesharpCore*` glob.
