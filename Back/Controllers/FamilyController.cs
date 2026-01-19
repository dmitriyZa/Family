using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FamilyController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetFamilyById(int id)
    {
        return Ok(new { Id = id, Name = "Doe Family" });
    }

    [HttpPost]
    public IActionResult CreateFamily(Family family)
    {
        // Логика для добавления нового элемента
        return CreatedAtAction(nameof(GetFamilyById), new { id = family.Id }, family);
    }
}

public record Family(int Id, string Name);
