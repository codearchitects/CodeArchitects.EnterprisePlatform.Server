namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IBuildResult<TField>
{
}

internal interface IBuildResult<TField, TVariables>
  where TVariables : notnull
{
}
