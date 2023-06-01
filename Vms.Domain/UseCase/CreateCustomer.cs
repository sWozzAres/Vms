using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.UseCase;

public class CreateCustomer
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateCustomer(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode)
            ?? throw new VmsDomainException("Company not found."), this);

        return await Company.CreateCustomerAsync(request.Code, request.Name, cancellationToken);
    }

    public class CompanyRole(Company self, CreateCustomer context)
    {
        public async Task<Customer> CreateCustomerAsync(string code, string name, CancellationToken cancellationToken)
        {
            var customer = new Customer(self.Code, code, name);
            await context.DbContext.AddAsync(customer, cancellationToken);
            return customer;
        }
    }
}

public record CreateCustomerRequest(string CompanyCode, string Code, string Name);