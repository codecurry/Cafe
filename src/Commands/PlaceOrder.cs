using System;
using System.Collections.Generic;
using Ddd.Events;

namespace Ddd.Commands
{
    public class PlaceOrder
    {
        public Guid Id;
        public List<OrderedItem> Items;
    }
}
