using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface ISimpleNavigationModel : INavigationModel
{
  [MemberNotNullWhen(false, nameof(NavigationEntity))]
  new bool IsOnDependent { get; } // TODO: Rename 'IsOnPrincipal' and change value accordingly

  IPrimaryKeyModel PrimaryKey { get; }

  IReadOnlyList<IKeyPair> KeyPairs { get; }

  new ISimpleNavigationModel Inverse { get; }

  IEntityModel? NavigationEntity { get; }
}
