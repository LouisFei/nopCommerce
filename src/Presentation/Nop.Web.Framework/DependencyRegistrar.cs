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
    /// ����ע��/�Ǽ�
    /// ͨ������Autofac��ص�API��������ע�롣
    /// </summary>
    /// <remarks>
    /// 1��InstancePerDependency
    /// ��ÿһ��������ÿһ�ε��ô���һ���µ�Ψһ��ʵ������Ҳ��Ĭ�ϵĴ���ʵ���ķ�ʽ��
    /// �ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() gets a new, unique instance(default.)
    /// 
    /// 2��InstancePerLifetimeScope
    /// ��һ�������������У�ÿһ����������ô���һ����һ�Ĺ����ʵ������ÿһ����ͬ������������ʵ����Ψһ�ģ�������ġ�
    /// �ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() 
    /// within a single ILifetimeScope gets the same, shared instance.Dependent components in different lifetime scopes 
    /// will get different instances.
    /// 
    /// 3��InstancePerMatchingLifetimeScope
    /// ��һ������ʶ�������������У�ÿһ����������ô���һ����һ�Ĺ����ʵ�������˱�ʶ�˵������������е��ӱ�ʶ���п��Թ��������е�ʵ����
    /// ���������̳в����û���ҵ����ʶ����������������׳��쳣��DependencyResolutionException��
    /// �ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() within 
    /// a ILifetimeScope tagged with any of the provided tags value gets the same, shared instance.Dependent components 
    /// in lifetime scopes that are children of the tagged scope will share the parent's instance. 
    /// If no appropriately tagged scope can be found in the hierarchy an DependencyResolutionException is thrown.
    /// 
    /// 4��InstancePerOwned
    /// ��һ����������������ӵ�е�ʵ�����������������У�ÿһ��������������Resolve()��������һ����һ�Ĺ����ʵ����
    /// �����������������������������е�ʵ�������ڼ̳в㼶��û�з��ֺ��ʵ�ӵ����ʵ��������������
    /// ���׳��쳣��DependencyResolutionException��
    /// �ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() 
    /// within a ILifetimeScope created by an owned instance gets the same, shared instance.Dependent components 
    /// in lifetime scopes that are children of the owned instance scope will share the parent's instance. 
    /// If no appropriate owned instance scope can be found in the hierarchy an DependencyResolutionException is thrown.
    /// 
    /// 5��SingleInstance
    /// ÿһ��������������Resolve()��������õ�һ����ͬ�Ĺ����ʵ������ʵ���ǵ���ģʽ��
    /// �ٷ��ĵ����ͣ�Configure the component so that every dependent component or call to Resolve() gets the same, shared instance.
    /// 
    /// 6��InstancePerHttpRequest
    /// ��һ��Http������������, ����һ�����ʵ������������asp.net mvc������
    /// 
    /// �ο����ӣ�
    /// autofac ����ʵ�������ܽ᣺http://www.cnblogs.com/manglu/p/4115128.html
    /// AutoFacʹ�÷����ܽ�:Part I��http://niuyi.github.io/blog/2012/04/06/autofac-by-unit-test/
    /// 
    /// һ�����
    /// ���������Ķ�����Ҫ�����������ȡ������Ĵ��������·�����
    /// 1�����ʹ���RegisterType
    /// AutoFac�ܹ�ͨ��������һ������,ѡ��һ�����ʵĹ��캯��,������������ʵ����
    /// ��Ҫͨ��RegisterType<T>() �� RegisterType(Type) �������������ַ�ʽ������
    /// ContainerBuilderʹ�� As() ������Component��װ���˷���ʹ�á�
    /// builder.RegisterType<AutoFacManager>();
    /// builder.RegisterType<Worker>().As<IPerson>();
    /// 
    /// 2��ʵ������
    /// builder.RegisterInstance<AutoFacManager>(new AutoFacManager(new Worker()));
    /// ����
    /// �ṩʾ���ķ�ʽ������һ�����ܣ����ǲ�Ӱ��ϵͳ��ԭ�еĵ�����
    /// builder.RegisterInstance(MySingleton.GetInstance()).ExternallyOwned();����//���Լ�ϵͳ��ԭ�еĵ���ע��Ϊ�����йܵĵ���
    /// 
    /// 3��Lambda���ʽ����
    /// Lambda�ķ�ʽҲ��Autofacͨ������ķ�ʽʵ��
    /// builder.Register(c => new AutoFacManager(c.Resolve<IPerson>()));
    /// builder.RegisterType<Worker>().As<IPerson>();
    /// 
    /// 4�����򼯴���
    /// ���򼯵Ĵ�����Ҫͨ��RegisterAssemblyTypes()����ʵ�֣�Autofac���Զ��ڳ����в���ƥ����������ڴ���ʵ����
    /// builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()); //�ڵ�ǰ�������еĳ�������
    /// builder.RegisterType<Worker>().As<IPerson>();
    /// 
    /// 5������ע��
    /// ����ע��ͨ��RegisterGeneric() �������ʵ�֣��������п��Դ��������͵ľ������
    /// builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>)).InstancePerLifetimeScope();
    /// 
    /// 6��Ĭ�ϵ�ע��
    /// ���һ�����ͱ����ע��,�����ע���Ϊ׼��ͨ��ʹ��PreserveExistingDefaults() ���η�������ָ��ĳ��ע��Ϊ��Ĭ��ֵ��
    /// 
    /// ��������
    /// Autofac�����ֵ��͵ķ�ʽ���ַ���ͬһ������Ĳ�ͬʵ�ֿ��������ͣ����ƺͼ����֡�
    /// 1������
    /// ��������������Ļ�������
    /// builder.RegisterType<Worker>().As<IPerson>();   //IPerson���͵ķ����Worker������������������������Դ���Worker���ʵ��
    /// 
    /// 2������
    /// ������Խ�һ��������ʶ��ʹ�����ַ�ʽʱ���� Named()ע�᷽������As()��ָ������:
    /// builder.RegisterType<Worker>().Named<IPerson>("worker");
    /// ʹ��Name���Լ������񴴽�ʵ����
    /// IPerson p = container.ResolveNamed<IPerson>("worker");
    /// ResolveNamed()ֻ��Resolve()�ļ����أ�ָ�����ֵķ�����ʵ��ָ�����ķ���ļ򵥰汾��
    /// 
    /// 3����
    /// ��Name�ķ�ʽ�ܷ��㣬����ֵ֧���ַ���������ʱ�����ǿ�����Ҫͨ����������������
    /// ���磬ʹ��ö����Ϊkey��
    /// public enum DeviceState { Worker, Student }
    /// ʹ��keyע�����ͨ��Keyed<T>()������
    /// builder.RegisterType<Student>().Keyed<IPerson>(DeviceState.Student);
    /// ��ʽ����
    /// ʹ��key���������Դ���ʵ����ͨ��ResolveKeyd()������
    /// IPerson p = container.ResolveKeyed<IPerson>(DeviceState.Student);
    /// ResolveKeyd()�ᵼ������������ Service Locatorʹ�ã����ǲ����Ƽ��ġ�Ӧ��ʹ��IIndex type�����
    /// 
    /// �����Զ�װ��
    /// �������еĿ��÷�����ѡ��һ�����캯�����������������̽����Զ�װ�䡣
    /// ���������ͨ������ʵ�ֵģ�����ʵ������������������Ϊ�Ƚ��ʺ��������û����С�
    /// 
    /// 1��ѡ���캯��
    /// AutofacĬ�ϴ�������ѡ��������Ĺ��캯���������Ҫѡ��һ����ͬ�Ĺ��캯��������Ҫ��ע���ʱ���ָ������
    /// builder.RegisterType(typeof(Worker)).UsingConstructor(typeof(int));
    /// ����д����ָ������Worker(int)���캯������ù��캯���������򱨴�
    /// 
    /// 2������Ĺ��캯������
    /// �����ַ�ʽ������Ӷ���Ĺ��캯����������ע���ʱ����ڼ�����ʱ����ʹ���Զ�װ��ʵ����ʱ�������ֶ����õ���
    /// 
    /// ע��ʱ��Ӳ���
    /// ʹ��WithParameters()������ÿһ�δ��������ʱ������Ͳ�������������
    /// List<NamedParameter> ListNamedParameter = new List<NamedParameter>() { new NamedParameter("Id", 1), new NamedParameter("Name", "����") };
    /// builder.RegisterType<Worker>().WithParameters(ListNamedParameter).As<IPerson>();
    /// 
    /// �ڼ����׶���Ӳ���
    /// ��Resolve()��ʱ���ṩ�Ĳ����Ḳ������������ͬ�Ĳ�������ע��׶��ṩ�Ĳ����Ḳ�����������п��ܵķ���
    /// 
    /// 3���Զ�װ��
    /// ����Ϊֹ���Զ�װ���������þ��Ǽ����ظ����á�������Ƶ�component����������ע�ᣬ������ͨ��ɨ��ʹ���Զ�װ�䡣
    /// builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).As<IPerson>();
    /// ����Ҫ��ʱ����Ȼ���Դ���ָ���Ĺ��캯������ָ�����ࡣ
    /// builder.Register(c => new Worker(2,"����"));
    /// 
    /// �ġ�����ɨ��
    /// 1��ɨ��
    /// Autofac����ʹ��Լ���ڳ�����ע�����Ѱ�������
    /// Autofac���Ը����û�ָ���Ĺ����ڳ�����ע��һϵ�е����ͣ����ַ�������convention-driven registration����ɨ�衣
    /// builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Manager"));
    /// ÿ��RegisterAssemblyTypes����ֻ��Ӧ��һ�׹�������ж��ײ�ͬ�ļ���Ҫע�ᣬ�Ǿ��б�Ҫ��ε���RegisterAssemblyTypes��
    /// 
    /// 2��ѡ������
    /// RegisterAssemblyTypes���ܳ��򼯵ļ��ϡ�Ĭ������£����������й���������඼�ᱻע�ᡣ
    /// �����Ҫ����ע������ͣ�����ʹ��Where.������������
    /// Where(t => t.Name.EndsWith("Manager"))
    /// �����Ҫ�ų�ĳЩ���ͣ�ʹ��Except()��Except<AutoFacManager)>()
    /// ���ߣ��Զ�����Щ�Ѿ��ų������͵�ע�᣺Except<Worker>(ct =>ct.As<IPerson>().SingleInstance())
    /// �������������ͬʱʹ�ã���ʱ����֮����AND�Ĺ�ϵ��
    /// 
    /// 3��ָ������
    /// RegisterAssemblyTypes���ע�᷽����ע�ᵥ�������ĳ�������������As�ķ���Ҳ�������ڳ����У�����
    /// As<IPerson>();
    /// As��Named������������������ط�������lambda���ʽ������������ṩʲô�������͡�
    /// 
    /// �塢�¼�
    /// ��component�������ڵĲ�ͬ�׶�ʹ���¼���
    /// Autofac��¶����¼��ӿڹ�ʵ���İ�����˳�����
    /// OnRegistered
    /// OnPreparing
    /// OnActivated
    /// OnActivating
    /// OnRelease
    /// ��Щ�¼�����ע���ʱ�򱻶��ģ����߱����ӵ�IComponentRegistration ��ʱ��
    /// builder.RegisterType<Worker>().As<IPerson>()
    ///         .OnRegistered(e => Console.WriteLine("��ע���ʱ�����!"))
    ///         .OnPreparing(e => Console.WriteLine("��׼��������ʱ�����!"))
    ///         .OnActivating(e => Console.WriteLine("�ڴ���֮ǰ����!"))
    ///         .OnActivated(e => Console.WriteLine("����֮�����!"))
    ///         .OnRelease(e => Console.WriteLine("���ͷ�ռ�õ���Դ֮ǰ����!"));
    /// 
    /// ��������ע��
    /// 
    /// �ߡ�����ע��
    /// 
    /// �ˡ�Resolve�Ĳ���
    /// 
    /// �š�Ԫ����
    /// 
    /// ʮ��ѭ������
    /// 
    /// ʮһ������
    /// 
    /// ʮ������������װ����
    /// 
    /// ʮ����ʵ����������
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
            #region ע��Http������
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
            //�Գ��������е�Controllerһ���Ե����ע�ᡣ(���еĿ�������ʵ���˽ӿ�IController)��
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            #region ���ݷ��ʲ�
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();

            //�������ݷ�����ʵ��
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

            //���
            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();
            builder.RegisterType<OfficialFeedManager>().As<IOfficialFeedManager>().InstancePerLifetimeScope();

            #region cache managers ����
            if (config.RedisCachingEnabled)
            {
                //redis����
                builder.RegisterType<RedisConnectionWrapper>().As<IRedisConnectionWrapper>().SingleInstance();
                builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").InstancePerLifetimeScope();
            }
            else
            {
                //�ڴ滺��
                builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").SingleInstance();
            }
            //http����
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

            //���һ��ע��Դ��������һ�ֳ���ע�ᡣ????
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

            #region ���ݿ��ʼ������װ��
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
           
            //·��
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            #region ע���¼�������
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
    /// ISettings�ӿڵ�ʵ�ַ���ʹ���ӳ�ע��
    /// </summary>
    public class SettingsSource : IRegistrationSource
    {
        //����ע��ķ���
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// ע�����
        /// </summary>
        /// <param name="service"></param>
        /// <param name="registrations"></param>
        /// <returns></returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService; //��������
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
