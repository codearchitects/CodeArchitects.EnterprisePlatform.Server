using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Buffers;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal ref struct DocumentRenderer
{
  private delegate void AppendAction<T>(ref DocumentRenderer self, T element);
  private delegate void AppendAction(ref DocumentRenderer self);

  private static readonly AppendAction s_appendNothing          = static (ref DocumentRenderer self) => { };
  private static readonly AppendAction s_appendLeftParenthesis  = static (ref DocumentRenderer self) => self._sb.AppendLeftParenthesis();
  private static readonly AppendAction s_appendRightParenthesis = static (ref DocumentRenderer self) => self._sb.AppendRightParenthesis();
  private static readonly AppendAction s_appendSpace            = static (ref DocumentRenderer self) => self._sb.AppendSpace();

  private readonly Utf8StringBuilder _sb;
  private readonly DocumentBuilderOptions _options;
  private int _indent;

  public DocumentRenderer(IBufferWriter<byte> writer, DocumentBuilderOptions options)
  {
    _sb = new Utf8StringBuilder(writer);
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

    AppendDirectives(operationDefinition.Directives);

    AppendSelectionSet(operationDefinition.SelectionSet);
  }

  private void AppendVariables(IEnumerable<IVariable> variables)
  {
    AppendSeparated(
      variables,
      static (ref DocumentRenderer self, IVariable variable) => self.AppendVariable(variable),
      s_appendLeftParenthesis,
      s_appendRightParenthesis);
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
    AppendSpaced(
      directives,
      static (ref DocumentRenderer self, IDirectiveNode directive) => self.AppendDirective(directive),
      s_appendSpace,
      s_appendNothing);
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
    _sb.AppendSpace();

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
    AppendLines(selections, static (ref DocumentRenderer self, ISelectionNode selection)
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

    AppendDirectives(selection.Directives);

    if (selection.SelectionSet is ISelectionSetNode selectionSet)
    {
      AppendSelectionSet(selectionSet);
    }
  }

  private void AppendArguments(IEnumerable<IArgumentNode> arguments)
  {
    AppendSeparated(
      arguments,
      static (ref DocumentRenderer self, IArgumentNode argument) => self.AppendArgument(argument),
      s_appendLeftParenthesis,
      s_appendRightParenthesis);
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

    AppendSeparated(objectValue.Fields, static (ref DocumentRenderer self, IObjectFieldNode field) => self.AppendObjectField(field));

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

    AppendSeparated(listValue.Values, static (ref DocumentRenderer self, object? value) => self.AppendValue(value));

    _sb.AppendRightBracket();
  }

  private void AppendLine()
  {
    _options.LinePolicy.AppendOn(_sb, _indent);
  }

  private void AppendLines<T>(IEnumerable<T> values, AppendAction<T> appendElement)
  {
    AppendJoin(values, appendElement, AppendLine, s_appendNothing, s_appendNothing);

    static void AppendLine(ref DocumentRenderer self) => self.AppendLine();
  }

  private void AppendSeparated<T>(IEnumerable<T> values, AppendAction<T> appendElement)
  {
    AppendJoin(values, appendElement, AppendSeparator, s_appendNothing, s_appendNothing);

    static void AppendSeparator(ref DocumentRenderer self) => self._options.Separator.AppendOn(self._sb);
  }

  private void AppendSpaced<T>(IEnumerable<T> values, AppendAction<T> appendElement)
  {
    AppendJoin(values, appendElement, AppendSpace, s_appendNothing, s_appendNothing);

    static void AppendSpace(ref DocumentRenderer self) => self._sb.AppendSpace();
  }

  private void AppendLines<T>(IEnumerable<T> values, AppendAction<T> appendElement, AppendAction appendBeforeFirst, AppendAction appendAfterLast)
  {
    AppendJoin(values, appendElement, AppendLine, appendBeforeFirst, appendAfterLast);

    static void AppendLine(ref DocumentRenderer self) => self.AppendLine();
  }

  private void AppendSeparated<T>(IEnumerable<T> values, AppendAction<T> appendElement, AppendAction appendBeforeFirst, AppendAction appendAfterLast)
  {
    AppendJoin(values, appendElement, AppendSeparator, appendBeforeFirst, appendAfterLast);

    static void AppendSeparator(ref DocumentRenderer self) => self._options.Separator.AppendOn(self._sb);
  }

  private void AppendSpaced<T>(IEnumerable<T> values, AppendAction<T> appendElement, AppendAction appendBeforeFirst, AppendAction appendAfterLast)
  {
    AppendJoin(values, appendElement, AppendSpace, appendBeforeFirst, appendAfterLast);

    static void AppendSpace(ref DocumentRenderer self) => self._sb.AppendSpace();
  }

  private void AppendJoin<T>(IEnumerable<T> values, AppendAction<T> appendElement, AppendAction appendSeparator, AppendAction appendBeforeFirst, AppendAction appendAfterLast)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return;

    appendBeforeFirst(ref this);
    appendElement(ref this, enumerator.Current);

    while (enumerator.MoveNext())
    {
      appendSeparator(ref this);
      appendElement(ref this, enumerator.Current);
    }

    appendAfterLast(ref this);
  }
}
