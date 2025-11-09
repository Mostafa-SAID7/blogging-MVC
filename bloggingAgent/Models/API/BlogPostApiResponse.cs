using System;
using System.Collections.Generic;
using BloggingAgent.Models.Enums;

namespace BloggingAgent.Models.API
{
    public class BlogPostApiResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PostStatus Status { get; set; }
        public BlogCategory Category { get; set; }
        public ContentTone Tone { get; set; }
        public List<string> Tags { get; set; }
        public int ReadingTimeMinutes { get; set; }
        public int WordCount { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }

        // SEO data
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }

        // Links
        public string Self { get; set; }
        public string Comments { get; set; }
        public string AuthorProfile { get; set; }
    }

    public class BlogPostListApiResponse
    {
        public List<BlogPostApiResponse> Posts { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        // Links
        public string Self { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
    }

    public class BlogPostDetailApiResponse : BlogPostApiResponse
    {
        public string Content { get; set; }
        public List<CommentApiResponse> Comments { get; set; }
        public List<BlogPostApiResponse> RelatedPosts { get; set; }
    }

    public class CommentApiResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentApiResponse> Replies { get; set; }
        public bool IsApproved { get; set; }
    }
}