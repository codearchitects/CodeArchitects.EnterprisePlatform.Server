using CodeArchitects.Platform.Common;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal record FakeInstruction(int Position, OpCode OpCode, Optional<object?> Argument);
