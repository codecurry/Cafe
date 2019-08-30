using System;
using System.Collections.Generic;

namespace Ddd.Models
{
    public interface IOpenTabQueries
    {
        List<string> ActiveTableNumbers();
        OpenTabs.TableInvoice InvoiceForTable(string table);
        Guid TabIdForTable(string table);
        OpenTabs.TableStatus TabForTable(string table);
        Dictionary<string, List<OpenTabs.Orderitem>> TodoListForWaiter(string waiter);
    }
}
