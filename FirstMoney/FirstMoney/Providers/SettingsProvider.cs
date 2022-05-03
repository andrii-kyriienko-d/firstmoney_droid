using FirstMoney.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstMoney.Providers
{
    public class SettingsProvider
    {
        private List<Setting> Settings = new List<Setting>();
        private SQLiteConnection _connection;
        public SettingsProvider(SQLiteConnection db)
        {
            _connection = db;
            _connection.CreateTable<Setting>();
            if(_connection.Table<Setting>().Count() == 0)
            {
                //add here default configs
                var defaultSettings = new Setting[]
                {
                    new Setting
                    {
                        Name = "DefaultCurrency",
                        Value = "USD"
                    }
                };
                _connection.InsertAll(defaultSettings);
            }

            Settings = _connection.Table<Setting>().ToList();
        }

        public string GetValue(string key)
        {
            return Settings.FirstOrDefault(x => x.Name == key).Value;
        }

        public bool UpdateSetting(string key, string value)
        {
            if(!(_connection is null))
            {
                var setting = _connection.Table<Setting>().Where(x => x.Name == key).FirstOrDefault();
                if (setting != null)
                {
                    setting.Value = value;
                    setting.UpdatedDate = DateTime.Now;
                    return false;
                }
                Settings = _connection.Table<Setting>().ToList();
            }

            return true;
        }
    }
}
