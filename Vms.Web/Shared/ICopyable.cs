namespace Vms.Web.Shared;

public interface ICopyable<T>
{
    public void CopyFrom(T source);
    //T ShallowCopy();
}
