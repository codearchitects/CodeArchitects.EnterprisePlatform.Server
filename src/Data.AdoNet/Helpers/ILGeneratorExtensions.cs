using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Diagnostics;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Helpers;

internal static class ILGeneratorExtensions
{
  public static void GetMember(this ILGenerator il, IPropertyModelBase model)
  {
    Debug.Assert(model.MemberAccess is MemberAccess.Field or MemberAccess.Property);
    switch (model.MemberAccess)
    {
      case MemberAccess.Field:
        il.Emit(OpCodes.Ldfld, model.Field!);
        break;
      case MemberAccess.Property:
        il.Emit(OpCodes.Callvirt, model.Property!.GetMethod);
        break;
      default:
        throw Errors.Unreacheable;
    }
  }

  public static void SetMember(this ILGenerator il, IPropertyModelBase model)
  {
    Debug.Assert(model.MemberAccess is MemberAccess.Field or MemberAccess.Property);
    switch (model.MemberAccess)
    {
      case MemberAccess.Field:
        il.Emit(OpCodes.Stfld, model.Field!);
        break;
      case MemberAccess.Property:
        il.Emit(OpCodes.Callvirt, model.Property!.SetMethod);
        break;
      default:
        throw Errors.Unreacheable;
    }
  }
}
