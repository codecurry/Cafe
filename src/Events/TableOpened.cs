using System;

namespace Events
{
    public class TableOpened
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; }
        public string Waiter { get; set; }
        
    }
}
