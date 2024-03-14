using AutoMapper;
using DishesAPI.Entities;
using MinimalAPI.Models;

namespace MinimalAPI.Profiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(
                d => d.DishId,
                o => o.MapFrom(s => s.Dishes.First().Id));
    }
}