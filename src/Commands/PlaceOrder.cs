using System;
using System.Collections.Generic;
using Events;

namespace Commands
{
    public class PlaceOrder
    {
        public Guid Id;
        public List<OrderedItem> Items;
    }
}
