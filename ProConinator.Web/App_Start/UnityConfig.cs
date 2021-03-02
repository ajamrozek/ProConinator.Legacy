using Microsoft.Practices.Unity;
using ProConinator.Web.Data;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace ProConinator.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IProConinatorRepository, ProConinatorRepository>(new InjectionConstructor(ConfigurationManager.ConnectionStrings["DocStore"].ConnectionString));
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}