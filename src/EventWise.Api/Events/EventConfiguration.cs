using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventWise.Api.Events;

public sealed class EventConfiguration : IEntityTypeConfiguration<BaseEvent>
{
    public void Configure(EntityTypeBuilder<BaseEvent> builder)
    {
        builder.Property(e => e.Name)
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Location)
            .HasMaxLength(100);

        builder.HasDiscriminator<string>("EventType")
            .HasValue<UserEvent>(nameof(UserEvent));
    }
}
