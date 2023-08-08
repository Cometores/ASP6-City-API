using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test;

/* Implements Construct and Dispose "sharing context" feature */
public class EmployeeFactoryTests : IDisposable
{
    private EmployeeFactory _employeeFactory;

    public EmployeeFactoryTests()
    {
        _employeeFactory = new EmployeeFactory();
    }
    
    public void Dispose()
    {
        // clean up the setup code, if required
    }
    
    [Fact(Skip = "Skipping for exploring functionality")]
    [Trait("Category", "EmployeeFactory_CreateEmployee_Salary")]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500()
    {
        var employee = (InternalEmployee) _employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.Equal(2500, employee.Salary);
    }
    
    [Fact]
    [Trait("Category", "EmployeeFactory_CreateEmployee_Salary")]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500and3500()
    {
        var employee = (InternalEmployee) _employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.True(employee.Salary >= 2500 && employee.Salary <= 3500,
            "Salary not in acceptable range.");
    }
    
    [Fact]
    [Trait("Category", "EmployeeFactory_CreateEmployee_Salary")]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500and3500_Alternative()
    {
        var employee = (InternalEmployee) _employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.True(employee.Salary >= 2500);
        Assert.True(employee.Salary <= 3500);
    }
    
    [Fact]
    [Trait("Category", "EmployeeFactory_CreateEmployee_Salary")]
    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500and3500_AlternativeWithInRange()
    {
        var employee = (InternalEmployee) _employeeFactory.CreateEmployee("Andrey", "Cometores");
        
        Assert.InRange(employee.Salary, 2500, 3500);
    }
    
    [Fact]
    [Trait("Category", "EmployeeFactory_CreateEmployee_Salary")]

    public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500_PrecisionExample()
    {
        var employee = (InternalEmployee) _employeeFactory.CreateEmployee("Andrey", "Cometores");
        employee.Salary = 2500.123m;
        
        Assert.Equal(2500, employee.Salary, 0);
    }

    [Fact]
    [Trait("Category", "EmployeeFactory_CreateEmployee_ReturnType")]
    public void CreateEmployee_IsExternalIsTrue_ReturnTypeMustBeExternalEmployee()
    {
        // Act
        var employee = _employeeFactory.CreateEmployee("Andrey", "Cometores", "Marvin", true);
        
        // Assert
        Assert.IsType<ExternalEmployee>(employee);
        // Assert.IsAssignableFrom<Employee>(employee);
    }
}