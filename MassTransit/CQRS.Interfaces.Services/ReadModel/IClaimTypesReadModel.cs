using System.Collections.Generic;

namespace MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel
{
    public interface IClaimTypesReadModel
    {
        IEnumerable<string> GetClaimTypes();
    }
}
