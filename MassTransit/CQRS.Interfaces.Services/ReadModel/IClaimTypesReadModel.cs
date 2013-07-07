using System.Collections.Generic;

namespace CQRS.Interfaces.Services.ReadModel
{
    public interface IClaimTypesReadModel
    {
        IEnumerable<string> GetClaimTypes();
    }
}
