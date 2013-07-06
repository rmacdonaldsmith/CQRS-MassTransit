using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHM.WinFlexOne.CQRS
{
    public class FakeDataBase
    {
        //public static Dictionary<Guid, ElectionDto> Elections = new Dictionary<Guid, ElectionDto>();
        public static Dictionary<Guid, object> Elections = new Dictionary<Guid, object>();

        //public static Dictionary<Guid, ClaimDto> Claims = new Dictionary<Guid, ClaimDto>();
        public static Dictionary<Guid, object> Claims = new Dictionary<Guid, object>();
    }
}
