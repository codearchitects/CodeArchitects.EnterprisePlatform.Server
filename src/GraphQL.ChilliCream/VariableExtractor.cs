using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal delegate (IReadOnlyDictionary<string, object?> VariableDictionary, IReadOnlyDictionary<string, Upload?> FileDictionary) VariableExtractor<TVariables>(TVariables variables)
  where TVariables : notnull;