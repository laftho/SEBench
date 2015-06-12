# SEBench
Space Engineers coding workbench. Tool design to make SE programming easier.

This tool makes it easier to write SE ingame scripts in Visual Studio and manage your ingame script libraries. Set it up as a post build event on your ingame script assembly project and the tool will compile/format the script and present it in a window to easily copy & paste directly into a Programmable Block.

This tool uses ILSpy to decompile attributed classes marked for ingame scripts (SEIGScript attribute).

Dependencies:

http://ilspy.net/

How to use:

Make an assembly project in Visual Studio, refrence SEBench.exe and the Space Engineers ingame script references.

Setup a Post build event in your project to run SEBench.exe with your compiled assembly as a parameter. If you have more than one SEIGScript class in your final assembly, SEBench will popup each of them in a window. If you only want to see a specific one, use the -class/-c className argument.

E.g. post build script command:

$(TargetDir)\SEBench.exe $(TargetDir)$(TargetFileName)

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

SEIGExclude - available to all types instructs SEBench to exclude these types from the compiled output. Useful to have variables that are required for Visual Studio to compile the assembly but that are not required for the Programmable block.

SEIGTemplate - currently only available to classes, compiles named templates that may be included in your scripts.

