using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;
using VideoTube.Data;
using VideoTube.Data.Services;

namespace VideoTube
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            //container.RegisterType<IConnectionConfiguration,ConnectionConfiguration>(new InjectionConstructor());
            //container.RegisterType<IConnectionConfiguration>(new InjectionFactory(c => new ConnectionConfiguration(SessionManager.ServiceKey)));
            container.RegisterType<IConnectionConfiguration, ConnectionConfiguration>();
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}