window.applyRussianMenu = () => {
    const checkInterval = setInterval(() => {
        // Проверяем, загрузилась ли библиотека
        if (typeof FamilyTree !== 'undefined') {
            clearInterval(checkInterval); // Перестаем ждать

            // 1. Устанавливаем русские фразы глобально
            FamilyTree.resLabels.ru = {
                addMother: "Добавить мать",
                addFather: "Добавить отца",
                addPartner: "Добавить супруга(у)",
                addChild: "Добавить ребенка",
                addSon: "Добавить сына",
                addDaughter: "Добавить дочь",
                edit: "Редактировать",
                details: "Подробности",
                remove: "Удалить"
            };

            // 2. Настраиваем обработчик меню для перевода динамических элементов
            FamilyTree.on('render-menu', function (sender, args) {
                if (args.menu) {
                    if (args.menu.mother) args.menu.mother.text = "Добавить мать";
                    if (args.menu.father) args.menu.father.text = "Добавить отца";
                }
            });

            console.log("Локализация FamilyTree успешно применена");
        }
    }, 100); // Проверка каждые 100мс

    // Остановить попытки через 10 секунд, если библиотека не загрузится
    setTimeout(() => clearInterval(checkInterval), 10000);
};

