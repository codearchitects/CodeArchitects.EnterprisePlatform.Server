namespace CodeArchitects.Platform.Emit.Testing;

internal record FakeLocalBuilder(int LocalIndex, Type LocalType, bool IsPinned) : ILocalBuilder;
