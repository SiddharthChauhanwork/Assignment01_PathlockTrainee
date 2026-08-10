
# IdentityHub

IdentityHub is a .NET web API solution for authentication and token management. The solution is split into several projects: `IdentityHub.API`, `IdentityHub.Application`, `IdentityHub.Infrastructure`, `IdentityHub.Shared`, and `IdentityHub.Tests`.

Team
- Siddharth Chauhan
- Puneet Khoiya
- Tarannum
- Anshdeep Singh
- Akshad Gupta
- Mohit Gupta

What this project does
- Provides an HTTP API for authentication, token issuance, and token revocation.
- Implements access tokens and refresh tokens with secure hashing and storage.
- Manages application clients and users with EF Core-backed persistence.
- Exposes services for token generation, validation, and refresh flows used by client applications.
- Includes database migrations, configuration for persistence, and unit/integration tests.

**Prerequisites**
- .NET SDK 10.0 or later (TargetFramework: `net10.0`)
- Optional: `dotnet-ef` tools for migrations (`dotnet tool install --global dotnet-ef`)

**Quick start**
1. Restore dependencies:
```powershell
dotnet restore
```
2. Build the solution:
```powershell
dotnet build
```
3. Run the API (from solution root):
```powershell
dotnet run --project IdentityHub.API
```

**Database / Migrations**
- Configure your connection string in `IdentityHub.API/appsettings.json` or via environment variables.
- To apply migrations using EF Core tools:
```powershell
dotnet ef database update --project IdentityHub.Infrastructure --startup-project IdentityHub.API
```

**Tests**
```powershell
dotnet test IdentityHub.Tests
```

**Git / Push**
- To push to a remote GitHub repository (HTTPS):
```powershell
git remote add origin https://github.com/<your-username>/<repo>.git
git branch -M main
git push -u origin main
```
- If you receive authentication errors, see GitHub auth options: use `gh auth login`, clear stored credentials in Windows Credential Manager, or switch to SSH keys.


