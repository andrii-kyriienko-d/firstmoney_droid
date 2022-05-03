using FirstMoney.Enumerables;
using FirstMoney.Model;
using FirstMoney.Models;
using FirstMoney.Pages.Crud;
using FirstMoney.Repository;

namespace FirstMoney
{
    public partial class MainPage : ContentPage
    {
        private readonly DbContext _dbContext = new DbContext();

        private OperationTypes _currentTransactionType = OperationTypes.Outgoing;

        private List<Currency> Currencies { get; set; }
        private Currency SelectedCurrency { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _dbContext.Initialize();
        }

        private void InitPage()
        {
            var dbTransactions = _dbContext.GetTransactionsWithCategory(_currentTransactionType);
            FillCurrencyPicker();
            ShowChart(dbTransactions);
        }

        private void FillCurrencyPicker()
        {
            Currencies = _dbContext.Database.Table<Currency>().ToList();

            CurrencyPicker.SetBinding(Picker.ItemsSourceProperty, "Currencies");
            CurrencyPicker.SetBinding(Picker.SelectedItemProperty, "SelectedCurrency");
            CurrencyPicker.ItemDisplayBinding = new Binding("Name");
            CurrencyPicker.ItemsSource = Currencies;
            CurrencyPicker.SelectedIndex = Currencies
                .IndexOf(
                    Currencies
                    .Where(x => x.Id == _dbContext.GetDefaultCurrencyId())
                    .FirstOrDefault()
                );
            SelectedCurrency = CurrencyPicker.SelectedItem as Currency;
        }

        private void ShowChart(Dictionary<string, double> dbTransactions)
        {
            var entries = new List<ChartEntry>();
            var step = 0;
            var labelColor = SkiaSharp.SKColor.FromHsv(step, 100, 70);
            if (dbTransactions.Count < 0)
            {
                dbTransactions.Add("Empty", 100);
            }
            foreach (var entry in dbTransactions)
            {
                if(SelectedCurrency is null)
                {
                    SelectedCurrency = _dbContext.Database.Table<Currency>()
                        .ToList()
                        .FirstOrDefault(x => x.Id == _dbContext.GetDefaultCurrencyId());
                }
                var newValue = (float)entry.Value / (float)SelectedCurrency.UsdExchangeRate;
                entries.Add(new ChartEntry(newValue)
                {
                    Label = entry.Key,
                    TextColor = SkiaSharp.SKColors.White,
                    ValueLabel = newValue.ToString(),
                    ValueLabelColor = labelColor,
                    Color = labelColor
                });
                step += (int)359 / (int)dbTransactions.Count;

                labelColor = SkiaSharp.SKColor.FromHsv(step, 100, 70);
            }

            var donutChart = new DonutChart();

            donutChart.BackgroundColor = SkiaSharp.SKColor.FromHsl(0, 0, 12);
            donutChart.LabelColor = SkiaSharp.SKColors.White;
            donutChart.LabelTextSize = 42;
            donutChart.LabelMode = LabelMode.RightOnly;

            donutChart.Entries = entries;

            UpdateTotalMoney();
           
            chartView.Chart = donutChart;
        }

        private void UpdateTotalMoney()
        {
            TotalMoneyLabel.Text = Math.Round((_dbContext.Database.Table<Operation>()
               .Where(x => x.OperationType == OperationTypes.Incoming)
               .Sum(x => x.Summ) -
               _dbContext.Database.Table<Operation>()
               .Where(x => x.OperationType == OperationTypes.Outgoing)
               .Sum(x => x.Summ)) / (float)SelectedCurrency.UsdExchangeRate).ToString();
        }

        private void AddTransactionButton_Clicked(object sender, EventArgs e)
        {
            var createTransactionPage = new CreateOperationPage(_dbContext, _currentTransactionType);
            Navigation.PushModalAsync(createTransactionPage);
        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            _currentTransactionType = _currentTransactionType == OperationTypes.Incoming ? OperationTypes.Outgoing : OperationTypes.Incoming;
            var dbTransactions = _dbContext.GetTransactionsWithCategory(_currentTransactionType);
            ShowChart(dbTransactions);
        }

        private void CurrencyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCurrency = CurrencyPicker.SelectedItem as Currency;

            var dbTransactions = _dbContext.GetTransactionsWithCategory(_currentTransactionType);
            chartView.Chart = new DonutChart();
            ShowChart(dbTransactions);
        }

        private void HomePage_Appearing(object sender, EventArgs e)
        {
            InitPage();
        }

        private void OnSwipedToList(object sender, SwipedEventArgs e)
        {
           
        }
    }
}
