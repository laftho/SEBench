using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SEBench.ExtSE
{
    public class SECompiler
    {
        DecompilationOptions decompilationOptions = null;

        public SECompiler(DecompilationOptions options = null)
        {
            decompilationOptions = options;

            if (decompilationOptions == null)
            {
                decompilationOptions = new DecompilationOptions();
                decompilationOptions.FullDecompilation = true;
                decompilationOptions.DecompilerSettings.UsingDeclarations = false;
                decompilationOptions.DecompilerSettings.FullyQualifyAmbiguousTypeNames = false;
                decompilationOptions.DecompilerSettings.ShowXmlDocumentation = false;
            }
        }

        public string Compile(string[] assemblyPaths, string className)
        {
            return Compile(assemblyPaths).FirstOrDefault(x => x.Key == className).Value;
        }

        public Dictionary<string, string> Compile(string[] assemblyPaths)
        {
            var module = Mono.Cecil.ModuleDefinition.ReadModule(Assembly.GetExecutingAssembly().Location);

            var findTemplates = new Func<TypeDefinition,bool>(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "SEIGTemplate"));
            var findScripts = new Func<TypeDefinition,bool>(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "SEIGScript"));

            var templates = module.Types.Where(findTemplates).ToList();
            var scripts = module.Types.Where(findScripts).ToList();

            foreach (var asmPath in assemblyPaths)
            {
                var m = Mono.Cecil.ModuleDefinition.ReadModule(asmPath);

                templates.AddRange(m.Types.Where(findTemplates));
                scripts.AddRange(m.Types.Where(findScripts));
            }

            var csharpLanguage = new CSharpLanguage();
            var textOutput = new PlainTextOutput();
            
            Dictionary<string, string> codeTemplates = new Dictionary<string, string>();

            foreach (var template in templates)
            {
                if (codeTemplates.ContainsKey(template.Name)) continue;

                textOutput = new PlainTextOutput();
                csharpLanguage.DecompileType(template, textOutput, decompilationOptions);
                var code = textOutput.ToString();

                code = SanitizeCode(Exclude(code, CollectExcludes(template)));

                codeTemplates.Add(template.Name, code);
            }

            Dictionary<string, string> ingameScripts = new Dictionary<string, string>();

            foreach (var script in scripts)
            {
                if (ingameScripts.ContainsKey(script.Name)) continue;

                var scriptTemplateNames = ((CustomAttributeArgument[])script.CustomAttributes.First(x => x.AttributeType.Name == "SEIGScript").ConstructorArguments[0].Value).Select(x => x.Value.ToString());

                var scriptTemplates = codeTemplates.Where(x => scriptTemplateNames.Contains(x.Key)).Select(x => x.Value);

                var excludeCode = CollectExcludes(script);

                textOutput = new PlainTextOutput();
                csharpLanguage.DecompileType(script, textOutput, decompilationOptions);
                var code = textOutput.ToString();

                code = BumpClass(script.Name, SanitizeCode(Exclude(code, CollectExcludes(script))));

                StringBuilder sb = new StringBuilder();

                foreach (var template in scriptTemplates)
                    sb.AppendLine(template);

                sb.AppendLine(code);

                ingameScripts.Add(script.Name, sb.ToString());
            }

            return ingameScripts;
        }

        private string Exclude(string code, List<string> excludes)
        {
            var lines = code.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var exclude in excludes)
            {
                StringBuilder sb = new StringBuilder();

                var excludeLines = exclude.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    bool match = true;

                    for (var j = 0; j < excludeLines.Length; j++)
                    {
                        var xline = excludeLines[j].Trim();

                        if (i + j >= lines.Length)
                            match = false;

                        if (lines[i + j].Trim() != xline)
                            match = false;

                        if (!match)
                            break;
                    }

                    if (!match)
                        sb.AppendLine(line);
                    else
                        i += excludeLines.Length - 1;
                }

                lines = sb.ToString().Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }

            return String.Join(Environment.NewLine, lines);
        }

        private List<string> CollectExcludes(TypeDefinition type)
        {
            var excludeCode = new List<string>();
            var csharpLanguage = new CSharpLanguage();
            var decompilationOptions = new DecompilationOptions();
            decompilationOptions.FullDecompilation = true;
            decompilationOptions.DecompilerSettings.UsingDeclarations = false;

            var findExlucdes = new Func<Mono.Cecil.ICustomAttributeProvider, bool>(p => p.CustomAttributes.Any(attr => attr.AttributeType.Name == "SEIGExclude"));

            var fieldExcludes = type.Fields.Where<FieldDefinition>(findExlucdes);
            var propertyExcludes = type.Properties.Where<PropertyDefinition>(findExlucdes);
            var methodExcludes = type.Methods.Where<MethodDefinition>(findExlucdes);
            var typeExcludes = type.NestedTypes.Where<TypeDefinition>(findExlucdes);

            foreach (var item in fieldExcludes)
            {
                var textOutput = new PlainTextOutput();
                csharpLanguage.DecompileField(item, textOutput, decompilationOptions);
                excludeCode.Add(SanitizeCode(textOutput.ToString()));
            }

            foreach (var item in propertyExcludes)
            {
                var textOutput = new PlainTextOutput();
                csharpLanguage.DecompileProperty(item, textOutput, decompilationOptions);
                excludeCode.Add(SanitizeCode(textOutput.ToString()));
            }

            foreach (var item in methodExcludes)
            {
                var textOutput = new PlainTextOutput();
                csharpLanguage.DecompileMethod(item, textOutput, decompilationOptions);
                excludeCode.Add(SanitizeCode(textOutput.ToString()));
            }

            foreach (var item in typeExcludes)
            {
                var textOutput = new PlainTextOutput();
                csharpLanguage.DecompileType(item, textOutput, decompilationOptions);
                excludeCode.Add(SanitizeCode(textOutput.ToString()));
            }

            return excludeCode;
        }

        private string SanitizeCode(string code)
        {
            StringBuilder sb = new StringBuilder();
            var lines = code.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int size = lines.Length;
            for (var i = 0; i < size; i++)
            {
                var line = lines[i];

                if (line.Trim().StartsWith("//"))
                    continue;

                if (i == 0 && line.Trim().StartsWith("namespace"))
                {
                    i += 1;
                    size -= 1; //remove closing }
                    continue;
                }

                if (line.Contains("[SEIGTemplate]"))
                    continue;

                if (line.StartsWith("\t"))
                    line = line.Remove(0, 1);

                sb.AppendLine(line.Replace("\t", "    "));
            }

            return sb.ToString();
        }

        private string BumpClass(string name, string code)
        {
            StringBuilder sb = new StringBuilder();
            var lines = code.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            bool begin = false;

            int size = lines.Length;
            for (var i = 0; i < size; i++)
            {
                var line = lines[i];

                if (line.Contains("class " + name))
                {
                    i += 1;
                    size -= 1;
                    begin = true;
                    continue;
                }

                //fixme this will totally break other references if your class is named anything similar
                line = line.Replace(name + ".", string.Empty); //clean local static references

                if (line.StartsWith("    "))
                    line = line.Remove(0, 4);

                if (begin)
                    sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
