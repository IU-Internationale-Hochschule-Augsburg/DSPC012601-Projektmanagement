namespace Projektmanagement_DesktopApp.Factorys;

public class GenericFactory : AbstractFactory
{
    private readonly Dictionary<Type, Func<object>> _creators = new();

    public void Register<TBase, TConcrete>()
        where TConcrete : TBase, new()
        where TBase : class
    {
        _creators[typeof(TBase)] = () => new TConcrete();
    }

    public void Register<TConcrete>()
        where TConcrete : class, new()
    {
        _creators[typeof(TConcrete)] = () => new TConcrete();
    }

    public override TBase Create<TBase>()
        where TBase : class
    {
        var baseType = typeof(TBase);

        if (_creators.TryGetValue(baseType, out var creator))
        {
            return (TBase)creator();
        }

        // Fallback: direkter new()-Constraint, falls möglich
        if (baseType.GetConstructor(Type.EmptyTypes) != null)
        {
            return (TBase)Activator.CreateInstance(baseType)!;
        }

        throw new InvalidOperationException(
            $"No factory registration and no parameterless constructor for type {baseType.FullName}.");
    }
}