using ExtSE;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEBench.Sample
{
    [SEIGScript("ExtTerminal")]
    public class RemoteControlPositioning
    {
        [SEIGExclude]
        public IMyGridTerminalSystem GridTerminalSystem;

        public void Main(string arguments)
        {
            var term = new ExtTerminal(GridTerminalSystem);

            var remotes = term.FindBlocksOfType<IMyRemoteControl>();

            term.ClearLog();

            for (var i = 0; i < remotes.Count; i++)
                term.Log(term.GetGPS(remotes[i]));
        }
    }
}
