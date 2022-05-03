using FirstMoney.Enumerables;
using FirstMoney.Model;
using FirstMoney.Models;
using FirstMoney.Repository;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstMoney.Pages.Crud
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateOperationPage : ContentPage
    {
        private List<OperationCategory> Categories { get; set; }
        private List<Currency> Currencies { get; set; }

        private OperationCategory SelectedCategory { get; set; }
        private Currency SelectedCurrency { get; set; }

        private DbContext _dbContext { get; set; }

        private Operation NewTransaction { get; set; } = new Operation();

        private OperationTypes _operationType { get; set; }
        public CreateOperationPage(DbContext dbContext, OperationTypes operationType)
        {
            InitializeComponent();

            _dbContext = dbContext;
            _operationType = operationType;

            FillCategoryPicker();
            FillCurrencyPicker();

            TransactionTimePicker.Time = DateTime.Now.TimeOfDay;
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

        private void FillCategoryPicker()
        {
            Categories = _dbContext.Database.Table<OperationCategory>()
                .Where(x => x.OperationType == _operationType)
                .ToList();

            CategoryPicker.SetBinding(Picker.ItemsSourceProperty, "Categories");
            CategoryPicker.SetBinding(Picker.SelectedItemProperty, "SelectedCategory");
            CategoryPicker.ItemDisplayBinding = new Binding("Name");
            CategoryPicker.ItemsSource = Categories;
        }

        private void AddNewInvoceButton_Clicked(object sender, EventArgs e)
        {
            if(SelectedCategory == null)
            {
                this.DisplayAlert("", "Pick a category", "Ok");
                CategoryPickerGrid.BackgroundColor = Color.FromHex("#3d0000");
                return;
            }

            if (string.IsNullOrWhiteSpace(SummEntry.Text))
            {
                this.DisplayAlert("", "Change total value", "Ok");
                SummEntryGrid.BackgroundColor = Color.FromHex("#3d0000");
                return;
            }

            NewTransaction.CategoryId = SelectedCategory.Id;
            NewTransaction.Summ = double.Parse(SummEntry.Text) * GetCurrencyMultiplier();
            NewTransaction.OperationType = SelectedCategory.OperationType;

            CopyTimeToDateStructure();
            try
            {
                _dbContext.Database.Insert(NewTransaction);
            }catch(Exception ex)
            {
                Console.WriteLine($"ERROR:{ex.Message}\nSTACKTRACE:{ex.StackTrace}");
            }
            Navigation.PopModalAsync();
        }

        private void CopyTimeToDateStructure()
        {
            var time = TransactionTimePicker.Time;
            var date = NewTransaction.OperationDate;
            NewTransaction.OperationDate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        private void CancelAddition_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCategory = CategoryPicker.SelectedItem as OperationCategory;
            CategoryPickerGrid.BackgroundColor = Color.Transparent;
            CurrencyPicker.SelectedIndex = CurrencyPicker.Items.IndexOf(_dbContext.Database.Table<Currency>()
                .Where(x => x.Id.Equals(SelectedCategory.CurrencyId))
                .FirstOrDefault().Name);
        }

        private void SummEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            SummEntryGrid.BackgroundColor = Color.Transparent;
            try
            {
                NewTransaction.Summ = double.Parse(SummEntry.Text) * GetCurrencyMultiplier();
            }catch (Exception ex) { }
        }

        private void NotesEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewTransaction.Notes = NotesEntry.Text;
        }

        private double GetCurrencyMultiplier()
        {
            return _dbContext.Database.Table<Currency>()
               .Where(x => x.Name.Equals(SelectedCurrency.Name))
               .FirstOrDefault().UsdExchangeRate;
        }

        private void CurrencyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCurrency = CurrencyPicker.SelectedItem as Currency;

            NewTransaction.Summ = NewTransaction.Summ * GetCurrencyMultiplier();
        }
    }
}