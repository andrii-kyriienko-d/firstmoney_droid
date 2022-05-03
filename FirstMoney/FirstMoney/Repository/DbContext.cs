using FirstMoney.Enumerables;
using FirstMoney.Model;
using FirstMoney.Models;
using FirstMoney.Providers;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FirstMoney.Repository
{
    public class DbContext
    {
        public SQLiteConnection Database { get; set; }
        private SettingsProvider _settingsProvider { get; set; }

        public void Initialize()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "firstmoney.db3");
            Database = new SQLiteConnection(dbPath);
            //disable after debug
            DropDatabase();
            
            _settingsProvider = new SettingsProvider(Database);

            InsertTables(Database);   
        }

        private void DropDatabase()
        {
            Database.DropTable<Currency>();
            Database.DropTable<OperationCategory>();
            Database.DropTable<Operation>();
            Database.DropTable<Setting>();
        }

        public Guid GetDefaultCurrencyId()
        {
            return Database.Table<Currency>()
                .ToList()
                .Where(x => x.Name == _settingsProvider.GetValue(nameof(SettingsEnum.DefaultCurrency)))
                .FirstOrDefault().Id;
        }

        public Dictionary<string,double> GetTransactionsWithCategory(OperationTypes operationType = OperationTypes.Outgoing)
        {
            var result = new Dictionary<string,double>();
            
            if(Database != null)
            {
                var invoices = Database.Table<Operation>().ToList();
                var categories = Database.Table<OperationCategory>().Where(x => x.OperationType == operationType).ToList();
                foreach(var category in categories)
                {
                    var typedInvoices = invoices
                        .Where(x => x.CategoryId == category.Id)
                        .Sum(x => x.Summ);
                    result.Add(category.Name, typedInvoices);
                }
            }

            return result;
        }

        private void InsertTables(SQLiteConnection db)
        {
            InsertCurrencies(db);
            InsertExpensesCategories(db);
            InsertInvoices(db);
        }

        private void InsertInvoices(SQLiteConnection db)
        {
            db.CreateTable<Operation>(CreateFlags.ImplicitPK);

            var invoices = new Operation[]
            {
                new Operation() { 
                    OperationType = OperationTypes.Outgoing,
                    Summ = 100.56,
                    CategoryId = db.Table<OperationCategory>().ElementAt(0).Id, 
                    Notes = "I'll got some water",
                },
                 new Operation() {
                    OperationType = OperationTypes.Outgoing,
                    Summ = 212.11,
                    CategoryId = db.Table<OperationCategory>().ElementAt(2).Id,
                    Notes = "Bought products in ATB, then went home",
                },
            };

            db.InsertAll(invoices);
        }

        private void InsertExpensesCategories(SQLiteConnection db)
        {
            db.CreateTable<OperationCategory>(CreateFlags.ImplicitPK);

            if(db.Table<OperationCategory>().Count() == 0)
            {
                var defaultCurrency = db.Table<Currency>().ToList()
                        .FirstOrDefault(x => x.Name == _settingsProvider.GetValue(nameof(SettingsEnum.DefaultCurrency))).Id;
                var defaultCategories = new OperationCategory[]
                {
                    new OperationCategory
                    {
                        Name = "Products",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory
                    {
                        Name = "Cafe",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory
                    {
                        Name = "Leisure",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory
                    {
                        Name = "Transport",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory
                    {
                        Name = "Health",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory
                    {
                        Name = "Shopping",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory
                    {
                        Name = "Gifts",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Outgoing
                    },
                    new OperationCategory {
                        Name = "Salary",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Incoming
                    },
                    new OperationCategory {
                        Name = "Other income",
                        ImageName = "default",
                        CurrencyId = defaultCurrency,
                        OperationType = OperationTypes.Incoming
                    }
                };
                db.InsertAll(defaultCategories);
            }
        }

        private void InsertCurrencies(SQLiteConnection db)
        {
            db.CreateTable<Currency>(CreateFlags.ImplicitPK);

            if (db.Table<Currency>().Count() == 0)
            {
                var defaultCurrencies = new Currency[] {
                    new Currency {
                        Name ="USD",
                        UpdatedDate = DateTime.Now,
                        UsdExchangeRate = 1
                    },
                    new Currency {
                        Name ="EUR",
                        UpdatedDate = DateTime.Now,
                        UsdExchangeRate = 1.03
                    },
                    new Currency {
                        Name ="UAH",
                        UpdatedDate = DateTime.Now,
                        UsdExchangeRate = 0.033
                    }
                };
                db.InsertAll(defaultCurrencies);
            }
        }
    }
}
