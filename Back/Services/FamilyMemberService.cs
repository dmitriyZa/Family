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
            ParentName = m.ParentName,
            DateOfBirth = m.DateOfBirth,
            Gender = m.Gender,
            Biography = m.Biography,
            PhotoUrl = m.Photo,
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

    public async Task<FamilyMember> GetFamilyMember(int id) => await _familyRepository.GetFamilyByIdAsync(id);
    public async Task UpdateMemberAsync(FamilyMember member) => await _familyRepository.UpdateFamilyMemberAsync(member);

    public async Task<FamilyMemberDto> AddMemberAsync(FamilyMemberDto dto)
    {

        var newMember = new FamilyMember
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ParentName = dto.ParentName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Biography = dto.Biography,
            Photo = dto.PhotoUrl
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

        if (dto.SpouseIds != null && dto.SpouseIds.Any())
        {
            foreach (var spouseId in dto.SpouseIds)
            {
                await _relationshipRepository.AddRelationshipAsync(new Relationship
                {
                    FamilyMemberId = newMember.Id,
                    RelatedMemberId = spouseId,
                    RelationTypeId = 3 // Супруг
                });

                // Для FamilyTreeJS важно создать и обратную связь!
                await _relationshipRepository.AddRelationshipAsync(new Relationship
                {
                    FamilyMemberId = spouseId,
                    RelatedMemberId = newMember.Id,
                    RelationTypeId = 3
                });
            }
        }

        dto.Id = newMember.Id;
        return dto;
    }
    public async Task UpdateMemberAsync(int id, FamilyMemberDto dto)
    {
        // 1. Обновляем основные данные человека
        var member = await _familyRepository.GetFamilyByIdAsync(id);
        if (member == null) return;

        member.FirstName = dto.FirstName;
        member.LastName = dto.LastName;
        member.ParentName = dto.ParentName;
        member.DateOfBirth = dto.DateOfBirth;
        member.Gender = dto.Gender;
        member.Biography = dto.Biography;
        if (!string.IsNullOrEmpty(dto.PhotoUrl))
        {
            member.Photo = dto.PhotoUrl;
        }

        await _familyRepository.UpdateFamilyMemberAsync(member);

        // 2. Очищаем старые связи, где этот человек был СУБЪЕКТОМ
        await _relationshipRepository.DeleteRelationshipsBySubjectIdAsync(id);

        // Загружаем все оставшиеся связи для проверки на дубликаты в памяти
        var allRemainingRelations = await _relationshipRepository.GetAllRelationshipsAsync();

        // 3. Записываем новые связи из DTO
        if (dto.FatherId.HasValue && dto.FatherId.Value != 0)
        {
            await _relationshipRepository.AddRelationshipAsync(new Relationship
            { FamilyMemberId = id, RelatedMemberId = dto.FatherId.Value, RelationTypeId = 1 });
        }

        if (dto.MotherId.HasValue && dto.MotherId.Value != 0)
        {
            await _relationshipRepository.AddRelationshipAsync(new Relationship
            { FamilyMemberId = id, RelatedMemberId = dto.MotherId.Value, RelationTypeId = 2 });
        }

        if (dto.SpouseIds != null)
        {
            foreach (var spouseId in dto.SpouseIds)
            {
                if (spouseId == 0 || spouseId == id) continue;

                // Прямая связь (ток если её еще нет)
                if (!allRemainingRelations.Any(r => r.FamilyMemberId == id && r.RelatedMemberId == spouseId && r.RelationTypeId == 3))
                {
                    await _relationshipRepository.AddRelationshipAsync(new Relationship
                    { FamilyMemberId = id, RelatedMemberId = spouseId, RelationTypeId = 3 });
                }

                // Обратная зеркальная связь (ток если её еще нет)
                if (!allRemainingRelations.Any(r => r.FamilyMemberId == spouseId && r.RelatedMemberId == id && r.RelationTypeId == 3))
                {
                    await _relationshipRepository.AddRelationshipAsync(new Relationship
                    { FamilyMemberId = spouseId, RelatedMemberId = id, RelationTypeId = 3 });
                }
            }
        }


    }

    public async Task DeleteMemberAsync(int id)
    {
        var allRelations = await _relationshipRepository.GetAllRelationshipsAsync();
        var relationsToDelete = allRelations
            .Where(r => r.FamilyMemberId == id || r.RelatedMemberId == id)
            .ToList();

        foreach (var rel in relationsToDelete)
        {
            // Передаем первичный ключ самой связи rel.Id, а не id человека!
            await _relationshipRepository.DeleteRelationshipAsync(rel.Id);
        }

        await _familyRepository.DeleteFamilyMemberAsync(id);
    }



}