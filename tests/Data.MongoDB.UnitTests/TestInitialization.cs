using CodeArchitects.Platform.Data.MongoDB.Serialization;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.MongoDB;

internal static class TestInitialization
{
  /// <summary>
  /// Registers the provider's serialization settings once, before any test runs.
  /// </summary>
  /// <remarks>
  /// The BSON serializer registry is process-wide, and the test classes run in parallel. A test that
  /// looks up the Guid serializer before the provider registered its own caches the driver's default,
  /// which has no representation: every later AddData then fails, whichever test it belongs to. Which
  /// test comes first depends on the thread scheduling, so the failure would only show on some machines.
  /// </remarks>
#pragma warning disable CA2255 // A test assembly is not a library: running before any test is the point.
  [ModuleInitializer]
#pragma warning restore CA2255
  internal static void Initialize() => MongoDBSerializationInitializer.EnsureInitialized();
}
