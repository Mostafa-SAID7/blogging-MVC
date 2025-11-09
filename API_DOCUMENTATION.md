# BloggingAgent API Documentation

## Overview

The BloggingAgent API provides RESTful endpoints for AI-powered blog content generation, management, and analytics. All endpoints return JSON responses and use standard HTTP status codes.

## Base URL
```
https://your-domain.com/api
```

## Authentication
Currently, the API does not require authentication. In production, consider implementing JWT or API key authentication.

## Content Types
- Request: `application/json`
- Response: `application/json`

---

## Blog Endpoints

### Generate Blog Post
Generate a new blog post using AI.

**Endpoint:** `POST /blog/generate`

**Request Body:**
```json
{
  "topic": "The Future of Artificial Intelligence",
  "keywords": "AI, machine learning, future technology",
  "targetWordCount": 1000,
  "tone": "professional",
  "targetAudience": "business professionals",
  "tags": ["AI", "Technology", "Future"],
  "includeImages": true
}
```

**Response:**
```json
{
  "id": 1,
  "title": "The Future of Artificial Intelligence",
  "slug": "the-future-of-artificial-intelligence",
  "content": "<p>Generated HTML content...</p>",
  "excerpt": "An overview of AI advancements...",
  "author": "AI Assistant",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z",
  "isPublished": false,
  "tags": ["AI", "Technology", "Future"]
}
```

**Status Codes:**
- `200` - Success
- `400` - Bad Request (validation error)
- `500` - Internal Server Error

### Get Blog Posts
Retrieve a paginated list of blog posts.

**Endpoint:** `GET /blog`

**Query Parameters:**
- `page` (integer, optional): Page number (default: 1)
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
      "author": "AI Assistant",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z",
      "isPublished": true,
      "tags": ["sample", "test"]
    }
  ],
  "currentPage": 1,
  "totalPages": 5,
  "searchQuery": "",
  "selectedTags": [],
  "tagCounts": {
    "AI": 10,
    "Technology": 8,
    "Future": 5
  }
}
```

### Get Blog Post by Slug
Retrieve a specific blog post by its slug.

**Endpoint:** `GET /blog/{slug}`

**Response:**
```json
{
  "post": {
    "id": 1,
    "title": "Sample Post",
    "slug": "sample-post",
    "content": "<p>Full post content...</p>",
    "excerpt": "Post excerpt...",
    "author": "AI Assistant",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z",
    "isPublished": true,
    "tags": ["sample", "test"]
  },
  "seoAnalysis": {
    "score": 85,
    "suggestions": ["Add more keywords", "Improve title length"],
    "checks": {
      "hasTitle": true,
      "titleLength": true,
      "contentLength": true
    }
  },
  "relatedPosts": [
    {
      "id": 2,
      "title": "Related Post",
      "slug": "related-post",
      "excerpt": "Related content...",
      "createdAt": "2024-01-14T09:00:00Z"
    }
  ],
  "canEdit": true
}
```

### Publish/Unpublish Post
Change the publication status of a blog post.

**Endpoint:** `POST /blog/publish/{id}` or `POST /blog/unpublish/{id}`

**Response:**
```json
{
  "success": true,
  "message": "Post published successfully"
}
```

---

## Analytics Endpoints

### Get Analytics Overview
Retrieve overall blog analytics.

**Endpoint:** `GET /analytics`

**Response:**
```json
{
  "postAnalytics": [
    {
      "id": 1,
      "blogPostId": 1,
      "views": 150,
      "uniqueViews": 120,
      "shares": 5,
      "comments": 3,
      "averageReadTime": 4.5,
      "bounceRate": 0.25,
      "lastUpdated": "2024-01-15T10:30:00Z",
      "trafficSources": {
        "Direct": 50,
        "Search": 40,
        "Social": 30
      }
    }
  ],
  "totalViews": 1500,
  "totalPosts": 10,
  "averageReadTime": 4.2,
  "topTags": {
    "AI": 25,
    "Technology": 20,
    "Future": 15
  },
  "trafficSources": [
    {
      "key": "Direct",
      "value": 500
    }
  ],
  "performanceMetrics": {
    "AverageViews": 150.0,
    "AverageBounceRate": 0.25,
    "AverageReadTime": 4.5,
    "TotalShares": 50,
    "TotalComments": 30
  }
}
```

### Get Post Analytics
Get detailed analytics for a specific post.

**Endpoint:** `GET /analytics/post/{id}`

**Response:**
```json
{
  "id": 1,
  "blogPostId": 1,
  "views": 150,
  "uniqueViews": 120,
  "shares": 5,
  "comments": 3,
  "averageReadTime": 4.5,
  "bounceRate": 0.25,
  "trafficSources": {
    "Direct": 50,
    "Search": 40,
    "Social": 30
  }
}
```

### Export Analytics Data
Export analytics data in various formats.

**Endpoint:** `GET /analytics/export`

**Query Parameters:**
- `format` (string): Export format - "json" or "csv"

**Response:** File download

---

## Settings Endpoints

### Get Settings
Retrieve current application settings.

**Endpoint:** `GET /settings`

**Response:**
```json
{
  "settings": {
    "defaultAuthor": "AI Assistant",
    "maxPostLength": 5000,
    "defaultTags": ["blog", "ai-generated"],
    "autoPublish": false,
    "theme": "default",
    "customSettings": {}
  },
  "saveSuccess": false,
  "errorMessage": null
}
```

### Update Settings
Update application settings.

**Endpoint:** `POST /settings/update`

**Request Body:**
```json
{
  "settings": {
    "defaultAuthor": "AI Assistant",
    "maxPostLength": 5000,
    "defaultTags": ["blog", "ai-generated"],
    "autoPublish": false,
    "theme": "default",
    "customSettings": {}
  }
}
```

### Reset Settings
Reset settings to defaults.

**Endpoint:** `POST /settings/reset`

### Export Settings
Export current settings as JSON file.

**Endpoint:** `GET /settings/export`

**Response:** JSON file download

### Import Settings
Import settings from JSON file.

**Endpoint:** `POST /settings/import`

**Request:** Multipart form data with settings file

---

## SEO Endpoints

### Analyze Content SEO
Analyze content for SEO optimization.

**Endpoint:** `POST /seo/analyze`

**Request Body:**
```json
{
  "content": "Your blog post content here...",
  "title": "Your Blog Post Title"
}
```

**Response:**
```json
{
  "score": 85,
  "suggestions": [
    "Add more keywords to the introduction",
    "Improve title length for better SEO"
  ],
  "checks": {
    "hasTitle": true,
    "titleLength": true,
    "contentLength": true,
    "hasHeadings": true,
    "hasImages": false,
    "hasLinks": true
  },
  "keywordDensity": "1.5%",
  "missingElements": ["meta description", "alt tags"],
  "keywordOccurrences": {
    "AI": 5,
    "technology": 3,
    "future": 2
  }
}
```

### Generate Meta Description
Generate a meta description for content.

**Endpoint:** `POST /seo/meta-description`

**Request Body:**
```json
{
  "content": "Your blog post content..."
}
```

**Response:**
```json
{
  "description": "Discover the latest advancements in AI technology and what the future holds for artificial intelligence in business and society."
}
```

### Suggest Keywords
Get keyword suggestions for content.

**Endpoint:** `POST /seo/keywords`

**Request Body:**
```json
{
  "content": "Your blog post content...",
  "count": 5
}
```

**Response:**
```json
["artificial intelligence", "machine learning", "AI technology", "future of AI", "AI applications"]
```

---

## Error Responses

All endpoints may return the following error responses:

### 400 Bad Request
```json
{
  "error": {
    "message": "Validation failed",
    "details": "Topic is required and must be at least 3 characters long",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

### 404 Not Found
```json
{
  "error": {
    "message": "Resource not found",
    "details": "Blog post with slug 'non-existent-post' was not found",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

### 500 Internal Server Error
```json
{
  "error": {
    "message": "An internal server error occurred",
    "details": "AI service temporarily unavailable",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

---

## Rate Limiting

- API requests are limited to 100 requests per minute per IP address
- Generation endpoints are limited to 10 requests per hour per IP address
- Analytics endpoints are limited to 50 requests per minute per IP address

---

## Webhooks (Future Feature)

The API supports webhooks for real-time notifications:

- Post published/unpublished
- Analytics updates
- Generation completed

Webhook configuration will be available in settings.

---

## SDKs and Libraries

### JavaScript SDK
```javascript
const client = new BloggingAgentClient('https://your-domain.com');

// Generate a post
const post = await client.generatePost({
  topic: 'AI in Healthcare',
  keywords: 'AI, healthcare, medical technology',
  targetWordCount: 800
});

// Get analytics
const analytics = await client.getAnalytics();
```

### .NET SDK
```csharp
var client = new BloggingAgentClient("https://your-domain.com");

// Generate content
var request = new GeneratePostRequest
{
    Topic = "Machine Learning Basics",
    TargetWordCount = 1000
};

var post = await client.GeneratePostAsync(request);
```

---

## Changelog

### Version 1.0.0
- Initial release with core blogging functionality
- AI-powered content generation
- SEO analysis and optimization
- Basic analytics tracking
- RESTful API with Swagger documentation

---

For more information or support, please visit our [documentation site](https://docs.bloggingagent.com) or contact our support team.