using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControleEstoque.Application.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Exceções custom
        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }

        public class ValidationException : Exception
        {
            public ValidationException(string message) : base(message) { }
        }

        public class BusinessException : Exception
        {
            public BusinessException(string message) : base(message) { }
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // chama próximo middleware
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            string message = ex.Message;

            switch (ex)
            {
                case ValidationException:
                case System.ComponentModel.DataAnnotations.ValidationException:
                    status = HttpStatusCode.BadRequest; // 400
                    break;
                case UnauthorizedException:
                    status = HttpStatusCode.Unauthorized; // 401
                    break;
                case NotFoundException:
                    status = HttpStatusCode.NotFound; // 404
                    break;
                case BusinessException:
                    status = HttpStatusCode.Conflict; // 409
                    break;
                default:
                    status = HttpStatusCode.InternalServerError; // 500
                    message = $"Erro interno inesperado.{ex.Message}";
                    break;
            }

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                status = (int)status
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(result);
        }
    }
}