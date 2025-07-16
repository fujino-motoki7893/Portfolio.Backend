namespace ToDoList.Domains.DTOs
{
    /// <summary>
    /// Todo を取得するためのペイロード
    /// </summary>
    public class ReadTodoPayload
    {
        /// <summary>
        /// Todo を取得するためのレスポンス
        /// </summary>
        public List<TodoItemDto> Items { get; set; } = [];
    }
}
