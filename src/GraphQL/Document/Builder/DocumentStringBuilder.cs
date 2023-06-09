using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal struct DocumentStringBuilder
{
  private const string s_nullKeyword = "null";
  private const string s_trueKeyword = "true";
  private const string s_falseKeyword = "false";
  private const char s_variablePrefix = '$';
  private const char s_directivePrefix = '@';

  private readonly StringBuilder _sb;
  private readonly DocumentBuilderOptions _options;
  private int _indent;

  public DocumentStringBuilder(StringBuilder sb, DocumentBuilderOptions options)
  {
    _sb = sb;
    _options = options;
  }

  private string Separator => _options.Separator.ToString();

  public void AppendOperationDefinition(IOperationDefinitionNode operationDefinition)
  {
    _sb.Append(operationDefinition.OperationType);

    if (operationDefinition.Name is string name)
    {
      _sb
        .Append(' ')
        .Append(name);
    }

    AppendVariables(operationDefinition.Variables);

    using (new SpaceScope(_sb))
    {
      AppendDirectives(operationDefinition.Directives);
    }

    using (new SpaceScope(_sb))
    {
      AppendSelectionSet(operationDefinition.SelectionSet);
    }
  }

  private void AppendVariables(IEnumerable<IVariable> variables)
  {
    using (new ArgumentScope(_sb))
    {
      _sb
        .AppendJoin(Separator, variables, static (sb, variable) => sb
          .Append(s_variablePrefix)
          .AppendCamelized(variable.Name)
          .Append(": ")
          .Append(variable.Type.Name));
    }
  }

  private void AppendDirectives(IEnumerable<IDirectiveNode> directives)
  {
    _sb.AppendJoin(' ', ref this, directives, static delegate (StringBuilder sb, ref DocumentStringBuilder self, IDirectiveNode directive)
    {
      sb.Append(s_directivePrefix)
        .Append(directive.Name);

      self.AppendArguments(directive.Arguments);
    });
  }

  private void AppendSelectionSet(ISelectionSetNode selectionSet)
  {
    _sb.Append('{');
    _indent++;
    AppendLine();

    AppendSelections(selectionSet.Selections);

    _indent--;
    AppendLine();
    _sb.Append('}');
  }

  private void AppendSelections(IEnumerable<ISelectionNode> selections)
  {
    _sb.AppendJoin(AppendLine, ref this, selections, static delegate (StringBuilder sb, ref DocumentStringBuilder self, ISelectionNode selection)
    {
      self.AppendSelection(selection);
    });
  }

  private void AppendSelection(ISelectionNode selection)
  {
    if (selection.Alias is string alias)
    {
      _sb
        .AppendCamelized(alias)
        .Append(": ");
    }

    _sb.AppendCamelized(selection.FieldName);

    AppendArguments(selection.Arguments);

    using (new SpaceScope(_sb))
    {
      AppendDirectives(selection.Directives);
    }

    if (selection.SelectionSet is ISelectionSetNode selectionSet)
    {
      _sb.Append(' ');
      AppendSelectionSet(selectionSet);
    }
  }

  private void AppendArguments(IEnumerable<IArgumentNode> arguments)
  {
    using (new ArgumentScope(_sb))
    {
      _sb.AppendJoin(Separator, ref this, arguments, static delegate (StringBuilder sb, ref DocumentStringBuilder self, IArgumentNode argument)
      {
        sb.AppendCamelized(argument.Name)
          .Append(": ");

        self.AppendValue(argument.Value);
      });
    }
  }

  private void AppendValue(object? value)
  {
    if (value is null)
    {
      _sb.Append(s_nullKeyword);
      return;
    }

    switch (Convert.GetTypeCode(value))
    {
      case >= TypeCode.SByte and <= TypeCode.UInt64:
        _sb.Append(Convert.ToInt64(value));
        return;

      case >= TypeCode.Single and <= TypeCode.Decimal:
        _sb.Append(Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture));
        return;

      case TypeCode.String:
        _sb
          .Append('"')
          .Append(Convert.ToString(value))
          .Append('"');
        return;

      case TypeCode.Boolean:
        _sb.Append(Convert.ToBoolean(value) ? s_trueKeyword : s_falseKeyword);
        return;

      case TypeCode.DateTime:
        throw new NotSupportedException("DateTime is not yet supported."); // TODO: Support
    }

    switch (value)
    {
      case MemberInfo member:
        _sb
          .Append(s_variablePrefix)
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
    _sb.Append("{ ");

    _sb.AppendJoin(Separator, ref this, objectValue.Fields, static delegate (StringBuilder sb, ref DocumentStringBuilder self, IObjectFieldNode field)
    {
      sb.AppendCamelized(field.Name)
        .Append(": ");

      self.AppendValue(field.Value);
    });

    _sb.Append(" }");
  }

  private void AppendListValue(IListValueNode listValue)
  {
    _sb.Append('[');

    _sb.AppendJoin(Separator, ref this, listValue.Values, static delegate (StringBuilder sb, ref DocumentStringBuilder self, object? value)
    {
      self.AppendValue(value);
    });

    _sb.Append(']');
  }

  private void AppendLine()
  {
    _options.LinePolicy.AppendLine(_sb, _indent);
  }

  private static void AppendLine(StringBuilder sb, ref DocumentStringBuilder self)
    => self.AppendLine();

  private readonly ref struct ArgumentScope
  {
    private readonly StringBuilder _sb;
    private readonly int _length;

    public ArgumentScope(StringBuilder sb)
    {
      _sb = sb;

      _sb.Append('(');
      _length = sb.Length;
    }

    public void Dispose()
    {
      if (_sb.Length == _length)
      {
        _sb.Remove(_length - 1, 1);
      }
      else
      {
        _sb.Append(')');
      }
    }
  }

  private readonly ref struct SpaceScope
  {
    private readonly StringBuilder _sb;
    private readonly int _length;

    public SpaceScope(StringBuilder sb)
    {
      _sb = sb;

      _sb.Append(' ');
      _length = sb.Length;
    }

    public void Dispose()
    {
      if (_sb.Length == _length)
      {
        _sb.Remove(_length - 1, 1);
      }
    }
  }
}
