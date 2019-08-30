using System;
using System.Collections.Generic;

namespace Ddd.Events
{
    public class DrinksOrdered
    {
        public Guid Id;
        public List<OrderedItem> Items;
    }
}
