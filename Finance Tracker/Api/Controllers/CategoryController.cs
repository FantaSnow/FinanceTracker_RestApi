using Api.Dtos.Categorys;
using Api.Modules;
using Api.Modules.Errors;
using Application.Categorys.Commands;
using Application.Common.Interfaces.Queries;
using Application.Tickets;
using Domain.Categorys;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("categorys")]
[ApiController]
public class CategoryController(ISender sender, ICategoryQueries categoryQueries) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("getAll/")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await categoryQueries.GetAll(cancellationToken);

        return entities.Select(CategoryDto.FromDomainModel).ToList();
    }

    [AllowAnonymous]
    [HttpGet("getById/{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> Get([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        var entity = await categoryQueries.GetById(new CategoryId(categoryId), cancellationToken);

        return entity.Match<ActionResult<CategoryDto>>(
            u => CategoryDto.FromDomainModel(u),
            () => NotFound());
    }

    [Authorize]
    [RequiresClaim(IdentityData.IsAdminClaimName, "True")]
    [HttpPost("create/")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateCategoryCommand
        {
            Name = request.Name,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            f => CategoryDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [Authorize]
    [RequiresClaim(IdentityData.IsAdminClaimName, "True")]
    [HttpDelete("delete/{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> Delete([FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        var input = new DeleteCategoryCommand
        {
            CategoryId = categoryId,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            u => CategoryDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [Authorize]
    [RequiresClaim(IdentityData.IsAdminClaimName, "True")]
    [HttpPut("update/{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(
        [FromRoute] Guid categoryId,
        [FromBody] CategoryUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryCommand
        {
            Name = request.Name,
            CategoryId = categoryId,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            f => CategoryDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}