using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test;

public class EmployeeFactoryTests
{
    [Fact]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500()
    {
        var employeeFactory = new EmployeeFactory();

        var employee = (InternalEmployee) employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.Equal(2500, employee.Salary);
    }
    
    [Fact]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500and3500()
    {
        var employeeFactory = new EmployeeFactory();

        var employee = (InternalEmployee) employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.True(employee.Salary >= 2500 && employee.Salary <= 3500,
            "Salary not in acceptable range.");
    }
    
    [Fact]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500and3500_Alternative()
    {
        var employeeFactory = new EmployeeFactory();

        var employee = (InternalEmployee) employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.True(employee.Salary >= 2500);
        Assert.True(employee.Salary <= 3500);
    }
    
    [Fact]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500and3500_AlternativeWithInRange()
    {
        var employeeFactory = new EmployeeFactory();

        var employee = (InternalEmployee) employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.InRange(employee.Salary, 2500, 3500);
    }
    
    [Fact]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500_PrecisionExample()
    {
        var employeeFactory = new EmployeeFactory();

        var employee = (InternalEmployee) employeeFactory.CreateEmployee("Andrey", "Cometores");
        employee.Salary = 2500.123m;
        
        Assert.Equal(2500, employee.Salary, 0);
    }

    [Fact]
    public void CreateEmployee_IsExternalIsTrue_ReturnTypeMustBeExternalEmployee()
    {
        // Arrange
        var factory = new EmployeeFactory();
        
        // Act
        var employee = factory.CreateEmployee("Andrey", "Cometores", "Marvin", true);
        
        // Assert
        Assert.IsType<ExternalEmployee>(employee);
        // Assert.IsAssignableFrom<Employee>(employee);
    }
}