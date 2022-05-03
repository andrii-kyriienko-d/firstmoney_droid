using FirstMoney.Models;
using SQLite;

namespace FirstMoney.Model
{
    [Table("Settings")]
    internal class Setting : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
