namespace Vms.Blazor.Admin;

public interface ICopyable<T>
{
    public void CopyFrom(T source);
}
