using AutoMapper;
using ToDoList.Domains.DTOs;
using ToDoList.Models.Entities;

namespace ToDoList.Profiles
{
    /// <summary>
    ///  マッパープロファイル
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MappingProfile()
        {
            // Item エンティティから TodoItemDto への変換
            CreateMap<Item, TodoItemDto>();

            // List<Item> から ReadTodoPayload への変換
            CreateMap<List<Item>, ReadTodoPayload>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src));
        }
    }
}
