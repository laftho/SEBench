# SEBench
Space Engineers coding workbench. Tool design to make SE programming easier.

This tool makes it easier to write SE ingame scripts in Visual Studio and manage your ingame script libraries. Set it up as a post build event on your ingame script Class Library project and the tool will compile/format the script and present it in a window to easily copy & paste directly into a Programmable Block.

This tool uses ILSpy to decompile attributed classes marked for ingame scripts (SEIGScript attribute).

Dependencies:

http://ilspy.net/
Space Engineers game assemblies (Sandbox.Common, VRage.Game, VRage.Math)

How to use:

Make a Class Library project in Visual Studio, refrence SEBench.exe and the Space Engineers ingame script references.

Setup a Post build event in your project to run SEBench.exe with your compiled assembly as a parameter. If you have more than one SEIGScript class in your final assembly, SEBench will popup each of them in a window. If you only want to see a specific one, use the -class/-c className argument. Ensure individual paths are quoted if they may contain spaces.

E.g. post build script command:

$(TargetDir)\SEBench.exe "$(TargetDir)$(TargetFileName)"

Set the post build event to run "always". You may need to run Rebuild if the window doesn't show if you haven't made any code changes.

E.g. sample script class:

```csharp
using ExtSE;
using Sandbox.ModAPI.Ingame;

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
```

The above sample is using the ExtTerminal template and adds the GridTerminalSystem field to allow it to be compiled but using SEIGExclude it marks it to be excluded in the SE compiled output.

Attributes:

SEIGScript - Attribute available to classes only, instructs SEBench to produce a ingame compiled script of this class' body. Takes params string arguments of named SEIGTemplate template classes. These named template classes will get included in your compiled output.

SEIGExclude - available to all types, instructs SEBench to exclude these types from the compiled output. Useful to have variables that are required for Visual Studio to compile the assembly but that are not required for the Programmable block.

SEIGTemplate - currently only available to classes, compiles named templates that may be included in your scripts.

Output produced by SEBench:

```csharp
public class ExtTerminal
{
    public string LOG_TAG = "[log]";
    public string LOG_FORMAT = "{0}\n";
    public string GPS_FORMAT = "GPS:{0}:{1}:{2}:{3}:";
    private IMyGridTerminalSystem gridTerminalSystem;
    private IMyProgrammableBlock self;
    private IMyTextPanel logPanel;
    public IMyProgrammableBlock Self
    {
        get
        {
            return this.self;
        }
    }
    public ExtTerminal(IMyGridTerminalSystem gridTerminalSystem, string logTag = null)
    {
        ExtTerminal <>4__this = this;
        this.gridTerminalSystem = gridTerminalSystem;
        this.self = this.FindFirstOrDefautBlockOfType<IMyProgrammableBlock>((IMyProgrammableBlock block) => block.IsRunning);
        this.logPanel = this.FindFirstOrDefautBlockOfType<IMyTextPanel>((IMyTextPanel block) => block.CustomName.Contains(logTag ?? <>4__this.LOG_TAG));
    }
    public void Log(string message)
    {
        if (this.logPanel != null)
        {
            this.logPanel.WritePublicText(string.Format(this.LOG_FORMAT, message), true);
        }
    }
    public void ClearLog()
    {
        if (this.logPanel != null)
        {
            this.logPanel.WritePublicText("", false);
        }
    }
    public List<T> FindBlocksOfType<T>(Func<T, bool> collect = null)
    {
        List<IMyTerminalBlock> list = new List<IMyTerminalBlock>();
        List<T> list2 = new List<T>();
        this.GetBlocksOfType<T>(list, (IMyTerminalBlock block) => collect == null || collect((T)((object)block)));
        for (int i = 0; i < list.Count; i++)
        {
            list2.Add((T)((object)list[i]));
        }
        return list2;
    }
    public T FindFirstOrDefautBlockOfType<T>(Func<T, bool> collect = null)
    {
        List<T> list = this.FindBlocksOfType<T>(collect);
        T result;
        if (list.Count > 0)
        {
            result = list[0];
        }
        else
        {
            result = default(T);
        }
        return result;
    }
    public IMyTerminalBlock GetBlockWithName(string name)
    {
        return this.gridTerminalSystem.GetBlockWithName(name);
    }
    public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
    {
        this.gridTerminalSystem.GetBlocksOfType<T>(blocks, collect);
    }
    public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
    {
        this.gridTerminalSystem.SearchBlocksOfName(name, blocks, collect);
    }
    public string GetGPS(IMyTerminalBlock block, string overrideName = null)
    {
        Vector3D vector3D = block.CubeGrid.GridIntegerToWorld(block.Position);
        double num = Math.Floor(vector3D.GetDim(0));
        double num2 = Math.Floor(vector3D.GetDim(1));
        double num3 = Math.Floor(vector3D.GetDim(2));
        return string.Format(this.GPS_FORMAT, new object[]
        {
            overrideName ?? block.CustomName,
            num,
            num2,
            num3
        });
    }
}

public void Main(string arguments)
{
    ExtTerminal extTerminal = new ExtTerminal(this.GridTerminalSystem, null);
    List<IMyRemoteControl> list = extTerminal.FindBlocksOfType<IMyRemoteControl>(null);
    extTerminal.ClearLog();
    for (int i = 0; i < list.Count; i++)
    {
        extTerminal.Log(extTerminal.GetGPS(list[i], null));
    }
}

```
