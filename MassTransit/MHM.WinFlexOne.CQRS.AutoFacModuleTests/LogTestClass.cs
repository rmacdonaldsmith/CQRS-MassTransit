using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace MHM.WinFlexOne.CQRS.AutoFacModuleTests
{
    public class LogTestClass
    {
        private readonly ILog m_log;

        public ILog Log
        {
            get { return m_log; }
        }

        public LogTestClass(ILog log)
        {
            m_log = log;
        }
    }
}
