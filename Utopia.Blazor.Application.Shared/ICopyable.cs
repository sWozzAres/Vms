namespace Utopia.Blazor.Application.Shared;

public interface ICopyable<T>
{
    public void CopyFrom(T source);
}