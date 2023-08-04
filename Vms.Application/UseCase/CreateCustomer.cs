using Vms.Application.Services;

namespace Vms.Application.UseCase;

public class CreateCustomer(VmsDbContext dbContext, ISearchManager searchManager)
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(new object[] { request.CompanyCode }, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var customer = Company.CreateCustomer(request.Code, request.Name);

        searchManager.Add(customer.CompanyCode, customer.Code, EntityKind.Customer, customer.Name,
            string.Join(" ", customer.Code, customer.Name));

        return customer;
    }

    class CompanyRole(Company self, CreateCustomer context)
    {
        public Customer CreateCustomer(string code, string name)
        {
            var customer = new Customer(self.Code, code, name);
            context.DbContext.Customers.Add(customer);
            return customer;
        }
    }
}

public record CreateCustomerRequest(string CompanyCode, string Code, string Name);