namespace ProjektManagement.Repositories;

public class _Factory
{
    public abstract TBase Create<TBase>() where TBase : class;
}