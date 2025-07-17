using ToDoList.Models.Entities;

namespace ToDoList.Infrastructures.Interface
{
    public interface IReadItemRepository
    {
        /// <summary>
        /// IQueryableを取得
        /// </summary>
        /// <returns>操作ログのIQueryable</returns>
        IQueryable<Item> Entities();
    }
}
