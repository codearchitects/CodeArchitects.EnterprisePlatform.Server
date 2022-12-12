using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet;

public delegate bool VisitNodeCallback<TState>(object node, IEntityModel model, NavigationContext context, TState state);

public delegate Task<bool> AsyncVisitNodeCallback<TState>(object node, IEntityModel model, NavigationContext context, TState state, CancellationToken cancellationToken);
