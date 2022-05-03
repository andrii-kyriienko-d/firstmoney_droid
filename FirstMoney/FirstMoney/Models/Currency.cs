using FirstMoney.Models;

namespace FirstMoney.Model
{
    internal class Currency : BaseEntity
    {
        public string Name { get; set; }
        public double UsdExchangeRate { get; set; }
    }
}
