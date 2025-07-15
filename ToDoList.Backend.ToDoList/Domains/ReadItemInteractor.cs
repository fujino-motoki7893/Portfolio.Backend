using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.ToDoList.Models.DbContexts;
using ToDoList.Backend.ToDoList.Usecases;
using ToDoList.Backend.ToDoList.Models.Entities;

namespace ToDoList.Backend.ToDoList.Domains
{
    /// <summary>
    ///  ToDo を取得するためのインタラクタ
    /// </summary>
    public class ReadItemInteractor(ApplicationDbContext context) : IReadItemUsecase
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        ///  ToDo を取得する
        /// </summary>
        /// <returns>ToDo </returns>
        public async Task<List<Item>> GetTodos()
        {
            return await _context.Items.ToListAsync();
        }
    }
}
