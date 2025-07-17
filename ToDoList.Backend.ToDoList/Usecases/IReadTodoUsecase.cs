using ToDoList.Domains.DTOs;

namespace ToDoList.Usecases
{
    public interface IReadTodoUsecase
    {
        /// <summary>
        ///  ToDo を取得する
        /// </summary>
        Task<ReadTodoPayload> GetTodos();
    }
}
