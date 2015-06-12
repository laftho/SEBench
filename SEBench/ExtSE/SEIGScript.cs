using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtSE
{
    [AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SEIGScript : Attribute
    {
        private string[] templates;

        public string[] Templates
        {
            get { return templates; }
        }

        public SEIGScript(params string[] templates)
        {
            this.templates = templates;
        }
    }
}
