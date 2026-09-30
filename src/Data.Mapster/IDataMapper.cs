using MapsterMapper;

namespace CodeArchitects.Platform.Data.Mapster;

/// <summary>
/// The mapper of the data layer, registered by <c>AddDataMapster</c>.
/// </summary>
/// <remarks>
/// A dedicated service rather than <see cref="IMapper"/>: the application can keep its own <see cref="IMapper"/>,
/// for example the one registered by Mapster's <c>AddMapster</c>, without the two configurations replacing each other.
/// </remarks>
public interface IDataMapper : IMapper
{
}
