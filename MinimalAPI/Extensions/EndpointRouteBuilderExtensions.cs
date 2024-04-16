using MinimalAPI.EndpointFilters;
using MinimalAPI.EndpointHandlers;
using MinimalAPI.Models;

namespace MinimalAPI.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var dishesEndpoints = endpointRouteBuilder.MapGroup("/dishes")
            .RequireAuthorization();
        var dishWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");
        var dishWithGuidIdEndpointsAndLockFilters = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}")
            .RequireAuthorization()
            .AddEndpointFilter(new DishIsLockedFilter(new("fd630a57-2352-4731-b25c-db9cc7601b16")));

        dishesEndpoints.MapGet("", DishesHandlers.GetDishesAsync);
        dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync)
            .WithName("GetDish")
            .WithOpenApi()
            .WithSummary("Get a dish by providing an id.")
            .WithDescription("Dishes are identified by a URI containing a dish identifier. This identifier is a GUID.");
        dishesEndpoints.MapGet("/{dishName}", DishesHandlers.GetDishByNameAsync)
            .AllowAnonymous()
            .WithOpenApi(operation =>
            {
                operation.Deprecated = true;
                return operation;
            });
        dishesEndpoints.MapPost("", DishesHandlers.CreateDishAsync)
            .RequireAuthorization("RequireAdminFromGermany")
            .AddEndpointFilter<ValidateAnnotationsFilter>()
            .ProducesValidationProblem(400)
            .Accepts<DishForCreationDto>(
                "application/json",
                "application/vnd.marvin.dishforcreation+json");
        dishWithGuidIdEndpointsAndLockFilters.MapPut("", DishesHandlers.UpdateDishAsync);
        dishWithGuidIdEndpointsAndLockFilters.MapDelete("", DishesHandlers.DeleteDishAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
    }

    public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientsEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients")
            .RequireAuthorization();

        ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
        ingredientsEndpoints.MapPost("", () => { throw new NotImplementedException(); });
    }
}