using System;

namespace Ddd.Commands
{
    public class OpenTable
    {
        public Guid Id;
        public string TableNumber { get; set; }
        public string Waiter { get; set; }
        
    }
}
