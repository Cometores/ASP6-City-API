using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Test.Services;
using Moq;

namespace EmployeeManagement.Test;

public class MoqTests
{
    [Fact]
    public void FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculated()
    {
        // Arrange
        var employeeManagementTestDataRepository = new EmployeeManagementTestDataRepository();
        // var employeeFactory = new EmployeeFactory();
        var employeeFactoryMock = new Mock<EmployeeFactory>();
        var employeeService = new EmployeeService(employeeManagementTestDataRepository, employeeFactoryMock.Object);

        // Act 
        var employee = employeeService.FetchInternalEmployee(Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb"));

        // Assert
        Assert.Equal(400, employee.SuggestedBonus);
    }

    [Fact]
    public void CreateInternalEmployee_InternalEmployeeCreated_SuggestedBonusMustBeCalculated()
    {
        // Arrange
        var employeeManagementTestDataRepository = new EmployeeManagementTestDataRepository();
        var employeeFactoryMock = new Mock<EmployeeFactory>();
        employeeFactoryMock.Setup(m =>
                m.CreateEmployee(
                    "Andrey",
                    It.IsAny<string>(),
                    null,
                    false))
            .Returns(new InternalEmployee("Andrey", "Cometores", 5, 2500, false, 1));
        var employeeService = new EmployeeService(employeeManagementTestDataRepository, employeeFactoryMock.Object);

        // suggested bonus for new employees = 
        // (years in service if > 0) * attended courses * 100
        decimal suggestedBonus = 1000;

        // Act
        var employee = employeeService.CreateInternalEmployee("Andrey", "Cometores");

        // Assert
        Assert.Equal(suggestedBonus, employee.SuggestedBonus);
    }

    [Fact]
    public void FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculated_MoqInterface()
    {
        // Arrange
        var employeeManagementTestDataRepositoryMock = new Mock<IEmployeeManagementRepository>();
        employeeManagementTestDataRepositoryMock
            .Setup(m => m.GetInternalEmployee(It.IsAny<Guid>()))
            .Returns(new InternalEmployee("Tony", "Hall", 2, 2500, false, 2)
            {
                AttendedCourses = new List<Course>()
                {
                    new Course("A course"), new Course("Another course")
                }
            });


        var employeeFactoryMock = new Mock<EmployeeFactory>();
        var employeeService =
            new EmployeeService(employeeManagementTestDataRepositoryMock.Object, employeeFactoryMock.Object);

        // Act
        var employee = employeeService.FetchInternalEmployee(Guid.Empty);

        // Assert
        Assert.Equal(400, employee.SuggestedBonus);
    }
    
    [Fact]
    public async Task FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculated_MoqInterface_Async()
    {
        // Arrange
        var employeeManagementTestDataRepositoryMock = new Mock<IEmployeeManagementRepository>();
        employeeManagementTestDataRepositoryMock
            .Setup(m => m.GetInternalEmployeeAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new InternalEmployee("Tony", "Hall", 2, 2500, false, 2)
            {
                AttendedCourses = new List<Course>()
                {
                    new Course("A course"), new Course("Another course")
                }
            });


        var employeeFactoryMock = new Mock<EmployeeFactory>();
        var employeeService =
            new EmployeeService(employeeManagementTestDataRepositoryMock.Object, employeeFactoryMock.Object);

        // Act
        var employee = await employeeService.FetchInternalEmployeeAsync(Guid.Empty);

        // Assert
        Assert.Equal(400, employee.SuggestedBonus);
    }
}