# SQL Server Setup Guide

Complete guide for configuring BloggingAgent with SQL Server.

## Connection String

Your Remote SQL Server Connection:
```
Server=db54433.public.databaseasp.net
Database=db54433
User Id=db54433
Password=h#7LNr-28=Xb
Encrypt=True
TrustServerCertificate=True
MultipleActiveResultSets=True
```

## Configuration

### 1. Update appsettings.json

Edit `bloggingAgent/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
  }
}
```

### 2. Update appsettings.Development.json

Edit `bloggingAgent/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
  }
}
```

### 3. Update Environment Variables

Create or update `.env` file:

```bash
DB_CONNECTION_STRING=Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;
```

### 4. Verify Program.cs Configuration

The `Program.cs` file already includes SQL Server configuration:

```csharp
// Configure Database (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelaySeconds: 30,
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(300); // 5 minutes timeout
        }
    ));
```

## Installation Steps

### 1. Install SQL Server Package

```bash
cd bloggingAgent
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
```

Or update via project file (already done in bloggingAgent.csproj).

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Create Migrations

```bash
# Create initial migration for SQL Server
dotnet ef migrations add InitialCreate

# If migrations already exist, create new migration
dotnet ef migrations add MigrateToSqlServer
```

### 4. Update Database

```bash
# Apply migrations to SQL Server
dotnet ef database update
```

### 5. Run Application

```bash
dotnet run
```

## Troubleshooting

### Connection Timeout

**Error:** "Timeout expired. The timeout period elapsed..."

**Solutions:**
1. Verify server is accessible from your network
2. Check firewall rules
3. Increase command timeout in Program.cs:
   ```csharp
   sqlOptions.CommandTimeout(600); // 10 minutes
   ```

### Authentication Failed

**Error:** "Login failed for user 'db54433'"

**Solutions:**
1. Verify credentials:
   - User: `db54433`
   - Password: `h#7LNr-28=Xb`
2. Check password contains special characters (already handled by connection string)
3. Verify database user has proper permissions

### Certificate Issues

**Error:** "The certificate chain was issued by an authority that is not trusted"

**Solutions:**
1. Current setting already trusts self-signed certificates:
   ```
   TrustServerCertificate=True
   ```
2. For production with valid certs, set to `False`

### Database Not Found

**Error:** "Cannot open database 'db54433'"

**Solutions:**
1. Verify database name in connection string
2. Check user has access to database
3. Ensure database exists on server
4. Contact database admin if needed

## Environment Variable Configuration

### Option 1: Direct Environment Variables

Set before running application:

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"

# Bash
export ConnectionStrings__DefaultConnection="Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"

# Windows CMD
set ConnectionStrings__DefaultConnection=Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;
```

### Option 2: .env File

Create `.env` file in project root:

```bash
ConnectionStrings__DefaultConnection=Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;
```

### Option 3: User Secrets (Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
```

## Database Initialization

When running for first time, the application will:

1. **Check connection** to SQL Server
2. **Create tables** if they don't exist
3. **Apply migrations** automatically
4. **Seed initial data** with sample blog posts

## Backup and Restore

### Backup Database

```sql
BACKUP DATABASE db54433 
TO DISK = 'D:\Backups\db54433_backup.bak'
WITH FORMAT;
```

### Restore Database

```sql
RESTORE DATABASE db54433 
FROM DISK = 'D:\Backups\db54433_backup.bak'
WITH REPLACE;
```

## Performance Optimization

### SQL Server Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True; Connection Lifetime=300; Pooling=true; Min Pool Size=5; Max Pool Size=100;"
  }
}
```

**Connection String Parameters:**
- `Connection Lifetime=300` - Recycle connections after 5 minutes
- `Pooling=true` - Enable connection pooling
- `Min Pool Size=5` - Minimum connections in pool
- `Max Pool Size=100` - Maximum connections in pool

### Entity Framework Configuration

```csharp
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
        sqlOptions.CommandTimeout(300);
        sqlOptions.MaxBatchSize(100);
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }
);
```

## Migration Between Databases

### From SQLite to SQL Server

1. **Create backup of SQLite data:**
   ```bash
   cp bloggingagent.db bloggingagent_backup.db
   ```

2. **Update connection string** to SQL Server

3. **Apply migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Migrate data** (if needed):
   - Export data from SQLite
   - Import to SQL Server
   - Verify data integrity

### From Previous SQL Server to New SQL Server

1. **Backup old database:**
   ```sql
   BACKUP DATABASE old_db TO DISK = 'backup.bak';
   ```

2. **Update connection string** to new server

3. **Restore or update database:**
   ```bash
   dotnet ef database update
   ```

## Security Considerations

### Connection String Security

**DO:**
- ✅ Use strong passwords
- ✅ Store in environment variables, not code
- ✅ Use encryption (Encrypt=True)
- ✅ Limit network access to SQL Server
- ✅ Use VPN for remote connections

**DON'T:**
- ❌ Commit connection strings to version control
- ❌ Use weak passwords
- ❌ Hardcode credentials in appsettings.json
- ❌ Use TrustServerCertificate=True in production with invalid certs
- ❌ Share credentials via email or chat

### Recommended Security Setup

```bash
# Use Azure Key Vault or similar
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"

# Or environment variable
export ConnectionStrings__DefaultConnection="your-connection-string"
```

## Monitoring and Logs

### Enable SQL Server Logging

```csharp
// In Program.cs
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
```

### Query Logs

Monitor SQL queries with:

```bash
# Enable query logging in appsettings.json
"Logging": {
  "LogLevel": {
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

## Support

- [Configuration Guide](./CONFIGURATION.md)
- [Getting Started](./GETTING_STARTED.md)
- [Deployment Guide](./DEPLOYMENT.md)
- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/sql-server/)

---

**Connection String Ready to Use:**
```
Server=db54433.public.databaseasp.net; Database=db54433; User Id=db54433; Password=h#7LNr-28=Xb; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;
```