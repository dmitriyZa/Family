public interface IRelationTypeRepository
{
    Task<FamilyRelationType?> GetByIdAsync(int id);
}
public class RelationTypeRepository : IRelationTypeRepository
{
    private readonly ApplicationDbContext _context;
    public RelationTypeRepository(ApplicationDbContext context) => _context = context;

    public async Task<FamilyRelationType?> GetByIdAsync(int id) =>
        await _context.FamilyRelationTypes.FindAsync(id);
}
