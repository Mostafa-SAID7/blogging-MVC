---
name: BloggingAgent setup
description: Key decisions and fixes made to get the GitHub-imported ASP.NET Core 9 blogging platform running on Replit with SQLite.
---

## The rule
Use `EnsureCreatedAsync()` (not `MigrateAsync()`) for DB initialization — `dotnet-ef` CLI tool version 10.0.8 is installed but requires .NET 10; project targets .NET 9. If migrations are needed later, install `dotnet-ef 9.x` locally via a tool manifest.

**Why:** `dotnet ef` global tool installed by `dotnet tool install --global dotnet-ef` installs the latest (10.x) which won't run on .NET 9 SDK.

**How to apply:** Keep `EnsureCreatedAsync()` in Program.cs for now. If the schema changes, delete the `.db` file so it gets recreated fresh.

## DI registrations that were missing
- `IEmailService` → `SmtpEmailService` (add to ServiceCollectionExtensions)
- `ISocialMediaService` → `SocialMediaService` (add to ServiceCollectionExtensions)
- `EmailSettings` must be configured via `Configure<EmailSettings>` in Program.cs
- Middleware (`ErrorHandlingMiddleware`, `RequestLoggingMiddleware`) must NOT be registered as scoped services — they take `RequestDelegate` in constructor and are managed by `UseMiddleware<>()`

## EF Core model gotchas
- `Dictionary<string, object>` properties (AgentMemory.Metadata, AgentSettings.CustomSettings, etc.) need `HasConversion` to serialize as JSON TEXT in SQLite
- `ApplicationUser` non-nullable string properties (FirstName, LastName, Bio, AvatarUrl) must have default values (`= ""`) or SQLite NOT NULL constraint fails on seed

## _Layout.cshtml
The original was only 34 lines (just navbar fragment). Replaced with a full HTML layout including `@RenderBody()` and `@RenderSection("Scripts", required: false)`.

## Admin credentials (seeded)
- Email: admin@bloggingagent.com
- Password: Admin123!
