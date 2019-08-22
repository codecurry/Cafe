using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commands
{
    public class TableNotOpen : Exception
    {
    }

    public class DrinksNotOutstanding : Exception
    {
    }

    public class FoodNotOutstanding : Exception
    {
    }

    public class FoodNotPrepared : Exception
    {
    }

    public class MustPayEnough : Exception
    {
    }

    public class TableHasUnservedItems : Exception
    {
    }
}
