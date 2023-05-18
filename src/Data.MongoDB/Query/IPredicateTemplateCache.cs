using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal interface IPredicateTemplateCache
{
  void AddTemplate(Type type, LambdaExpression template);
  bool TryGetTemplate(Type type, [NotNullWhen(true)] out LambdaExpression? template);
}
