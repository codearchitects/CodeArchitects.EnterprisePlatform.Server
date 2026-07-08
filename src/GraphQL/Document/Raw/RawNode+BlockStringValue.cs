using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IBlockStringValueNode
{
  IEnumerable<ReadOnlyMemory<char>> IBlockStringValueNode.Lines
  {
    get
    {
      ReadOnlyMemory<char> value = _lexer.ValueMemory[3..^3];
      if (value.Length == 0)
        yield break;

      (int commonIndent, int length) = BlockStringStateMachine.GetBlockStringData(value.Span);

      int position = 0;
      SkipBlankLines(value.Span, length, ref position);

      bool isFirstLine = position == 0;
      while (position < length)
      {
        var line = NextLine(value, commonIndent, length, isFirstLine, ref position);
        yield return line;
        isFirstLine = false;
      }

      _lexer.MoveNext();
    }
  }

  private static ReadOnlyMemory<char> NextLine(ReadOnlyMemory<char> value, int commonIndent, int length, bool isFirstLine, ref int position)
  {
    ReadOnlySpan<char> valueSpan = value.Span;

    int start = position;
    int end;
    bool isWhitespaceLine = true;
    while (position < length)
    {
      char current = valueSpan[position];

      switch (current)
      {
        case '\n':
          end = position;
          position++;
          if (isWhitespaceLine)
            return ReadOnlyMemory<char>.Empty;

          return value[start..end];

        case '\r':
          if (valueSpan[position + 1] is '\n')
          {
            end = position;
            position += 2;
            if (isWhitespaceLine)
              return ReadOnlyMemory<char>.Empty;

            return value[start..end];
          }

          goto case '\n';

        case not ' ' when isWhitespaceLine:
          if (!isFirstLine)
          {
            start += commonIndent;
          }
          isWhitespaceLine = false;
          break;
      }

      position++;
    }

    return value[start..position];
  }

  private static void SkipBlankLines(ReadOnlySpan<char> value, int length, ref int position)
  {
    while (position < length && value[position] is '\r' or '\n')
    {
      position++;
    }

    while (position < length)
    {
      int start = position;

      while (position < length)
      {
        char current = value[position];

        if (current is '\r' or '\n')
          break;

        if (current is not ' ')
        {
          position = start;
          return;
        }

        position++;
      }
    }
  }

  private ref struct BlockStringStateMachine
  {
    private int _position;
    private int _state;
    private int _commonIndent;
    private int _indent;
    private int _end;

    public bool MoveNext(char input)
    {
      switch (_state)
      {
        case 0:
          return OnStringCharacter(input);

        case 1:
          return OnLineTerminator(input);

        case 2:
          return OnWhitespace(input);

        case 3:
          return OnNoIndentationStringCharacter(input);

        case 4:
          return OnNoIndentationLineTerminator(input);

        default:
          Debug.Fail("Invalid state.");
          return false;
      }
    }

    private bool OnStringCharacter(char input)
    {
      switch (input)
      {
        case '\r' or '\n':
          _end = _position;
          _state = 1;
          break;

        case '\0':
          _end = _position;
          return false;
      }

      return true;
    }

    private bool OnLineTerminator(char input)
    {
      switch (input)
      {
        case ' ':
          _indent++;
          _state = 2;
          break;

        case '\r' or '\n':
          break;

        case '\0':
          return false;

        default:
          _commonIndent = 0;
          _state = 3;
          break;
      }

      return true;
    }

    private bool OnWhitespace(char input)
    {
      switch (input)
      {
        case ' ':
          _indent++;
          break;

        case '\r' or '\n':
          _indent = 0;
          _state = 1;
          break;

        case '\0':
          return false;

        default:
          if (_indent < _commonIndent)
          {
            _commonIndent = _indent;
          }
          _indent = 0;
          _state = 0;
          break;
      }

      return true;
    }

    private bool OnNoIndentationStringCharacter(char input)
    {
      switch (input)
      {
        case '\r' or '\n':
          _end = _position;
          _state = 4;
          break;

        case '\0':
          _end = _position;
          return false;
      }

      return true;
    }

    private bool OnNoIndentationLineTerminator(char input)
    {
      switch (input)
      {
        case '\r' or '\n' or ' ':
          break;

        case '\0':
          return false;

        default:
          _state = 3;
          break;
      }

      return true;
    }

    public static (int CommonIndent, int End) GetBlockStringData(ReadOnlySpan<char> value)
    {
      BlockStringStateMachine stateMachine = new()
      {
        _commonIndent = int.MaxValue
      };

      bool advance;
      do
      {
        char input = stateMachine._position >= value.Length
          ? '\0'
          : value[stateMachine._position];

        advance = stateMachine.MoveNext(input);
        stateMachine._position++;
      } while (advance);

      int commonIndent = stateMachine._commonIndent;
      if (commonIndent == int.MaxValue)
      {
        commonIndent = 0;
      }

      return (commonIndent, stateMachine._end);
    }
  }
}
