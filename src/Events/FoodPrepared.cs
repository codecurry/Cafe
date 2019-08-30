using System;
using System.Collections.Generic;

namespace Ddd.Events
{
    public class FoodPrepared
    {
        public Guid Id;
        public List<int> MenuNumbers;
    }
}
