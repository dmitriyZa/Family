using Microsoft.EntityFrameworkCore;

public interface IRelationTypeRepository
{
    Task<FamilyRelationType> GetByIdAsync(int id);
    Task<List<FamilyRelationTypeDto>> GetAllRelationTypes();
}
public class RelationTypeRepository : IRelationTypeRepository
{
    private readonly ApplicationDbContext _context;
    public RelationTypeRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<FamilyRelationTypeDto>> GetAllRelationTypes()
    {
        var types = await _context.FamilyRelationTypes.ToListAsync();
        var dtos = types.Select(t => new FamilyRelationTypeDto
        {
            Id = t.Id,
            Code = t.Code,
            DisplayName = t.DisplayName,
            ReverseRelationTypeId = t.ReverseRelationTypeId,
            Emoji = t.Emoji,
            Description = t.Description,
            BaseType = t.BaseType
        }).ToList();
        return dtos;
    }

    public async Task<FamilyRelationType?> GetByIdAsync(int id) =>
        await _context.FamilyRelationTypes.FindAsync(id);
}
