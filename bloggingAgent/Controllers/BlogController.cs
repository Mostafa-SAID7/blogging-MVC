        private BlogPost MapToDomain(BlogPostDto dto)
        {
            return new BlogPost
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Content = dto.Content,
                Excerpt = dto.Excerpt,
                AuthorId = dto.Author, // This should be AuthorId, but we'll handle it properly
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                IsPublished = dto.IsPublished,
                Tags = dto.Tags,
                SeoMetadata = new SeoMetadata(), // Will be populated by SEO service
                Analytics = new ContentAnalytics() // Will be populated by analytics service
            };
        }