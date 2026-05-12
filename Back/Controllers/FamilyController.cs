using Microsoft.AspNetCore.Mvc;
using Family.Shared;

[ApiController]
[Route("api/[controller]")]
public class FamilyController : ControllerBase
{
    private readonly FamilyMemberService _familyMemberService;

    public FamilyController(FamilyMemberService familyMemberService)
    {
        _familyMemberService = familyMemberService;
    }

    /* [HttpGet("{id}")]
     public async Task<ActionResult<FamilyMemberDto>> GetFamilyById(int id)
     {
         var member = await _familyRepository.GetFamilyByIdAsync(id);

         if (member == null) return NotFound();

         // Превращаем сущность БД в DTO для передачи в MAUI
         return Ok(MapToDto(member));
     }*/

    [HttpGet("all")]
    public async Task<ActionResult<List<FamilyMemberDto>>> GetAllForTree()
    {
        try
        {
            var treeData = await _familyMemberService.GetFamilyTreeAsync();

            if (treeData == null || !treeData.Any())
            {
                return NotFound("Данные о членах семьи не найдены.");
            }

            return Ok(treeData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }


    [HttpPost("add")]
    public async Task<IActionResult> AddMember([FromBody] FamilyMemberDto dto)
    {
        if (dto == null) return BadRequest();
        var newMember = await _familyMemberService.AddMemberAsync(dto);

        return Ok(newMember);
    }

    // Обновление данных родственника
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(int id, [FromBody] FamilyMemberDto dto)
    {
        await _familyMemberService.UpdateMemberAsync(id, dto);
        return NoContent();
    }

    // Удаление родственника и его связей
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        await _familyMemberService.DeleteMemberAsync(id);
        return NoContent();
    }
    [HttpPost("{id}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

        // 1. Определяем путь к папке (wwwroot/photos)
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // 2. Генерируем уникальное имя, чтобы избежать конфликтов
        var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        // 3. Сохраняем файл на диск
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 4. Обновляем путь в базе данных
        // ВАЖНО: сохраняем относительный путь для фронтенда
        var member = await _familyMemberService.GetFamilyMember(id);
        if (member != null)
        {
            member.Photo = $"photos/{fileName}";
            await _familyMemberService.UpdateMemberAsync(member);
        }

        return Ok(new { url = member.Photo });
    }


}


