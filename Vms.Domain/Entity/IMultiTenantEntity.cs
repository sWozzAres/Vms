namespace Vms.Domain.Entity;

public interface IMultiTenantEntity
{
    string CompanyCode { get; set; }
}