namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal static class NamingConventionExtensions
{
  public static bool MatchesAutoGenConvention(this string name, string backingFieldName)
  {
    if (backingFieldName.Length != name.Length + 17)
      return false;

    if (backingFieldName[0] != '<')
      return false;

    if (char.ToLower(backingFieldName[0]) != char.ToLower(name[0]))
      return false;

    if (!backingFieldName.AsSpan(2, name.Length - 1).SequenceEqual(name.AsSpan(1, name.Length - 1)))
      return false;

    if (!backingFieldName.AsSpan(name.Length + 1, 16).SequenceEqual(">k__BackingField".AsSpan()))
      return false;

    return true;
  }

  public static bool MatchesCamelCaseConvention(this string name, string fieldOrLocalName)
  {
    if (fieldOrLocalName.Length != name.Length)
      return false;

    if (fieldOrLocalName[0] != char.ToLower(name[0]))
      return false;

    if (!fieldOrLocalName.AsSpan(1, name.Length - 1).SequenceEqual(name.AsSpan(1, name.Length - 1)))
      return false;

    return true;
  }

  public static bool MatchesUnderscorePrefixConvention(this string name, string fieldName)
  {
    if (fieldName.Length != name.Length + 1)
      return false;

    if (fieldName[0] != '_')
      return false;

    if (fieldName[1] != char.ToLower(name[0]))
      return false;

    if (!fieldName.AsSpan(2, name.Length - 1).SequenceEqual(name.AsSpan(1, name.Length - 1)))
      return false;

    return true;
  }

  public static bool MatchesMemberPrefixConvention(this string name, string fieldName)
  {
    if (fieldName.Length != name.Length + 2)
      return false;

    if (fieldName[0] != 'm' || fieldName[1] != '_')
      return false;

    if (fieldName[2] != char.ToLower(name[0]))
      return false;

    if (!fieldName.AsSpan(3, name.Length - 1).SequenceEqual(name.AsSpan(1, name.Length - 1)))
      return false;

    return true;
  }
}
