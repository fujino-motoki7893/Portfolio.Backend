using ToDoList.Models.DbContexts;
using ToDoList.Models.Entities;

namespace ToDoList.Controllers
{
    /// <summary>
    /// クエリクラス
    /// </summary>
    public class Query
    {
        /// <summary>
        /// アイテムを取得するメソッド
        /// </summary>
        public IQueryable<Item> GetItems([Service] ApplicationDbContext context) =>
            context.Items;
    }
}
