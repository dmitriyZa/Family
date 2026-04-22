using Microsoft.AspNetCore.Mvc;

// Здесь пусть будет /api/relationtypes
[ApiController]
[Route("api/[controller]")]
public class RelationTypesController : ControllerBase
{
    private readonly IRelationTypeRepository _relationTypeRepository;

    public RelationTypesController(IRelationTypeRepository relationTypeRepository)
    {
        _relationTypeRepository = relationTypeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRelationTypes()
    {
        var types = await _relationTypeRepository.GetAllRelationTypes();
        return Ok(types);
    }
}
