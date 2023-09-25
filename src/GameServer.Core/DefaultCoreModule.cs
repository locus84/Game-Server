using Autofac;
using GameServer.Core.Interfaces;

namespace GameServer.Core;
public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    // builder.RegisterType<ToDoItemSearchService>()
    //     .As<IToDoItemSearchService>().InstancePerLifetimeScope();
    //
    // builder.RegisterType<DeleteContributorService>()
    //     .As<IDeleteContributorService>().InstancePerLifetimeScope();
  }
}
