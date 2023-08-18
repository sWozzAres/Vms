namespace Vms.Domain.ServiceBookingProcess
{
    public enum ServiceBookingStatus : int
    {
        Cancelled = -2,
        Complete = -1,
        None = 0,
        Assign = 1,
        Book = 2,
        Confirm = 3,
        CheckArrival = 4,
        CheckWorkStatus = 5,
        ChaseDriver = 6,
        RebookDriver = 7,
        NotifyCustomer = 8,
        NotifyCustomerDelay = 9
    };
}
