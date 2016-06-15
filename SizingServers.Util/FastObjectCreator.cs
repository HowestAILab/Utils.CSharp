/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SizingServers.Util {
    /// <summary>
    /// <para>Use this to create a new instance of a type using an empty public constructor and Intermediate Language. This used to be faster than Activator.CreateInstance(...),but speaking from own experience this is no longer the case with modern .Net.</para>
    /// <para>However, the type's constructor is cached when calling CreateInstance, wich makes this faster when creating a lot of the same objects.</para>
    /// <para>I'm not at all the inventor of this way of working, check for instance this blog post: https://codingsolution.wordpress.com/2013/07/12/activator-createinstance-is-slow/ </para>
    /// <para>You would use this (or Activator) when you know only a class type, e.g.</para>
    /// <para>You load a custom text-based save file, if it is a custom type you can cast to an interface in the return type definition:</para>
    /// <para><![CDATA[ var compilerUnit = new CompilerUnit(); ]]></para>
    /// <para><![CDATA[ CompilerResults compilerResults = null; ]]></para>
    /// <para><![CDATA[ Type t = null; ]]></para>
    /// <para><![CDATA[ Assembly as = compilerUnit.Compile("<Insert code here>", debug, out compilerResults); ]]></para>
    /// <para><![CDATA[ if (!compilerResults.Errors.HasErrors) { ]]></para>
    /// <para><![CDATA[ t = _connectionProxyAssembly.GetType("MyNamespace.MyClass"); ]]></para>
    /// <para><![CDATA[ var obj = FastObjectCreator<IMyInterface>.CreateInstance(t); ]]></para>
    /// <para><![CDATA[ } ]]></para>
    /// <para>For primitives and structs Activator is used since they cannot have an explicit parameterless constructor.</para>
    /// </summary>  
    public static class FastObjectCreator {
        private delegate object Ctor();

        //To cache constructors.
        private static FunctionOutputCache _functionOutputCache = new FunctionOutputCache();

        /// <summary>
        /// <para>Use this to create a new instance of a type using an empty public constructor and Intermediate Language. This used to be faster than Activator.CreateInstance(...),but speaking from own experience this is no longer the case with modern .Net.</para>
        /// <para>However, the type's constructor is cached when calling CreateInstance, wich makes this faster when creating a lot of the same objects.</para>
        /// <para>I'm not at all the inventor of this way of working, check for instance this blog post: https://codingsolution.wordpress.com/2013/07/12/activator-createinstance-is-slow/ </para>
        /// <para>You would use this (or Activator) when you know only a class type, e.g.</para>
        /// <para>You load a custom text-based save file, if it is a custom type you can cast to an interface in the return type definition:</para>
        /// <para><![CDATA[ var compilerUnit = new CompilerUnit(); ]]></para>
        /// <para><![CDATA[ CompilerResults compilerResults = null; ]]></para>
        /// <para><![CDATA[ Type t = null; ]]></para>
        /// <para><![CDATA[ Assembly as = compilerUnit.Compile("<Insert code here>", debug, out compilerResults); ]]></para>
        /// <para><![CDATA[ if (!compilerResults.Errors.HasErrors) { ]]></para>
        /// <para><![CDATA[ t = _connectionProxyAssembly.GetType("MyNamespace.MyClass"); ]]></para>
        /// <para><![CDATA[ var obj = FastObjectCreator<IMyInterface>.CreateInstance(t); ]]></para>
        /// <para><![CDATA[ } ]]></para>
        /// <para>For primitives and structs Activator is used since they cannot have an explicit parameterless constructor.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type type) {
            if (type.IsValueType)
                return (T)Activator.CreateInstance(type);
            return (T)(GetConstructor(type)());
        }

        private static Ctor GetConstructor(Type type) {            
            var cacheEntry = _functionOutputCache.GetOrAdd(MethodBase.GetCurrentMethod(), type);

            if (cacheEntry.ReturnValue == null) {
                var method = new DynamicMethod(string.Empty, type, null);

                ILGenerator gen = method.GetILGenerator();
                gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));// new Ctor
                gen.Emit(OpCodes.Ret);

                cacheEntry.ReturnValue = method.CreateDelegate(typeof(Ctor));
            }

            return cacheEntry.ReturnValue as Ctor;
        }
    }
}
