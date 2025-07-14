namespace ToDoList.Domains.DTOs
{
    /// <summary>
    /// テストのためのItemクラス
    /// </summary>
    public class ItemDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 題名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string? Content { get; set; }
    }
}
