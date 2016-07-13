using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Template.WebApp.App_Start
{
    public class IoCConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DefaultMembershipRebootDatabase, Configuration>());

            //builder.RegisterInstance(config).As<MembershipRebootConfiguration>();
            //builder.RegisterType<UserAccountService>().AsSelf();
            //builder.RegisterType<SamAuthenticationService>().As<AuthenticationService>();
            //builder.RegisterType<DefaultUserAccountRepository>().As<IUserAccountQuery>().InstancePerRequest();
            //builder.RegisterType<DefaultUserAccountRepository>().As<IUserAccountRepository>().InstancePerRequest();
            //builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IGenericRepository<>));

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}