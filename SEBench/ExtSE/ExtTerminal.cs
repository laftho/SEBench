using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtSE
{
    [SEIGTemplate]
    public class ExtTerminal
    {
        public string LOG_TAG = "[log]";
        public string LOG_FORMAT = "{0}\n";
        public string GPS_FORMAT = "GPS:{0}:{1}:{2}:{3}:";

        IMyGridTerminalSystem gridTerminalSystem;
        IMyProgrammableBlock self;
        IMyTextPanel logPanel;

        public IMyProgrammableBlock Self
        {
            get { return self; }
        }

        public ExtTerminal(IMyGridTerminalSystem gridTerminalSystem, string logTag = null)
        {
            this.gridTerminalSystem = gridTerminalSystem;

            self = this.FindFirstOrDefautBlockOfType<IMyProgrammableBlock>(block => block.IsRunning);

            logPanel = this.FindFirstOrDefautBlockOfType<IMyTextPanel>(block => block.CustomName.Contains(logTag ?? LOG_TAG));
        }

        public void Log(string message)
        {
            if (logPanel != null)
                logPanel.WritePublicText(string.Format(LOG_FORMAT, message), true);
        }

        public void ClearLog()
        {
            if (logPanel != null)
                logPanel.WritePublicText("");
        }

        public List<T> FindBlocksOfType<T>(Func<T, bool> collect = null)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<T> typedBlocks = new List<T>();

            this.GetBlocksOfType<T>(blocks, block => (collect != null) ? collect((T)block) : true);

            for (int i = 0; i < blocks.Count; i++)
                typedBlocks.Add((T)blocks[i]);

            return typedBlocks;
        }

        public T FindFirstOrDefautBlockOfType<T>(Func<T, bool> collect = null)
        {
            var blocks = FindBlocksOfType<T>(collect);

            if (blocks.Count > 0)
                return blocks[0];

            return default(T);
        }

        #region IMyGridTerminalSystem Members

        public IMyTerminalBlock GetBlockWithName(string name)
        {
            return gridTerminalSystem.GetBlockWithName(name);
        }

        public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            gridTerminalSystem.GetBlocksOfType<T>(blocks, collect);
        }

        public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            gridTerminalSystem.SearchBlocksOfName(name, blocks, collect);
        }

        #endregion

        public string GetGPS(IMyTerminalBlock block, string overrideName = null)
        {
            var pos = block.CubeGrid.GridIntegerToWorld(block.Position);

            double x = Math.Floor(pos.GetDim(0));
            double y = Math.Floor(pos.GetDim(1));
            double z = Math.Floor(pos.GetDim(2));

            return string.Format(GPS_FORMAT, overrideName ?? block.CustomName, x, y, z);
        }
    }
}
