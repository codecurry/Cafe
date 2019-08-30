using System;
using System.Collections.Generic;

namespace Ddd.Events
{
    public class DrinksServed
    {
        public Guid Id;
        public List<int> MenuNumbers;
    }
}
