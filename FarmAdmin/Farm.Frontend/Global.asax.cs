﻿using Autofac;
using Autofac.Features.ResolveAnything;
using Autofac.Integration.Mvc;
using Farm.AutoMapperConfig;
using Farm.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

namespace FarmAdmin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //IOC容器
            AutofacRegister();

            //自动映射
            AutoMapperRegister();
        }


        private void AutofacRegister()
        {
            var builder = new ContainerBuilder();

            ////注册MvcApplication程序集中所有的控制器
            //builder.RegisterControllers(typeof(MvcApplication).Assembly);

            ////注册仓储层服务
            ////builder.RegisterType<PostRepository>().As<IPostRepository>();
            ////注册基于接口约束的实体
            //var assembly = AppDomain.CurrentDomain.GetAssemblies();
            //builder.RegisterAssemblyTypes(assembly)
            //    .Where(
            //        t => t.GetInterfaces()
            //            .Any(i => i.IsAssignableFrom(typeof(IDependency)))
            //    )
            //    .AsImplementedInterfaces()
            //    .InstancePerDependency();

            ////注册过滤器
            //builder.RegisterFilterProvider();
            //builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            //批量注入IDependency接口实现对象
            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            var baseType = typeof(IDependency);
            //InstancePerLifetimeScope 保证生命周期基于请求
            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract).AsImplementedInterfaces().InstancePerLifetimeScope();
            //MVC控制器注入
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //MVC过滤器注入
            builder.RegisterFilterProvider();

            var container = builder.Build();
            //设置依赖注入解析器
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }

        /// <summary>
        /// AutoMapper的配置初始化
        /// </summary>
        private void AutoMapperRegister()
        {
            new AutoMapperStartupTask().Execute();
        }
    }
}
