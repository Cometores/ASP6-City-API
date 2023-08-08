using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Test.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Test.Fixtures;

public class EmployeeServiceWithAspNetCoreDiFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public IEmployeeManagementRepository EmployeeManagementTestDataRepository =>
        _serviceProvider.GetService<IEmployeeManagementRepository>()!;

    public IEmployeeService EmployeeService => _serviceProvider.GetService<IEmployeeService>()!;

    public EmployeeServiceWithAspNetCoreDiFixture()
    {
        var services = new ServiceCollection();
        services.AddScoped<EmployeeFactory>();
        services.AddScoped<IEmployeeManagementRepository, EmployeeManagementTestDataRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        // build provider
        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        // clean up the setup code, if required
    }
}