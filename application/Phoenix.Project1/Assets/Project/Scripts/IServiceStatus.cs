using Regulus.Utility;

namespace Phoenix.Project1.Client
{
    interface IServiceStatus : IStatus
    {
        Regulus.Remote.INotifierQueryable Queryable { get; }
    }
}
