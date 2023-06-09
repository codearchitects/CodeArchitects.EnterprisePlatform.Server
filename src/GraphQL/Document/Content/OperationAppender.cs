using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Content;

internal struct OperationAppender<TSymbol>
{
  private delegate void AppendAction<T>(ref OperationAppender<TSymbol> self, T current);
  private delegate void SeparatorCallback(ref OperationAppender<TSymbol> self);

  private readonly IDocumentContentBuilder<TSymbol> _content;
  private readonly DocumentBuilderOptions _options;
  private int _indent;

  public OperationAppender(IDocumentContentBuilder<TSymbol> content, DocumentBuilderOptions options)
  {
    _content = content;
    _options = options;
  }

  public void AppendOperationDefinition(IOperationDefinitionNode operationDefinition)
  {
    operationDefinition.OperationType.Append(_content);

    if (operationDefinition.Name is string name)
    {
      _content
        .Append(_content.Trivias.Space)
        .Append(name);
    }

    AppendVariables(operationDefinition.Variables);

    using (new SpaceScope(_content))
    {
      AppendDirectives(operationDefinition.Directives);
    }

    _content.Append(_content.Trivias.Space);
    AppendSelectionSet(operationDefinition.SelectionSet);
  }

  private void AppendVariables(IEnumerable<IVariable> variables)
  {
    using (new ArgumentScope(_content))
    {
      AppendSeparated(variables, static (ref OperationAppender<TSymbol> self, IVariable variable)
        => self.AppendVariable(variable));
    }
  }

  private void AppendVariable(IVariable variable)
  {
    _content
      .Append(_content.Punctuators.DollarSign)
      .AppendCamelized(variable.Name)
      .Append(_content.Punctuators.Colon)
      .Append(_content.Trivias.Space)
      .Append(variable.Type.Name);
  }

  private void AppendDirectives(IEnumerable<IDirectiveNode> directives)
  {
    AppendSpaced(directives, static (ref OperationAppender<TSymbol> self, IDirectiveNode directive)
      => self.AppendDirective(directive));
  }

  private void AppendDirective(IDirectiveNode directive)
  {
    _content
      .Append(_content.Punctuators.AtSign)
      .Append(directive.Name);

    AppendArguments(directive.Arguments);
  }

  private void AppendSelectionSet(ISelectionSetNode selectionSet)
  {
    _content.Append(_content.Punctuators.LeftBrace);
    _indent++;
    AppendLine();

    AppendSelections(selectionSet.Selections);

    _indent--;
    AppendLine();
    _content.Append(_content.Punctuators.RightBrace);
  }

  private void AppendSelections(IEnumerable<ISelectionNode> selections)
  {
    AppendLines(selections, static (ref OperationAppender<TSymbol> self, ISelectionNode selection)
      => self.AppendSelection(selection));
  }

  private void AppendSelection(ISelectionNode selection)
  {
    if (selection.Alias is string alias)
    {
      _content
        .AppendCamelized(alias)
        .Append(_content.Punctuators.Colon)
        .Append(_content.Trivias.Space);
    }

    _content.AppendCamelized(selection.FieldName);

    AppendArguments(selection.Arguments);

    using (new SpaceScope(_content))
    {
      AppendDirectives(selection.Directives);
    }

    if (selection.SelectionSet is ISelectionSetNode selectionSet)
    {
      _content.Append(_content.Trivias.Space);
      AppendSelectionSet(selectionSet);
    }
  }

  private void AppendArguments(IEnumerable<IArgumentNode> arguments)
  {
    using (new ArgumentScope(_content))
    {
      AppendSeparated(arguments, static (ref OperationAppender<TSymbol> self, IArgumentNode argument)
        => self.AppendArgument(argument));
    }
  }

  private void AppendArgument(IArgumentNode argument)
  {
    _content
      .AppendCamelized(argument.Name)
      .Append(_content.Punctuators.Colon)
      .Append(_content.Trivias.Space);

    AppendValue(argument.Value);
  }

  private void AppendValue(object? value)
  {
    if (value is null)
    {
      _content.Append(_content.Keywords.Null);
      return;
    }

    switch (Convert.GetTypeCode(value))
    {
      case >= TypeCode.SByte and <= TypeCode.UInt64:
        _content.AppendLiteral(Convert.ToInt32(value));
        return;

      case >= TypeCode.Single and <= TypeCode.Decimal:
        _content.AppendLiteral(Convert.ToSingle(value));
        return;

      case TypeCode.String:
        _content.AppendLiteral(Convert.ToString(value));
        return;

      case TypeCode.Boolean:
        _content.Append(Convert.ToBoolean(value)
          ? _content.Keywords.True
          : _content.Keywords.False);
        return;

      case TypeCode.DateTime:
        throw new NotSupportedException("DateTime is not yet supported."); // TODO: Support
    }

    switch (value)
    {
      case MemberInfo member:
        _content
          .Append(_content.Punctuators.DollarSign)
          .AppendCamelized(member.Name);
        return;

      case IObjectValueNode objectValue:
        AppendObjectValue(objectValue);
        return;

      case IListValueNode listValue:
        AppendListValue(listValue);
        return;
    }

    throw new NotSupportedException($"Value of type '{value.GetType()}' is not supported.");
  }

  private void AppendObjectValue(IObjectValueNode objectValue)
  {
    _content
      .Append(_content.Punctuators.LeftBrace)
      .Append(_content.Trivias.Space);

    AppendSeparated(objectValue.Fields, static (ref OperationAppender<TSymbol> self, IObjectFieldNode field)
      => self.AppendObjectField(field));

    _content
      .Append(_content.Trivias.Space)
      .Append(_content.Punctuators.RightBrace);
  }

  private void AppendObjectField(IObjectFieldNode field)
  {
    _content
      .AppendCamelized(field.Name)
      .Append(_content.Punctuators.Colon)
      .Append(_content.Trivias.Space);

    AppendValue(field.Value);
  }

  private void AppendListValue(IListValueNode listValue)
  {
    _content.Append(_content.Punctuators.LeftBracket);

    AppendSeparated(listValue.Values, static (ref OperationAppender<TSymbol> self, object? value)
      => self.AppendValue(value));

    _content.Append(_content.Punctuators.RightBracket);
  }

  private void AppendLine()
  {
    _options.LinePolicy.Append(_content, _indent);
  }

  private void AppendLines<T>(IEnumerable<T> values, AppendAction<T> append)
  {
    AppendJoin(values, AppendLine, append);

    static void AppendLine(ref OperationAppender<TSymbol> self)
      => self.AppendLine();
  }

  private void AppendSeparated<T>(IEnumerable<T> values, AppendAction<T> append)
  {
    AppendJoin(values, AppendSeparator, append);

    static void AppendSeparator(ref OperationAppender<TSymbol> self)
      => self._options.Separator.Append(self._content);
  }

  private void AppendSpaced<T>(IEnumerable<T> values, AppendAction<T> append)
  {
    AppendJoin(values, AppendSpace, append);

    static void AppendSpace(ref OperationAppender<TSymbol> self)
      => self._content.Append(self._content.Trivias.Space);
  }

  private void AppendJoin<T>(IEnumerable<T> values, SeparatorCallback separatorCallback, AppendAction<T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return;

    append(ref this, enumerator.Current);

    while (enumerator.MoveNext())
    {
      separatorCallback(ref this);
      append(ref this, enumerator.Current);
    }
  }

  private readonly ref struct ArgumentScope
  {
    private readonly IDocumentContentBuilder<TSymbol> _content;
    private readonly int _length;

    public ArgumentScope(IDocumentContentBuilder<TSymbol> content)
    {
      _content = content;

      _content.Append(_content.Punctuators.LeftParenthesis);
      _length = content.Length;
    }

    public void Dispose()
    {
      if (_content.Length == _length)
      {
        _content.Pop();
      }
      else
      {
        _content.Append(_content.Punctuators.RightParenthesis);
      }
    }
  }

  private readonly ref struct SpaceScope
  {
    private readonly IDocumentContentBuilder<TSymbol> _content;
    private readonly int _length;

    public SpaceScope(IDocumentContentBuilder<TSymbol> content)
    {
      _content = content;

      _content.Append(_content.Trivias.Space);
      _length = content.Length;
    }

    public void Dispose()
    {
      if (_content.Length == _length)
      {
        _content.Pop();
      }
    }
  }
}
