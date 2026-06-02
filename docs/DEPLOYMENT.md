# Deployment Guide

## Production Checklist

- [ ] Update `appsettings.Production.json`
- [ ] Set all required environment variables
- [ ] Verify API keys are configured
- [ ] Test with production database
- [ ] Configure HTTPS certificates
- [ ] Enable CORS for production domain
- [ ] Set up monitoring and logging
- [ ] Configure backups
- [ ] Performance test under load

## Local Production Build

```bash
# Build release version
dotnet publish -c Release -o ./publish

# Run published version
cd publish
./bloggingAgent
```

Set environment variable before running:
```bash
set ASPNETCORE_ENVIRONMENT=Production
# or
export ASPNETCORE_ENVIRONMENT=Production
```

## Docker Deployment

### Build Image

```bash
docker build -t blogging-agent:latest .
```

### Run Single Container

```bash
docker run -d \
  -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e OPENAI_API_KEY=sk-your-key \
  --name blogging-agent \
  blogging-agent:latest
```

### Run with Docker Compose

```bash
docker-compose up -d
docker-compose logs -f
docker-compose down
```

### Docker Compose Configuration

```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "5000:80"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      OPENAI_API_KEY: ${OPENAI_API_KEY}
    depends_on:
      - db
    volumes:
      - ./data:/app/data
      - ./logs:/app/logs

  db:
    image: sqlite:latest
    volumes:
      - ./data:/app/data
```

## Cloud Deployment

### Azure App Service

1. **Create App Service Plan**
   ```bash
   az appservice plan create \
     --name blogging-plan \
     --resource-group myResourceGroup \
     --sku B1
   ```

2. **Create Web App**
   ```bash
   az webapp create \
     --name blogging-agent \
     --plan blogging-plan \
     --resource-group myResourceGroup \
     --runtime "DOTNET|7.0"
   ```

3. **Deploy Code**
   ```bash
   # Using Git
   git remote add azure <azure-git-url>
   git push azure main
   
   # Or use zip deployment
   dotnet publish -c Release -o ./publish
   cd publish && zip -r app.zip . && cd ..
   az webapp deployment source config-zip \
     --resource-group myResourceGroup \
     --name blogging-agent \
     --src-path app.zip
   ```

4. **Configure Application Settings**
   ```bash
   az webapp config appsettings set \
     --name blogging-agent \
     --resource-group myResourceGroup \
     --settings \
     ASPNETCORE_ENVIRONMENT=Production \
     OPENAI_API_KEY=sk-your-key
   ```

### AWS Elastic Beanstalk

1. **Prepare Application**
   ```bash
   dotnet publish -c Release -o ./publish
   cd publish
   zip -r app.zip .
   cd ..
   ```

2. **Deploy**
   ```bash
   eb create blogging-agent-env
   eb deploy --staged
   ```

### Google Cloud Run

1. **Build Container**
   ```bash
   gcloud builds submit --tag gcr.io/PROJECT_ID/blogging-agent:latest
   ```

2. **Deploy to Cloud Run**
   ```bash
   gcloud run deploy blogging-agent \
     --image gcr.io/PROJECT_ID/blogging-agent:latest \
     --platform managed \
     --region us-central1 \
     --set-env-vars OPENAI_API_KEY=sk-your-key
   ```

## Linux/Ubuntu Server Deployment

### 1. Install .NET Runtime

```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 7.0
```

### 2. Create Application Directory

```bash
sudo mkdir -p /var/www/blogging-agent
sudo chown $USER:$USER /var/www/blogging-agent
```

### 3. Deploy Application

```bash
dotnet publish -c Release -o ./publish
cp -r publish/* /var/www/blogging-agent/
```

### 4. Create Systemd Service

Create `/etc/systemd/system/blogging-agent.service`:

```ini
[Unit]
Description=BloggingAgent
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/blogging-agent
ExecStart=/usr/bin/dotnet /var/www/blogging-agent/bloggingAgent.dll
Restart=always
RestartSec=10
SyslogIdentifier=blogging-agent
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://0.0.0.0:5000"
Environment="OPENAI_API_KEY=sk-your-key"

[Install]
WantedBy=multi-user.target
```

### 5. Start Service

```bash
sudo systemctl enable blogging-agent
sudo systemctl start blogging-agent
sudo systemctl status blogging-agent
```

## Nginx Reverse Proxy

### Install Nginx

```bash
sudo apt update
sudo apt install nginx
```

### Configure as Reverse Proxy

Create `/etc/nginx/sites-available/blogging-agent`:

```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    
    # Redirect HTTP to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;
    
    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    
    # Proxy to application
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_buffering off;
        proxy_request_buffering off;
    }
}
```

Enable site:
```bash
sudo ln -s /etc/nginx/sites-available/blogging-agent /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

## SSL/TLS Certificate

### Using Let's Encrypt

```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot certonly --nginx -d yourdomain.com -d www.yourdomain.com
```

Auto-renewal:
```bash
sudo systemctl enable certbot.timer
sudo systemctl start certbot.timer
```

## Database Configuration

### Production Database Setup

For production, consider using PostgreSQL:

```bash
# Install PostgreSQL
sudo apt install postgresql postgresql-contrib

# Create database and user
sudo -u postgres createdb blogging_agent
sudo -u postgres createuser blogging_agent_user
sudo -u postgres psql -c "ALTER USER blogging_agent_user WITH PASSWORD 'strong-password';"
```

Update connection string in `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blogging_agent;Username=blogging_agent_user;Password=strong-password"
  }
}
```

## Backup Strategy

### Database Backups

SQLite:
```bash
# Regular backup via cron
0 2 * * * cp /var/www/blogging-agent/bloggingagent.db /backups/bloggingagent_$(date +\%Y\%m\%d).db
```

PostgreSQL:
```bash
# Scheduled backup
0 2 * * * pg_dump blogging_agent > /backups/blogging_agent_$(date +\%Y\%m\%d).sql
```

### File Backups

```bash
# Backup application and data directories
0 3 * * * tar -czf /backups/blogging-agent_$(date +\%Y\%m\%d).tar.gz /var/www/blogging-agent
```

## Monitoring & Logging

### Application Logging

Configure in `appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  }
}
```

Logs to:
- Console (systemd journal)
- File (if configured)

### Health Checks

Configure health check endpoint:

```bash
curl http://localhost:5000/health
```

### Performance Monitoring

Use Cloudflare, New Relic, or Datadog for APM.

## Scaling Considerations

### Horizontal Scaling

For multiple instances:
1. Load balance with Nginx/HAProxy
2. Use distributed cache (Redis)
3. Use centralized database (PostgreSQL)

### Vertical Scaling

- Increase server resources (CPU, RAM)
- Optimize database queries
- Enable caching
- Use production build optimization

## Security Hardening

### Environment Variables

Never commit secrets:
```bash
# Use secure secret management
export OPENAI_API_KEY=$(aws secretsmanager get-secret-value --secret-id openai-key --query SecretString --output text)
```

### Firewall

```bash
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
```

### Fail2Ban Protection

```bash
sudo apt install fail2ban

# Configure for API rate limiting
sudo systemctl start fail2ban
```

## Troubleshooting Deployment

### Application Won't Start

Check logs:
```bash
sudo journalctl -u blogging-agent -n 50
```

### Database Connection Issues

Verify connection string and database is accessible:
```bash
dotnet bloggingAgent.dll --connection-test
```

### Port Already in Use

```bash
# Find process using port
sudo lsof -i :5000

# Kill process
sudo kill -9 <PID>
```

### Performance Issues

- Check database queries
- Monitor CPU/RAM usage
- Enable caching
- Consider database optimization

## Next Steps

- [Configuration Guide](./CONFIGURATION.md)
- [API Documentation](./API.md)
- Set up monitoring and alerting
- Plan backup and disaster recovery
