using Family.Shared;
using WebFamily.Models;
using BTJS = Blazor.FamilyTreeJS.Components.Interop.Options.Gender;
using MyGender = Family.Shared.Gender;

namespace WebFamily.Services
{
    public class FamilyMappingService
    {
        /// <summary>
        /// Преобразует ноду из JS-библиотеки обратно в DTO для отправки на бэкенд
        /// </summary>
        public FamilyMemberDto MapNodeToDto(CustomNode node)
        {
            var names = node.Name?.Split(' ') ?? new string[] { "", "" };

            return new FamilyMemberDto
            {
                Id = int.TryParse(node.Id, out var id) ? id : 0,
                FirstName = names.ElementAtOrDefault(0) ?? "",
                LastName = names.ElementAtOrDefault(1) ?? "",
                ParentName = names.ElementAtOrDefault(2) ?? "",

                // Конвертация пола: из типа библиотеки в твой Enum
                Gender = node.Gender == BTJS.Male ? MyGender.Male : MyGender.Female,

                Biography = node.Biography,

                // Парсим дату (убедись, что формат совпадает с тем, что в фабрике)
                DateOfBirth = DateTime.TryParseExact(node.BirthDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date)
                              ? date : DateTime.Today,

                // Связи (если библиотека их возвращает в ноде)
                FatherId = int.TryParse(node.FatherId, out var fId) ? fId : null,
                MotherId = int.TryParse(node.MotherId, out var mId) ? mId : null
            };
        }
    }
}

