using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;


namespace DesignPatterns
{
    //public class TypeGenerator<T> where T : new()
    //{
    public static class TypeGenerator<T>
    {
        //public static Type CreateType(Type baseType, string name, FieldInfo field, object obj, string fieldName)
        //{
        //    // Create the assembly and module
        //    AssemblyBuilder assemblyBuilder = CreateAssembly("TypeGeneratorAssembly");
        //    ModuleBuilder moduleBuilder = CreateModule(assemblyBuilder, "TypeGeneratorModule");

        //    // Create the new type
        //    TypeBuilder typeBuilder = CreateType(moduleBuilder, name, baseType);

        //    // Add the constructor
        //    ConstructorBuilder constructor = CreateConstructor(typeBuilder, baseType);

        //    // Add the wrapper properties and methods
        //    CreateProperties(typeBuilder, field, obj, fieldName);
        //    CreateMethods(typeBuilder, obj);

        //    // Create and return the new type
        //    return typeBuilder.CreateType();

        public static Type CreateType(string typeName, FieldInfo wrappedField, object wrappedObject, Type baseType)
        {
            AssemblyName assemblyName = new AssemblyName(typeName + "Assembly");
            AssemblyBuilder assemblyBuilder = CreateAssembly(assemblyName);

            ModuleBuilder moduleBuilder = CreateModule(assemblyBuilder, typeName);

            TypeBuilder typeBuilder = CreateType(moduleBuilder, typeName, wrappedObject);

            ConstructorBuilder constructorBuilder = CreateConstructor(typeBuilder, baseType);

            //CreateProperties(typeBuilder, wrappedField, wrappedObject);
            CreateProperties(typeBuilder, wrappedField);
            CreateMethods(typeBuilder, wrappedField, wrappedObject);

            var type = typeBuilder.CreateType();
            typeBuilder = null;

            return type;
        }

        private static AssemblyBuilder CreateAssembly(AssemblyName assemblyName)
        {
            return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }

        private static ModuleBuilder CreateModule(AssemblyBuilder assemblyBuilder, string typeName)
        {
            return assemblyBuilder.DefineDynamicModule(typeName + "Module");
        }

        private static TypeBuilder CreateType(ModuleBuilder moduleBuilder, string typeName, object wrappedObject)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                typeof(object)
            );

            return typeBuilder;
        }
        //private static AssemblyBuilder CreateAssembly(string assemblyName)
        //{
        //    AssemblyName name = new AssemblyName(assemblyName);
        //    return AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
        //}

        //private static ModuleBuilder CreateModule(AssemblyBuilder assemblyBuilder, string moduleName)
        //{
        //    return assemblyBuilder.DefineDynamicModule(moduleName);
        //}

        //private static TypeBuilder CreateType(ModuleBuilder moduleBuilder, string typeName, Type baseType)
        //{
        //    return moduleBuilder.DefineType(typeName, TypeAttributes.Public, baseType);
        //}

        //private static ConstructorBuilder CreateConstructor(TypeBuilder typeBuilder, Type baseType)
        //{
        //    ConstructorBuilder constructor = typeBuilder.DefineConstructor(
        //        MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(object) }
        //    );
        //    ILGenerator ilGenerator = constructor.GetILGenerator();
        //    ilGenerator.Emit(OpCodes.Ldarg_0);
        //    ilGenerator.Emit(OpCodes.Ldarg_1);
        //    ilGenerator.Emit(OpCodes.Call, baseType.GetConstructor(new[] { typeof(object) }));
        //    ilGenerator.Emit(OpCodes.Ret);
        //    return constructor;
        //}

        private static ConstructorBuilder CreateConstructor(TypeBuilder typeBuilder, Type baseType)
        {
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes
            );
            ILGenerator ilGenerator = constructor.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Ret);
            return constructor;
        }

        //public static Type? CreateType(
        //    object _lock, Type? runTimeType, string assemblyName, FieldInfo fieldInfo, T obj, string domainName
        //)
        //{
        //    if (runTimeType == null)
        //    {
        //        lock (_lock)
        //        {
        //            if (runTimeType == null)
        //            {
        //                return GenerateType(assemblyName, fieldInfo, obj, domainName);
        //            }
        //        }
        //    }
        //    return runTimeType;
        //}

        //private static Type GenerateType(string name, FieldInfo fieldInfo, T obj, string domainName)
        //{
        //    // Create a TypeBuilder for the Wrapper class
        //    AssemblyName assemblyName = new AssemblyName(name + "Assembly");
        //    AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        //    ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name + "Module");
        //    TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);
        //    FieldBuilder fieldBuilder = typeBuilder.DefineField(domainName, fieldInfo.FieldType, FieldAttributes.Public);

        //    GenerateProperties(typeBuilder, fieldInfo);
        //    GenerateMethods(typeBuilder, fieldInfo);

        //    //// Create the Wrapper type
        //    Type wrapperType = typeBuilder.CreateType();
        //    //// Create an instance of the Wrapper type
        //    object wrapper = Activator.CreateInstance(wrapperType);

        //    FieldInfo[] fs = wrapperType.GetFields();
        //    foreach (var field in fs) { Console.WriteLine("field: " + field.Name);  }

        //    // Check if the Wrapper type has any properties
        //    PropertyInfo[] properties = wrapperType.GetProperties();
        //    if (properties.Length == 0)
        //    {
        //        Console.WriteLine("The Wrapper type has no properties.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("The Wrapper type has the following properties:");
        //        foreach (PropertyInfo property in properties)
        //        {
        //            Console.WriteLine(property.Name);
        //        }
        //    }

        //    MethodInfo[] methods = wrapperType.GetMethods();
        //    if (properties.Length == 0)
        //    {
        //        Console.WriteLine("The Wrapper type has no methods.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("The Wrapper type has the following methods:");
        //        foreach (var property in methods)
        //        {
        //            Console.WriteLine(property.Name);
        //        }
        //    }

        //    Console.WriteLine("type of wrapper: "  + wrapper);

        //    //T domainObject = new T();
        //    //fieldInfo.SetValue(wrapper, domainObject);
        //    //wrapperType.GetMethod("Hello").Invoke(wrapper, new object[] { domainObject });
        //    //try
        //    //{
        //    //    wrapperType.GetMethod("Hello").Invoke(wrapper, null);
        //    //}
        //    //catch (TargetInvocationException ex)
        //    //{
        //    //    Console.WriteLine("EXCEPTION: " + ex.InnerException.Message);
        //    //}

        //    return wrapperType;
        //}

        //private static void GenerateProperties(TypeBuilder typeBuilder, FieldInfo fieldInfo)
        //private static void CreateProperties(
        //    TypeBuilder typeBuilder, FieldInfo field, object obj, string fieldName
        //)
        //private static void CreateProperties(TypeBuilder typeBuilder, FieldInfo wrappedField, object wrappedObject)
        //{
        //    //Type type = typeof(T);
        //    Type type = wrappedObject.GetType();
        //    PropertyInfo[] properties = type.GetProperties();

        //    foreach (PropertyInfo property in properties)
        //    {
        //        string propertyName = property.Name;
        //        Type propertyType = property.PropertyType;
        //        PropertyBuilder builder = typeBuilder.DefineProperty(
        //            propertyName, PropertyAttributes.None, propertyType, null
        //        );

        //        MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
        //        ILGenerator getIL = getMethodBuilder.GetILGenerator();
        //        getIL.Emit(OpCodes.Ldarg_0);
        //        //getIL.Emit(OpCodes.Ldfld, fieldInfo);
        //        getIL.Emit(OpCodes.Ldfld, wrappedField);
        //        getIL.Emit(OpCodes.Callvirt, property.GetGetMethod());
        //        getIL.Emit(OpCodes.Ret);
        //        builder.SetGetMethod(getMethodBuilder);

        //        MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { propertyType });
        //        ILGenerator setIL = setMethodBuilder.GetILGenerator();
        //        setIL.Emit(OpCodes.Ldarg_0);
        //        //setIL.Emit(OpCodes.Ldfld, fieldInfo);
        //        setIL.Emit(OpCodes.Ldfld, wrappedField);
        //        setIL.Emit(OpCodes.Ldarg_1);
        //        setIL.Emit(OpCodes.Callvirt, property.GetSetMethod());
        //        setIL.Emit(OpCodes.Ret);
        //        builder.SetSetMethod(setMethodBuilder);
        //    }
        //}

        private static void CreateProperties(TypeBuilder typeBuilder, FieldInfo wrappedField)
        {
            PropertyInfo[] wrappedObjectProperties = wrappedField.FieldType.GetProperties();
            foreach (PropertyInfo wrappedObjectProperty in wrappedObjectProperties)
            {
                string propertyName = wrappedObjectProperty.Name;
                Type propertyType = wrappedObjectProperty.PropertyType;

                FieldBuilder backingField = typeBuilder.DefineField($"_{propertyName.ToLower()}", propertyType, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
                getILGenerator.Emit(OpCodes.Ldarg_0);
                getILGenerator.Emit(OpCodes.Ldfld, wrappedField);
                getILGenerator.Emit(OpCodes.Callvirt, wrappedObjectProperty.GetGetMethod());
                getILGenerator.Emit(OpCodes.Ret);

                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { propertyType });
                ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();
                setILGenerator.Emit(OpCodes.Ldarg_0);
                setILGenerator.Emit(OpCodes.Ldfld, wrappedField);
                setILGenerator.Emit(OpCodes.Ldarg_1);
                setILGenerator.Emit(OpCodes.Callvirt, wrappedObjectProperty.GetSetMethod());
                setILGenerator.Emit(OpCodes.Ret);
            }
        }

        //private static void GenerateMethods(TypeBuilder typeBuilder, FieldInfo fieldInfo)
        private static void CreateMethods(
            TypeBuilder typeBuilder, FieldInfo wrappedField, object wrappedObject
        )
        {
            //Type type = typeof(T);
            Type type = wrappedObject.GetType();
            MethodInfo[] methods = type.GetMethods();

            //foreach (MethodInfo method in methods)
            //{
            //    string methodName = method.Name;
            //    Type[] parameterTypes = method.GetParameters().Select(
            //        p => p.ParameterType).ToArray();
            //    Type returningType = method.ReturnType;

            //    MethodBuilder methodBuilder = typeBuilder.DefineMethod(
            //        methodName, MethodAttributes.Public, returningType, parameterTypes
            //    );
            //    ILGenerator methodIL = methodBuilder.GetILGenerator();
            //    methodIL.Emit(OpCodes.Ldarg_0);
            //    methodIL.Emit(OpCodes.Ldfld, fieldInfo);

            //    for (int i = 1; i <= parameterTypes.Length; i++)
            //    {
            //        methodIL.Emit(OpCodes.Ldarg_1);
            //    }

            //    methodIL.Emit(OpCodes.Callvirt, method);
            //    methodIL.Emit(OpCodes.Ret);
            //}
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.IsPublic && methodInfo.DeclaringType != typeof(object))
                {
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    Type[] parameterTypes = parameterInfos.Select(p => p.ParameterType).ToArray();

                    MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                        methodInfo.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        methodInfo.ReturnType,
                        parameterTypes);

                    ILGenerator ilGenerator = methodBuilder.GetILGenerator();
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, wrappedField);
                    for (int i = 1; i <= parameterInfos.Length; i++)
                    {
                        ilGenerator.Emit(OpCodes.Ldarg, i);
                    }
                    ilGenerator.Emit(OpCodes.Callvirt, methodInfo);
                    ilGenerator.Emit(OpCodes.Ret);
                }
            }
        }
    }
}
