using FastEndpoints;

namespace Example.Application.Employees;

public sealed class EmployeesGroup : Group
{
    public EmployeesGroup()
    {
        Configure("employees", ep =>
        {
            // Configuration
        });
    }
}
