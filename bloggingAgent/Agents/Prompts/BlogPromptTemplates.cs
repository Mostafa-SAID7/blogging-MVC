using System.Collections.Generic;

namespace BloggingAgent.Agents.Prompts
{
    public static class BlogPromptTemplates
    {
        public static string GenerateBlogPost(string topic, string keywords, int wordCount, string tone, string targetAudience)
        {
            return $@"
Write a comprehensive blog post about: {topic}

CONTENT REQUIREMENTS:
- Target word count: {wordCount} words
- Tone: {tone}
- Target audience: {targetAudience}
- Keywords to include naturally: {keywords}

STRUCTURE:
1. ENGAGING INTRODUCTION (150-200 words)
   - Hook the reader with a compelling question or statistic
   - Briefly introduce the topic and its importance
   - Preview what readers will learn

2. MAIN CONTENT SECTIONS (use H2 headings)
   - Break down the topic into 3-5 logical sections
   - Each section should be 200-400 words
   - Include practical examples, case studies, or data
   - Use bullet points or numbered lists where appropriate

3. PRACTICAL APPLICATIONS
   - Provide actionable steps or tips
   - Include real-world examples
   - Address common challenges and solutions

4. CONCLUSION (100-150 words)
   - Summarize key takeaways
   - End with a call-to-action
   - Encourage reader engagement

SEO OPTIMIZATION:
- Use the primary keyword in the first paragraph
- Include keywords naturally throughout the content
- Use descriptive headings and subheadings
- Ensure content provides genuine value

WRITING STYLE:
- Write conversationally but professionally
- Use active voice when possible
- Include transitions between sections
- End paragraphs with strong statements

Make this post informative, engaging, and optimized for both readers and search engines.";
        }

        public static string OptimizeContentForSEO(string content, string[] keywords)
        {
            var keywordsList = string.Join(", ", keywords);
            return $@"
Optimize the following blog post content for SEO while maintaining readability and value:

TARGET KEYWORDS: {keywordsList}

CONTENT TO OPTIMIZE:
{content}

OPTIMIZATION REQUIREMENTS:
1. Ensure primary keyword appears in:
   - Title (if not already optimized)
   - First paragraph
   - At least one H2 heading
   - Conclusion

2. Secondary keywords should appear naturally throughout the content

3. Improve content structure:
   - Clear, descriptive headings
   - Logical flow between sections
   - Internal linking opportunities (suggest where to add links)

4. Enhance readability:
   - Shorter paragraphs (3-5 sentences)
   - Bullet points for lists
   - Bold key terms
   - Transition sentences

5. SEO elements to add:
   - Meta description suggestions
   - Image alt text suggestions
   - Internal linking suggestions

Return the optimized content with improvements clearly marked.";
        }

        public static string GenerateMetaDescription(string content, string title)
        {
            return $@"
Generate a compelling meta description for this blog post:

TITLE: {title}

CONTENT PREVIEW:
{content.Substring(0, Math.Min(500, content.Length))}

REQUIREMENTS:
- Length: 120-160 characters
- Include primary keyword naturally
- Create curiosity or urgency
- End with call-to-action if appropriate
- No quotes around the description

Return only the meta description text.";
        }

        public static string SuggestTags(string content, int count = 8)
        {
            return $@"
Analyze this blog post content and suggest {count} relevant tags:

CONTENT:
{content}

REQUIREMENTS:
- Tags should be 1-3 words each
- Focus on topics, themes, and key concepts
- Include both specific and broader category tags
- Consider search intent and user queries
- Avoid generic tags like 'blog' or 'article'

Return the tags as a comma-separated list.";
        }

        public static string GenerateExcerpt(string content, int maxLength = 150)
        {
            return $@"
Create a compelling excerpt from this blog post content:

CONTENT:
{content}

REQUIREMENTS:
- Maximum length: {maxLength} characters
- Capture the essence and value proposition
- End with a complete sentence
- Create curiosity to encourage clicks
- Include primary keyword if natural

Return only the excerpt text.";
        }

        public static string AnalyzeContentQuality(string content, string title)
        {
            return $@"
Analyze the quality and SEO effectiveness of this blog post:

TITLE: {title}

CONTENT:
{content}

EVALUATION CRITERIA:
1. Content Quality (0-100 points)
   - Depth and comprehensiveness
   - Originality and value
   - Readability and engagement

2. SEO Optimization (0-100 points)
   - Keyword usage and density
   - Content structure and headings
   - Internal/external linking opportunities
   - Mobile-friendliness considerations

3. Technical SEO (0-100 points)
   - Title length and optimization
   - Meta description potential
   - URL structure suggestions
   - Image optimization needs

4. User Experience (0-100 points)
   - Content flow and organization
   - Call-to-action effectiveness
   - Shareability potential

Provide scores and specific recommendations for improvement.";
        }

        public static string GenerateStructuredData(string title, string description, string content, List<string> keywords)
        {
            var keywordsList = string.Join(", ", keywords);
            return $@"
Generate JSON-LD structured data for this blog post:

TITLE: {title}
DESCRIPTION: {description}
KEYWORDS: {keywordsList}

CONTENT PREVIEW:
{content.Substring(0, Math.Min(300, content.Length))}

REQUIREMENTS:
- Use BlogPosting schema
- Include all relevant properties
- Add breadcrumb navigation if applicable
- Include author information placeholder
- Add publication and modification dates placeholders
- Include image placeholder if images are mentioned

Return valid JSON-LD markup.";
        }
    }
}