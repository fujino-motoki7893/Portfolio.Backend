using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.ToDoList.Models.Entities;

namespace ToDoList.Backend.ToDoList.Models.DbContexts
{
    /// <summary>
    /// ApplicationDbContextクラス
    /// </summary>
    /// <remarks>
    /// コンストラクタ
    /// </remarks>
    /// <param name="options">DbContextOptions</param>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        /// <summary>
        /// DbSet<Items>プロパティ
        /// </summary>
        public DbSet<Item> Items { get; set; } = default!;
    }
}
