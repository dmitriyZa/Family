using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Family.Mobile.Views
{
    public class ModalPage : ContentPage
    {
        public ModalPage()
        {
            Title = "Modal";
            Button backButton = new Button { Text = "Back", HorizontalOptions = LayoutOptions.Start };
            Label label = new Label { Text = "Modal Message..." };
            //  переход с модальной страницы назад
            backButton.Clicked += async (o, e) => await Navigation.PopModalAsync();
            Content = new StackLayout { Children = { label, backButton } };
        }
    }
}
