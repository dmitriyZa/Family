using Microsoft.AspNetCore.Mvc;
using Family.Shared;

[ApiController]
[Route("api/[controller]")]
public class RelationshipController : ControllerBase
{
    private readonly IRelationshipRepository _relationshipRepository;
    private readonly IRelationshipService _relationshipService;
    private readonly IFamilyRepository _familyRepository;

    public RelationshipController(IRelationshipRepository relationshipRepository, IRelationshipService relationshipService,
    IFamilyRepository familyRepository)
    {
        _relationshipRepository = relationshipRepository;
        _relationshipService = relationshipService;
        _familyRepository = familyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRelationships()
    {
        var relationships = await _relationshipRepository.GetAllRelationshipsAsync();
        var dtos = relationships.Select(r => new RelationshipDto
        {
            FamilyMemberId = r.FamilyMemberId,
            RelatedMemberId = r.RelatedMemberId,
            RelationTypeId = r.RelationTypeId
        });
        return Ok(dtos);
    }


    [HttpGet("{memberId}")]
    public async Task<IActionResult> GetRelationshipById(int memberId)
    {
        var relationships = await _relationshipRepository.GetRelationshipsByMemberIdAsync(memberId);

        // Если репозиторий возвращает null, то можно проверить на null, но чаще возвращается пустой список, так что лучше так:
        if (relationships == null || relationships.Count == 0)
            return NotFound();

        // Маппим к DTO
        var relationshipsDto = relationships.Select(fm => new RelationshipDto
        {
            FamilyMemberId = fm.FamilyMemberId,
            RelatedMemberId = fm.RelatedMemberId,
            RelationTypeId = fm.RelationTypeId
        }).ToList();

        return Ok(relationshipsDto);
    }


    [HttpPost]
    public async Task<IActionResult> CreateRelationship([FromBody] RelationshipDto dto)
    {
        if (dto == null) return BadRequest();

        try
        {
            await _relationshipService.CreateFullRelationshipAsync(dto);
            return Ok(new { Message = "Связи успешно созданы" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при создании связи: {ex.Message}");
        }
    }


}
