using CodeArchitects.Platform.Common;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit.Testing;

internal record FakeInstruction(OpCode OpCode, Optional<object?> Argument);
