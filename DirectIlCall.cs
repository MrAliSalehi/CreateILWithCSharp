using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CreateILWithCSharp
{
    internal static class DirectIlCall
    {
        public static void DirectCall()
        {
            var asmName = new AssemblyName
            {
                Name = "HellAsm"
            };
            var moduleBuilder = AssemblyBuilder
                 .DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run)
                 .DefineDynamicModule("HellModule");

            var typeBuilder = moduleBuilder.DefineType("HellType", TypeAttributes.Public);

            var methodBuilder = typeBuilder.DefineMethod("Print", MethodAttributes.Public | MethodAttributes.Static);

            var generator = methodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldstr, "Hello Boys From Directly Call!");

            var paramTypes = new Type[1];
            paramTypes[0] = typeof(string);

            var consoleType = typeof(Console);
            var methodInfo = consoleType.GetMethod(nameof(Console.WriteLine), paramTypes);

            generator.Emit(OpCodes.Call, methodInfo!);
            generator.Emit(OpCodes.Ret);

            var type = typeBuilder.CreateType();

            type!.GetMethod("Print")!.Invoke(null, null);
        }
    }

}
