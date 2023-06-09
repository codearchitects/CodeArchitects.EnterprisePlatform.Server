using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal struct OperationAppender
{
  private delegate void AppendAction<T>(ref OperationAppender self, T current);
  private delegate void SeparatorCallback(ref OperationAppender self);

  private readonly Utf8StringBuilder _sb;
  private readonly DocumentBuilderOptions _options;
  private int _indent;

  public OperationAppender(MemoryStream ms, DocumentBuilderOptions options)
  {
    _sb = new Utf8StringBuilder(ms);
    _options = options;
  }

  public void AppendOperationDefinition(IOperationDefinitionNode operationDefinition)
  {
    _sb.AppendOperationType(operationDefinition.OperationType);

    if (operationDefinition.Name is string name)
    {
      _sb
        .AppendSpace()
        .Append(name);
    }

    AppendVariables(operationDefinition.Variables);

    using (new SpaceScope(_sb))
    {
      AppendDirectives(operationDefinition.Directives);
    }

    _sb.AppendSpace();
    AppendSelectionSet(operationDefinition.SelectionSet);
  }

  private void AppendVariables(IEnumerable<IVariable> variables)
  {
    using (new ArgumentScope(_sb))
    {
      AppendSeparated(variables, static (ref OperationAppender self, IVariable variable)
        => self.AppendVariable(variable));
    }
  }

  private void AppendVariable(IVariable variable)
  {
    _sb
      .AppendVariablePrefix()
      .AppendCamelized(variable.Name)
      .AppendColon()
      .AppendSpace()
      .Append(variable.Type.Name);
  }

  private void AppendDirectives(IEnumerable<IDirectiveNode> directives)
  {
    AppendSpaced(directives, static (ref OperationAppender self, IDirectiveNode directive)
      => self.AppendDirective(directive));
  }

  private void AppendDirective(IDirectiveNode directive)
  {
    _sb
      .AppendDirectivePrefix()
      .Append(directive.Name);

    AppendArguments(directive.Arguments);
  }

  private void AppendSelectionSet(ISelectionSetNode selectionSet)
  {
    _sb.AppendLeftBrace();
    _indent++;
    AppendLine();

    AppendSelections(selectionSet.Selections);

    _indent--;
    AppendLine();
    _sb.AppendRightBrace();
  }

  private void AppendSelections(IEnumerable<ISelectionNode> selections)
  {
    AppendLines(selections, static (ref OperationAppender self, ISelectionNode selection)
      => self.AppendSelection(selection));
  }

  private void AppendSelection(ISelectionNode selection)
  {
    if (selection.Alias is string alias)
    {
      _sb
        .AppendCamelized(alias)
        .AppendColon()
        .AppendSpace();
    }

    _sb.AppendCamelized(selection.FieldName);

    AppendArguments(selection.Arguments);

    using (new SpaceScope(_sb))
    {
      AppendDirectives(selection.Directives);
    }

    if (selection.SelectionSet is ISelectionSetNode selectionSet)
    {
      _sb.AppendSpace();
      AppendSelectionSet(selectionSet);
    }
  }

  private void AppendArguments(IEnumerable<IArgumentNode> arguments)
  {
    using (new ArgumentScope(_sb))
    {
      AppendSeparated(arguments, static (ref OperationAppender self, IArgumentNode argument)
        => self.AppendArgument(argument));
    }
  }

  private void AppendArgument(IArgumentNode argument)
  {
    _sb
      .AppendCamelized(argument.Name)
      .AppendColon()
      .AppendSpace();

    AppendValue(argument.Value);
  }

  private void AppendValue(object? value)
  {
    if (value is null)
    {
      _sb.AppendNullKeyword();
      return;
    }

    switch (Convert.GetTypeCode(value))
    {
      case >= TypeCode.SByte and <= TypeCode.UInt64:
        _sb.AppendLiteral(Convert.ToInt32(value));
        return;

      case >= TypeCode.Single and <= TypeCode.Decimal:
        _sb.AppendLiteral(Convert.ToSingle(value));
        return;

      case TypeCode.String:
        _sb.AppendLiteral(Convert.ToString(value));
        return;

      case TypeCode.Boolean:
        _sb.AppendLiteral(Convert.ToBoolean(value));
        return;

      case TypeCode.DateTime:
        throw new NotSupportedException("DateTime is not yet supported."); // TODO: Support
    }

    switch (value)
    {
      case MemberInfo member:
        _sb
          .AppendVariablePrefix()
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
    _sb
      .AppendLeftBrace()
      .AppendSpace();

    AppendSeparated(objectValue.Fields, static (ref OperationAppender self, IObjectFieldNode field)
      => self.AppendObjectField(field));

    _sb
      .AppendSpace()
      .AppendRightBrace();
  }

  private void AppendObjectField(IObjectFieldNode field)
  {
    _sb
      .AppendCamelized(field.Name)
      .AppendColon()
      .AppendSpace();

    AppendValue(field.Value);
  }

  private void AppendListValue(IListValueNode listValue)
  {
    _sb.AppendLeftBracket();

    AppendSeparated(listValue.Values, static (ref OperationAppender self, object? value)
      => self.AppendValue(value));

    _sb.AppendRightBracket();
  }

  private void AppendLine()
  {
    _options.LinePolicy.AppendOn(_sb, _indent);
  }

  private void AppendLines<T>(IEnumerable<T> values, AppendAction<T> append)
  {
    AppendJoin(values, AppendLine, append);

    static void AppendLine(ref OperationAppender self)
      => self.AppendLine();
  }

  private void AppendSeparated<T>(IEnumerable<T> values, AppendAction<T> append)
  {
    AppendJoin(values, AppendSeparator, append);

    static void AppendSeparator(ref OperationAppender self)
      => self._options.Separator.AppendOn(self._sb);
  }

  private void AppendSpaced<T>(IEnumerable<T> values, AppendAction<T> append)
  {
    AppendJoin(values, AppendSpace, append);

    static void AppendSpace(ref OperationAppender self)
      => self._sb.AppendSpace();
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
    private readonly Utf8StringBuilder _sb;
    private readonly long _length;

    public ArgumentScope(Utf8StringBuilder sb)
    {
      _sb = sb;

      _sb.AppendLeftParenthesis();
      _length = sb.Length;
    }

    public void Dispose()
    {
      if (_sb.Length == _length)
      {
        _sb.Pop();
      }
      else
      {
        _sb.AppendRightParenthesis();
      }
    }
  }

  private readonly ref struct SpaceScope
  {
    private readonly Utf8StringBuilder _sb;
    private readonly long _length;

    public SpaceScope(Utf8StringBuilder sb)
    {
      _sb = sb;

      _sb.AppendSpace();
      _length = sb.Length;
    }

    public void Dispose()
    {
      if (_sb.Length == _length)
      {
        _sb.Pop();
      }
    }
  }
}
