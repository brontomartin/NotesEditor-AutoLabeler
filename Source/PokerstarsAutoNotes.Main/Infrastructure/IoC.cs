using Autofac;
using PokerstarsAutoNotes.Infrastructure.Database;
using PokerstarsAutoNotes.Ratings;
using PokerstarsAutoNotes.Resolver;
using PokerstarsAutoNotes.Xml.Definitions;

namespace PokerstarsAutoNotes.Infrastructure
{
    public static class IoC
    {
        private static IContainer BaseContainer { get; set; }

        public static void Build()
        {
            if (BaseContainer != null) return;

            var builder = new ContainerBuilder();

            builder.RegisterType<RatingText>().AsSelf();

            builder.RegisterType<SinglePlayerRating>().AsSelf().SingleInstance();

            builder.RegisterType<PlayersRating>().AsSelf();

            builder.RegisterType<DatabaseResolver>().AsSelf();

            builder.RegisterType<AvailableDatabaseResolver>().AsSelf();

            builder.RegisterType<NoteFileResolver>().AsSelf().SingleInstance();

            builder.RegisterType<RatingDefinitions>().As<IRatingDefinitions>().SingleInstance();

            BaseContainer = builder.Build();
        }

        public static T Resolve<T>()
        {
            return BaseContainer.Resolve<T>();
        }
    }
}
