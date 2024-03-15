using AutoMapper;
using DishesAPI.Entities;
using MinimalAPI.Models;

namespace MinimalAPI.Profiles;

public class DishProfile : Profile
{
    public DishProfile()
    {
        CreateMap<Dish, DishDto>();
        CreateMap<DishForCreationDto, Dish>();
        CreateMap<DishForUpdateDto, Dish>();
    }
}