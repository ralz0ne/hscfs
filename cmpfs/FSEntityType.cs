using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmpfs
{
    public enum FSEntityType : int
    {
        FILE = 0,
        DIR = 1,
        SYML = 2,
        HARDL = 3,
        CONTROL = 4
    }
}
