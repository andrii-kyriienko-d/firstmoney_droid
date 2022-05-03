using FirstMoney.Enumerables;
using SQLite;
using System;

namespace FirstMoney.Models
{
    [Table("Operations")]
    internal class Operation : BaseEntity
    {
        public DateTime OperationDate { get; set; } = DateTime.Now;
        public double Summ { get; set; }
        public string Notes { get; set; }
        public Guid CategoryId { get; set; }
        public OperationTypes OperationType { get; set; } = OperationTypes.Incoming;
    }
}
