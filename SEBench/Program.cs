using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy;
using Mono.Cecil;
using SEBench.ExtSE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEBench
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            List<string> assemblyPaths = new List<string>();
            string specificClass = string.Empty;

            for (var i = 0; i < args.Length; i++)
            {
                if ((args[i] == "-class" || args[i] == "-c") && i + 1 < args.Length)
                {
                    specificClass = args[++i];
                    continue;
                }

                if (File.Exists(args[i]))
                    assemblyPaths.Add(args[i]);
            }

            if (assemblyPaths.Count <= 0)
            {
                var openFile = new OpenFileDialog();

                //openFile.Filter = "*.dll|*.exe";

                openFile.Multiselect = true;
                openFile.SupportMultiDottedExtensions = true;
                openFile.Title = "Select your SE script assemblies";

                if (DialogResult.OK == openFile.ShowDialog())
                    assemblyPaths.AddRange(openFile.FileNames);
            }

            if (assemblyPaths.Count <= 0)
                return;

            SECompiler compiler = new SECompiler();

            if (string.IsNullOrEmpty(specificClass))
            {
                var scripts = compiler.Compile(assemblyPaths.ToArray());

                foreach (var script in scripts)
                    Application.Run(new CodeForm(script.Value, script.Key));
            }
            else
            {
                var code = compiler.Compile(assemblyPaths.ToArray(), specificClass);

                new CodeForm(code, specificClass).ShowDialog();
            }
        }

        
    }
}
