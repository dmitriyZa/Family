using Family.Shared;

public class FamilyMemberService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IRelationshipRepository _relationshipRepository;

    public FamilyMemberService(
        IFamilyRepository familyRepository,
        IRelationshipRepository relationshipRepository)
    {
        _familyRepository = familyRepository;
        _relationshipRepository = relationshipRepository;
    }

    public async Task<List<FamilyMemberDto>> GetFamilyTreeAsync()
    {
        // 1. Загружаем всех людей
        var members = await _familyRepository.GetAllFamilyMembersAsync();
        // 2. Загружаем ВСЕ связи
        var relations = await _relationshipRepository.GetAllRelationshipsAsync();
        // 3. Собираем DTO
        var dtos = members.Select(m => new FamilyMemberDto
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            DateOfBirth = m.DateOfBirth,
            Gender = m.Gender,
            Biography = m.Biography,

            // Мапим отца (ищем связь, где текущий человек - субъект, а тип связи - 1 (Отец))
            // Предположим Id 1 - Отец, 2 - Мать, 3 - Супруг
            FatherId = relations.FirstOrDefault(r => r.FamilyMemberId == m.Id && r.RelationTypeId == 1)?.RelatedMemberId,

            // Мапим мать
            MotherId = relations.FirstOrDefault(r => r.FamilyMemberId == m.Id && r.RelationTypeId == 2)?.RelatedMemberId,

            // Мапим список супругов
            SpouseIds = relations.Where(r => r.FamilyMemberId == m.Id && r.RelationTypeId == 3)
                                 .Select(r => r.RelatedMemberId)
                                 .ToList()
        }).ToList();

        return dtos;
    }

    public async Task<FamilyMemberDto> AddMemberAsync(FamilyMemberDto dto)
    {
        // 1. Создаем сущность для БД (убедись, что тип FamilyMember соответствует твоей Entity)
        var newMember = new FamilyMember
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Biography = dto.Biography
        };

        // 2. Сохраняем основного человека через репозиторий
        await _familyRepository.AddFamilyMemberAsync(newMember);
        // После сохранения у newMember появится Id

        // 3. Если в DTO переданы ID родителей или супругов, создаем связи
        if (dto.FatherId.HasValue)
        {
            await _relationshipRepository.AddRelationshipAsync(new Relationship
            {
                FamilyMemberId = newMember.Id,
                RelatedMemberId = dto.FatherId.Value,
                RelationTypeId = 1 // Отец
            });
        }

        if (dto.MotherId.HasValue)
        {
            await _relationshipRepository.AddRelationshipAsync(new Relationship
            {
                FamilyMemberId = newMember.Id,
                RelatedMemberId = dto.MotherId.Value,
                RelationTypeId = 2 // Мать
            });
        }

        // 4. Возвращаем созданный объект (обновляем ID из базы)
        dto.Id = newMember.Id;
        return dto;
    }
}