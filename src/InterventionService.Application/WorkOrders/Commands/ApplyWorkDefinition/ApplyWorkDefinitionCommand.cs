using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterventionService.Application.WorkOrders.Commands.ApplyWorkDefinition
{
    public sealed record ApplyWorkDefinitionCommand(Guid WorkOrderId, Guid DefinitionId)
     : IRequest<Result<WorkOrderDto>>;
}
