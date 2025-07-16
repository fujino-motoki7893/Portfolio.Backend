using Microsoft.EntityFrameworkCore;
using ToDoList.Models.DbContexts;
using ToDoList.Usecases;
using AutoMapper;
using ToDoList.Domains.DTOs;

namespace ToDoList.Domains
{
    /// <summary>
    ///  ToDo を取得するためのインタラクタ
    /// </summary>
    public class ReadItemInteractor(ApplicationDbContext context, IMapper mapper) : IReadItemUsecase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        ///  ToDo を取得する
        /// </summary>
        /// <returns>ToDo </returns>
        public async Task<ReadTodoPayload> GetTodos()
        {
            var result = await _context.Items.ToListAsync();
            var todoItems = _mapper.Map<List<TodoItemDto>>(result);
            return new ReadTodoPayload 
            { 
                Items = todoItems 
            };
        }
    }
}
