using Microsoft.AspNetCore.Mvc;
using Family.Shared;

[ApiController]
[Route("api/[controller]")]
public class FamilyController : ControllerBase
{
    private readonly FamilyMemberService _familyMemberService;
    private readonly IFileStorageService _fileStorageService;

    public FamilyController(FamilyMemberService familyMemberService, IFileStorageService fileStorageService)
    {
        _familyMemberService = familyMemberService;
        _fileStorageService = fileStorageService;
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
        var member = await _familyMemberService.GetFamilyMember(id);
        if (member == null) return NotFound("Родственник не найден");

        if (string.IsNullOrEmpty(dto.PhotoUrl))
        {
            dto.PhotoUrl = member.Photo;
        }

        await _familyMemberService.UpdateMemberAsync(id, dto);
        return NoContent();
    }

    // Удаление родственника и его связей
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        var member = await _familyMemberService.GetFamilyMember(id);
        if (!string.IsNullOrEmpty(member.Photo))
        {
            _fileStorageService.DeletePhoto(member.Photo);
        }
        await _familyMemberService.DeleteMemberAsync(id);
        return NoContent();
    }
    [HttpPost("{id}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
    {
        try
        {
            var member = await _familyMemberService.GetFamilyMember(id);
            if (member == null) return NotFound("Родственник не найден");

            // 1. Удаляем старое фото через сервис, если оно было
            if (!string.IsNullOrEmpty(member.Photo))
            {
                _fileStorageService.DeletePhoto(member.Photo);
            }
            // 2. Сохраняем новое фото через сервис
            var relativePath = await _fileStorageService.SavePhotoAsync(id, file);

            // 3. Обновляем сущность
            member.Photo = relativePath;
            await _familyMemberService.UpdateMemberAsync(member);

            return Ok(new { url = member.Photo });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при загрузке: {ex.Message}");
        }
    }


}


