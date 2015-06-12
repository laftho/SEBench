using ExtSE;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEBench.ExtSE
{
    [SEIGTemplate]
    public class ExtUtility
    {
        public static void Explain(ExtTerminal term, IMyTerminalBlock block)
        {
            if (block == null)
            {
                term.Log("Explain: null");
                return;
            }

            List<ITerminalProperty> properties = new List<ITerminalProperty>();
            List<ITerminalAction> actions = new List<ITerminalAction>();

            block.GetProperties(properties);
            block.GetActions(actions);

            term.Log(string.Format("Explain: {0}", block.CustomName));
            term.Log(string.Format("Details: {0}", block.DetailedInfo));
            term.Log(string.Format("DefinitionDisplayNameText: {0}", block.DefinitionDisplayNameText));
            term.Log(string.Format("DisplayNameText: {0}", block.DisplayNameText));
            term.Log(string.Format("ToString: {0}", block.ToString()));
            term.Log(string.Format("Position: {0}", term.GetGPS(block)));

            term.Log("Properties:");

            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                term.Log(string.Format("Id: {0}, TypeName: {1}, ToString: {2}", prop.Id, prop.TypeName, prop.ToString()));
            }

            term.Log("Actions:");

            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                term.Log(string.Format("Id: {0}, Name: {1}, ToString: {2}", action.Id, action.Name, action.ToString()));
            }
        }
    }
}
