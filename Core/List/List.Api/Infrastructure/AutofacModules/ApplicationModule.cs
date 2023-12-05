using Autofac;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using RecAll.Core.List.Infrastructure.Repositories;

namespace RecAll.Core.List.Api.Infrastructure.AutofacModules;

public class ApplicationModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<MockIdentityService>().As<IIdentityService>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ListRepository>().As<IListRepository>()
            .InstancePerLifetimeScope();
        builder.RegisterType<SetRepository>().As<ISetRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ListQueryService>().As<IListQueryService>()
            .InstancePerLifetimeScope();
        builder.RegisterType<SetQueryService>().As<ISetQueryService>()
            .InstancePerLifetimeScope();
    }
}