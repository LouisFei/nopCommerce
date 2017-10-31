using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Infrastructure.Mapper;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// 具体的Nop引擎
    /// </summary>
    public class NopEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        /// <summary>
        /// Register dependencies
        /// 注册依赖（基于Autofac依赖注入框架）
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterDependencies(NopConfig config)
        {
            /*
             1、ContainerBuilder：组件通过它来进行注册。
             2、组件，对象需要从组件中来获取。
             3、哪些元素可以作为组件：Lambda表达式，一个类型，一个预编译的实例，实例类型所在的程序集。
             4、容器：ContainerBuilder的Build()方法可以创建容器，从容器的Resolve()方法能够获得对象。
             5、为指定组件服务是某一接口：As()方法将用于注册时指定。
             6、组件的依赖关系：组件的依赖关系主要通过接口实现。
             */

            //创建组件容器
            var builder = new ContainerBuilder();
            
            //注册依赖
            //dependencies
            var typeFinder = new WebAppTypeFinder();

            //注册对象实例，并指定作为单例使用。
            builder.RegisterInstance(config).As<NopConfig>().SingleInstance(); //Nop配置
            builder.RegisterInstance(this).As<IEngine>().SingleInstance(); //引擎
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance(); //类型发现器

            //register dependencies provided by other assemblies
            //查找所有依赖注册具体实现类。然后实例化他们。调用他们的依赖注册方法。
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
            {
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            }                

            //sort
            //排序
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
            {
                //执行依赖注册
                dependencyRegistrar.Register(builder, typeFinder, config);
            }

            var container = builder.Build();
            this._containerManager = new ContainerManager(container);

            //set dependency resolver
            // ???
            //使用指定的依赖关系解析程序接口，为依赖关系解析程序提供一个注册点。
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //DependencyResolver
            //为实现 IDependencyResolver 或常用服务定位器 IServiceLocator 接口的依赖关系解析程序提供一个注册点。


            //Autofac.Integration.WebApi;
            //// Set the dependency resolver for Web API.
            //var webApiResolver = new AutofacWebApiDependencyResolver(container);
            //GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;

            //// Set the dependency resolver for MVC.
            //var resolver = new AutofacDependencyResolver(container);
            //DependencyResolver.SetResolver(resolver);

        }

        /// <summary>
        /// Register mapping
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterMapperConfiguration(NopConfig config)
        {
            //dependencies
            var typeFinder = new WebAppTypeFinder();

            //register mapper configurations provided by other assemblies
            var mcTypes = typeFinder.FindClassesOfType<IMapperConfiguration>();
            var mcInstances = new List<IMapperConfiguration>();
            foreach (var mcType in mcTypes)
                mcInstances.Add((IMapperConfiguration)Activator.CreateInstance(mcType));
            //sort
            mcInstances = mcInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            //get configurations
            var configurationActions = new List<Action<IMapperConfigurationExpression>>();
            foreach (var mc in mcInstances)
                configurationActions.Add(mc.GetConfiguration());
            //register
            AutoMapperConfiguration.Init(configurationActions);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// 在nop环境中初始化组件和插件。
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(NopConfig config)
        {
            //register dependencies
            //注册依赖
            RegisterDependencies(config);

            //register mapper configurations
            //注册映射配置
            RegisterMapperConfiguration(config);

            //startup tasks
            //启动任务
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }

        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
		{
            return ContainerManager.Resolve<T>();
		}

        /// <summary>
        ///  Resolve dependency
        ///  获得注册的依赖组件
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }
        
        /// <summary>
        /// Resolve dependencies
        /// 获得多个依赖组件
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

		#endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// 容器封装器
        /// </summary>
        public virtual ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
