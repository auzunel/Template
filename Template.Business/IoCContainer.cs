using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Business
{
    public class IoCContainer
    {
        public static IContainer BaseContainer { get; private set; }
        
        public IoCContainer(IContainer container)
        {
            BaseContainer = container;
        }

        public static ILifetimeScope BeginLifetimeScope()
        {
            return BaseContainer.BeginLifetimeScope();
        }
    }
}
