using ToDoList.Domains.DTOs;
using ToDoList.Usecases;

namespace ToDoList.Controllers
{
    /// <summary>
    /// クエリクラス（GraphQL用）
    /// </summary>
    public class Query
    {
        /// <summary>
        /// アイテムを取得するメソッド
        /// </summary>
        public Task<ReadTodoPayload> Get([Service] IReadTodoUsecase usecase) => usecase.GetTodos();
    }
}
