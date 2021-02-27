using System;
using System.Collections.Generic;

namespace PMSWebApplication.Models.DomainModels
{
    public class Payment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }

        public decimal? PaymentAmount { get; set; }
        public string PayMethod { get; set; }
    }
}
