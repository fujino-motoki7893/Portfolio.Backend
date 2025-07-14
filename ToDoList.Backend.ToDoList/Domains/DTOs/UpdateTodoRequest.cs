using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Backend.ToDoList.Domains.DTOs
{
    /// <summary>
    ///  UpdateTodoRequestクラス
    /// </summary>
    public class UpdateTodoRequest
    {
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
