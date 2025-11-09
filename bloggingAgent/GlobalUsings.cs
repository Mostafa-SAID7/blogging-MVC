// System namespaces
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Text;
global using System.Text.Json;
global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;

// Microsoft.AspNetCore namespaces
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

// Entity Framework namespaces
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

// BloggingAgent namespaces
global using BloggingAgent.Models.Domain;
global using BloggingAgent.Models.DTOs;
global using BloggingAgent.Models.ViewModels;
global using BloggingAgent.Services.LLM;
global using BloggingAgent.Services.SEO;
global using BloggingAgent.Services.Content;
global using BloggingAgent.Services.Cache;
global using BloggingAgent.Services.Memory;
global using BloggingAgent.Services.Email;
global using BloggingAgent.Services.SocialMedia;
global using BloggingAgent.Data.Repositories;
global using BloggingAgent.Agents;
global using BloggingAgent.Configuration;
global using BloggingAgent.Utilities;
global using BloggingAgent.Extensions;
global using BloggingAgent.Middleware;