using System;

namespace Events
{
    public class TableClosed
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; }
        public string Waiter { get; set; }
        public decimal AmountPaid;
        public decimal OrderValue;
        public decimal TipValue;
        
    }
}
