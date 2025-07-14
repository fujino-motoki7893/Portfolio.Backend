using System.ComponentModel.DataAnnotations;

namespace ToDoList.Backend.ToDoList.Models.Entities
{
    /// <summary>
    /// テストのためのItemクラス
    /// </summary>
    public class Item
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 題名
        /// </summary>
        [StringLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [StringLength(5000)]
        public string? Content { get; set; }
    }
}
