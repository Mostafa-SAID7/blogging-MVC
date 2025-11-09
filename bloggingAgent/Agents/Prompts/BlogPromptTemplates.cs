using System.Collections.Generic;

namespace BloggingAgent.Agents.Prompts
{
    public static class BlogPromptTemplates
    {
        // Blog Post Generation Templates
        public static class BlogPostGeneration
        {
            public const string BaseGenerationPrompt = @"
You are an expert content creator specializing in {TopicCategory}. Create a comprehensive, engaging blog post about: {Topic}

Requirements:
- Target word count: {TargetWordCount} words
- Tone: {Tone}
- Target audience: {TargetAudience}
- Include relevant keywords: {Keywords}

Structure the post with:
1. An attention-grabbing introduction with a hook
2. 3-5 main sections with descriptive headings
3. Practical examples, case studies, or actionable insights
4. A compelling conclusion with key takeaways
5. Call-to-action for reader engagement

Make it SEO-optimized, informative, and shareable. Use conversational language while maintaining professionalism.";

            public const string TechnicalBlogPostPrompt = @"
Write a technical blog post about: {Topic}

Technical Specifications:
- Difficulty level: {DifficultyLevel}
- Prerequisites: {Prerequisites}
- Code examples: {IncludeCodeExamples}
- Framework/Language: {TechnologyStack}

Structure:
1. Problem statement and context
2. Step-by-step solution or explanation
3. Code examples with detailed comments
4. Best practices and common pitfalls
5. Performance considerations
6. Further reading and resources

Ensure code is production-ready and follows industry standards.";

            public const string TutorialBlogPostPrompt = @"
Create a comprehensive tutorial on: {Topic}

Tutorial Structure:
- Learning objectives
- Prerequisites and requirements
- Step-by-step instructions
- Code examples and screenshots
- Troubleshooting section
- Best practices
- Next steps and advanced topics

Make it beginner-friendly with clear explanations and practical examples.";
        }

        // SEO Optimization Templates
        public static class SeoOptimization
        {
            public const string MetaDescriptionPrompt = @"
Generate a compelling meta description (150-160 characters) for this blog post:

Title: {Title}
Content Preview: {ContentPreview}
Target Keywords: {Keywords}

The meta description should:
- Include primary keyword naturally
- Create curiosity or urgency
- Encourage clicks
- Be under 160 characters
- Match search intent";

            public const string KeywordOptimizationPrompt = @"
Analyze this content and suggest keyword optimizations:

Content: {Content}
Current Title: {Title}
Target Keywords: {Keywords}

Provide:
1. Keyword density analysis
2. Missing keyword opportunities
3. Suggested title improvements
4. Content gaps to fill
5. Internal linking suggestions";

            public const string TitleOptimizationPrompt = @"
Optimize this blog post title for SEO and click-through rate:

Current Title: {CurrentTitle}
Topic: {Topic}
Target Keywords: {Keywords}

Requirements:
- Include primary keyword
- Keep under 60 characters
- Create curiosity or benefit
- Match search intent
- Be compelling and clickable";
        }

        // Content Enhancement Templates
        public static class ContentEnhancement
        {
            public const string ReadabilityImprovementPrompt = @"
Improve the readability of this blog post content:

Original Content: {Content}

Improvements needed:
- Break up long paragraphs
- Add subheadings
- Use bullet points and lists
- Simplify complex sentences
- Add transition words
- Include white space

Maintain the original meaning while making it more scannable and engaging.";

            public const string EngagementOptimizationPrompt = @"
Enhance this blog post for better reader engagement:

Content: {Content}
Current Tone: {Tone}

Add:
- Rhetorical questions
- Personal anecdotes or examples
- Statistics or data points
- Quotes from experts
- Calls-to-action
- Interactive elements suggestions

Make it more conversational and compelling.";

            public const string ContentExpansionPrompt = @"
Expand this blog post section to reach the target word count:

Current Content: {Content}
Target Word Count: {TargetWordCount}
Section Topic: {SectionTopic}

Add:
- More detailed explanations
- Additional examples
- Supporting evidence
- Related concepts
- Practical applications

Ensure the expansion is natural and adds value.";
        }

        // Social Media & Sharing Templates
        public static class SocialMedia
        {
            public const string TwitterThreadPrompt = @"
Create a Twitter thread summarizing this blog post:

Blog Post Title: {Title}
Key Points: {KeyPoints}
URL: {Url}

Requirements:
- 6-8 connected tweets
- Each tweet under 280 characters
- Include compelling hook
- End with call-to-action
- Use relevant hashtags
- Include the blog post URL";

            public const string LinkedInPostPrompt = @"
Create a LinkedIn post promoting this blog post:

Title: {Title}
Key Takeaway: {KeyTakeaway}
Target Audience: {TargetAudience}
URL: {Url}

Make it professional, engaging, and shareable with relevant hashtags.";

            public const string NewsletterSnippetPrompt = @"
Create a newsletter snippet for this blog post:

Title: {Title}
Content Summary: {Summary}
Key Benefits: {Benefits}
URL: {Url}

Make it compelling for email subscribers with clear value proposition.";
        }

        // Quality Assurance Templates
        public static class QualityAssurance
        {
            public const string ContentReviewPrompt = @"
Review this blog post for quality and completeness:

Title: {Title}
Content: {Content}
Target Keywords: {Keywords}

Check for:
- Grammatical errors
- Factual accuracy
- SEO optimization
- Readability
- Engagement factors
- Call-to-action effectiveness
- Internal/external links
- Image alt texts

Provide specific recommendations for improvement.";

            public const string FactCheckingPrompt = @"
Fact-check this blog post content:

Content: {Content}
Topic: {Topic}

Verify:
- Statistical claims
- Technical accuracy
- Source credibility
- Date relevance
- Industry standards

Flag any inaccuracies or outdated information.";

            public const string PlagiarismCheckPrompt = @"
Analyze this content for potential plagiarism or originality issues:

Content: {Content}
Topic: {Topic}

Check for:
- Unique value proposition
- Original insights
- Proper attribution
- Common phrases that might be overused
- Areas needing more original content

Suggest improvements for uniqueness.";
        }

        // Utility Methods
        public static class TemplateHelpers
        {
            public static string FormatPrompt(string template, Dictionary<string, string> parameters)
            {
                var result = template;
                foreach (var param in parameters)
                {
                    result = result.Replace($"{{{param.Key}}}", param.Value);
                }
                return result;
            }

            public static string GetToneDescription(ContentTone tone)
            {
                return tone switch
                {
                    ContentTone.Professional => "formal, authoritative, and trustworthy",
                    ContentTone.Casual => "conversational, friendly, and approachable",
                    ContentTone.Expert => "highly technical, detailed, and authoritative",
                    ContentTone.Inspirational => "motivational, uplifting, and encouraging",
                    ContentTone.Educational => "informative, structured, and easy to follow",
                    ContentTone.Promotional => "benefit-focused, persuasive, and action-oriented",
                    _ => "balanced and engaging"
                };
            }

            public static string GetDifficultyDescription(string difficulty)
            {
                return difficulty.ToLower() switch
                {
                    "beginner" => "suitable for newcomers with no prior experience",
                    "intermediate" => "requires basic knowledge and some experience",
                    "advanced" => "demands deep understanding and significant experience",
                    "expert" => "intended for professionals with extensive knowledge",
                    _ => "accessible to a general audience"
                };
            }
        }
    }

    // Enums for template parameters
    public enum ContentTone
    {
        Professional,
        Casual,
        Expert,
        Inspirational,
        Educational,
        Promotional
    }

    public enum BlogCategory
    {
        Technology,
        Business,
        Marketing,
        Design,
        Development,
        Tutorial,
        News,
        Opinion,
        CaseStudy,
        Review
    }
}