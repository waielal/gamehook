using GameHook.Domain.Interfaces;
using GameHook.WebAPI.Controllers;
using Mapster;

namespace GameHook.WebAPI
{
    public static class Mapping
    {
        public static void Setup()
        {
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;

            TypeAdapterConfig<KeyValuePair<string, IGameHookProperty>, PropertyModel>.NewConfig()
                .Map(dest => dest.Path, src => src.Key)
                .Map(dest => dest.Type, src => src.Value.Type)
                .Map(dest => dest.Address, src => src.Value.Address)
                .Map(dest => dest.Size, src => src.Value.Size)
                .Map(dest => dest.Position, src => src.Value.Fields.Position)
                .Map(dest => dest.Reference, src => src.Value.Fields.Reference)
                .Map(dest => dest.Value, src => src.Value.Value)
                .Map(dest => dest.Bytes, src => src.Value.Bytes)
                .Map(dest => dest.Frozen, src => src.Value.Frozen)
                .Map(dest => dest.Description, src => src.Value.Fields.Description);
        }
    }
}
