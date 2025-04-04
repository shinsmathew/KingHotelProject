﻿using FluentValidation;
using KingHotelProject.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace KingHotelProject.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails();

            switch (exception)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails = new ValidationProblemDetails
                    {
                        Title = "Validation Error",
                        Status = context.Response.StatusCode,
                        Detail = "One or more validation errors occurred",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Instance = context.Request.Path,
                        Errors = validationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(x => x.ErrorMessage).ToArray())
                    };
                    break;

                case NotFoundException notFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Not Found",
                        Status = context.Response.StatusCode,
                        Detail = notFoundException.Message,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                        Instance = context.Request.Path
                    };
                    break;

                case BadRequestException badRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Bad Request",
                        Status = context.Response.StatusCode,
                        Detail = badRequestException.Message,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Instance = context.Request.Path
                    };
                    break;

                case Core.Exceptions.UnauthorizedAccessException unauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Status = context.Response.StatusCode,
                        Detail = unauthorizedAccessException.Message,
                        Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                        Instance = context.Request.Path
                    };
                    break;

                case ForbiddenException forbiddenException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Forbidden",
                        Status = context.Response.StatusCode,
                        Detail = forbiddenException.Message,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                        Instance = context.Request.Path
                    };
                    break;

                case ConflictException conflictException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Conflict",
                        Status = context.Response.StatusCode,
                        Detail = conflictException.Message,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                        Instance = context.Request.Path
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Internal Server Error",
                        Status = context.Response.StatusCode,
                        Detail = _env.IsDevelopment() ? exception.ToString() : "An unexpected error occurred",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Instance = context.Request.Path
                    };
                    break;
            }

            // Add exception stack trace in development
            if (_env.IsDevelopment())
            {
                problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }

   
    
}