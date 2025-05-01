using System.ComponentModel.DataAnnotations.Schema;
using DotnetCqrs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetCqrs.Features.Users;

[Table("Users")]
public class UsersEntity : IdEntity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public TimeOnly? StartTime { get; set; }
}

public class UserConfig : IEntityTypeConfiguration<UsersEntity>
{
    public void Configure(EntityTypeBuilder<UsersEntity> builder)
    {
        builder.Property(m => m.Username)
            .HasMaxLength(50);
        builder.HasIndex(m => m.Username)
            .IsUnique();
        builder.Property(m => m.Email)
            .HasMaxLength(200);
        builder.Property(m => m.StartDate)
            .HasConversion(
                v => v,
                v => v == null ? null : v.Value.AddHours(-7)
            );
    }
}