using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface ISubNavigationSpec
{
  void WriteQueryPart(StringBuilder stringBuilder);
}
