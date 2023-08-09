using Microsoft.Extensions.Logging;

namespace Vms.Application.Commands;

public class CreateCustomer(VmsDbContext dbContext, ISearchManager searchManager, ILogger logger)
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating customer {customercode} {customername}", request.Code, request.Name);

        Company = new(await DbContext.Companies.FindAsync(new object[] { request.CompanyCode }, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var customer = Company.CreateCustomer(request.Code, request.Name);

        searchManager.Add(customer.CompanyCode, customer.Code, EntityKind.Customer, customer.Name,
            string.Join(" ", customer.Code, customer.Name));

        return customer;
    }

    class CompanyRole(Company self, CreateCustomer ctx)
    {
        public Customer CreateCustomer(string code, string name)
        {
            var customer = new Customer(self.Code, code, name);
            ctx.DbContext.Customers.Add(customer);
            return customer;
        }
    }
}

public record CreateCustomerRequest(string CompanyCode, string Code, string Name);