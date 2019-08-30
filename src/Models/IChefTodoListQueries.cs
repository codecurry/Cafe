using System;
using System.Collections.Generic;

namespace Ddd.Models
{
    public interface IChefTodoListQueries
    {
        List<ChefTodoList.TodoListGroup> GetTodoList();
    }
}
