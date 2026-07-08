namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IBuilder
{
}

public interface IBuilder<TVariables>
  where TVariables : notnull
{
}
