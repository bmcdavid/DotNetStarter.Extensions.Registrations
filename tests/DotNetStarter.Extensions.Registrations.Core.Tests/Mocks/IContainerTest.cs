using System.Collections.Generic;

namespace DotNetStarter.Extensions.Registrations.Core.Tests.Mocks
{
    public interface IContainerTest
    {
        string ContainerName { get; }

        T Get<T>();

        IEnumerable<T> All<T>();

        void Configure();
    }
}
