using System;
using System.Collections.Generic;

namespace BloggingAgent.Models.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
    }

    public enum UserRole
    {
        Reader,
        Author,
        Editor,
        Administrator
    }
}