namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal interface IVariableExtractorProvider
{
  VariableExtractor<TVariables> GetExtractor<TVariables>()
    where TVariables : notnull;
}
