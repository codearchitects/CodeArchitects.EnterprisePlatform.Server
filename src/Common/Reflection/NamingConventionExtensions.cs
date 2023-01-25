namespace CodeArchitects.Platform.Common.Reflection;

internal static class NamingConventionExtensions
{
  public static bool MatchesAutoGenConvention(this string propertyName, string fieldName)
  {
    if (fieldName.Length != propertyName.Length + 17)
      return false;

    if (fieldName[0] != '<')
      return false;

    if (!fieldName.AsSpan(1, propertyName.Length).SequenceEqual(propertyName.AsSpan()))
      return false;

    if (!fieldName.AsSpan(propertyName.Length + 1, 16).SequenceEqual(">k__BackingField".AsSpan()))
      return false;

    return true;
  }

  public static bool MatchesCamelCaseConvention(this string propertyName, string fieldOrLocalName)
  {
    if (fieldOrLocalName.Length != propertyName.Length)
      return false;

    if (char.ToLower(fieldOrLocalName[0]) != char.ToLower(propertyName[0]))
      return false;

    if (!fieldOrLocalName.AsSpan(1, propertyName.Length - 1).SequenceEqual(propertyName.AsSpan(1, propertyName.Length - 1)))
      return false;

    return true;
  }

  public static bool MatchesUnderscorePrefixConvention(this string propertyName, string fieldName)
  {
    if (fieldName[0] != '_')
      return false;

    if (char.ToLower(fieldName[1]) != char.ToLower(propertyName[0]))
      return false;

    if (!fieldName.AsSpan(2, propertyName.Length - 1).SequenceEqual(propertyName.AsSpan(1, propertyName.Length - 1)))
      return false;

    return true;
  }

  public static bool MatchesMemberPrefixConvention(this string propertyName, string fieldName)
  {
    if (fieldName[0] != 'm' || fieldName[1] != '_')
      return false;

    if (char.ToLower(fieldName[2]) != char.ToLower(propertyName[0]))
      return false;

    if (!fieldName.AsSpan(3, propertyName.Length - 1).SequenceEqual(propertyName.AsSpan(1, propertyName.Length - 1)))
      return false;

    return true;
  }
}
