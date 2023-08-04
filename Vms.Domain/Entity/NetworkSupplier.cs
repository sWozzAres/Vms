namespace Vms.Domain.Entity
{
    public class NetworkSupplier
    {
        public string CompanyCode { get; private set; } = null!;
        public string NetworkCode { get; private set; } = null!;
        public string SupplierCode { get; private set; } = null!;
        public virtual Network Network { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
        private NetworkSupplier() { }
        public NetworkSupplier(string companyCode, string networkCode, string supplierCode)
            => (CompanyCode, NetworkCode, SupplierCode) = (companyCode, networkCode, supplierCode);
    }
}
