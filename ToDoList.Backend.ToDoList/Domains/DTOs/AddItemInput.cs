namespace ToDoList.Backend.ToDoList.Domains.DTOs
{
    /// <summary>
    ///  AddItemInputクラス
    /// </summary>
    public class AddItemInput
    {
        /// <summary>
        /// 題名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
