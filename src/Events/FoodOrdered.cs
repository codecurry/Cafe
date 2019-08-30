using System;
using System.Collections.Generic;

namespace Ddd.Events
{
    public class FoodOrdered
    {
        public Guid Id;
        public List<OrderedItem> Items;
    }
}
