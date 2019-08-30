using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ddd.Commands;
using Ddd.Common;
using Ddd.Models;

namespace Ddd.Api
{
    public static class Domain
    {
        public static MessageDispatcher Dispatcher;
        public static IOpenTabQueries OpenTabQueries;
        public static IChefTodoListQueries ChefTodoListQueries;

        public static void Setup()
        {
            Dispatcher = new MessageDispatcher(new InMemoryEventStore());
            
            Dispatcher.ScanInstance(new TableAggregate());

            OpenTabQueries = new OpenTabs();
            Dispatcher.ScanInstance(OpenTabQueries);

            ChefTodoListQueries = new ChefTodoList();
            Dispatcher.ScanInstance(ChefTodoListQueries);
        }
    }
}