using System.Collections.Generic;

namespace BloggingAgent.Agents.Prompts
{
    public static class BlogPromptTemplates
    {
        public static class ContentGeneration
        {
            public const string BlogPostTemplate = @"
Write a comprehensive, engaging blog post about: {Topic}

CONTENT REQUIREMENTS:
- Target word count: {WordCount} words
- Tone: {Tone}
- Target audience: {TargetAudience}
- Keywords to include: {Keywords}

STRUCTURE REQUIREMENTS:
1. **Attention-grabbing introduction** (150-200 words)
   - Hook the reader with a compelling question, statistic, or story
   - Briefly introduce the main topic
   - Preview what readers will learn

2. **Main content sections** with descriptive H2/H3 headings
   - Break down complex topics into digestible sections
   - Include practical examples, case studies, or real-world applications
   - Use bullet points and numbered lists for clarity
   - Add relevant statistics, research, or expert opinions

3. **Practical implementation or actionable insights**
   - Step-by-step guides where applicable
   - Best practices and recommendations
   - Common pitfalls to avoid

4. **Conclusion with key takeaways** (100-150 words)
   - Summarize main points
   - Call-to-action for readers
   - Future outlook or related topics to explore

WRITING GUIDELINES:
- Use conversational yet professional tone
- Include relevant subheadings for scannability
- Add transition sentences between sections
- Incorporate the specified keywords naturally
- Write for the target audience's knowledge level
- Include 2-3 relevant internal/external links
- End with a question to encourage comments

SEO OPTIMIZATION:
- Primary keyword in title, introduction, and conclusion
- Secondary keywords distributed throughout content
- Natural keyword density (1-2% for primary, 0.5-1% for secondary)
- Compelling meta description potential in introduction

FORMATTING:
- Use markdown formatting for emphasis and structure
- Include alt text for any images mentioned
- Use short paragraphs (3-5 sentences max)
- Bold important terms and key phrases
";

            public const string TechnicalArticleTemplate = @"
Create a detailed technical article about: {Topic}

TECHNICAL SPECIFICATIONS:
- Complexity Level: {ComplexityLevel}
- Target Audience: {TargetAudience}
- Prerequisites: {Prerequisites}
- Estimated Reading Time: {ReadingTime} minutes

ARTICLE STRUCTURE:
1. **Executive Summary** (100-150 words)
   - Overview of the technology/concept
   - Key benefits and use cases
   - Prerequisites and requirements

2. **Technical Background** (200-300 words)
   - Historical context and evolution
   - Core concepts and terminology
   - Related technologies and comparisons

3. **Detailed Implementation** (400-600 words)
   - Step-by-step technical implementation
   - Code examples with explanations
   - Configuration and setup instructions
   - Best practices and patterns

4. **Advanced Topics** (200-300 words)
   - Performance optimization techniques
   - Security considerations
   - Scalability and monitoring
   - Troubleshooting common issues

5. **Conclusion and Next Steps** (100-150 words)
   - Summary of key learnings
   - Recommended resources for further study
   - Future developments and trends

TECHNICAL WRITING STANDARDS:
- Use precise technical terminology
- Include code snippets with syntax highlighting
- Provide clear, numbered steps for procedures
- Explain complex concepts with analogies
- Include warnings for potential issues
- Reference official documentation and standards

CODE AND EXAMPLES:
- Use appropriate programming languages
- Include error handling examples
- Provide both basic and advanced implementations
- Comment code for clarity
- Test examples for accuracy

SEO ELEMENTS:
- Technical keywords in title and headings
- Long-tail keywords for specific use cases
- Include search-friendly section headings
- Add internal links to related technical content
";

            public const string TutorialTemplate = @"
Create a comprehensive step-by-step tutorial for: {Topic}

TUTORIAL SPECIFICATIONS:
- Skill Level: {SkillLevel}
- Time Required: {TimeRequired}
- Prerequisites: {Prerequisites}
- Tools/Software Needed: {ToolsRequired}

TUTORIAL STRUCTURE:
1. **Introduction** (150-200 words)
   - What readers will accomplish
   - Why this tutorial is valuable
   - Prerequisites and requirements check
   - Expected outcomes and learning objectives

2. **Preparation and Setup** (100-150 words)
   - Required tools and software installation
   - Environment setup and configuration
   - Testing the setup with simple examples

3. **Step-by-Step Instructions** (Main content - 500-800 words)
   - Clear, numbered steps with detailed explanations
   - Code examples for each step
   - Screenshots descriptions or placeholders
   - Common mistakes and how to avoid them
   - Progress checkpoints and verification steps

4. **Advanced Techniques** (200-300 words)
   - Optimization and best practices
   - Customization options
   - Alternative approaches
   - Performance considerations

5. **Troubleshooting** (150-200 words)
   - Common errors and solutions
   - Debugging techniques
   - Community resources and support

6. **Conclusion** (100 words)
   - Summary of what was accomplished
   - Next steps and further learning
   - Encouragement to experiment and build upon

PEDAGOGICAL ELEMENTS:
- Break complex tasks into small, manageable steps
- Include 'why' explanations for each step
- Provide context and real-world applications
- Encourage experimentation and exploration
- Include tips for different skill levels

VISUAL AND INTERACTIVE ELEMENTS:
- Code snippets with syntax highlighting
- Image placeholders for screenshots
- Interactive examples where applicable
- Downloadable resources or templates
- Links to live demos or working examples

ACCESSIBILITY:
- Clear, descriptive headings
- Alternative text for images
- Keyboard navigation friendly
- Screen reader compatible content
";
        }

        public static class ContentOptimization
        {
            public const string SeoOptimizationTemplate = @"
Optimize the following blog post for better SEO performance:

ORIGINAL TITLE: {OriginalTitle}
ORIGINAL CONTENT: {OriginalContent}

SEO REQUIREMENTS:
- Primary Keywords: {PrimaryKeywords}
- Secondary Keywords: {SecondaryKeywords}
- Target Search Intent: {SearchIntent}
- Target Word Count: {TargetWordCount}

OPTIMIZATION TASKS:
1. **Title Optimization**
   - Include primary keyword naturally
   - Make it compelling and click-worthy
   - Keep under 60 characters for SERP display
   - Consider emotional triggers and benefits

2. **Meta Description Enhancement**
   - Write compelling 150-160 character description
   - Include primary keyword
   - Add call-to-action
   - Match search intent

3. **Content Structure Optimization**
   - Add strategic H2/H3 headings with keywords
   - Improve content flow and readability
   - Add internal and external links
   - Include relevant statistics and data

4. **Keyword Optimization**
   - Natural keyword placement throughout content
   - Primary keyword in first paragraph
   - Secondary keywords in subheadings and body
   - Maintain 1-2% keyword density

5. **User Experience Improvements**
   - Add table of contents for long articles
   - Include relevant images with alt text
   - Break up long paragraphs
   - Add bullet points and numbered lists

6. **Technical SEO Elements**
   - Optimize for featured snippets
   - Add schema markup suggestions
   - Improve internal linking structure
   - Ensure mobile-friendly formatting

OUTPUT FORMAT:
- Provide optimized title
- Provide optimized meta description
- Provide fully optimized content with improvements highlighted
- Include SEO recommendations and rationale
";

            public const string ReadabilityImprovementTemplate = @"
Improve the readability and user engagement of this blog post:

ORIGINAL CONTENT: {OriginalContent}

READABILITY GOALS:
- Target Reading Level: {TargetReadingLevel}
- Average Sentence Length: {TargetSentenceLength} words
- Average Paragraph Length: {TargetParagraphLength} sentences
- Target Reading Time: {TargetReadingTime} minutes

IMPROVEMENT STRATEGIES:
1. **Sentence Structure**
   - Break long sentences into shorter ones
   - Use active voice instead of passive
   - Remove unnecessary jargon and complex words
   - Vary sentence length for rhythm

2. **Paragraph Structure**
   - Limit paragraphs to 3-5 sentences
   - Start paragraphs with topic sentences
   - Use transition words between paragraphs
   - Add white space for visual breaks

3. **Word Choice**
   - Replace complex words with simpler alternatives
   - Use contractions for conversational tone
   - Explain technical terms when necessary
   - Use familiar, everyday language

4. **Formatting and Visual Elements**
   - Add subheadings every 200-300 words
   - Use bullet points for lists
   - Include relevant images or diagrams
   - Add bold/italic formatting for emphasis

5. **Engagement Elements**
   - Add rhetorical questions
   - Include real-world examples
   - Add calls-to-action throughout
   - End with thought-provoking questions

6. **Accessibility Improvements**
   - Use descriptive link text
   - Add alt text for images
   - Ensure color contrast in formatting
   - Use semantic HTML structure

OUTPUT REQUIREMENTS:
- Provide readability score before and after
- Show specific improvements made
- Maintain original meaning and key information
- Ensure content remains SEO-friendly
- Include engagement metrics suggestions
";
        }

        public static class ContentAnalysis
        {
            public const string SeoAnalysisTemplate = @"
Analyze the SEO performance and optimization opportunities for this content:

CONTENT TO ANALYZE:
Title: {Title}
Content: {Content}
Target Keywords: {TargetKeywords}

ANALYSIS CATEGORIES:
1. **Title Optimization**
   - Keyword presence and placement
   - Length and SERP display suitability
   - Click-through potential and appeal

2. **Content Quality**
   - Word count adequacy
   - Keyword density and distribution
   - Content depth and comprehensiveness
   - User engagement potential

3. **Technical SEO**
   - Heading structure (H1, H2, H3 usage)
   - Internal/external linking
   - Image optimization (alt text, file names)
   - Mobile-friendliness

4. **On-Page SEO Elements**
   - Meta description quality
   - URL structure and slug optimization
   - Schema markup opportunities
   - Social media optimization

5. **Content Structure**
   - Readability and scannability
   - Use of formatting elements
   - Call-to-action placement
   - User experience considerations

6. **Competitive Analysis**
   - Keyword difficulty assessment
   - Search intent alignment
   - Content gap identification
   - Unique value proposition

OUTPUT FORMAT:
- Overall SEO Score (0-100)
- Detailed analysis for each category
- Specific recommendations for improvement
- Priority action items
- Estimated impact of recommended changes
";

            public const string ContentQualityTemplate = @"
Evaluate the overall quality and effectiveness of this blog content:

CONTENT TO EVALUATE:
Title: {Title}
Content: {Content}
Target Audience: {TargetAudience}
Content Type: {ContentType}

QUALITY DIMENSIONS:
1. **Content Relevance**
   - Alignment with target audience needs
   - Topic timeliness and importance
   - Value proposition clarity
   - Problem-solution fit

2. **Content Accuracy**
   - Factual correctness
   - Data and statistics validity
   - Source credibility
   - Technical accuracy

3. **Content Structure**
   - Logical flow and organization
   - Heading hierarchy effectiveness
   - Transition smoothness
   - Conclusion strength

4. **Engagement Potential**
   - Hook effectiveness
   - Storytelling quality
   - Emotional connection
   - Shareability factors

5. **Practical Value**
   - Actionable information provided
   - Implementation guidance
   - Real-world applicability
   - Resource quality

6. **SEO and Discoverability**
   - Keyword optimization
   - Search intent matching
   - Internal linking opportunities
   - Social sharing optimization

SCORING METHODOLOGY:
- Rate each dimension 1-10
- Provide detailed justification
- Identify strengths and weaknesses
- Suggest specific improvements
- Estimate content performance potential

OUTPUT FORMAT:
- Overall Quality Score
- Dimension-by-dimension breakdown
- Strengths and weaknesses summary
- Actionable improvement recommendations
- Content performance predictions
";
        }

        public static class SocialMedia
        {
            public const string SocialPostTemplate = @"
Create engaging social media posts to promote this blog content:

BLOG POST DETAILS:
Title: {BlogTitle}
Excerpt: {BlogExcerpt}
URL: {BlogUrl}
Target Keywords: {Keywords}

SOCIAL MEDIA PLATFORMS:
1. **Twitter/X Thread**
   - Hook tweet (280 characters)
   - 3-5 connected tweets with key points
   - Call-to-action with link
   - Relevant hashtags

2. **LinkedIn Post**
   - Professional hook and value proposition
   - Key insights and takeaways
   - Thought leadership angle
   - Call-to-action

3. **Facebook Post**
   - Engaging question or statement
   - Brief content summary
   - Visual element suggestion
   - Community engagement prompt

4. **Instagram Caption**
   - Attention-grabbing opening
   - Key value points
   - Hashtags and emojis
   - Link in bio instruction

CONTENT STRATEGY:
- Highlight unique value proposition
- Include compelling statistics or quotes
- Ask engaging questions
- Use platform-specific formatting
- Include relevant emojis and hashtags
- Encourage shares and comments

ENGAGEMENT OPTIMIZATION:
- Time-sensitive hooks
- Controversy or curiosity gaps
- Social proof elements
- User-generated content prompts
- Question-based engagement

TRACKING AND MEASUREMENT:
- UTM parameters for link tracking
- Call-to-action specificity
- Engagement metric goals
- A/B testing suggestions
";
        }

        public static class EmailMarketing
        {
            public const string NewsletterTemplate = @"
Create an email newsletter featuring this blog content:

BLOG CONTENT:
Title: {BlogTitle}
Excerpt: {BlogExcerpt}
Author: {Author}
Publish Date: {PublishDate}

NEWSLETTER STRUCTURE:
1. **Subject Line Options**
   - 3-5 compelling subject lines
   - A/B testing recommendations
   - Open rate optimization tips

2. **Email Header**
   - Personalized greeting
   - Brief introduction
   - Value proposition

3. **Main Content**
   - Engaging content summary
   - Key takeaways and insights
   - Author credibility elements
   - Social proof or testimonials

4. **Call-to-Action**
   - Clear next step instructions
   - Multiple CTA options
   - Urgency or scarcity elements

5. **Additional Content**
   - Related posts or resources
   - Author bio and social links
   - Newsletter subscription benefits

6. **Email Footer**
   - Contact information
   - Social media links
   - Unsubscribe instructions

EMAIL MARKETING BEST PRACTICES:
- Mobile-responsive design
- Clear visual hierarchy
- Compelling imagery
- Personalization opportunities
- Spam compliance (CAN-SPAM)

ENGAGEMENT STRATEGIES:
- Question-based subject lines
- Benefit-focused content
- Social sharing encouragement
- Forward-to-friend prompts
- Comment and discussion invitations

ANALYTICS AND TRACKING:
- Open rate tracking
- Click-through rate monitoring
- Conversion goal setting
- A/B testing framework
- Performance benchmarking
";
        }

        public static Dictionary<string, string> GetTemplate(string category, string templateName)
        {
            return category.ToLower() switch
            {
                "contentgeneration" => templateName.ToLower() switch
                {
                    "blogpost" => new Dictionary<string, string> { ["template"] = BlogPostTemplate },
                    "technical" => new Dictionary<string, string> { ["template"] = TechnicalArticleTemplate },
                    "tutorial" => new Dictionary<string, string> { ["template"] = TutorialTemplate },
                    _ => new Dictionary<string, string> { ["template"] = BlogPostTemplate }
                },
                "contentoptimization" => templateName.ToLower() switch
                {
                    "seo" => new Dictionary<string, string> { ["template"] = SeoOptimizationTemplate },
                    "readability" => new Dictionary<string, string> { ["template"] = ReadabilityImprovementTemplate },
                    _ => new Dictionary<string, string> { ["template"] = SeoOptimizationTemplate }
                },
                "contentanalysis" => templateName.ToLower() switch
                {
                    "seo" => new Dictionary<string, string> { ["template"] = SeoAnalysisTemplate },
                    "quality" => new Dictionary<string, string> { ["template"] = ContentQualityTemplate },
                    _ => new Dictionary<string, string> { ["template"] = SeoAnalysisTemplate }
                },
                "socialmedia" => new Dictionary<string, string> { ["template"] = SocialPostTemplate },
                "emailmarketing" => new Dictionary<string, string> { ["template"] = NewsletterTemplate },
                _ => new Dictionary<string, string> { ["template"] = BlogPostTemplate }
            };
        }
    }
}