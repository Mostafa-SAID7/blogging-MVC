# Examples and Use Cases

This document provides practical examples and common use cases for BloggingAgent.

## Table of Contents

- [API Usage Examples](#api-usage-examples)
- [Configuration Examples](#configuration-examples)
- [Use Case Scenarios](#use-case-scenarios)
- [Integration Examples](#integration-examples)
- [Automation Examples](#automation-examples)

---

## API Usage Examples

### Blog Post Generation

#### Basic Post Generation
```bash
curl -X POST http://localhost:5000/api/blog/generate \
  -H "Content-Type: application/json" \
  -d '{
    "topic": "Introduction to Machine Learning",
    "targetWordCount": 800,
    "tone": "educational",
    "targetAudience": "beginners"
  }'
```

#### Advanced Post Generation with SEO
```bash
curl -X POST http://localhost:5000/api/blog/generate \
  -H "Content-Type: application/json" \
  -d '{
    "topic": "Best Practices for React Development",
    "keywords": "React, JavaScript, best practices, performance, hooks",
    "targetWordCount": 1200,
    "tone": "professional",
    "targetAudience": "developers",
    "tags": ["React", "JavaScript", "Web Development"],
    "includeImages": true
  }'
```

#### Bulk Generation
```bash
# Generate multiple posts
for topic in "AI Ethics" "Machine Learning Basics" "Future of Technology"; do
  curl -X POST http://localhost:5000/api/blog/generate \
    -H "Content-Type: application/json" \
    -d "{
      \"topic\": \"$topic\",
      \"targetWordCount\": 600,
      \"tone\": \"informative\"
    }"
  sleep 2
done
```

### Content Management

#### List Posts with Filtering
```bash
# Get published posts about AI
curl "http://localhost:5000/api/blog?searchQuery=AI&published=true"

# Get posts by tag
curl "http://localhost:5000/api/blog?tag=Technology&page=2"
```

#### Get Post Details
```bash
# Get post by slug with analytics
curl "http://localhost:5000/api/blog/introduction-to-machine-learning"
```

#### Publish/Unpublish Posts
```bash
# Publish a post
curl -X POST http://localhost:5000/api/blog/publish/1

# Unpublish a post
curl -X POST http://localhost:5000/api/blog/unpublish/1
```

### SEO Analysis

#### Analyze Existing Content
```bash
curl -X POST http://localhost:5000/api/seo/analyze \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Your blog post content here...",
    "title": "Your Blog Post Title"
  }'
```

#### Generate Meta Description
```bash
curl -X POST http://localhost:5000/api/seo/meta-description \
  -H "Content-Type: application/json" \
  -d '{
    "content": "This is a comprehensive guide to understanding machine learning fundamentals..."
  }'
```

#### Get Keyword Suggestions
```bash
curl -X POST http://localhost:5000/api/seo/keywords \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Machine learning is transforming industries...",
    "count": 10
  }'
```

### Analytics

#### Get Overall Analytics
```bash
curl "http://localhost:5000/api/analytics"
```

#### Get Post-Specific Analytics
```bash
curl "http://localhost:5000/api/analytics/post/1"
```

#### Export Analytics Data
```bash
# Export as JSON
curl "http://localhost:5000/api/analytics/export?format=json" > analytics.json

# Export as CSV
curl "http://localhost:5000/api/analytics/export?format=csv" > analytics.csv
```

---

## Configuration Examples

### OpenAI Configuration

#### Basic Setup
```json
{
  "OpenAISettings": {
    "ApiKey": "sk-your-key-here",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7,
    "MaxTokens": 1000
  }
}
```

#### Advanced Configuration
```json
{
  "OpenAISettings": {
    "ApiKey": "sk-your-key-here",
    "Model": "gpt-4",
    "Temperature": 0.8,
    "MaxTokens": 2000,
    "FrequencyPenalty": 0.1,
    "PresencePenalty": 0.1,
    "TopP": 0.9
  }
}
```

### Ollama Configuration

#### Local Setup
```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2",
    "Temperature": 0.7,
    "MaxTokens": 1000
  }
}
```

#### Remote Ollama Server
```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://your-ollama-server:11434",
    "OllamaModel": "neural-chat",
    "Temperature": 0.6,
    "MaxTokens": 1500,
    "Timeout": 30
  }
}
```

### Production Configuration

#### Complete Production Setup
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blogging_agent;Username=blog_user;Password=secure_password"
  },
  "OpenAISettings": {
    "ApiKey": "sk-production-key",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7
  },
  "SeoSettings": {
    "MinTitleLength": 30,
    "MaxTitleLength": 60,
    "AutoGenerateMetaDescription": true,
    "AutoOptimizeContent": true
  },
  "CacheSettings": {
    "ExpirationMinutes": 60,
    "EnableDistributedCache": true,
    "RedisConnectionString": "localhost:6379"
  }
}
```

---

## Use Case Scenarios

### Content Marketing Agency

**Scenario**: A marketing agency needs to generate blog content for multiple clients.

**Setup**:
```json
{
  "ContentSettings": {
    "DefaultAuthor": "Marketing Team",
    "DefaultTags": ["marketing", "business"],
    "AutoPublish": false,
    "MaxPostLength": 2000
  }
}
```

**Workflow**:
1. Generate posts for different clients
2. Review and edit in draft mode
3. Customize for each client's brand
4. Publish when approved

**Example API Calls**:
```bash
# Generate for tech client
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{"topic": "Digital Transformation", "tone": "professional", "targetAudience": "executives"}'

# Generate for health client  
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{"topic": "Wellness Tips", "tone": "friendly", "targetAudience": "general public"}'
```

### Educational Institution

**Scenario**: A university wants to create educational content for students.

**Configuration**:
```json
{
  "ContentSettings": {
    "DefaultAuthor": "Faculty",
    "DefaultTags": ["education", "learning"],
    "AutoPublish": true
  },
  "SeoSettings": {
    "MinTitleLength": 40,
    "MaxTitleLength": 70
  }
}
```

**Example Content Generation**:
```bash
# Computer Science course content
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{
    "topic": "Introduction to Algorithms",
    "keywords": "algorithms, computer science, data structures",
    "targetWordCount": 1500,
    "tone": "educational",
    "targetAudience": "students",
    "tags": ["Computer Science", "Algorithms", "Education"]
  }'
```

### E-commerce Business

**Scenario**: An online store needs product-related blog content for SEO.

**SEO-Focused Configuration**:
```json
{
  "SeoSettings": {
    "MinTitleLength": 50,
    "MaxTitleLength": 60,
    "AutoGenerateMetaDescription": true,
    "AutoOptimizeContent": true,
    "MinReadingTimeMinutes": 3,
    "MaxReadingTimeMinutes": 8
  }
}
```

**Product Content Examples**:
```bash
# Product guide
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{
    "topic": "Ultimate Guide to Choosing Running Shoes",
    "keywords": "running shoes, athletic wear, fitness, sports",
    "targetWordCount": 1000,
    "tone": "helpful",
    "targetAudience": "fitness enthusiasts"
  }'

# Comparison post
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{
    "topic": "Wireless vs Wired Headphones: Which is Better?",
    "keywords": "headphones, audio, wireless, wired, comparison",
    "targetWordCount": 800,
    "tone": "analytical"
  }'
```

### Tech Startup

**Scenario**: A startup needs technical blog content for developer outreach.

**Developer-Focused Setup**:
```json
{
  "ContentSettings": {
    "DefaultAuthor": "Dev Team",
    "DefaultTags": ["tech", "development"],
    "MaxPostLength": 3000
  }
}
```

**Technical Content Examples**:
```bash
# Tutorial content
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{
    "topic": "Building REST APIs with .NET Core",
    "keywords": ".NET Core, REST API, C#, web development",
    "targetWordCount": 2000,
    "tone": "technical",
    "targetAudience": "developers"
  }'

# Best practices
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{
    "topic": "Database Design Best Practices",
    "keywords": "database, design patterns, SQL, architecture",
    "targetWordCount": 1500,
    "tone": "authoritative"
  }'
```

---

## Integration Examples

### WordPress Integration

**Scenario**: Sync generated posts to WordPress.

**Script Example**:
```bash
#!/bin/bash

# Generate post via BloggingAgent
POST_DATA=$(curl -s -X POST http://localhost:5000/api/blog/generate \
  -H "Content-Type: application/json" \
  -d '{"topic": "'"$1"'", "targetWordCount": 800}')

# Extract data
TITLE=$(echo $POST_DATA | jq -r '.title')
CONTENT=$(echo $POST_DATA | jq -r '.content')

# Post to WordPress via WP-CLI
wp post create --post_title="$TITLE" --post_content="$CONTENT" --post_status=draft
```

### Social Media Automation

**LinkedIn Post Generation**:
```python
import requests
import json

# Generate blog post
response = requests.post('http://localhost:5000/api/blog/generate', 
    json={
        'topic': 'Industry Trends 2024',
        'targetWordCount': 300,
        'tone': 'professional'
    })

post_data = response.json()

# Create LinkedIn-style summary
summary = post_data['excerpt'][:200] + "..."
link = f"https://yourblog.com/posts/{post_data['slug']}"

linkedin_post = f"{summary}\n\nRead more: {link}"
```

### Newsletter Integration

**Mailchimp Campaign**:
```python
import requests
from mailchimp3 import MailChimp

# Generate newsletter content
posts_response = requests.get('http://localhost:5000/api/blog?limit=3')
recent_posts = posts_response.json()['posts']

# Format for newsletter
newsletter_content = ""
for post in recent_posts:
    newsletter_content += f"""
    <h2>{post['title']}</h2>
    <p>{post['excerpt']}</p>
    <a href="https://yourblog.com/posts/{post['slug']}">Read More</a>
    """

# Send via Mailchimp
client = MailChimp(mc_api='your-api-key', mc_user='your-username')
client.campaigns.actions.send(campaign_id='campaign-id')
```

---

## Automation Examples

### Scheduled Content Generation

**Cron Job for Daily Posts**:
```bash
# crontab -e
# Generate daily post at 9 AM
0 9 * * * /path/to/generate-daily-post.sh

# generate-daily-post.sh
#!/bin/bash
TOPICS=("Tech News" "Industry Updates" "Tips and Tricks" "Case Studies")
TOPIC=${TOPICS[$RANDOM % ${#TOPICS[@]}]}

curl -X POST http://localhost:5000/api/blog/generate \
  -H "Content-Type: application/json" \
  -d "{
    \"topic\": \"Daily Insight: $TOPIC\",
    \"targetWordCount\": 500,
    \"tone\": \"informative\"
  }"
```

### GitHub Actions Workflow

**Automated Content Pipeline**:
```yaml
name: Generate Weekly Content

on:
  schedule:
    - cron: '0 10 * * 1'  # Every Monday at 10 AM
  workflow_dispatch:

jobs:
  generate-content:
    runs-on: ubuntu-latest
    steps:
      - name: Generate Blog Post
        run: |
          curl -X POST ${{ secrets.BLOG_API_URL }}/api/blog/generate \
            -H "Content-Type: application/json" \
            -d '{
              "topic": "Weekly Tech Roundup",
              "targetWordCount": 1000,
              "tone": "informative",
              "tags": ["weekly", "tech", "roundup"]
            }'
```

### Slack Bot Integration

**Content Generation Bot**:
```python
from slack_bolt import App
import requests

app = App(token="your-slack-token")

@app.command("/generate-post")
def generate_post(ack, respond, command):
    ack()
    
    topic = command['text']
    
    # Generate via BloggingAgent
    response = requests.post('http://localhost:5000/api/blog/generate',
        json={'topic': topic, 'targetWordCount': 600})
    
    if response.status_code == 200:
        post = response.json()
        respond(f"✅ Generated: {post['title']}\nPreview: {post['excerpt'][:100]}...")
    else:
        respond("❌ Failed to generate post")
```

### Analytics Dashboard

**Custom Dashboard with Real-time Data**:
```javascript
// Fetch analytics every 5 minutes
setInterval(async () => {
    const response = await fetch('/api/analytics');
    const data = await response.json();
    
    // Update dashboard charts
    updateViewsChart(data.totalViews);
    updateTopPosts(data.postAnalytics);
    updateTrafficSources(data.trafficSources);
}, 300000);

function updateViewsChart(totalViews) {
    // Chart.js or similar library
    viewsChart.data.datasets[0].data.push(totalViews);
    viewsChart.update();
}
```

---

## Advanced Examples

### Multi-Language Content

**Generate Content in Different Languages**:
```bash
# English version
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{"topic": "Artificial Intelligence Trends", "targetWordCount": 800}'

# Spanish version (if AI model supports it)
curl -X POST http://localhost:5000/api/blog/generate \
  -d '{"topic": "Tendencias de Inteligencia Artificial", "targetWordCount": 800, "language": "es"}'
```

### A/B Testing Content

**Generate Multiple Versions**:
```python
import requests

topic = "Productivity Tips for Remote Work"

# Version A: Formal tone
version_a = requests.post('http://localhost:5000/api/blog/generate',
    json={'topic': topic, 'tone': 'professional', 'targetAudience': 'business'})

# Version B: Casual tone  
version_b = requests.post('http://localhost:5000/api/blog/generate',
    json={'topic': topic, 'tone': 'friendly', 'targetAudience': 'general'})

# Compare performance after publishing both
```

### Content Optimization Loop

**Iterative Improvement**:
```python
import requests
import time

def optimize_content(post_id, target_score=80):
    while True:
        # Get current SEO analysis
        analysis = requests.post('http://localhost:5000/api/seo/analyze',
            json={'content': get_post_content(post_id)})
        
        current_score = analysis.json()['score']
        
        if current_score >= target_score:
            print(f"✅ Optimization complete! Score: {current_score}")
            break
            
        # Get suggestions and regenerate
        suggestions = analysis.json()['suggestions']
        print(f"Current score: {current_score}, applying suggestions...")
        
        # Apply AI-powered improvements
        improved_content = requests.post('http://localhost:5000/api/blog/optimize',
            json={'content': get_post_content(post_id), 'suggestions': suggestions})
        
        time.sleep(2)  # Rate limiting
```

---

## Performance Examples

### Batch Operations

**Efficient Bulk Generation**:
```python
import asyncio
import aiohttp

async def generate_post(session, topic):
    async with session.post('http://localhost:5000/api/blog/generate',
                          json={'topic': topic, 'targetWordCount': 600}) as response:
        return await response.json()

async def bulk_generate(topics):
    async with aiohttp.ClientSession() as session:
        tasks = [generate_post(session, topic) for topic in topics]
        results = await asyncio.gather(*tasks)
        return results

# Generate 10 posts concurrently
topics = [f"Topic {i}" for i in range(1, 11)]
posts = asyncio.run(bulk_generate(topics))
```

### Caching Strategy

**Smart Content Caching**:
```python
import requests
import redis
import json
import hashlib

redis_client = redis.Redis(host='localhost', port=6379, db=0)

def generate_with_cache(topic, **kwargs):
    # Create cache key from request parameters
    cache_key = hashlib.md5(
        json.dumps({'topic': topic, **kwargs}, sort_keys=True).encode()
    ).hexdigest()
    
    # Check cache first
    cached = redis_client.get(cache_key)
    if cached:
        return json.loads(cached)
    
    # Generate new content
    response = requests.post('http://localhost:5000/api/blog/generate',
                           json={'topic': topic, **kwargs})
    
    result = response.json()
    
    # Cache for 1 hour
    redis_client.setex(cache_key, 3600, json.dumps(result))
    
    return result
```

---

## Next Steps

- [Getting Started Guide](./GETTING_STARTED.md)
- [API Documentation](./API.md)
- [Configuration Guide](./CONFIGURATION.md)
- [Deployment Guide](./DEPLOYMENT.md)

For more examples and community contributions, visit our [GitHub Discussions](https://github.com/Mostafa-SAID7/bloggingAgent/discussions).