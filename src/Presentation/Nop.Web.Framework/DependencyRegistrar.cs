using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;

using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Fakes;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Infrastructure;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Dependency registrar
    /// 依赖注入/登记
    /// 通过调用Autofac相关的API进行依赖注入。
    /// </summary>
    /// <remarks>
    /// 1、InstancePerDependency
    /// 对每一个依赖或每一次调用创建一个新的唯一的实例。这也是默认的创建实例的方式。
    /// 官方文档解释：Configure the component so that every dependent component or call to Resolve() gets a new, unique instance(default.)
    /// 
    /// 2、InstancePerLifetimeScope
    /// 在一个生命周期域中，每一个依赖或调用创建一个单一的共享的实例，且每一个不同的生命周期域，实例是唯一的，不共享的。
    /// 官方文档解释：Configure the component so that every dependent component or call to Resolve() 
    /// within a single ILifetimeScope gets the same, shared instance.Dependent components in different lifetime scopes 
    /// will get different instances.
    /// 
    /// 3、InstancePerMatchingLifetimeScope
    /// 在一个做标识的生命周期域中，每一个依赖或调用创建一个单一的共享的实例。打了标识了的生命周期域中的子标识域中可以共享父级域中的实例。
    /// 若在整个继承层次中没有找到打标识的生命周期域，则会抛出异常：DependencyResolutionException。
    /// 官方文档解释：Configure the component so that every dependent component or call to Resolve() within 
    /// a ILifetimeScope tagged with any of the provided tags value gets the same, shared instance.Dependent components 
    /// in lifetime scopes that are children of the tagged scope will share the parent's instance. 
    /// If no appropriately tagged scope can be found in the hierarchy an DependencyResolutionException is thrown.
    /// 
    /// 4、InstancePerOwned
    /// 在一个生命周期域中所拥有的实例创建的生命周期中，每一个依赖组件或调用Resolve()方法创建一个单一的共享的实例，
    /// 并且子生命周期域共享父生命周期域中的实例。若在继承层级中没有发现合适的拥有子实例的生命周期域，
    /// 则抛出异常：DependencyResolutionException。
    /// 官方文档解释：Configure the component so that every dependent component or call to Resolve() 
    /// within a ILifetimeScope created by an owned instance gets the same, shared instance.Dependent components 
    /// in lifetime scopes that are children of the owned instance scope will share the parent's instance. 
    /// If no appropriate owned instance scope can be found in the hierarchy an DependencyResolutionException is thrown.
    /// 
    /// 5、SingleInstance
    /// 每一次依赖组件或调用Resolve()方法都会得到一个相同的共享的实例。其实就是单例模式。
    /// 官方文档解释：Configure the component so that every dependent component or call to Resolve() gets the same, shared instance.
    /// 
    /// 6、InstancePerHttpRequest
    /// 在一次Http请求上下文中, 共享一个组件实例。仅适用于asp.net mvc开发。
    /// 
    /// 参考链接：
    /// autofac 创建实例方法总结：http://www.cnblogs.com/manglu/p/4115128.html
    /// AutoFac使用方法总结:Part I：http://niuyi.github.io/blog/2012/04/06/autofac-by-unit-test/
    /// 
    /// 一、组件
    /// 创建出来的对象需要从组件中来获取。组件的创建有如下方法：
    /// 1、类型创建RegisterType
    /// AutoFac能够通过反射检查一个类型,选择一个合适的构造函数,创造这个对象的实例。
    /// 主要通过RegisterType<T>() 和 RegisterType(Type) 两个方法以这种方式建立。
    /// ContainerBuilder使用 As() 方法将Component封装成了服务使用。
    /// builder.RegisterType<AutoFacManager>();
    /// builder.RegisterType<Worker>().As<IPerson>();
    /// 
    /// 2、实例创建
    /// builder.RegisterInstance<AutoFacManager>(new AutoFacManager(new Worker()));
    /// 单例
    /// 提供示例的方式，还有一个功能，就是不影响系统中原有的单例：
    /// builder.RegisterInstance(MySingleton.GetInstance()).ExternallyOwned();　　//将自己系统中原有的单例注册为容器托管的单例
    /// 
    /// 3、Lambda表达式创建
    /// Lambda的方式也是Autofac通过反射的方式实现
    /// builder.Register(c => new AutoFacManager(c.Resolve<IPerson>()));
    /// builder.RegisterType<Worker>().As<IPerson>();
    /// 
    /// 4、程序集创建
    /// 程序集的创建主要通过RegisterAssemblyTypes()方法实现，Autofac会自动在程序集中查找匹配的类型用于创建实例。
    /// builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()); //在当前正在运行的程序集中找
    /// builder.RegisterType<Worker>().As<IPerson>();
    /// 
    /// 5、泛型注册
    /// 泛型注册通过RegisterGeneric() 这个方法实现，在容易中可以创建出泛型的具体对象。
    /// builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>)).InstancePerLifetimeScope();
    /// 
    /// 6、默认的注册
    /// 如果一个类型被多次注册,以最后注册的为准。通过使用PreserveExistingDefaults() 修饰符，可以指定某个注册为非默认值。
    /// 
    /// 二、服务
    /// Autofac有三种典型的方式区分服务，同一个服务的不同实现可以由类型，名称和键区分。
    /// 1、类型
    /// 类型是描述服务的基本方法
    /// builder.RegisterType<Worker>().As<IPerson>();   //IPerson类型的服务和Worker的组件连接起来，这个服务可以创建Worker类的实例
    /// 
    /// 2、名字
    /// 服务可以进一步按名字识别。使用这种方式时，用 Named()注册方法代替As()以指定名字:
    /// builder.RegisterType<Worker>().Named<IPerson>("worker");
    /// 使用Name可以检索服务创建实例：
    /// IPerson p = container.ResolveNamed<IPerson>("worker");
    /// ResolveNamed()只是Resolve()的简单重载，指定名字的服务其实是指定键的服务的简单版本。
    /// 
    /// 3、键
    /// 有Name的方式很方便，但是值支持字符串，但有时候我们可能需要通过其他类型作键。
    /// 例如，使用枚举作为key：
    /// public enum DeviceState { Worker, Student }
    /// 使用key注册服务，通过Keyed<T>()方法：
    /// builder.RegisterType<Student>().Keyed<IPerson>(DeviceState.Student);
    /// 显式检索
    /// 使用key检索服务以创建实例，通过ResolveKeyd()方法：
    /// IPerson p = container.ResolveKeyed<IPerson>(DeviceState.Student);
    /// ResolveKeyd()会导致容器被当做 Service Locator使用，这是不被推荐的。应该使用IIndex type替代。
    /// 
    /// 三、自动装配
    /// 从容器中的可用服务中选择一个构造函数来创造对象，这个过程叫做自动装配。
    /// 这个过程是通过反射实现的，所以实际上容器创造对象的行为比较适合用在配置环境中。
    /// 
    /// 1、选择构造函数
    /// Autofac默认从容器中选择参数最多的构造函数。如果想要选择一个不同的构造函数，就需要在注册的时候就指定它。
    /// builder.RegisterType(typeof(Worker)).UsingConstructor(typeof(int));
    /// 这种写法将指定调用Worker(int)构造函数，如该构造函数不存在则报错。
    /// 
    /// 2、额外的构造函数参数
    /// 有两种方式可以添加额外的构造函数参数，在注册的时候和在检索的时候。在使用自动装配实例的时候这两种都会用到。
    /// 
    /// 注册时添加参数
    /// 使用WithParameters()方法在每一次创建对象的时候将组件和参数关联起来。
    /// List<NamedParameter> ListNamedParameter = new List<NamedParameter>() { new NamedParameter("Id", 1), new NamedParameter("Name", "张三") };
    /// builder.RegisterType<Worker>().WithParameters(ListNamedParameter).As<IPerson>();
    /// 
    /// 在检索阶段添加参数
    /// 在Resolve()的时候提供的参数会覆盖所有名字相同的参数，在注册阶段提供的参数会覆盖容器中所有可能的服务。
    /// 
    /// 3、自动装配
    /// 至今为止，自动装配最大的作用就是减少重复配置。许多相似的component无论在哪里注册，都可以通过扫描使用自动装配。
    /// builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).As<IPerson>();
    /// 在需要的时候，依然可以创建指定的构造函数创建指定的类。
    /// builder.Register(c => new Worker(2,"关羽"));
    /// 
    /// 四、程序集扫描
    /// 1、扫描
    /// Autofac可以使用约定在程序集中注册或者寻找组件。
    /// Autofac可以根据用户指定的规则在程序集中注册一系列的类型，这种方法叫做convention-driven registration或者扫描。
    /// builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Manager"));
    /// 每个RegisterAssemblyTypes方法只能应用一套规则。如果有多套不同的集合要注册，那就有必要多次调用RegisterAssemblyTypes。
    /// 
    /// 2、选择类型
    /// RegisterAssemblyTypes接受程序集的集合。默认情况下，程序集中所有公共具体的类都会被注册。
    /// 如果想要过滤注册的类型，可以使用Where.向下面这样：
    /// Where(t => t.Name.EndsWith("Manager"))
    /// 如果想要排除某些类型，使用Except()：Except<AutoFacManager)>()
    /// 或者，自定义那些已经排除的类型的注册：Except<Worker>(ct =>ct.As<IPerson>().SingleInstance())
    /// 多个过滤器可以同时使用，这时他们之间是AND的关系。
    /// 
    /// 3、指定服务
    /// RegisterAssemblyTypes这个注册方法是注册单个方法的超集，所以类似As的方法也可以用在程序集中，例如
    /// As<IPerson>();
    /// As和Named这两个方法额外的重载方法接受lambda表达式来决定服务会提供什么样的类型。
    /// 
    /// 五、事件
    /// 在component生命周期的不同阶段使用事件。
    /// Autofac暴露五个事件接口供实例的按如下顺序调用
    /// OnRegistered
    /// OnPreparing
    /// OnActivated
    /// OnActivating
    /// OnRelease
    /// 这些事件会在注册的时候被订阅，或者被附加到IComponentRegistration 的时候。
    /// builder.RegisterType<Worker>().As<IPerson>()
    ///         .OnRegistered(e => Console.WriteLine("在注册的时候调用!"))
    ///         .OnPreparing(e => Console.WriteLine("在准备创建的时候调用!"))
    ///         .OnActivating(e => Console.WriteLine("在创建之前调用!"))
    ///         .OnActivated(e => Console.WriteLine("创建之后调用!"))
    ///         .OnRelease(e => Console.WriteLine("在释放占用的资源之前调用!"));
    /// 
    /// 六、属性注入
    /// 
    /// 七、方法注入
    /// 
    /// 八、Resolve的参数
    /// 
    /// 九、元数据
    /// 
    /// 十、循环依赖
    /// 
    /// 十一、泛型
    /// 
    /// 十二、适配器和装饰器
    /// 
    /// 十三、实例生命周期
    /// 
    /// </remarks>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            #region 注入Http抽象类
            //HTTP context and other related stuff
            builder.Register(c => 
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();
            #endregion

            #region helper
            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            //user agent helper
            builder.RegisterType<UserAgentHelper>().As<IUserAgentHelper>().InstancePerLifetimeScope();
            #endregion

            //controllers
            //对程序集中所有的Controller一次性的完成注册。(所有的控制器都实现了接口IController)。
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            #region 数据访问层
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();

            //创建数据访问者实例
            builder.Register(x => x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new NopObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            }
            else
            {
                builder.Register<IDbContext>(c => new NopObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerLifetimeScope();
            }

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            #endregion

            //插件
            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();
            builder.RegisterType<OfficialFeedManager>().As<IOfficialFeedManager>().InstancePerLifetimeScope();

            #region cache managers 缓存
            if (config.RedisCachingEnabled)
            {
                //redis缓存
                builder.RegisterType<RedisConnectionWrapper>().As<IRedisConnectionWrapper>().SingleInstance();
                builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").InstancePerLifetimeScope();
            }
            else
            {
                //内存缓存
                builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").SingleInstance();
            }
            //http缓存
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_per_request").InstancePerLifetimeScope();
            #endregion

            if (config.RunOnAzureWebApps)
            {
                builder.RegisterType<AzureWebAppsMachineNameProvider>().As<IMachineNameProvider>().SingleInstance();
            }
            else
            {
                builder.RegisterType<DefaultMachineNameProvider>().As<IMachineNameProvider>().SingleInstance();
            }

            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            //store context
            builder.RegisterType<WebStoreContext>().As<IStoreContext>().InstancePerLifetimeScope();

            //services
            builder.RegisterType<BackInStockSubscriptionService>().As<IBackInStockSubscriptionService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CompareProductsService>().As<ICompareProductsService>().InstancePerLifetimeScope();
            builder.RegisterType<RecentlyViewedProductsService>().As<IRecentlyViewedProductsService>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerService>().As<IManufacturerService>().InstancePerLifetimeScope();
            builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterType<CopyProductService>().As<ICopyProductService>().InstancePerLifetimeScope();
            builder.RegisterType<SpecificationAttributeService>().As<ISpecificationAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductTemplateService>().As<IProductTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryTemplateService>().As<ICategoryTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerTemplateService>().As<IManufacturerTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<TopicTemplateService>().As<ITopicTemplateService>().InstancePerLifetimeScope();
            //use static cache (between HTTP requests)
            builder.RegisterType<ProductTagService>().As<IProductTagService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<AddressAttributeFormatter>().As<IAddressAttributeFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<AddressAttributeParser>().As<IAddressAttributeParser>().InstancePerLifetimeScope();
            builder.RegisterType<AddressAttributeService>().As<IAddressAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerLifetimeScope();
            builder.RegisterType<VendorService>().As<IVendorService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchTermService>().As<ISearchTermService>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerLifetimeScope();
            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomerAttributeFormatter>().As<ICustomerAttributeFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerAttributeParser>().As<ICustomerAttributeParser>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerAttributeService>().As<ICustomerAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRegistrationService>().As<ICustomerRegistrationService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerReportService>().As<ICustomerReportService>().InstancePerLifetimeScope();

            //use static cache (between HTTP requests)
            builder.RegisterType<PermissionService>().As<IPermissionService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();
            //use static cache (between HTTP requests)
            builder.RegisterType<AclService>().As<IAclService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();
            //use static cache (between HTTP requests)
            builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<GeoLookupService>().As<IGeoLookupService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>().InstancePerLifetimeScope();
            builder.RegisterType<MeasureService>().As<IMeasureService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().InstancePerLifetimeScope();

            builder.RegisterType<StoreService>().As<IStoreService>().InstancePerLifetimeScope();
            //use static cache (between HTTP requests)
            builder.RegisterType<StoreMappingService>().As<IStoreMappingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            //use static cache (between HTTP requests)
            builder.RegisterType<DiscountService>().As<IDiscountService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            //use static cache (between HTTP requests)
            builder.RegisterType<SettingService>().As<ISettingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            //添加一个注册源，好像是一种迟延注册。????
            builder.RegisterSource(new SettingsSource());

            //use static cache (between HTTP requests)
            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            //use static cache (between HTTP requests)
            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerLifetimeScope();

            builder.RegisterType<DownloadService>().As<IDownloadService>().InstancePerLifetimeScope();
            //picture service
            var useAzureBlobStorage = !String.IsNullOrEmpty(config.AzureBlobStorageConnectionString);
            if (useAzureBlobStorage)
            {
                //Windows Azure BLOB
                builder.RegisterType<AzurePictureService>().As<IPictureService>().InstancePerLifetimeScope();
            }
            else
            {
                //standard file system
                builder.RegisterType<PictureService>().As<IPictureService>().InstancePerLifetimeScope();
            }

            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<NewsLetterSubscriptionService>().As<INewsLetterSubscriptionService>().InstancePerLifetimeScope();
            builder.RegisterType<CampaignService>().As<ICampaignService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerLifetimeScope();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();

            builder.RegisterType<CheckoutAttributeFormatter>().As<ICheckoutAttributeFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutAttributeParser>().As<ICheckoutAttributeParser>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutAttributeService>().As<ICheckoutAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<GiftCardService>().As<IGiftCardService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderReportService>().As<IOrderReportService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderProcessingService>().As<IOrderProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderTotalCalculationService>().As<IOrderTotalCalculationService>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnRequestService>().As<IReturnRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<RewardPointService>().As<IRewardPointService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomNumberFormatter>().As<ICustomNumberFormatter>().InstancePerLifetimeScope();

            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerLifetimeScope();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();

            //use static cache (between HTTP requests)
            builder.RegisterType<UrlRecordService>().As<IUrlRecordService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<ShipmentService>().As<IShipmentService>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingService>().As<IShippingService>().InstancePerLifetimeScope();
            builder.RegisterType<DateRangeService>().As<IDateRangeService>().InstancePerLifetimeScope();

            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<TaxService>().As<ITaxService>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();

            //use static cache (between HTTP requests)
            builder.RegisterType<CustomerActivityService>().As<ICustomerActivityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            #region 数据库初始化（安装）
            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            if (!databaseInstalled)
            {
                //installation service
                if (config.UseFastInstallationService)
                {
                    builder.RegisterType<SqlFileInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
                }
                else
                {
                    builder.RegisterType<CodeFirstInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
                }
            }
            #endregion

            builder.RegisterType<ForumService>().As<IForumService>().InstancePerLifetimeScope();

            builder.RegisterType<PollService>().As<IPollService>().InstancePerLifetimeScope();
            builder.RegisterType<BlogService>().As<IBlogService>().InstancePerLifetimeScope();
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerLifetimeScope();
            builder.RegisterType<TopicService>().As<ITopicService>().InstancePerLifetimeScope();
            builder.RegisterType<NewsService>().As<INewsService>().InstancePerLifetimeScope();

            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<SitemapGenerator>().As<ISitemapGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();

            builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerLifetimeScope();
            builder.RegisterType<ImportManager>().As<IImportManager>().InstancePerLifetimeScope();
            builder.RegisterType<PdfService>().As<IPdfService>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerLifetimeScope();


            builder.RegisterType<ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerLifetimeScope();
            builder.RegisterType<OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerLifetimeScope();
           
            //路由
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            #region 注册事件消费者
            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
            #endregion
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }


    /// <summary>
    /// ISettings接口的实现服务使用延迟注册
    /// </summary>
    public class SettingsSource : IRegistrationSource
    {
        //构建注册的方法
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="service"></param>
        /// <param name="registrations"></param>
        /// <returns></returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService; //服务类型
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    var currentStoreId = c.Resolve<IStoreContext>().CurrentStore.Id;
                    //uncomment the code below if you want load settings per store only when you have two stores installed.
                    //var currentStoreId = c.Resolve<IStoreService>().GetAllStores().Count > 1
                    //    c.Resolve<IStoreContext>().CurrentStore.Id : 0;

                    //although it's better to connect to your database and execute the following SQL:
                    //DELETE FROM [Setting] WHERE [StoreId] > 0
                    return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentStoreId);
                })
                .InstancePerLifetimeScope()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }

}
