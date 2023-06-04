using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Application.UseCase;

public class CreateCustomer
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateCustomer(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        return Company.CreateCustomer(request.Code, request.Name);
    }

    public class CompanyRole(Company self, CreateCustomer context)
    {
        public Customer CreateCustomer(string code, string name)
        {
            var customer =  new Customer(self.Code, code, name);
            context.DbContext.Customers.Add(customer);
            return customer;
        }
    }
}

public record CreateCustomerRequest(string CompanyCode, string Code, string Name);