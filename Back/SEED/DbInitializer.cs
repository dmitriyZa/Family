using Family.Shared;
using Microsoft.EntityFrameworkCore;


public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        if (await context.FamilyMembers.AnyAsync<FamilyMember>()) return;

        // 1. Создаем список ТИПОВ СВЯЗЕЙ без обратных ID (ставим null)
        var relTypes = new List<FamilyRelationType>
    {
        new FamilyRelationType { Id = 1, Code = "FATHER", DisplayName = "Отец", ReverseRelationTypeId = null },
        new FamilyRelationType { Id = 2, Code = "MOTHER", DisplayName = "Мать", ReverseRelationTypeId = null },
        new FamilyRelationType { Id = 3, Code = "SPOUSE", DisplayName = "Супруг(а)", ReverseRelationTypeId = null },
        new FamilyRelationType { Id = 4, Code = "SON", DisplayName = "Сын", ReverseRelationTypeId = null },
        new FamilyRelationType { Id = 5, Code = "DAUGHTER", DisplayName = "Дочь", ReverseRelationTypeId = null }
    };

        await context.FamilyRelationTypes.AddRangeAsync(relTypes);
        await context.SaveChangesAsync(); // Сохраняем "каркас"

        // 2. Теперь обновляем обратные связи, когда все записи уже в базе
        var father = relTypes.First(x => x.Id == 1); father.ReverseRelationTypeId = 4;
        var mother = relTypes.First(x => x.Id == 2); mother.ReverseRelationTypeId = 5;
        var spouse = relTypes.First(x => x.Id == 3); spouse.ReverseRelationTypeId = 3;
        var son = relTypes.First(x => x.Id == 4); son.ReverseRelationTypeId = 1;
        var daughter = relTypes.First(x => x.Id == 5); daughter.ReverseRelationTypeId = 2;

        await context.SaveChangesAsync(); // Обновляем связи

        // 3. Добавляем людей
        var members = new List<FamilyMember>
    {
        new FamilyMember { Id = 1, FirstName = "Иван", LastName = "Иванов", ParentName = "", Gender = Gender.Male, DateOfBirth = new DateTime(1960, 5, 10) },
        new FamilyMember { Id = 2, FirstName = "Мария", LastName = "Иванова", ParentName = "", Gender = Gender.Female, DateOfBirth = new DateTime(1965, 8, 20) },
        new FamilyMember { Id = 3, FirstName = "Петр", LastName = "Иванов", ParentName = "", Gender = Gender.Male, DateOfBirth = new DateTime(1990, 1, 15) }
    };
        await context.FamilyMembers.AddRangeAsync(members);
        await context.SaveChangesAsync();

        // 4. Добавляем отношения
        var relations = new List<Relationship>
    {
        new Relationship { FamilyMemberId = 3, RelatedMemberId = 1, RelationTypeId = 1 },
        new Relationship { FamilyMemberId = 3, RelatedMemberId = 2, RelationTypeId = 2 },
        new Relationship { FamilyMemberId = 1, RelatedMemberId = 2, RelationTypeId = 3 },
        new Relationship { FamilyMemberId = 2, RelatedMemberId = 1, RelationTypeId = 3 }
    };
        await context.Relationships.AddRangeAsync(relations);
        await context.SaveChangesAsync();
    }

}
