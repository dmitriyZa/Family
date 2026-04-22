using Blazor.FamilyTreeJS.Components.Interop.Options;
using Family.Shared;
using WebFamily.Models;
using BTJS = Blazor.FamilyTreeJS.Components.Interop.Options.Gender; // Алиас для библиотеки
using MyGender = Family.Shared.Gender;    // Алиас для твоего Enum

namespace WebFamily.Services
{
    public class FamilyNodeFactory
    {
        public CustomNode CreateNode(FamilyMemberDto dto)
        {
            return new CustomNode
            {
                Id = dto.Id.ToString(),
                Name = $"{dto.FirstName} {dto.LastName}",
                FatherId =dto.FatherId?.ToString(),
                MotherId = dto.MotherId?.ToString(),             
                
                PartnerIds = dto.SpouseIds != null ? new[] { dto.SpouseIds.ToString() } : null,                
                Gender = dto.Gender == MyGender.Male ? BTJS.Male : BTJS.Female,
                Biography = dto.Biography,
                BirthDate = dto.DateOfBirth.ToString("dd.MM.yyyy")
            };
        }

        public List<CustomNode> CreateTree(IEnumerable<FamilyMemberDto> dtos, List<RelationshipDto> relations)
        {
            return dtos.Select(CreateNode).ToList();
        }
    }

}
