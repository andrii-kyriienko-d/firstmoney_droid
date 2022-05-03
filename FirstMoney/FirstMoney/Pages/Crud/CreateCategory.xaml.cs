using FirstMoney.Model;
using FirstMoney.Repository;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstMoney.Pages.Crud
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateCategory : ContentPage
    {
        private OperationCategory NewCategory { get; set; } = new OperationCategory();

        private List<Currency> Currencies { get; set; }
        private Currency SelectedCurrency { get; set; }

        private DbContext _dbContext { get; set; }

        public CreateCategory(DbContext dbContext)
        {
            InitializeComponent();

            _dbContext = dbContext;

            FillCurrencyPicker();
        }

        private void FillCurrencyPicker()
        {
            Currencies = _dbContext.Database.Table<Currency>().ToList();

            CurrencyPicker.SetBinding(Picker.ItemsSourceProperty, "Currencies");
            CurrencyPicker.SetBinding(Picker.SelectedItemProperty, "SelectedCurrency");
            CurrencyPicker.ItemDisplayBinding = new Binding("Name");
            CurrencyPicker.ItemsSource = Currencies;
            CurrencyPicker.SelectedIndex = 0;
        }

        private void AddNewCategoryButton_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewCategory.Name))
            {
                this.DisplayAlert("", "Fill the name", "Ok");
                return;
            }

            Navigation.PopModalAsync();
        }

        private void CancelAddition_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameEntryGrid.BackgroundColor = Color.Transparent;
        }

        private void CurrencyPicker_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SelectedCurrency = CurrencyPicker.SelectedItem as Currency;
            NewCategory.CurrencyId = SelectedCurrency.Id;
        }
    }
}