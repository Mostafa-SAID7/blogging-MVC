# API Documentation

## Overview

The BloggingAgent API provides RESTful endpoints for AI-powered blog content generation, management, and analytics.

**Base URL:** `http://localhost:5000/api` (local development)  
**API Format:** JSON  
**Authentication:** None (configure in production)

## Blog Endpoints

### Generate Blog Post

Create a new AI-generated blog post.

```
POST /blog/generate
```

**Request:**
```json
{
  "topic": "The Future of AI",
  "keywords": "AI, machine learning, future",
  "targetWordCount": 1000,
  "tone": "professional",
  "targetAudience": "business professionals",
  "tags": ["AI", "Technology"],
  "includeImages": true
}
```

**Response (200):**
```json
{
  "id": 1,
  "title": "The Future of Artificial Intelligence",
  "slug": "the-future-of-artificial-intelligence",
  "content": "<p>Generated content...</p>",
  "excerpt": "An overview of AI...",
  "author": "AI Assistant",
  "createdAt": "2024-01-15T10:30:00Z",
  "isPublished": false,
  "tags": ["AI", "Technology"]
}
```

### List Blog Posts

Retrieve paginated list of posts with optional filtering.

```
GET /blog?page=1&searchQuery=ai&tag=Technology
```

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `searchQuery` (string, optional): Search term
- `tag` (string, optional): Filter by tag

**Response:**
```json
{
  "posts": [
    {
      "id": 1,
      "title": "Sample Post",
      "slug": "sample-post",
      "excerpt": "Post excerpt...",
      "createdAt": "2024-01-15T10:30:00Z",
      "isPublished": true,
      "tags": ["sample"]
    }
  ],
  "currentPage": 1,
  "totalPages": 5,
  "tagCounts": {
    "AI": 10,
    "Technology": 8
  }
}
```

### Get Blog Post

Retrieve a specific post with SEO analysis and related posts.

```
GET /blog/{slug}
```

**Response:**
```json
{
  "post": {
    "id": 1,
    "title": "Sample Post",
    "slug": "sample-post",
    "content": "<p>Full content...</p>",
    "excerpt": "Excerpt...",
    "createdAt": "2024-01-15T10:30:00Z",
    "isPublished": true,
    "tags": ["sample"]
  },
  "seoAnalysis": {
    "score": 85,
    "suggestions": ["Add more keywords"],
    "checks": {
      "hasTitle": true,
      "titleLength": true
    }
  },
  "relatedPosts": []
}
```

### Publish/Unpublish Post

Change publication status.

```
POST /blog/publish/{id}
POST /blog/unpublish/{id}
```

**Response:**
```json
{
  "success": true,
  "message": "Post published successfully"
}
```

## Analytics Endpoints

### Get Analytics Overview

Retrieve overall blog analytics.

```
GET /analytics
```

**Response:**
```json
{
  "totalViews": 1500,
  "totalPosts": 10,
  "averageReadTime": 4.2,
  "topTags": {
    "AI": 25,
    "Technology": 20
  },
  "postAnalytics": [
    {
      "id": 1,
      "blogPostId": 1,
      "views": 150,
      "uniqueViews": 120,
      "shares": 5,
      "bounceRate": 0.25,
      "trafficSources": {
        "Direct": 50,
        "Search": 40
      }
    }
  ]
}
```

### Get Post Analytics

Get metrics for a specific post.

```
GET /analytics/post/{id}
```

**Response:**
```json
{
  "id": 1,
  "views": 150,
  "uniqueViews": 120,
  "shares": 5,
  "comments": 3,
  "averageReadTime": 4.5,
  "bounceRate": 0.25,
  "trafficSources": {
    "Direct": 50,
    "Search": 40
  }
}
```

### Export Analytics

Export analytics data in JSON or CSV format.

```
GET /analytics/export?format=json
GET /analytics/export?format=csv
```

**Query Parameters:**
- `format` (string): "json" or "csv"

## Settings Endpoints

### Get Settings

Retrieve application settings.

```
GET /settings
```

**Response:**
```json
{
  "defaultAuthor": "AI Assistant",
  "maxPostLength": 5000,
  "defaultTags": ["blog"],
  "autoPublish": false,
  "theme": "default"
}
```

### Update Settings

Update application settings.

```
POST /settings/update
```

**Request:**
```json
{
  "defaultAuthor": "AI Assistant",
  "maxPostLength": 5000,
  "autoPublish": false
}
```

### Reset Settings

Reset to default configuration.

```
POST /settings/reset
```

## SEO Endpoints

### Analyze Content

Perform SEO analysis on content.

```
POST /seo/analyze
```

**Request:**
```json
{
  "content": "Your blog content...",
  "title": "Your Title"
}
```

**Response:**
```json
{
  "score": 85,
  "suggestions": [
    "Add more keywords to introduction"
  ],
  "checks": {
    "hasTitle": true,
    "titleLength": true,
    "contentLength": true,
    "hasHeadings": true
  },
  "keywordDensity": "1.5%"
}
```

### Generate Meta Description

Create an optimized meta description.

```
POST /seo/meta-description
```

**Request:**
```json
{
  "content": "Your blog content..."
}
```

**Response:**
```json
{
  "description": "Optimized meta description..."
}
```

### Get Keyword Suggestions

Generate keyword suggestions.

```
POST /seo/keywords
```

**Request:**
```json
{
  "content": "Your content...",
  "count": 5
}
```

**Response:**
```json
[
  "keyword 1",
  "keyword 2",
  "keyword 3",
  "keyword 4",
  "keyword 5"
]
```

## Error Handling

All endpoints return consistent error responses:

### 400 Bad Request
```json
{
  "error": {
    "message": "Validation failed",
    "details": "Topic is required",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

### 404 Not Found
```json
{
  "error": {
    "message": "Resource not found",
    "details": "Blog post not found",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

### 500 Server Error
```json
{
  "error": {
    "message": "Internal server error",
    "details": "AI service unavailable",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

## Rate Limiting

- General requests: 100/minute per IP
- Generation endpoints: 10/hour per IP
- Analytics requests: 50/minute per IP

## Testing with cURL

```bash
# List posts
curl http://localhost:5000/api/blog

# Generate post
curl -X POST http://localhost:5000/api/blog/generate \
  -H "Content-Type: application/json" \
  -d '{
    "topic": "AI in Healthcare",
    "keywords": "AI, healthcare",
    "targetWordCount": 500
  }'

# Get analytics
curl http://localhost:5000/api/analytics

# Analyze content
curl -X POST http://localhost:5000/api/seo/analyze \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Your blog post content...",
    "title": "Your Title"
  }'
```

## Interactive API Documentation

When the app is running, visit [http://localhost:5000/swagger](http://localhost:5000/swagger) for interactive Swagger UI.
