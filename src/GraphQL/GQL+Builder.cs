using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL;

public static partial class GQL
{
  public abstract class Builder : Builder<IDocumentRoot>
  {
    private static Type s_compilerType = typeof(DefaultCompiler<>);

    private Builder() { }

    internal static void SetCompilerType(Type compilerType)
    {
      bool isValid =
        compilerType.IsGenericType &&
        compilerType.BaseType.IsGenericType &&
        compilerType.BaseType.GetGenericTypeDefinition() == typeof(Builder<>) &&
        compilerType.GetConstructor(Type.EmptyTypes) is not null;

      if (!isValid)
        throw new ArgumentException($"'{nameof(compilerType)}' should be a generic concrete type extending '{nameof(Builder<object>)}'<> with a parameterless constructor.");

      s_compilerType = compilerType;
    }

    internal static Builder<TDocumentRoot> CreateInstance<TDocumentRoot>()
      where TDocumentRoot : class
    {
      return (Builder<TDocumentRoot>)Activator.CreateInstance(s_compilerType.MakeGenericType(typeof(TDocumentRoot)));
    }
  }

  public abstract class Builder<TDocumentRoot>
    where TDocumentRoot : class
  {
    private static readonly Lazy<Builder<TDocumentRoot>> s_lazyInstance = new(Builder.CreateInstance<TDocumentRoot>);

    private protected Builder() { }

    private protected abstract IGraphDocument<TResult> CompileDocument<TResult>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult>> buildDocument)
      where TResult : class;

    private protected abstract IGraphDocument<TResult, TVariables> CompileDocument<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult, TVariables>> buildDocument)
      where TResult : class
      where TVariables : class;

    public static IGraphDocument<TResult> Compile<TResult>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult>> buildDocument)
      where TResult : class
    {
      return s_lazyInstance.Value.CompileDocument(buildDocument);
    }

    public static IGraphDocument<TResult, TVariables> Compile<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult, TVariables>> buildDocument)
      where TResult : class
      where TVariables : class
    {
      return s_lazyInstance.Value.CompileDocument(buildDocument);
    }
  }

  private sealed class DefaultCompiler<TDocumentRoot> : Builder<TDocumentRoot>
    where TDocumentRoot : class
  {
    private protected override IGraphDocument<TResult> CompileDocument<TResult>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult>> buildDocument)
      => throw NotImplemented();

    private protected override IGraphDocument<TResult, TVariables> CompileDocument<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult, TVariables>> buildDocument)
      => throw NotImplemented();

    private static NotImplementedException NotImplemented() => new NotImplementedException("The current CodeArchitects.Platform.GraphQL implementation does not support document compilation.");
  }
}

public static class C
{
  public static readonly IGraphDocument<object> Doc = GQL.Builder.Compile(x => x.Query(_ => _
    .ExpandRef())
}