use [vms.data]
go

insert networksuppliers
select n.companycode, n.code, s.code
from suppliers s
cross join networks n

insert CustomerNetworks(CustomerCode, NetworkCode, CompanyCode)
select c.code, n.code, n.companycode
from customers c
cross join networks n
where c.companycode=n.CompanyCode

insert FleetNetworks (FleetCode, NetworkCode, CompanyCode)
select f.code, n.code, n.companycode
from fleets f
cross join networks n
where f.companycode=n.CompanyCode

-- random mot due dates in the future
UPDATE VehicleMots
SET Due = DATEADD(day, ABS(CHECKSUM(vehicleid)) % 365, GETDATE());