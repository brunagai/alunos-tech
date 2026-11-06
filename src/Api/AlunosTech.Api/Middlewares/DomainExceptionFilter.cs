using AlunosTech.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlunosTech.Api.Middlewares;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DomainException domainException)
        {
            return;
        }

        context.Result = new BadRequestObjectResult(new { mensagem = domainException.Message });
        context.ExceptionHandled = true;
    }
}
