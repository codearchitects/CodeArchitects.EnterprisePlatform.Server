using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Raw;
using System.Buffers;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal ref struct DocumentRenderer
{
  private delegate void AppendAction<T>(ref DocumentRenderer self, T element);
  private delegate void AppendAction(ref DocumentRenderer self);

  private readonly Utf8StringBuilder _sb;
  private readonly DocumentSerializationOptions _options;
  private int _indent;

  public DocumentRenderer(ArrayBufferWriter<byte> writer, DocumentSerializationOptions options)
  {
    _sb = new Utf8StringBuilder(writer);
    _options = options;
  }

  public void AppendDocument(IDocumentNode node)
  {
    AppendLines(node.Definitions, static (ref DocumentRenderer self, IDefinitionNode definition) => self.AppendDefinition(definition));
  }

  private void AppendDefinition(IDefinitionNode definition)
  {
    switch (definition.DefinitionKind)
    {
      case DefinitionNodeKind.OperationDefinition:
        AppendOperationDefinition((IOperationDefinitionNode)definition);
        break;

      case DefinitionNodeKind.FragmentDefinition:
        AppendFragmentDefinition((IFragmentDefinitionNode)definition);
        break;

      default:
        throw new NotSupportedException("Unsupported definition node kind.");
    }
  }

  private void AppendOperationDefinition(IOperationDefinitionNode operationDefinition)
  {
    if (operationDefinition.IsQueryShortHand)
    {
      AppendSelectionSet(operationDefinition.SelectionSet);
      return;
    }

    _sb.AppendOperationType(operationDefinition.OperationType);

    if (operationDefinition.Name is { Length: > 0 } name)
    {
      _sb
        .AppendSpace()
        .Append(name);
    }

    if (operationDefinition.VariableDefinitionList is { } variableDefinitions)
    {
      AppendVariableDefinitions(variableDefinitions);
    }

    if (operationDefinition.DirectiveList is { } directiveList)
    {
      _sb.AppendSpace();

      AppendDirectiveList(directiveList);
    }

    _sb.AppendSpace();

    AppendSelectionSet(operationDefinition.SelectionSet);
  }

  private void AppendFragmentDefinition(IFragmentDefinitionNode fragmentDefinition)
  {
    _sb
      .Append("fragment ")
      .Append(fragmentDefinition.FragmentName)
      .AppendSpace();

    AppendTypeCondition(fragmentDefinition.TypeCondition);

    if (fragmentDefinition.DirectiveList is { } directiveList)
    {
      _sb.AppendSpace();

      AppendDirectiveList(directiveList);
    }

    _sb.AppendSpace();

    AppendSelectionSet(fragmentDefinition.SelectionSet);
  }

  private void AppendVariableDefinitions(IVariableDefinitionListNode variableDefinitions)
  {
    _sb.AppendLeftParenthesis();

    AppendSeparated(variableDefinitions.VariableDefinitions, static (ref DocumentRenderer self, IVariableDefinitionNode variableDefinition) => self.AppendVariableDefinition(variableDefinition));

    _sb.AppendRightParenthesis();
  }

  private void AppendVariableDefinition(IVariableDefinitionNode variableDefinition)
  {
    AppendVariable(variableDefinition.Variable);

    _sb
      .AppendColon()
      .AppendSpace();

    AppendType(variableDefinition.Type);
  }

  private void AppendVariable(IVariableNode variable)
  {
    _sb
      .AppendDollar()
      .Append(variable.Name);
  }

  private void AppendType(ITypeNode type)
  {
    switch (type.TypeKind)
    {
      case TypeNodeKind.NamedType:
        AppendNamedType((INamedTypeNode)type);
        break;

      case TypeNodeKind.ListType:
        AppendListType((IListTypeNode)type);
        break;

      case TypeNodeKind.NonNullType:
        AppendNonNullType((INonNullTypeNode)type);
        break;

      default:
        throw new InvalidOperationException($"Type node of kind {type.TypeKind} is not supported.");
    }
  }

  private void AppendNamedType(INamedTypeNode namedType)
  {
    _sb.Append(namedType.Name);
  }

  private void AppendListType(IListTypeNode namedType)
  {
    _sb.AppendLeftBracket();

    AppendType(namedType.ItemType);
    
    _sb.AppendRightBracket();
  }

  private void AppendNonNullType(INonNullTypeNode nonNullType)
  {
    AppendType(nonNullType.NullableType);

    _sb.AppendBang();
  }

  private void AppendDirectiveList(IDirectiveListNode directiveList)
  {
    AppendSpaced(directiveList.Directives, static (ref DocumentRenderer self, IDirectiveNode directive) => self.AppendDirective(directive));
  }

  private void AppendDirective(IDirectiveNode directive)
  {
    _sb
      .AppendAt()
      .Append(directive.Name);

    if (directive.ArgumentList is { } argumentList)
    {
      AppendArgumentList(argumentList);
    }
  }

  private void AppendSelectionSet(ISelectionSetNode selectionSet)
  {
    _sb.AppendLeftBrace();
    _indent++;
    AppendLine();

    AppendLines(selectionSet.Selections, static (ref DocumentRenderer self, ISelectionNode selection) => self.AppendSelection(selection));

    _indent--;
    AppendLine();
    _sb.AppendRightBrace();
  }

  private void AppendSelection(ISelectionNode selection)
  {
    switch (selection.SelectionKind)
    {
      case SelectionNodeKind.Field:
        AppendField((IFieldNode)selection);
        break;

      case SelectionNodeKind.FragmentSpread:
        AppendFragmentSpread((IFragmentSpreadNode)selection);
        break;

      case SelectionNodeKind.InlineFragment:
        AppendInlineFragment((IInlineFragmentNode)selection);
        break;

      default:
        throw new InvalidOperationException($"Selection node of kind {selection.SelectionKind} is not supported.");
    }
  }

  private void AppendField(IFieldNode field)
  {
    if (field.Alias is { Length: > 0 } alias)
    {
      _sb
        .Append(alias)
        .AppendColon()
        .AppendSpace();
    }

    _sb.Append(field.FieldName);

    if (field.ArgumentList is { } argumentList)
    {
      AppendArgumentList(argumentList);
    }

    if (field.DirectiveList is { } directiveList)
    {
      _sb.AppendSpace();

      AppendDirectiveList(directiveList);
    }

    if (field.SelectionSet is ISelectionSetNode selectionSet)
    {
      _sb.AppendSpace();

      AppendSelectionSet(selectionSet);
    }
  }

  private void AppendFragmentSpread(IFragmentSpreadNode fragmentSpread)
  {
    _sb
      .AppendSpread()
      .Append(fragmentSpread.FragmentName);

    if (fragmentSpread.DirectiveList is { } directiveList)
    {
      _sb.AppendSpace();

      AppendDirectiveList(directiveList);
    }
  }

  private void AppendInlineFragment(IInlineFragmentNode inlineFragment)
  {
    if (inlineFragment.TypeCondition is { } typeCondition)
    {
      _sb.AppendSpace();
      AppendTypeCondition(typeCondition);
    }

    if (inlineFragment.DirectiveList is { } directiveList)
    {
      _sb.AppendSpace();

      AppendDirectiveList(directiveList);
    }

    _sb.AppendSpace();

    AppendSelectionSet(inlineFragment.SelectionSet);
  }

  private void AppendTypeCondition(ITypeConditionNode typeCondition)
  {
    _sb.Append("on ");

    AppendNamedType(typeCondition.Type);
  }

  private void AppendArgumentList(IArgumentListNode argumentList)
  {
    _sb.AppendLeftParenthesis();

    AppendSeparated(argumentList.Arguments, static (ref DocumentRenderer self, IArgumentNode argument) => self.AppendArgument(argument));

    _sb.AppendRightParenthesis();
  }

  private void AppendArgument(IArgumentNode argument)
  {
    _sb
      .Append(argument.Name)
      .AppendColon()
      .AppendSpace();

    AppendValue(argument.Value);
  }

  private void AppendValue(IValueNode value)
  {
    if (value is IRawLiteralNode rawValue)
    {
      _sb.Append(rawValue.ValueText);
      return;
    }

    switch (value.ValueKind)
    {
      case ValueNodeKind.Variable:
        AppendVariable((IVariableNode)value);
        break;

      case ValueNodeKind.IntValue:
        AppendIntValue((IIntValueNode)value);
        break;

      case ValueNodeKind.FloatValue:
        AppendFloatValue((IFloatValueNode)value);
        break;

      case ValueNodeKind.StringValue:
        AppendStringValue((IStringValueNode)value);
        break;

      case ValueNodeKind.BlockStringValue:
        AppendBlockStringValue((IBlockStringValueNode)value);
        break;

      case ValueNodeKind.BooleanValue:
        AppendBooleanValue((IBooleanValueNode)value);
        break;

      case ValueNodeKind.NullValue:
        _sb.AppendNull();
        break;

      case ValueNodeKind.EnumValue:
        AppendEnumValue((IEnumValueNode)value);
        break;

      case ValueNodeKind.ListValue:
        AppendListValue((IListValueNode)value);
        break;

      case ValueNodeKind.ObjectValue:
        AppendObjectValue((IObjectValueNode)value);
        break;
    }
  }

  private void AppendIntValue(IIntValueNode intValue)
  {
    if (intValue is IRawLiteralNode rawValue)
    {
      AppendRawLiteral(rawValue);
      return;
    }

    _sb.AppendLiteral(intValue.Value);
  }

  private void AppendFloatValue(IFloatValueNode floatValue)
  {
    if (floatValue is IRawLiteralNode rawValue)
    {
      AppendRawLiteral(rawValue);
      return;
    }

    _sb.AppendLiteral(floatValue.Value);
  }

  private void AppendStringValue(IStringValueNode stringValue)
  {
    if (stringValue is IRawLiteralNode rawValue)
    {
      AppendRawLiteral(rawValue);
      return;
    }

    _sb.AppendLiteral(stringValue.Value);
  }

  private void AppendBlockStringValue(IBlockStringValueNode blockStringValue)
  {
    throw new NotImplementedException();
  }

  private void AppendBooleanValue(IBooleanValueNode booleanValue)
  {
    if (booleanValue is IRawLiteralNode rawValue)
    {
      AppendRawLiteral(rawValue);
      return;
    }

    _sb.AppendLiteral(booleanValue.Value);
  }

  private void AppendEnumValue(IEnumValueNode enumValue)
  {
    if (enumValue is IRawLiteralNode rawValue)
    {
      AppendRawLiteral(rawValue);
      return;
    }

    _sb.Append(enumValue.Value);
  }

  private void AppendListValue(IListValueNode listValue)
  {
    _sb.AppendLeftBracket();

    AppendSeparated(listValue.Values, static (ref DocumentRenderer self, IValueNode value) => self.AppendValue(value));

    _sb.AppendRightBracket();
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
      .Append(field.Name)
      .AppendColon()
      .AppendSpace();

    AppendValue(field.Value);
  }

  private void AppendRawLiteral(IRawLiteralNode rawLiteral)
  {
    _sb.Append(rawLiteral.ValueText);
  }

  private void AppendLine()
  {
    _options.LinePolicy.AppendOn(_sb, _indent);
  }

  private void AppendLines<T>(IEnumerable<T> values, AppendAction<T> appendElement)
  {
    AppendJoin(values, appendElement, AppendLine);

    static void AppendLine(ref DocumentRenderer self) => self.AppendLine();
  }

  private void AppendSeparated<T>(IEnumerable<T> values, AppendAction<T> appendElement)
  {
    AppendJoin(values, appendElement, AppendSeparator);

    static void AppendSeparator(ref DocumentRenderer self) => self._options.Separator.AppendOn(self._sb);
  }

  private void AppendSpaced<T>(IEnumerable<T> values, AppendAction<T> appendElement)
  {
    AppendJoin(values, appendElement, AppendSpace);

    static void AppendSpace(ref DocumentRenderer self) => self._sb.AppendSpace();
  }

  private void AppendJoin<T>(IEnumerable<T> values, AppendAction<T> appendElement, AppendAction appendSeparator)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return;

    appendElement(ref this, enumerator.Current);

    while (enumerator.MoveNext())
    {
      appendSeparator(ref this);
      appendElement(ref this, enumerator.Current);
    }
  }
}
