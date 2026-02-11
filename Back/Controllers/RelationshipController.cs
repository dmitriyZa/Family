using Microsoft.AspNetCore.Mvc;

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

        var relationship = new Relationship
        {
            FamilyMemberId = dto.FamilyMemberId,
            RelatedMemberId = dto.RelatedMemberId,
            RelationTypeId = dto.RelationTypeId
        };
        await _relationshipRepository.AddRelationshipAsync(relationship);

        var relatedMember = await _familyRepository.GetFamilyByIdAsync(dto.FamilyMemberId);
        var relatedGender = relatedMember?.Gender;
        // Предположим, что dto.RelationTypeId теперь обязательно есть (ты должен передавать его с клиента)
        var reverseType = await _relationshipService.GetReverseRelationAsync(dto.RelationTypeId, relatedGender);

        if (reverseType != null)
        {
            var reverseRelationship = new Relationship
            {
                FamilyMemberId = dto.RelatedMemberId,
                RelatedMemberId = dto.FamilyMemberId,
                Description = reverseType.DisplayName // или reverseType.Code
            };
            await _relationshipRepository.AddRelationshipAsync(reverseRelationship);
        }

        return Ok(relationship);
    }

}
