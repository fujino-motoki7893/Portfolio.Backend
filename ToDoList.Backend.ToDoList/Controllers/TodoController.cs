using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.ToDoList.Models.DbContexts;
using ToDoList.Backend.ToDoList.Models.Entities;
using ToDoList.Backend.ToDoList.Domains.DTOs;

namespace ToDoList.Backend.ToDoList.Controllers
{
    /// <summary>
    ///  ToDoList のコントローラクラス
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        ///  ToDo を取得する
        /// </summary>
        /// <returns>ToDo </returns>
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var items = await _context.Items.ToListAsync();
            return Ok(items);
        }

        /// <summary>
        ///  Id を指定してToDo を取得する
        /// </summary>
        /// <param name="id">Item のId</param>
        /// <returns>取得した結果</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        /// <summary>
        ///  ToDo を作成する
        /// </summary>
        /// <param name="request">Item を作成するためのrequest</param>
        /// <returns>指定されたToDoを取得した結果 </returns>
        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request)
        {
            var item = new Item
            {
                Name = request.Name,
                Content = request.Content
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodo), new { id = item.Id }, item);
        }

        /// <summary>
        ///  ToDo を更新する
        /// </summary>
        /// <param name="id">更新するItem のid </param>
        /// <param name="request">更新するItem のリクエスト</param>
        /// <returns>更新した結果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            item.Name = request.Name;
            item.Content = request.Content;

            await _context.SaveChangesAsync();
            return Ok(item);
        }

        /// <summary>
        ///  ToDo を削除する
        /// </summary>
        /// <param name="id">削除するItem のid </param>
        /// <returns>削除した結果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
