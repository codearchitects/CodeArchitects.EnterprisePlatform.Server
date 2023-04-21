using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal class PredicateTemplateCache : IPredicateTemplateCache
{
  private readonly IDictionary<Type, LambdaExpression> _templates;

  public PredicateTemplateCache(IDictionary<Type, LambdaExpression> templates)
  {
    _templates = templates;
  }

  public void AddTemplate(Type type, LambdaExpression template)
  {
    _templates.Add(type, template);
  }

  public bool TryGetTemplate(Type type, [NotNullWhen(true)] out LambdaExpression? template)
  {
    return _templates.TryGetValue(type, out template);
  }

  public static PredicateTemplateCache Create()
  {
    return new(new ConcurrentDictionary<Type, LambdaExpression>());
  }
}
