namespace CodeArchitects.Platform.Data.AdoNet.DependencyInjection;

[Flags]
public enum AdoNetServiceTypes
{
  None = 0,

  ModelConfiguration = 1,
  CommandInterceptors = 2,

  All = ModelConfiguration | CommandInterceptors
}
