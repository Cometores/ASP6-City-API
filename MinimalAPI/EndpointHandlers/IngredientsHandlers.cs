using AutoMapper;
using DishesAPI.DbContexts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.EndpointHandlers;

public class IngredientsHandlers
{
    public static async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> GetIngredientsAsync(
        DishesDbContext dishesDbContext,
        IMapper mapper,
        Guid dishId)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes
            .Include(d => d.Ingredients)
            .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients));
    }
}