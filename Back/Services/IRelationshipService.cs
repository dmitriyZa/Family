using Family.Shared;

public interface IRelationshipService
{
    // Находит обратный тип (например, для "Отец" найдет "Сын" или "Дочь")
    Task<FamilyRelationType?> GetReverseRelationAsync(int relationTypeId, Gender? gender = null);

    // Создает основную связь и, если нужно, автоматическую обратную
    Task CreateFullRelationshipAsync(RelationshipDto dto);
}

public class RelationshipService : IRelationshipService
{
    private readonly IRelationTypeRepository _relationTypeRepository;
    private readonly IRelationshipRepository _relationshipRepository;
    private readonly IFamilyRepository _familyRepository;

    public RelationshipService(
        IRelationTypeRepository relationTypeRepository,
        IRelationshipRepository relationshipRepository,
        IFamilyRepository familyRepository)
    {
        _relationTypeRepository = relationTypeRepository;
        _relationshipRepository = relationshipRepository;
        _familyRepository = familyRepository;
    }

    public async Task CreateFullRelationshipAsync(RelationshipDto dto)
    {
        // 1. Создаем прямую связь
        var mainRelationship = new Relationship
        {
            FamilyMemberId = dto.FamilyMemberId,
            RelatedMemberId = dto.RelatedMemberId,
            RelationTypeId = dto.RelationTypeId
        };
        await _relationshipRepository.AddRelationshipAsync(mainRelationship);

        // 2. Определяем пол того, КТО является субъектом связи (FamilyMemberId)
        // Это нужно, чтобы понять: он "Сын" или "Дочь" для родителя
        var member = await _familyRepository.GetFamilyByIdAsync(dto.FamilyMemberId);

        // 3. Ищем обратный тип связи
        var reverseType = await GetReverseRelationAsync(dto.RelationTypeId, member?.Gender);

        if (reverseType != null)
        {
            // 4. Создаем обратную связь
            var reverseRelationship = new Relationship
            {
                FamilyMemberId = dto.RelatedMemberId, // Меняем местами
                RelatedMemberId = dto.FamilyMemberId,
                RelationTypeId = reverseType.Id, // Обязательно передаем Id типа!
                Description = $"Auto-generated: {reverseType.DisplayName}"
            };
            await _relationshipRepository.AddRelationshipAsync(reverseRelationship);
        }
    }

    public async Task<FamilyRelationType?> GetReverseRelationAsync(int relationTypeId, Gender? gender)
    {
        var relType = await _relationTypeRepository.GetByIdAsync(relationTypeId);
        if (relType == null || relType.ReverseRelationTypeId == null)
            return null;

        // Тут можно добавить логику: 
        // если ReverseRelationTypeId ведет на общий тип "Child", 
        // а gender == Male, то вернуть тип "Son".

        return await _relationTypeRepository.GetByIdAsync(relType.ReverseRelationTypeId.Value);
    }
}


