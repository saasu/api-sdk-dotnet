using System;
using Microsoft.Extensions.DependencyInjection;

namespace Saasu.API.Client.Framework
{

    public sealed class DependencyInjectionWrapper
    {
        private static readonly Lazy<DependencyInjectionWrapper> Lazy =
                new Lazy<DependencyInjectionWrapper>(() => new DependencyInjectionWrapper());

        public static DependencyInjectionWrapper Instance { get { return Lazy.Value; } }

        public readonly ServiceProvider ServiceProvider;

        private DependencyInjectionWrapper()
        {
            ServiceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
        }
    }
}