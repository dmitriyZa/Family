using Microsoft.EntityFrameworkCore;

public interface IRelationshipRepository
{
    Task<Relationship> GetRelationshipByIdAsync(int id);
    Task<List<Relationship>> GetRelationshipsByMemberIdAsync(int memberId);
    Task AddRelationshipAsync(Relationship relationship);
    Task DeleteRelationshipAsync(int id);
}

public class RelationshipRepository : IRelationshipRepository
{
    private readonly ApplicationDbContext _context;

    public RelationshipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Relationship> GetRelationshipByIdAsync(int id)
    {
        return await _context.Relationships
            .Include(r => r.FamilyMember)
            .Include(r => r.RelatedMember)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Relationship>> GetRelationshipsByMemberIdAsync(int memberId)
    {
        return await _context.Relationships
            .Include(r => r.FamilyMember)
            .Include(r => r.RelatedMember)
            .Where(r => r.FamilyMemberId == memberId || r.RelatedMemberId == memberId)
            .ToListAsync();
    }

    public async Task AddRelationshipAsync(Relationship relationship)
    {
        _context.Relationships.Add(relationship);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRelationshipAsync(int id)
    {
        var rel = await _context.Relationships.FindAsync(id);
        if (rel != null)
        {
            _context.Relationships.Remove(rel);
            await _context.SaveChangesAsync();
        }
    }
}

