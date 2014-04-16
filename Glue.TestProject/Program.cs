using System;
using System.Collections.Generic;
using System.Text;

using Flame;
using Flame.ObjectModels.SourceCode;
using Flame.ObjectModels.SourceCode.CodeElements;
using Flame.ObjectModels.SourceCode.CodeElements.CodeActionElements;

using Flame.DataFormats.Programming;
using Flame.DataFormats.Programming.Java;

namespace Glue.TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            #region test 1
            CodeClassElement clss = new CodeClassElement();
            clss.Name = "StaticPriceHandler";

            CodePropertyElement prop1 = new CodePropertyElement();
            prop1.Name = "Price";
            prop1.DataType = DataType.Int32;
            // prop1.Value = new Value(5300);
            prop1.AutoGenerateSetMethod = false;
            prop1.AccessModifiers = CodeAccessModifiers.Public;
            clss.Elements.Add(prop1);

            CodeMethodElement methodMain = new CodeMethodElement();
            methodMain.Name = "Main";
            methodMain.IsStatic = true;
            methodMain.AccessModifiers = CodeAccessModifiers.Public;
            methodMain.DataType = Flame.DataType.Void;

            CodeMethodCallActionElement c = new CodeMethodCallActionElement();
            c.ObjectName = new string[] { "System", "Windows", "Forms", "MessageBox", "Show" };
            c.Parameters.Add("prompt", DataType.String, new CodeElementReference(new CodeLiteralElement("This is a dynamically-created MessageBox via Glue!")));
            c.Parameters.Add("title", DataType.String, new CodeElementReference(new CodeLiteralElement("What do you think?")));
            c.Parameters.Add("buttons", DataType.Int32, new CodeElementReference(new CodeLiteralElement("System.Windows.Forms.MessageBoxButtons.OK")));
            c.Parameters.Add("icon", DataType.Int32, new CodeElementReference(new CodeLiteralElement("System.Windows.Forms.MessageBoxIcon.Information")));
            methodMain.Actions.Add(c);

            clss.Elements.Add(methodMain);

            // object obj = Glue.CodeCompiler.CreateObject(clss);

            CodeObjectModel env = new CodeObjectModel();
            env.Elements.Add(new CodeNamespaceElement("TestProject"));
            (env.Elements[0] as CodeNamespaceElement).Elements.Add(clss);

            // CodeCompiler compiler = new CodeCompiler();
            // compiler.AssemblyReferences.Add("System.Windows.Forms.dll");
            // compiler.CreateAssemblyFile(env, @"C:\Documents and Settings\BEC16770\Desktop\City of Orlando\Coda\SQL\test.exe", CodeCompilerTarget.Executable);

            // CodeExecutor executor = new CodeExecutor();
            // object w = executor.CreateObject(env, "TestProject.StaticPriceHandler");
            // object r = executor.ExecuteMethod(w, "Main");

            // object winform = executor.CreateObject(env, "System.Windows.Forms.Form");
            // executor.ExecuteProperty(winform, "Text", new CodeElementReference(new CodeLiteralElement("New value")));
            // executor.ExecuteProperty(winform, "Width", new CodeElementReference(new CodeLiteralElement(900)));
            // executor.ExecuteProperty(winform, "Height", new CodeElementReference(new CodeLiteralElement(400)));

            // executor.ExecuteMethod(env, "System.Windows.Forms.MessageBox.Show", new Value("Form width is: " + Glue.ManagedCode.ExecuteProperty(winform, "Height").ToString()), new Value("test box"), new Value("System.Windows.Forms.MessageBoxButtons.RetryCancel", true), new Value("System.Windows.Forms.MessageBoxIcon.Error", true));

            // object textbox = Glue.ManagedCode.CreateObject(env, "System.Windows.Forms.TextBox");
            // executor.ExecuteProperty(textbox, "Width", new Value(600));
            // executor.ExecuteProperty(textbox, "Anchor", new Value("System.Windows.Forms.AnchorStyles.Right", true));


            // object controlsCollection = Glue.ManagedCode.ExecuteProperty(winform, "Controls");
            // Glue.ManagedCode.ExecuteMethod(controlsCollection, "Add", textbox);


            // Glue.ManagedCode.ExecuteMethod(winform, "ShowDialog");

            #endregion
            #region test 2
            /*
            Flame.Script sc = new Flame.Script();

            // TODO: Fix this!!
            // sc.LoadFile(@"F:\Profiles\Mike Becker\Documents\Projects\Software\Common Language Runtime\Applications\Open Reality Engine\bin\Debug\ORE libraries\OREMainTest1.or", Flame.Languages.CSharp);
            
            Flame.Environment env = new Flame.Environment();

            sc.Elements.Add(new Flame.LanguageElements.Class("TestClass"));
            (sc.Elements["TestClass"] as Flame.LanguageElements.Class).Elements.Add(new Flame.LanguageElements.Property("myTestProp", Flame.AccessModifiers.Public, Flame.DataType.Int16, new Flame.Value(322)));

            env.Scripts.Add(sc);

            object tc = Glue.CodeCompiler.CreateObject(env, "TestClass");

            Glue.CodeCompiler.ExecuteMethod(tc, "Main");
             */
            #endregion
        }
    }
}
