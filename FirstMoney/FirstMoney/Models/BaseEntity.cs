using FirstMoney.Models.Interfaces;
using System;

namespace FirstMoney.Models
{
    internal class BaseEntity : ICommonEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
