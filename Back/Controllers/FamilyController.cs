using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FamilyController : ControllerBase
{
    private readonly IFamilyRepository _familyRepository;

    public FamilyController(IFamilyRepository familyRepository)
    {
        _familyRepository = familyRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFamilyById(int id)
    {
        var familyMember = await _familyRepository.GetFamilyByIdAsync(id);

        if (familyMember == null)
        {
            return NotFound();
        }

        return Ok(familyMember);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllFamilyMembers()
    {
        var members = await _familyRepository.GetAllFamilyMembersAsync();
        return Ok(members);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFamily([FromBody] FamilyMember familyMember)
    {
        if (familyMember == null)
        {
            return BadRequest("Неверный формат данных.");
        }

        await _familyRepository.AddFamilyMemberAsync(familyMember);
        return CreatedAtAction(nameof(GetFamilyById), new { id = familyMember.Id }, familyMember);
    }
}

