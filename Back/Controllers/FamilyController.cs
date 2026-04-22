using Microsoft.AspNetCore.Mvc;
using Family.Shared; // Подключаем нашу общую библиотеку

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
            // Логируем ошибку и возвращаем 500
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }

    /*
        [HttpPost]
        public async Task<IActionResult> CreateFamily([FromBody] FamilyMemberDto dto)
        {
            if (dto == null) return BadRequest("Неверный формат данных.");

            // Превращаем DTO обратно в сущность БД для сохранения
            var entity = new FamilyMember
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ParentName = dto.ParentName,
                DateOfBirth = dto.DateOfBirth,
                Biography = dto.Biography,
                Gender = dto.Gender
            };

            await _familyRepository.AddFamilyMemberAsync(entity);

            // Возвращаем ID созданной записи
            dto.Id = entity.Id;
            return CreatedAtAction(nameof(GetFamilyById), new { id = dto.Id }, dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFamilyMember(int id)
        {
            // 1. Проверяем, существует ли такой родственник
            var member = await _familyRepository.GetFamilyByIdAsync(id);
            if (member == null)
            {
                return NotFound($"Родственник с ID {id} не найден.");
            }

            // 2. Удаляем из репозитория
            await _familyRepository.DeleteFamilyMemberAsync(id);

            // 3. Возвращаем 204 No Content (успешное удаление без тела ответа)
            return NoContent();
        }

        // Вспомогательный метод маппинга (в идеале использовать AutoMapper)
        private static FamilyMemberDto MapToDto(FamilyMember m) => new FamilyMemberDto
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            DateOfBirth = m.DateOfBirth,
            Biography = m.Biography,
            Gender = m.Gender,
            ParentName = m.ParentName
        };*/
}


