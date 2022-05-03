using System;

namespace FirstMoney.Models.Interfaces
{
    internal interface ICommonEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
