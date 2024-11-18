    using Api.Dtos.Statistics;
    using Api.Modules.Errors;
    using Application.Statistics.Commands;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace Api.Controllers;

    [Route("statistics")]
    [ApiController]
    public class StatisticController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("getByTimeAndCategory/{startDate:datetime}/{endDate:datetime}/{categoryId:guid}/user=/{userId:guid}")]
        public async Task<ActionResult<StatisicDto>> GetByTimeAndCategory(
            [FromRoute] Guid userId, [FromRoute] DateTime startDate, [FromRoute] DateTime endDate,
            [FromRoute] Guid categoryId,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
            var userIdFromToken = new Guid(userIdClaim!.Value);
            
            var input = new GetByTimeAndCategoryCommand
            {
                StartDate = startDate,
                EndDate = endDate,
                CategoryId = categoryId,
                UserId = userId,
                UserIdFromToken=userIdFromToken
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<StatisicDto>>(
                t => StatisicDto.FromDomainModel(t),
                e => e.ToObjectResult());
        }
        
        [Authorize]
        [HttpGet("getByTimeAndCategory/{startDate:datetime}/{endDate:datetime}/user=/{userId:guid}")]
        public async Task<ActionResult<StatisicDto>> GetByTimeAndCategory(
            [FromRoute] Guid userId, [FromRoute] DateTime startDate, [FromRoute] DateTime endDate,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
            var userIdFromToken = new Guid(userIdClaim!.Value);

            var input = new GetByTimeAndCategoryForAllCommand
            {
                StartDate = startDate,
                EndDate = endDate,
                UserId = userId,
                UserIdFromToken=userIdFromToken
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<StatisicDto>>(
                t => StatisicDto.FromDomainModel(t),
                e => e.ToObjectResult());
        }

        [Authorize]
        [HttpGet("getForAllCategorys/{startDate:datetime}/{endDate:datetime}/user=/{userId:guid}")]
        public async Task<ActionResult<List<StatisicCategoryDto>>> GetForAllCategorys(
            [FromRoute] Guid userId, [FromRoute] DateTime startDate, [FromRoute] DateTime endDate,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
            var userIdFromToken = new Guid(userIdClaim!.Value);
            
            var input = new GetByTimeForCategoryCommand
            {
                StartDate = startDate,
                EndDate = endDate,
                UserId = userId,
                UserIdFromToken= userIdFromToken
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<List<StatisicCategoryDto>>>(
                stats => stats.Select(StatisicCategoryDto.FromDomainModel).ToList(),
                e => e.ToObjectResult()
            );
        }
    }