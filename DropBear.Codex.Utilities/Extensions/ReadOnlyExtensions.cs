#region

using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace DropBear.Codex.Utilities.Extensions;

public static class ReadOnlyExtensions
{
    public static object? GetReadOnlyVersion(this object? obj)
    {
        if (obj is null)
        {
            return null;
        }

        var type = obj.GetType();

        // For value types (structs), just return the original instance
        if (type.IsValueType)
        {
            return obj;
        }

        // For reference types (classes), create a read-only wrapper
        var readOnlyType = CreateReadOnlyType(type);
        var readOnlyInstance = Activator.CreateInstance(readOnlyType, obj);
        return readOnlyInstance;
    }

    private static Type CreateReadOnlyType(Type type)
    {
        var assemblyName = new AssemblyName("ReadOnlyAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("ReadOnlyModule");
        var typeName = $"ReadOnly{type.Name}";
        var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

        typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

        var instanceField =
            typeBuilder.DefineField("_instance", type, FieldAttributes.Private | FieldAttributes.InitOnly);

        var constructor = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.HasThis,
            new[] { type }
        );

        // ReSharper disable once InconsistentNaming
        var constructorIL = constructor.GetILGenerator();
        constructorIL.Emit(OpCodes.Ldarg_0);
        constructorIL.Emit(OpCodes.Ldarg_1);
        constructorIL.Emit(OpCodes.Stfld, instanceField);
        constructorIL.Emit(OpCodes.Ret);

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var getMethod = property.GetGetMethod();
            if (getMethod is null)
            {
                continue; // Skip properties without a getter
            }

            var readOnlyProperty =
                typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);
            var getMethodBuilder = typeBuilder.DefineMethod($"get_{property.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, property.PropertyType,
                Type.EmptyTypes);

            // ReSharper disable once InconsistentNaming
            var getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, instanceField);
            getIL.Emit(OpCodes.Call, getMethod);
            getIL.Emit(OpCodes.Ret);

            readOnlyProperty.SetGetMethod(getMethodBuilder);
        }

        return typeBuilder.CreateType();
    }
}
