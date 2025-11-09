        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user already exists
            var existingUser = await _userRepository.SingleOrDefaultAsync(u =>
                u.Username == model.Username || u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username or email already exists.");
                return View(model);
            }

            // Create new user
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = HashPassword(model.Password), // TODO: Implement proper hashing
                Role = UserRole.Reader,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            _logger.LogInformation("New user registered: {Username}", user.Username);

            // Send welcome email
            try
            {
                await _emailService.SendWelcomeEmailAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
                // Don't fail registration if email fails
            }

            TempData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }