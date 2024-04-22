using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;

namespace DropBear.Codex.Utilities.Configurations;

public static class MessagePackSerializerPresets
{
    public static MessagePackSerializerOptions GetDefault()
    {
        var options = MessagePackSerializerOptions.Standard
            .WithResolver(CompositeResolver.Create(
                ImmutableCollectionResolver.Instance,
                StandardResolverAllowPrivate.Instance,
                StandardResolver.Instance
            ))
            .WithSecurity(MessagePackSecurity.UntrustedData);

        return options;
    }
}
