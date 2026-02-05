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
    public async Task<IActionResult> CreateRelationship([FromBody] RelationshipCreateDto dto)
    {
        if (dto == null) return BadRequest();
        var relationship = new Relationship
        {
            FamilyMemberId = dto.FamilyMemberId,
            RelatedMemberId = dto.RelatedMemberId,
            Description = dto.Description
        };
        await _relationshipRepository.AddRelationshipAsync(relationship);
        return Ok(relationship);
    }
}
