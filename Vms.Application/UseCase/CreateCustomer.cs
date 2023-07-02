namespace Vms.Application.UseCase;

public class CreateCustomer(VmsDbContext dbContext)
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var company = Company.CreateCustomer(request.Code, request.Name);

        //await DbContext.SaveChangesAsync(cancellationToken);

        return company;
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