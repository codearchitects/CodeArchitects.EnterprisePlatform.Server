namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal interface IMultitenancyContextBypasser
{
  IDisposable BypassMultitenancy();
}
