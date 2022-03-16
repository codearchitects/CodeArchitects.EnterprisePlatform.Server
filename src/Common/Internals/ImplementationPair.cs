using System;

namespace CodeArchitects.Platform.Common.Internals;

internal readonly record struct ImplementationPair(Type InterfaceType, Type ImplementationType);