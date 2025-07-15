using ToDoList.Models.Entities;

namespace ToDoList.Usecases
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
