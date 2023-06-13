namespace Vms.Blazor.Shared;

public interface ICopyable<T>
{
    public void CopyFrom(T source);
}
