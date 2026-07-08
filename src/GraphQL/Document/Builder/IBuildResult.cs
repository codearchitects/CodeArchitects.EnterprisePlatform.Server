namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IBuildResult<TField>
{
}

public interface IBuildResult<TField, TVariables>
  where TVariables : notnull
{
}
