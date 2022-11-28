using CodeArchitects.Platform.Common.Utils;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal static class ILGeneratorExtensions
{
  public static void LoadArgs(this ILGenerator il, int count)
  {
    for (int i = 1; i <= count; i++)
    {
      il.LoadArg(i);
    }
  }

  public static void LoadArg(this ILGenerator il, int index)
  {
    switch (index)
    {
      case 1:
        il.Emit(OpCodes.Ldarg_1);
        break;
      case 2:
        il.Emit(OpCodes.Ldarg_2);
        break;
      case 3:
        il.Emit(OpCodes.Ldarg_3);
        break;
      case < 256:
        il.Emit(OpCodes.Ldarg_S, index);
        break;
      default:
        il.Emit(OpCodes.Ldarg, index);
        break;
    }
  }

  public static void InitFields(this ILGenerator il, int startIndex, IReadOnlyList<FieldInfo> fields)
  {
    for (int i = 0; i < fields.Count; i++)
    {
      int argumentIndex = i + startIndex;

      il.Emit(OpCodes.Ldarg_0);
      switch (argumentIndex)
      {
        case 1:
          il.Emit(OpCodes.Ldarg_1);
          break;
        case 2:
          il.Emit(OpCodes.Ldarg_2);
          break;
        case 3:
          il.Emit(OpCodes.Ldarg_3);
          break;
        case < 256:
          il.Emit(OpCodes.Ldarg_S, argumentIndex);
          break;
        default:
          il.Emit(OpCodes.Ldarg, argumentIndex);
          break;
      }
      il.Emit(OpCodes.Stfld, fields[i]);
    }
  }

  public static void InitFields(this ILGenerator il, int startIndex, params FieldInfo[] fields)
  {
    InitFields(il, startIndex, fields as IReadOnlyList<FieldInfo>);
  }

  public static void LoadFields(this ILGenerator il, IEnumerable<FieldInfo> fields)
  {
    foreach (FieldInfo field in fields)
    {
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Ldfld, field);
    }
  }

  public static void LoadFields(this ILGenerator il, params FieldInfo[] fields)
  {
    LoadFields(il, fields as IEnumerable<FieldInfo>);
  }

  public static void LoadInt(this ILGenerator il, int number)
  {
    switch (number)
    {
      case 0:
        il.Emit(OpCodes.Ldc_I4_0);
        break;
      case 1:
        il.Emit(OpCodes.Ldc_I4_1);
        break;
      case 2:
        il.Emit(OpCodes.Ldc_I4_2);
        break;
      case 3:
        il.Emit(OpCodes.Ldc_I4_3);
        break;
      case 4:
        il.Emit(OpCodes.Ldc_I4_4);
        break;
      case 5:
        il.Emit(OpCodes.Ldc_I4_5);
        break;
      case 6:
        il.Emit(OpCodes.Ldc_I4_6);
        break;
      case 7:
        il.Emit(OpCodes.Ldc_I4_7);
        break;
      case 8:
        il.Emit(OpCodes.Ldc_I4_8);
        break;
      case -1:
        il.Emit(OpCodes.Ldc_I4_M1);
        break;
      case < -1 and >= -128 or > 8 and < 128:
        il.Emit(OpCodes.Ldc_I4_S, number);
        break;
      case < -128 or >= 128:
        il.Emit(OpCodes.Ldc_I4, number);
        break;
      default:
        throw Errors.Unreacheable;
    }
  }
}
