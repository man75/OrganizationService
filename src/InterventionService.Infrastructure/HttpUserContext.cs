


using InterventionService.Application.Common;
using Microsoft.AspNetCore.Http;

namespace StockService.Infrastructure
{
    public sealed class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private readonly IHeaderDictionary? _headers = httpContextAccessor.HttpContext?.Request.Headers;

        public Guid UserId => Guid.TryParse(_headers?["X-User-Id"], out var id) ? id : Guid.Empty;
        public Guid OrganizationId => Guid.TryParse(_headers?["X-Org-Id"], out var id) ? id : Guid.Empty;
        public bool IsAuthenticated => UserId != Guid.Empty && OrganizationId != Guid.Empty;
    }
}
