using System;
using System.Collections.Generic;

using System.Text;

using System.CodeDom;
using System.CodeDom.Compiler;

namespace Glue
{
    public class GenericCodeProvider : System.CodeDom.Compiler.CodeDomProvider, ICodeCompiler, ICodeGenerator
    {
        public override ICodeCompiler CreateCompiler()
        {
            return this;
        }
        public override ICodeGenerator CreateGenerator()
        {
            return this;
        }

        #region ICodeCompiler Members

        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            return cp.CompileAssemblyFromDom(options, compilationUnit);
        }
        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] compilationUnits)
        {
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            return cp.CompileAssemblyFromDom(options, compilationUnits);
        }

        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            throw new NotImplementedException();
        }
        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            throw new NotImplementedException();
        }

        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            throw new NotSupportedException("Must specify a language to use the GenericCodeProvider with source");
        }
        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            throw new NotSupportedException("Must specify a language to use the GenericCodeProvider with source");
        }

        #endregion

        #region ICodeGenerator Members


        public void ValidateIdentifier(string value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
