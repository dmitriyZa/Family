using Family.Shared;

public interface IRelationshipService
{
    //todo здесь наверняка будут еще хендлеры ...
    Task<FamilyRelationType?> GetReverseRelationAsync(int relationTypeId, Gender? gender = null);
}

public class RelationshipService : IRelationshipService
{
    private readonly IRelationTypeRepository _relationTypeRepository;
    public RelationshipService(IRelationTypeRepository relationTypeRepository) =>
        _relationTypeRepository = relationTypeRepository;

    public async Task<FamilyRelationType?> GetReverseRelationAsync(int relationTypeId, Gender? gender)
    {
        var relType = await _relationTypeRepository.GetByIdAsync(relationTypeId);
        if (relType == null || relType.ReverseRelationTypeId == null)
            return null;

        var reverseType = await _relationTypeRepository.GetByIdAsync(relType.ReverseRelationTypeId.Value);
        return reverseType;
    }
}

