
using Api.Dtos;
using Api.Modules.Errors;
using Application.Categorys.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Categorys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("categorys")]
[ApiController]
public class CategoryController(ISender sender, ICategoryQueries categoryQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await categoryQueries.GetAll(cancellationToken);

        return entities.Select(CategoryDto.FromDomainModel).ToList();
    }
    
    [HttpGet("{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> Get([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        var entity = await categoryQueries.GetById(new CategoryId(categoryId), cancellationToken);

        return entity.Match<ActionResult<CategoryDto>>(
            u => CategoryDto.FromDomainModel(u),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto request, CancellationToken cancellationToken)
    {
        var input = new CreateCategoryCommand
        {
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<CategoryDto>>(
            f => CategoryDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> Delete([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        var input = new DeleteCategoryCommand
        {
            CategoryId = categoryId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            u => CategoryDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    [HttpPut]
    public async Task<ActionResult<CategoryDto>> Update(
        [FromBody] CategoryDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryCommand
        {
            Name = request.Name,
            CategoryId = request.Id!.Value
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            f => CategoryDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}