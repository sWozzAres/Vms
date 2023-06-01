using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Company = new(await DbContext.Companies.FindAsync(request.CompanyId)
            ?? throw new VmsDomainException("Company not found."), this);

        var customer = await Company.CreateCustomerAsync(request.Code, request.Name, cancellationToken);
        //await DbContext.SaveChangesAsync(cancellationToken);
        return customer;
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

public record CreateCustomerRequest(int CompanyId, string Code, string Name);