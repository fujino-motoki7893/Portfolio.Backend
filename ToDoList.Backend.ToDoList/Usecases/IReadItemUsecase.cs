using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Backend.ToDoList.Models.Entities;

namespace ToDoList.Backend.ToDoList.Usecases
{
    /// <summary>
    ///  ToDo を取得するためのユースケース
    /// </summary>
    public interface IReadItemUsecase
    {
        /// <summary>
        ///  ToDo を取得する
        /// </summary>
        public Task<List<Item>> GetTodos();
    }
}
