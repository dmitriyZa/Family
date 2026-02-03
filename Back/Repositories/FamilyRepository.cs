using Microsoft.EntityFrameworkCore;

public interface IFamilyRepository
{
    Task<FamilyMember> GetFamilyByIdAsync(int id);
    Task AddFamilyMemberAsync(FamilyMember familyMember);
    Task<List<FamilyMember>> GetAllFamilyMembersAsync();
}

public class FamilyRepository : IFamilyRepository
{
    private readonly ApplicationDbContext _context;

    public FamilyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FamilyMember> GetFamilyByIdAsync(int id)
    {
        return await _context.FamilyMembers.FindAsync(id);
    }

    public async Task<List<FamilyMember>> GetAllFamilyMembersAsync()
    {
        return await _context.FamilyMembers.ToListAsync();
    }

    public async Task AddFamilyMemberAsync(FamilyMember familyMember)
    {
        _context.FamilyMembers.Add(familyMember);
        await _context.SaveChangesAsync();
    }
}
