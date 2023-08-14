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

    public InternalEmployeeControllerTests()
    {
        var employeeServiceMock = new Mock<IEmployeeService>();
        employeeServiceMock
            .Setup(m => m.FetchInternalEmployeesAsync())
            .ReturnsAsync(new List<InternalEmployee>()
            {
                new InternalEmployee("Megan", "Jones", 2, 3000, false, 2),
                new InternalEmployee("Jaimy", "Johnson", 3, 3400, true, 1),
                new InternalEmployee("Megan", "Adams", 3, 4000, false, 3),
            });

        _internalEmployeesController = new InternalEmployeesController(employeeServiceMock.Object, null);
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
    }
}