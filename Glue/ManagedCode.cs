using System;
using System.Collections.Generic;

using System.Text;

using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Diagnostics;

namespace Glue
{
    /// <summary>
    /// Provides functions that allow Flame scripts to interoperate
    /// with managed code, as well as allowing Flame scripts to be
    /// compiled into managed code libraries for faster execution.
    /// </summary>
    public static class ManagedCode
    {
        /// <summary>
        /// Creates an instance of the specified Flame class as a .NET object.
        /// </summary>
        /// <param name="clss"></param>
        /// <returns></returns>
        public static object CreateObject(Flame.LanguageElements.Class clss)
        {
            Flame.Environment env = new Flame.Environment();
            env.Scripts.Add(new Flame.Script());
            env.Scripts[0].Elements.Add(clss);

            return CreateObject(env, clss);
        }
        public static object CreateObject(Flame.Environment env, Flame.LanguageElements.Class clss)
        {
            Assembly asm = CreateAssembly(env);
            return asm.CreateInstance(clss.FullName);
        }
        public static object CreateObject(Flame.Environment env, string ClassName)
        {
            Flame.LanguageElement element = env.FindElement(ClassName);
            if (element == null)
            {
                Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in asms)
                {
                    object o = asm.CreateInstance(ClassName);
                    if (o != null)
                    {
                        return o;
                    }
                }
            }
            else if (element is Flame.LanguageElements.Class)
            {
                Flame.LanguageElements.Class clss = (element as Flame.LanguageElements.Class);
                return CreateObject(env, clss);
            }
            return null;
        }

        public static Type GetObjectReference(Flame.Environment env, string ClassName)
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asms)
            {
                Type t = asm.GetType(ClassName);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        private static CodeTypeReference GetCodeTypeReference(Flame.DataType dataType)
        {
            if (dataType.IsArray)
            {
                return new CodeTypeReference(dataType.GetFullName(), dataType.ArrayLength);
            }
            return new CodeTypeReference(dataType.GetFullName());
        }

        public static Assembly CreateAssembly(Flame.Environment env)
        {
            return CreateAssembly(env, CodeCompilerTarget.Library);
        }
        public static Assembly CreateAssembly(Flame.Environment env, CodeCompilerTarget target)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            GenericCodeProvider gp = new GenericCodeProvider();
            CompilerParameters options = new CompilerParameters();

            options.GenerateInMemory = false;
            switch (target)
            {
                case CodeCompilerTarget.Executable:
                    options.CompilerOptions += " /target:exe";
                    break;
                case CodeCompilerTarget.Library:
                    options.CompilerOptions += " /target:library";
                    break;
                case CodeCompilerTarget.Module:
                    options.CompilerOptions += " /target:module";
                    break;
                case CodeCompilerTarget.WindowsExecutable:
                    options.CompilerOptions += " /target:winexe";
                    break;
            }

            foreach (Flame.Script script in env.Scripts)
            {
                foreach (Flame.LanguageElement e in script.Elements)
                {
                    AppendLanguageElement(env, e, unit);
                }
            }

            foreach (string s in env.AssemblyReferences)
            {
                unit.ReferencedAssemblies.Add(s);
            }

            CompilerResults results = gp.CompileAssemblyFromDom(options, unit);
            if (results.Errors.HasErrors)
            {
                string errors = String.Empty;
                foreach (CompilerError err in results.Errors)
                {
                    errors += err.ErrorText + "\r\n\r\n";
                }

                throw new CodeCompilerException(errors);
            }

            return results.CompiledAssembly;
        }
        public static void CreateAssemblyFile(Flame.Environment env, string FileName)
        {
            CreateAssemblyFile(env, FileName, CodeCompilerTarget.Library);
        }
        public static void CreateAssemblyFile(Flame.Environment env, string FileName, CodeCompilerTarget target)
        {
            Assembly asm = CreateAssembly(env, target);
            System.IO.File.Copy(asm.Location, FileName, true);
        }

        private static void AppendLanguageElement(Flame.Environment env, Flame.LanguageElement e, CodeCompileUnit parentUnit)
        {
            if (e is Flame.LanguageElements.Namespace)
            {
                CodeNamespace ns = new CodeNamespace(e.Name);
                foreach (Flame.LanguageElement e1 in (e as Flame.LanguageElements.Namespace).Elements)
                {
                    AppendLanguageElement(env, e1, parentUnit, ns);
                }
                parentUnit.Namespaces.Add(ns);
            }
            else if (e is Flame.LanguageElements.Class)
            {
                CodeNamespace defaultNamespace = new CodeNamespace();
                AppendLanguageElement(env, e, parentUnit, defaultNamespace);
                if (!parentUnit.Namespaces.Contains(defaultNamespace))
                {
                    parentUnit.Namespaces.Add(defaultNamespace);
                }
            }
        }
        private static void AppendLanguageElement(Flame.Environment env, Flame.LanguageElement e, CodeCompileUnit parentUnit, CodeNamespace ns)
        {
            /*
             * // TODO: Figure out how to fix this
            if (e is Flame.LanguageElements.Namespace)
            {
                AppendLanguageElement(e, parentUnit);
            }
            else */ if (e is Flame.LanguageElements.Class)
            {
                CodeTypeDeclaration decl = new CodeTypeDeclaration(e.Name);
                foreach (Flame.LanguageElement e1 in (e as Flame.LanguageElements.Class).Elements)
                {
                    AppendLanguageElement(env, e1, parentUnit, decl);
                }
                ns.Types.Add(decl);
            }
        }
        private static void AppendLanguageElement(Flame.Environment env, Flame.LanguageElement e, CodeCompileUnit parentUnit, CodeTypeDeclaration decl)
        {
            if (e is Flame.LanguageElements.Class)
            {
                CodeTypeDeclaration decl1 = new CodeTypeDeclaration(e.Name);
                foreach (Flame.LanguageElement e1 in (e as Flame.LanguageElements.Class).Elements)
                {
                    AppendLanguageElement(env, e1, parentUnit, decl1);
                }
                decl.Members.Add(decl1);
            }
            else if (e is Flame.LanguageElements.Method)
            {
                Flame.LanguageElements.Method meth = (e as Flame.LanguageElements.Method);

                CodeMemberMethod prop = new CodeMemberMethod();
                prop.Name = meth.Name;
                prop.Attributes = 0;

                foreach (Flame.Call c in meth.Calls)
                {
                    if (c.CallType == Flame.CallType.Method)
                    {
                        CodeExpression[] parameters = new CodeExpression[c.Parameters.Count];
                        for (int i = 0; i < c.Parameters.Count; i++)
                        {
                            if (c.Parameters[i].Value.DataType.IsPrimitive || (c.Parameters[i].Value.DataType == typeof(string)))
                            {
                                if (c.Parameters[i].Value.DataType == typeof(string) && c.Parameters[i].Value.IsEnumValue)
                                {
                                    string enumNameAndValue = c.Parameters[i].Value.GetValue().ToString();
                                    string enumName = enumNameAndValue.Substring(0, enumNameAndValue.LastIndexOf('.'));
                                    string enumValue = enumNameAndValue.Substring(enumName.Length + 1);

                                    parameters[i] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(enumName), enumValue);
                                }
                                else
                                {
                                    parameters[i] = new CodePrimitiveExpression(c.Parameters[i].Value.GetValue());
                                }
                            }
                            else
                            {
                                parameters[i] = new CodeObjectCreateExpression(c.Parameters[i].Value.DataType);
                            }
                        }

                        CodeTypeReferenceExpression objectref = new CodeTypeReferenceExpression(String.Join(".", c.ObjectName, 0, c.ObjectName.Length - 1));
                        
                        CodeMethodInvokeExpression methcall = new CodeMethodInvokeExpression(objectref, c.ObjectName[c.ObjectName.Length - 1], parameters);
                        prop.Statements.Add(methcall);
                    }
                }

                if (meth.DataType.IsArray)
                {
                    prop.ReturnType = new CodeTypeReference(meth.DataType.GetFullName(), meth.DataType.ArrayLength);
                }
                else
                {
                    prop.ReturnType = new CodeTypeReference(meth.DataType.GetFullName());
                }

                if (meth.IsStatic)
                {
                    prop.Attributes |= MemberAttributes.Static;
                }
                if (meth.IsAbstract)
                {
                    prop.Attributes |= MemberAttributes.Abstract;
                }
                decl.Members.Add(prop);
            }
            else if (e is Flame.LanguageElements.Property)
            {
                Flame.LanguageElements.Property meth = (e as Flame.LanguageElements.Property);

                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Name = meth.Name;
                prop.Type = GetCodeTypeReference(meth.DataType);
                prop.Attributes = 0;

                if (meth.AutoGenerateGetMethod || meth.AutoGenerateSetMethod)
                {
                    CodeMemberField field = new CodeMemberField(GetCodeTypeReference(meth.DataType), "m_" + meth.Name);
                    if (meth.Value.HasValue)
                    {
                        if (meth.Value.DataType.IsPrimitive || (meth.Value.DataType == typeof(string)))
                        {
                            field.InitExpression = new CodePrimitiveExpression(meth.Value.GetValue());
                        }
                        else
                        {
                            field.InitExpression = new CodeObjectCreateExpression(meth.Value.DataType);
                        }
                    }
                    decl.Members.Add(field);
                }

                if (meth.GetMethod != null)
                {
                    prop.GetStatements.Add(new CodeExpression());
                }
                else if (meth.AutoGenerateGetMethod)
                {
                    prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("m_" + meth.Name)));
                }

                if (meth.SetMethod != null)
                {
                    prop.SetStatements.Add(new CodeExpression());
                }
                else if (meth.AutoGenerateSetMethod)
                {
                    prop.SetStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("m_" + prop.Name), new CodeVariableReferenceExpression("value")));
                }

                if (meth.IsStatic)
                {
                    prop.Attributes |= MemberAttributes.Static;
                }
                if (meth.IsAbstract)
                {
                    prop.Attributes |= MemberAttributes.Abstract;
                }
                if (!meth.IsOverridable)
                {
                    prop.Attributes |= MemberAttributes.Final;
                }

                switch (meth.AccessModifiers)
                {
                    case Flame.AccessModifiers.Public:
                        prop.Attributes |= MemberAttributes.Public;
                        break;
                    case Flame.AccessModifiers.Private:
                        prop.Attributes |= MemberAttributes.Private;
                        break;
                    case Flame.AccessModifiers.Assembly:
                        prop.Attributes |= MemberAttributes.Assembly;
                        break;
                    case Flame.AccessModifiers.FamilyANDAssembly:
                        prop.Attributes |= MemberAttributes.FamilyAndAssembly;
                        break;
                    case Flame.AccessModifiers.FamilyORAssembly:
                        prop.Attributes |= MemberAttributes.FamilyOrAssembly;
                        break;
                    case Flame.AccessModifiers.Family:
                        prop.Attributes |= MemberAttributes.Family;
                        break;
                }

                decl.Members.Add(prop);
            }
        }

        public static object ExecuteMethod(Type staticref, string method)
        {
            return ExecuteMethod(staticref, method, new object[0]);
        }
        public static object ExecuteMethod(Type staticref, string method, params object[] parameters)
        {
            Flame.Value[] paramz = new Flame.Value[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                paramz[i] = new Flame.Value(parameters[i]);
            }
            return ExecuteMethod(staticref, method, paramz);
        }
        public static object ExecuteMethod(Type staticref, string method, params Flame.Value[] parameters)
        {
            Type[] parameterTypes = new Type[parameters.Length];

            object[] parameterz = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].DataType == typeof(string) && parameters[i].IsEnumValue)
                {
                    string enumNameAndValue = parameters[i].GetValue().ToString();
                    string enumName = enumNameAndValue.Substring(0, enumNameAndValue.LastIndexOf('.'));
                    string enumValue = enumNameAndValue.Substring(enumName.Length + 1);

                    Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly asm in asms)
                    {
                        Type fi = asm.GetType(enumName);
                        if (fi != null)
                        {
                            parameterz[i] = fi.GetField(enumValue).GetValue(enumValue);
                            if (parameterz[i] != null)
                            {
                                parameterTypes[i] = parameterz[i].GetType();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    parameterz[i] = parameters[i].GetValue();
                    parameterTypes[i] = parameterz[i].GetType();
                }
            }
            System.Reflection.MethodInfo mi = staticref.GetMethod(method, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, parameterTypes, new System.Reflection.ParameterModifier[0]);


            return mi.Invoke(null, parameterz);
        }

        [DebuggerNonUserCode()]
        public static object ExecuteMethod(object objref, string method)
        {
            return ExecuteMethod(objref, method, new object[0]);
        }

        [DebuggerNonUserCode()]
        public static object ExecuteMethod(object objref, string method, params object[] parameters)
        {
            Type typ = objref.GetType();

            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }

            System.Reflection.MethodInfo mi = typ.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, types, new ParameterModifier[0]);
            if (mi == null)
            {
                throw new MemberAccessException("Could not find method \"" + method + "\" in object of type \"" + objref.GetType().FullName + "\"");
            }

            return mi.Invoke(objref, parameters);
        }

        public static object ExecuteMethod(Flame.Environment env, string method, params Flame.Value[] parameters)
        {
            string className = method.Substring(0, method.LastIndexOf('.'));
            string methodName = method.Substring(className.Length + 1);

            Type msgbox = Glue.ManagedCode.GetObjectReference(env, className);
            return ExecuteMethod(msgbox, methodName, parameters);
        }

        /// <summary>
        /// Executes a property SET method.
        /// </summary>
        /// <param name="obj">The object on which to set the property.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="propertyValue">The new value to which the property should be set.</param>
        /// <param name="indexValues"></param>
        public static void ExecuteProperty(object obj, string propertyName, Flame.Value propertyValue)
        {
            ExecuteProperty(obj, propertyName, propertyValue, new object[0]);
        }
        /// <summary>
        /// Executes a property SET method.
        /// </summary>
        /// <param name="obj">The object on which to set the property.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="propertyValue">The new value to which the property should be set.</param>
        /// <param name="indexValues"></param>
        public static void ExecuteProperty(object obj, string propertyName, Flame.Value propertyValue, params object[] indexValues)
        {
            Type typ = obj.GetType();

            Type[] indexTypes = new Type[indexValues.Length];
            for (int i = 0; i < indexValues.Length; i++)
            {
                indexTypes[i] = indexValues[i].GetType();
            }

            object v = propertyValue.GetValue();
            if (propertyValue.DataType == typeof(string) && propertyValue.IsEnumValue)
            {
                string enumNameAndValue = propertyValue.GetValue().ToString();
                string enumName = enumNameAndValue.Substring(0, enumNameAndValue.LastIndexOf('.'));
                string enumValue = enumNameAndValue.Substring(enumName.Length + 1);

                Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in asms)
                {
                    Type fi = asm.GetType(enumName);
                    if (fi != null)
                    {
                        v = fi.GetField(enumValue).GetValue(enumValue);
                        if (v != null)
                        {
                            break;
                        }
                    }
                }
            }

            System.Reflection.PropertyInfo pi = typ.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, v.GetType(), indexTypes, new ParameterModifier[0]);
            if (pi == null)
            {
                throw new MemberAccessException("Could not find property with name \"" + propertyName + "\" on type \"" + obj.GetType().FullName + "\"");
            }
            pi.GetSetMethod().Invoke(obj, new object[] { v });
        }
        /// <summary>
        /// Executes a property SET method.
        /// </summary>
        /// <param name="obj">The object on which to set the property.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="propertyValue">The new value to which the property should be set.</param>
        /// <param name="indexValues"></param>
        public static object ExecuteProperty(object obj, string propertyName)
        {
            return ExecuteProperty(obj, propertyName, new object[0]);
        }
        /// <summary>
        /// Executes a property SET method.
        /// </summary>
        /// <param name="obj">The object on which to set the property.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="propertyValue">The new value to which the property should be set.</param>
        /// <param name="indexValues"></param>
        public static object ExecuteProperty(object obj, string propertyName, params object[] indexValues)
        {
            Type typ = obj.GetType();

            Type[] indexTypes = new Type[indexValues.Length];
            for (int i = 0; i < indexValues.Length; i++)
            {
                indexTypes[i] = indexValues[i].GetType();
            }

            System.Reflection.PropertyInfo pi = typ.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, null, indexTypes, new ParameterModifier[0]);
            if (pi == null)
            {
                throw new MemberAccessException("Could not find property with name \"" + propertyName + "\" on type \"" + obj.GetType().FullName + "\"");
            }
            return pi.GetGetMethod().Invoke(obj, new object[0]);
        }
    }
}
