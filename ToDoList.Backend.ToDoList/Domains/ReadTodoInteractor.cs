using AutoMapper;
using ToDoList.Domains.DTOs;
using ToDoList.Infrastructures.Interface;
using ToDoList.Usecases;

namespace ToDoList.Domains
{
    /// <summary>
    ///  ToDo を取得するためのインタラクタ
    /// </summary>
    public class ReadTodoInteractor(IReadItemRepository repository, IMapper mapper) : IReadTodoUsecase
    {
        private readonly IReadItemRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        ///  ToDo を取得する
        /// </summary>
        /// <returns>ToDo </returns>
        public Task<ReadTodoPayload> GetTodos()
        {
            var result = _repository.Entities();
            var todoItems = _mapper.Map<List<TodoItemDto>>(result);
            return Task.FromResult(new ReadTodoPayload
            {
                Items = todoItems
            });
        }
    }
}
