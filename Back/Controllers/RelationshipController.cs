using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RelationshipController : ControllerBase
{
    private readonly IRelationshipRepository _relationshipRepository;

    public RelationshipController(IRelationshipRepository relationshipRepository)
    {
        _relationshipRepository = relationshipRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRelationship([FromBody] Relationship relationship)
    {
        if (relationship == null)
            return BadRequest("Неверный формат данных.");

        await _relationshipRepository.AddRelationshipAsync(relationship);
        return Ok(relationship);
    }
}
