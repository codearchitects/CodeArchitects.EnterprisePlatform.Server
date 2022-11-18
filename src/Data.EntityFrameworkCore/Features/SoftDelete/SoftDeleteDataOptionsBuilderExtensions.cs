using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

public static class SoftDeleteDataOptionsBuilderExtensions
{
  public static DataOptionsBuilder UseSoftDelete(this DataOptionsBuilder builder, ISoftDeleteDescriptor descriptor)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(descriptor);

    return builder.UseSoftDeleteCore(descriptor);
  }

  public static DataOptionsBuilder UseSoftDelete<TSoftDeleteDescriptor>(this DataOptionsBuilder builder)
    where TSoftDeleteDescriptor : ISoftDeleteDescriptor, new()
  {
    ArgumentNullException.ThrowIfNull(builder);

    return builder.UseSoftDeleteCore(new TSoftDeleteDescriptor());
  }

  public static DataOptionsBuilder UseSoftDelete(this DataOptionsBuilder builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    return builder.UseSoftDeleteCore(new DefaultSoftDeleteDescriptor());
  }

  private static DataOptionsBuilder UseSoftDeleteCore(this DataOptionsBuilder builder, ISoftDeleteDescriptor descriptor)
  {
    if (descriptor.SoftDeleteContextType is null && descriptor.SoftDeleteContextImplementationFactory is null)
      throw new ArgumentException($"The descriptor must specify a {nameof(ISoftDeleteDescriptor)} type or factory.", nameof(descriptor));

    (builder as IDataOptionsBuilderInfrastructure).AddOrUpdatePlugin(new SoftDeleteDataPlugin(descriptor));
    return builder;
  }
}
