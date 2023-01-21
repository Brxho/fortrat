using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creeper.Helper
{
    internal class Compile
    {
        public static byte[] Compiler(string reference, string sourceCode)
        {
            string[] referencedAssemblies = reference.Split(new[] { "-=>" }, StringSplitOptions.RemoveEmptyEntries);

            var providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };

            var compilerOptions = "/platform:anycpu /optimize+";

            using (var cSharpCodeProvider = new CSharpCodeProvider(providerOptions))
            {
                var compilerParameters = new CompilerParameters(referencedAssemblies)
                {
                    GenerateExecutable = false,
                    GenerateInMemory = false,
                    CompilerOptions = compilerOptions,
                    TreatWarningsAsErrors = false,
                    IncludeDebugInformation = false
                };

                var compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, sourceCode);
                if (compilerResults.Errors.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError compilerError in compilerResults.Errors)
                        sb.AppendLine(
                            $"{compilerError.ErrorText}\nLine: {compilerError.Line} - Column: {compilerError.Column}\nFile: {compilerError.FileName}");
                    Debug.WriteLine(sb.ToString());
                    return null;
                }

                var assembly = File.ReadAllBytes(compilerResults.PathToAssembly);
                File.Delete(compilerResults.PathToAssembly);
                return assembly;
            }
        }
    }
}
