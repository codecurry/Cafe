using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ddd.Common;
using Ddd.Events;

namespace Ddd.Commands
{
    public class TableAggregate : Aggregate,
        IHandleCommand<OpenTable>,
        IHandleCommand<PlaceOrder>,
        IHandleCommand<MarkDrinksServed>,
        IHandleCommand<MarkFoodPrepared>,
        IHandleCommand<MarkFoodServed>,
        IHandleCommand<CloseTable>,
        IApplyEvent<TableOpened>,
        IApplyEvent<DrinksOrdered>,
        IApplyEvent<FoodOrdered>,
        IApplyEvent<DrinksServed>,
        IApplyEvent<FoodPrepared>,
        IApplyEvent<FoodServed>,
        IApplyEvent<TableClosed>
    {
        private List<OrderedItem> outstandingDrinks = new List<OrderedItem>();
        private List<OrderedItem> outstandingFood = new List<OrderedItem>();
        private List<OrderedItem> preparedFood = new List<OrderedItem>();
        private bool open;
        private decimal servedItemsValue;

        public IEnumerable Handle(OpenTable c)
        {
            yield return new TableOpened
            {
                Id = c.Id,
                TableNumber = c.TableNumber,
                Waiter = c.Waiter
            };
        }

        public IEnumerable Handle(PlaceOrder c)
        {
            if (!open)
                throw new TableNotOpen();

            var drink = c.Items.Where(i => i.IsDrink).ToList();
            if (drink.Any())
                yield return new DrinksOrdered
                {
                    Id = c.Id,
                    Items = drink
                };

            var food = c.Items.Where(i => !i.IsDrink).ToList();
            if (food.Any())
                yield return new FoodOrdered
                {
                    Id = c.Id,
                    Items = food
                };
        }

        public IEnumerable Handle(MarkDrinksServed c)
        {
            if (!AreDrinksOutstanding(c.MenuNumbers))
                throw new DrinksNotOutstanding();

            yield return new DrinksServed
            {
                Id = c.Id,
                MenuNumbers = c.MenuNumbers
            };
        }

        public IEnumerable Handle(MarkFoodPrepared c)
        {
            if (!IsFoodOutstanding(c.MenuNumbers))
                throw new FoodNotOutstanding();

            yield return new FoodPrepared
            {
                Id = c.Id,
                MenuNumbers = c.MenuNumbers
            };
        }

        public IEnumerable Handle(MarkFoodServed c)
        {
            if (!IsFoodPrepared(c.MenuNumbers))
                throw new FoodNotPrepared();

            yield return new FoodServed
            {
                Id = c.Id,
                MenuNumbers = c.MenuNumbers
            };
        }

        public IEnumerable Handle(CloseTable c)
        {
            if (!open)
                throw new TableNotOpen();
            if (HasUnservedItems())
                throw new TableHasUnservedItems();
            if (c.AmountPaid < servedItemsValue)
                throw new MustPayEnough();

            yield return new TableClosed
            {
                Id = c.Id,
                AmountPaid = c.AmountPaid,
                OrderValue = servedItemsValue,
                TipValue = c.AmountPaid - servedItemsValue
            };
        }

        private bool AreDrinksOutstanding(List<int> menuNumbers)
        {
            return AreAllInList(want: menuNumbers, have: outstandingDrinks);
        }

        private bool IsFoodOutstanding(List<int> menuNumbers)
        {
            return AreAllInList(want: menuNumbers, have: outstandingFood);
        }

        private bool IsFoodPrepared(List<int> menuNumbers)
        {
            return AreAllInList(want: menuNumbers, have: preparedFood);
        }

        private static bool AreAllInList(List<int> want, List<OrderedItem> have)
        {
            var curHave = new List<int>(have.Select(i => i.MenuNumber));
            foreach (var num in want)
                if (curHave.Contains(num))
                    curHave.Remove(num);
                else
                    return false;
            return true;
        }

        public bool HasUnservedItems()
        {
            return outstandingDrinks.Any() || outstandingFood.Any() || preparedFood.Any();
        }

        public void Apply(TableOpened e)
        {
            open = true;
        }

        public void Apply(DrinksOrdered e)
        {
            outstandingDrinks.AddRange(e.Items);
        }

        public void Apply(FoodOrdered e)
        {
            outstandingFood.AddRange(e.Items);
        }

        public void Apply(DrinksServed e)
        {
            foreach (var num in e.MenuNumbers)
            {
                var item = outstandingDrinks.First(d => d.MenuNumber == num);
                outstandingDrinks.Remove(item);
                servedItemsValue += item.Price;
            }
        }

        public void Apply(FoodPrepared e)
        {
            foreach (var num in e.MenuNumbers)
            {
                var item = outstandingFood.First(f => f.MenuNumber == num);
                outstandingFood.Remove(item);
                preparedFood.Add(item);
            }
        }

        public void Apply(FoodServed e)
        {
            foreach (var num in e.MenuNumbers)
            {
                var item = preparedFood.First(f => f.MenuNumber == num);
                preparedFood.Remove(item);
                servedItemsValue += item.Price;
            }
        }

        public void Apply(TableClosed e)
        {
            open = false;
        }
    }
}
