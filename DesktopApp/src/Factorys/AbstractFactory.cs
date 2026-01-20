namespace Projektmanagement_DesktopApp.Factorys;

public abstract class AbstractFactory
{
    public abstract TBase Create<TBase>() where TBase : class;
}