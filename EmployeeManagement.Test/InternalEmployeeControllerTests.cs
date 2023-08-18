using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.Test;

public class InternalEmployeeControllerTests
{
    private readonly InternalEmployeesController _internalEmployeesController;
    private readonly InternalEmployee _firstEmployee;

    public InternalEmployeeControllerTests()
    {
        _firstEmployee = new InternalEmployee("Megan", "Jones", 2, 3000, false, 2)
        {
            Id = Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb"),
            SuggestedBonus = 400
        };

        var employeeServiceMock = new Mock<IEmployeeService>();
        employeeServiceMock
            .Setup(m => m.FetchInternalEmployeesAsync())
            .ReturnsAsync(new List<InternalEmployee>()
            {
                _firstEmployee,
                new InternalEmployee("Jaimy", "Johnson", 3, 3400, true, 1),
                new InternalEmployee("Megan", "Adams", 3, 4000, false, 3),
            });

        // var mapperMock = new Mock<IMapper>();
        // mapperMock.Setup(m => m.Map<InternalEmployee, Models.InternalEmployeeDto>
        //         (It.IsAny<InternalEmployee>()))
        //     .Returns(new InternalEmployeeDto());
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles.EmployeeProfile>());
        var mapper = new Mapper(mapperConfiguration);

        _internalEmployeesController = new InternalEmployeesController(employeeServiceMock.Object, mapper);
    }

    [Fact]
    public async Task GetInternalEmployees_GetAction_MustReturnOkObjectResult()
    {
        // Act
        var result = await _internalEmployeesController.GetInternalEmployees();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
        Assert.IsType<OkObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task GetInternalEmployees_GetAction_MustReturnIEnumerableOfInternalEmployeeDtoAsModelType()
    {
        // Act
        var result = await _internalEmployeesController.GetInternalEmployees();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
        Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>(((OkObjectResult)actionResult.Result).Value);
    }

    [Fact]
    public async Task GetInternalEmployees_GetAction_MustReturnNumberOfInputtedInternalEmployees()
    {
        // Act
        var result = await _internalEmployeesController.GetInternalEmployees();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
        Assert.Equal(3, ((IEnumerable<InternalEmployeeDto>)((OkObjectResult)actionResult.Result).Value).Count());
    }

    [Fact]
    public async Task GetInternalEmployees_GetAction_ReturnsOkObjectResultWithCorrectAmountOfInternalEmployees()
    {
        // Act 
        var result = await _internalEmployeesController.GetInternalEmployees();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>(okObjectResult.Value);
        Assert.Equal(3, dtos.Count());
        var firstEmployee = dtos.First();
        Assert.Equal(_firstEmployee.Id, firstEmployee.Id);
        Assert.Equal(_firstEmployee.FirstName, firstEmployee.FirstName);
        Assert.Equal(_firstEmployee.LastName, firstEmployee.LastName);
        Assert.Equal(_firstEmployee.Salary, firstEmployee.Salary);
        Assert.Equal(_firstEmployee.SuggestedBonus, firstEmployee.SuggestedBonus);
        Assert.Equal(_firstEmployee.YearsInService, firstEmployee.YearsInService);
    }
}