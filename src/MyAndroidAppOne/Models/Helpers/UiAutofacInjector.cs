using Autofac;

public class UiAutofacInjector
{

	public static void Initialize(ContainerBuilder builder)
	{
		// Register services
		var domainAssembly = typeof(IService).Assembly;
		var infrastructureAssembly = typeof(BaseApiService).Assembly;
		builder.RegisterAssemblyTypes(infrastructureAssembly, infrastructureAssembly, domainAssembly)
			.Where(x => x.IsClass && !x.IsAbstract && (typeof(IService).IsAssignableFrom(x)))
			.AsImplementedInterfaces()
			.InstancePerDependency();
	}
}