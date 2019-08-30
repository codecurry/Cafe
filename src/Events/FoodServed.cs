using System;
using System.Collections.Generic;
namespace Ddd.Events
{
    public class FoodServed
    {
        public Guid Id;
        public List<int> MenuNumbers;
    }
}
