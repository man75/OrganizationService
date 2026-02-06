using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterventionService.Application.Common.Behaviors
{
    public class SecurityValidationBehavior<TRequest, TResponse>(IUserContext userContext)
      : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!userContext.IsAuthenticated)
            {
                // Utilise ton AppException existante que l'on voit sur ton image !
                throw new Exception("Security context is missing or invalid.");
            }

            return await next();
        }
    }
}