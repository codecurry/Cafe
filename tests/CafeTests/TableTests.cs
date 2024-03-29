using System;
using System.Collections.Generic;
using Ddd.Events;
using Xunit;
using Ddd.Commands;

namespace CafeTests
{
    public class TableTests : BddTests<TableAggregate>
    {
        private Guid testId;
        private string testTable;
        private string testWaiter;
        private OrderedItem testDrink1;
        private OrderedItem testDrink2;
        private OrderedItem testFood1;
        private OrderedItem testFood2;

        public TableTests()
        {
            testId = Guid.NewGuid();
            testTable = "42";
            testWaiter = "Derek";

            testDrink1 = new OrderedItem
            {
                MenuNumber = 4,
                Description = "Sprite",
                Price = 1.50M,
                IsDrink = true
            };
            testDrink2 = new OrderedItem
            {
                MenuNumber = 10,
                Description = "Beer",
                Price = 2.50M,
                IsDrink = true
            };

            testFood1 = new OrderedItem
            {
                MenuNumber = 16,
                Description = "Beef Noodles",
                Price = 7.50M,
                IsDrink = false
            };
            testFood2 = new OrderedItem
            {
                MenuNumber = 25,
                Description = "Vegetable Curry",
                Price = 6.00M,
                IsDrink = false
            };
        }

        [Fact]
        public void CanOpenANewTab()
        {
            Test(
                Given(),
                When(new OpenTable
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                Then(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }));
        }

        [Fact]
        public void CanNotOrderWithUnopenedTab()
        {
            Test(
                Given(),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1 }
                }),
                ThenFailWith<TableNotOpen>());
        }

        [Fact]
        public void CanPlaceDrinksOrder()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1, testDrink2 }
                }),
                Then(new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1, testDrink2 }
                }));
        }

        [Fact]
        public void CanPlaceFoodOrder()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood1 }
                }),
                Then(new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood1 }
                }));
        }

        [Fact]
        public void CanPlaceFoodAndDrinkOrder()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testDrink2 }
                }),
                Then(new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink2 }
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1 }
                }));
        }

        [Fact]
        public void OrderedDrinksCanBeServed()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1, testDrink2 }
                }),
                When(new MarkDrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink1.MenuNumber, testDrink2.MenuNumber }
                }),
                Then(new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink1.MenuNumber, testDrink2.MenuNumber }
                }));
        }

        [Fact]
        public void CanNotServeAnUnorderedDrink()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1 }
                }),
                When(new MarkDrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink2.MenuNumber }
                }),
                ThenFailWith<DrinksNotOutstanding>());
        }

        [Fact]
        public void CanNotServeAnOrderedDrinkTwice()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1 }
                },
                new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink1.MenuNumber }
                }),
                When(new MarkDrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink1.MenuNumber }
                }),
                ThenFailWith<DrinksNotOutstanding>());
        }

        [Fact]
        public void OrderedFoodCanBeMarkedPrepared()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood1 }
                }),
                When(new MarkFoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber, testFood1.MenuNumber }
                }),
                Then(new FoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber, testFood1.MenuNumber }
                }));
        }

        [Fact]
        public void FoodNotOrderedCanNotBeMarkedPrepared()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new MarkFoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber }
                }),
                ThenFailWith<FoodNotOutstanding>());
        }

        [Fact]
        public void CanNotMarkFoodAsPreparedTwice()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood1 }
                },
                new FoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber, testFood1.MenuNumber }
                }),
                When(new MarkFoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber }
                }),
                ThenFailWith<FoodNotOutstanding>());
        }

        [Fact]
        public void CanServePreparedFood()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood2 }
                },
                new FoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber, testFood2.MenuNumber }
                }),
                When(new MarkFoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber, testFood1.MenuNumber }
                }),
                Then(new FoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber, testFood1.MenuNumber }
                }));
        }

        [Fact]
        public void CanNotServePreparedFoodTwice()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood2 }
                },
                new FoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber, testFood2.MenuNumber }
                },
                new FoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber, testFood1.MenuNumber }
                }),
                When(new MarkFoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber, testFood1.MenuNumber }
                }),
                ThenFailWith<FoodNotPrepared>());
        }

        [Fact]
        public void CanNotServeUnorderedFood()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1 }
                }),
                When(new MarkFoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber }
                }),
                ThenFailWith<FoodNotPrepared>());
        }

        [Fact]
        public void CanNotServeOrderedButUnpreparedFood()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1 }
                }),
                When(new MarkFoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber }
                }),
                ThenFailWith<FoodNotPrepared>());
        }

        [Fact]
        public void CanCloseTableByPayingExactAmount()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood2 }
                },
                new FoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber, testFood2.MenuNumber }
                },
                new FoodServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood2.MenuNumber, testFood1.MenuNumber }
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testFood1.Price + testFood2.Price
                }),
                Then(new TableClosed
                {
                    Id = testId,
                    AmountPaid = testFood1.Price + testFood2.Price,
                    OrderValue = testFood1.Price + testFood2.Price,
                    TipValue = 0.00M
                }));
        }

        [Fact]
        public void CanCloseTableWithTip()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink2 }
                },
                new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink2.MenuNumber }
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testDrink2.Price + 0.50M
                }),
                Then(new TableClosed
                {
                    Id = testId,
                    AmountPaid = testDrink2.Price + 0.50M,
                    OrderValue = testDrink2.Price,
                    TipValue = 0.50M
                }));
        }

        [Fact]
        public void MustPayEnoughToCloseTable()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink2 }
                },
                new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink2.MenuNumber }
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testDrink2.Price - 0.50M
                }),
                ThenFailWith<MustPayEnough>());
        }

        [Fact]
        public void CanNotCloseTableTwice()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink2 }
                },
                new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink2.MenuNumber }
                },
                new TableClosed
                {
                    Id = testId,
                    AmountPaid = testDrink2.Price + 0.50M,
                    OrderValue = testDrink2.Price,
                    TipValue = 0.50M
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testDrink2.Price
                }),
                ThenFailWith<TableNotOpen>());
        }

        [Fact]
        public void CanNotCloseTableWithUnservedDrinksItems()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink2 }
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testDrink2.Price
                }),
                ThenFailWith<TableHasUnservedItems>());
        }

        [Fact]
        public void CanNotCloseTableWithUnpreparedFoodItems()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1 }
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testFood1.Price
                }),
                ThenFailWith<TableHasUnservedItems>());
        }

        [Fact]
        public void CanNotCloseTableWithUnservedFoodItems()
        {
            Test(
                Given(new TableOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1 }
                },
                new FoodPrepared
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testFood1.MenuNumber }
                }),
                When(new CloseTable
                {
                    Id = testId,
                    AmountPaid = testFood1.Price
                }),
                ThenFailWith<TableHasUnservedItems>());
        }
    }
}
