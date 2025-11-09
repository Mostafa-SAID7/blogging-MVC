            await _commentRepository.AddAsync(comment);

            // Update post analytics
            await UpdatePostCommentCountAsync(postId);

            _logger.LogInformation("Comment added to post {PostId} by user {UserId}", postId, user.Id);

            // Send notification email to post author
            try
            {
                await _emailService.SendCommentNotificationAsync(post, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send comment notification for post {PostId}", postId);
                // Don't fail the comment if email fails
            }