using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtSE
{
    [AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SEIGExclude : Attribute
    {
    }
}
