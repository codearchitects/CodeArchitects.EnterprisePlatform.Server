using System;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMIndirect
{
  public class PrimarySecondary : IAssociation<Guid, Guid>
  {
    public Guid PrimaryId { get; set; }
    public Guid SecondaryId { get; set; }

    Guid IAssociation<Guid, Guid>.Id1 => PrimaryId;
    Guid IAssociation<Guid, Guid>.Id2 => SecondaryId;

    object IAssociation.Id1 => PrimaryId;
    object IAssociation.Id2 => SecondaryId;
  }
}
