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
            var baseAddress = "http://localhost:5274/";
            string datesDisplay = dto.DateOfBirth.ToString("dd.MM.yyyy");
            if (dto.DateOfDeath.HasValue)
            {
                datesDisplay += $" — {dto.DateOfDeath.Value.ToString("dd.MM.yyyy")}";
            }
            return new CustomNode
            {

                Id = dto.Id.ToString(),
                Name = $"{dto.LastName} {dto.FirstName} {dto.ParentName}",
                MotherId = dto.FatherId?.ToString(),
                FatherId = dto.MotherId?.ToString(),

                PartnerIds = dto.SpouseIds?.Select(id => id.ToString()).ToArray(),
                Gender = dto.Gender == MyGender.Male ? BTJS.Male : BTJS.Female,
                Occupation = dto.Occupation,
                Biography = dto.Biography,
                BirthDate = datesDisplay,
                Photo = string.IsNullOrEmpty(dto.PhotoUrl)
                ? null
                : (dto.PhotoUrl.StartsWith("http") ? dto.PhotoUrl : baseAddress + dto.PhotoUrl),
            };
        }

        public List<CustomNode> CreateTree(IEnumerable<FamilyMemberDto> dtos)
        {
            return dtos.Select(CreateNode).ToList();
        }
    }

}
