using ToDoList.Infrastructures.Interface;
using ToDoList.Models.DbContexts;
using ToDoList.Models.Entities;

namespace ToDoList.Infrastructures
{
    public class ReadItemRepository(ApplicationDbContext context) : IReadItemRepository
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// OperationLog エンティティのクエリを取得
        /// </summary>
        /// <returns>OperationLog の IQueryable</returns>
        public virtual IQueryable<Item> Entities()
        {
            return _context.Set<Item>();
        }
    }
}
