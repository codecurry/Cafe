using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ddd.Events;
using Ddd.Common;

namespace Ddd.Models
{
    public class OpenTabs : IOpenTabQueries,
        ISubscribeTo<TableOpened>,
        ISubscribeTo<DrinksOrdered>,
        ISubscribeTo<FoodOrdered>,
        ISubscribeTo<FoodPrepared>,
        ISubscribeTo<DrinksServed>,
        ISubscribeTo<FoodServed>,
        ISubscribeTo<TableClosed>
    {
        public class Orderitem
        {
            public int MenuNumber;
            public string Description;
            public decimal Price;
        }

        public class TableStatus
        {
            public Guid TableId;
            public string TableNumber;
            public List<Orderitem> ToServe;
            public List<Orderitem> InPreparation;
            public List<Orderitem> Served;
        }

        public class TableInvoice
        {
            public Guid TableId;
            public string TableNumber;
            public List<Orderitem> Items;
            public decimal Total;
            public bool HasUnservedItems;
        }

        private class Table
        {
            public string TableNumber;
            public string Waiter;
            public List<Orderitem> ToServe;
            public List<Orderitem> InPreparation;
            public List<Orderitem> Served;
        }

        private Dictionary<Guid, Table> todoByTab =
            new Dictionary<Guid,Table>();

        public List<string> ActiveTableNumbers()
        {
            lock (todoByTab)
                return (from tab in todoByTab
                        select tab.Value.TableNumber
                       ).OrderBy(i => i).ToList();
        }

        public Dictionary<string, List<Orderitem>> TodoListForWaiter(string waiter)
        {
            lock (todoByTab)
                return (from tab in todoByTab
                        where tab.Value.Waiter == waiter
                        select new
                        {
                            TableNumber = tab.Value.TableNumber,
                            ToServe = CopyItems(tab.Value, t => t.ToServe)
                        })
                        .Where(t => t.ToServe.Count > 0)
                        .ToDictionary(k => k.TableNumber, v => v.ToServe);
        }

        public Guid TabIdForTable(string table)
        {
            lock (todoByTab)
                return (from tab in todoByTab
                        where tab.Value.TableNumber == table
                        select tab.Key
                       ).First();
        }

        public TableStatus TabForTable(string table)
        {
            lock (todoByTab)
                return (from tab in todoByTab
                        where tab.Value.TableNumber == table
                        select new TableStatus
                        {
                            TableId = tab.Key,
                            TableNumber = tab.Value.TableNumber,
                            ToServe = CopyItems(tab.Value, t => t.ToServe),
                            InPreparation = CopyItems(tab.Value, t => t.InPreparation),
                            Served = CopyItems(tab.Value, t => t.Served)
                        })
                        .First();
        }

        public TableInvoice InvoiceForTable(string table)
        {
            KeyValuePair<Guid, Table> tab;
            lock (todoByTab)
                tab = todoByTab.First(t => t.Value.TableNumber == table);

            lock (tab.Value)
                return new TableInvoice
                {
                    TableId = tab.Key,
                    TableNumber = tab.Value.TableNumber,
                    Items = new List<Orderitem>(tab.Value.Served),
                    Total = tab.Value.Served.Sum(i => i.Price),
                    HasUnservedItems = tab.Value.InPreparation.Any() || tab.Value.ToServe.Any()
                };
        }

        private List<Orderitem> CopyItems(Table tableTodo, Func<Table, List<Orderitem>> selector)
        {
            lock (tableTodo)
                return new List<Orderitem>(selector(tableTodo));
        }

        public void Handle(TableOpened e)
        {
            lock (todoByTab)
                todoByTab.Add(e.Id, new Table
                {
                    TableNumber = e.TableNumber,
                    Waiter = e.Waiter,
                    ToServe = new List<Orderitem>(),
                    InPreparation = new List<Orderitem>(),
                    Served = new List<Orderitem>()
                });
        }

        public void Handle(DrinksOrdered e)
        {
            AddItems(e.Id,
                e.Items.Select(drink => new Orderitem
                    {
                        MenuNumber = drink.MenuNumber,
                        Description = drink.Description,
                        Price = drink.Price
                    }),
                t => t.ToServe);
        }

        public void Handle(FoodOrdered e)
        {
            AddItems(e.Id,
                e.Items.Select(drink => new Orderitem
                {
                    MenuNumber = drink.MenuNumber,
                    Description = drink.Description,
                    Price = drink.Price
                }),
                t => t.InPreparation);
        }

        public void Handle(FoodPrepared e)
        {
            MoveItems(e.Id, e.MenuNumbers, t => t.InPreparation, t => t.ToServe);
        }

        public void Handle(DrinksServed e)
        {
            MoveItems(e.Id, e.MenuNumbers, t => t.ToServe, t => t.Served);
        }

        public void Handle(FoodServed e)
        {
            MoveItems(e.Id, e.MenuNumbers, t => t.ToServe, t => t.Served);
        }

        public void Handle(TableClosed e)
        {
            lock (todoByTab)
                todoByTab.Remove(e.Id);
        }

        private Table getTab(Guid id)
        {
            lock (todoByTab)
                return todoByTab[id];
        }

        private void AddItems(Guid tabId, IEnumerable<Orderitem> newItems, Func<Table, List<Orderitem>> to)
        {
            var tab = getTab(tabId);
            lock (tab)
                to(tab).AddRange(newItems);
        }

        private void MoveItems(Guid tabId, List<int> menuNumbers,
            Func<Table, List<Orderitem>> from, Func<Table, List<Orderitem>> to)
        {
            var tab = getTab(tabId);
            lock (tab)
            {
                var fromList = from(tab);
                var toList = to(tab);
                foreach (var num in menuNumbers)
                {
                    var serveItem = fromList.First(f => f.MenuNumber == num);
                    fromList.Remove(serveItem);
                    toList.Add(serveItem);
                }
            }
        }
    }
}
