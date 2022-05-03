using FirstMoney.Enumerables;
using FirstMoney.Models;
using System;

namespace FirstMoney.Model
{
    internal class OperationCategory : BaseEntity
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public OperationTypes OperationType { get; set; }
        public Guid CurrencyId { get; set; }
    }
}
