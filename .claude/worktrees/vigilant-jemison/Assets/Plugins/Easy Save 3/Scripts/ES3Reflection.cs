using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ES3Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ES3Internal
{
    public static class ES3Reflection
    {
        public const string memberFieldPrefix = "m_";
        public const string componentTagFieldName = "tag";
        public const string componentNameFieldName = "name";
        public static readonly string[] excludedPropertyNames = { "runInEditMode", "useGUILayout", "hideFlags" };

        public static readonly Type serializableAttributeType = typeof(SerializableAttribute);
        public static readonly Type serializeFieldAttributeType = typeof(SerializeField);
        public static readonly Type obsoleteAttributeType = typeof(ObsoleteAttribute);
        public static readonly Type nonSerializedAttributeType = typeof(NonSerializedAttribute);
        public static readonly Type es3SerializableAttributeType = typeof(ES3Serializable);
        public static readonly Type es3NonSerializableAttributeType = typeof(ES3NonSerializable);

        public static Type[] EmptyTypes = new Type[0];

        private static Assembly[] _assemblies;

        private static Assembly[] Assemblies
        {
            get
            {
                if (_assemblies == null)
                {
                    var assemblyNames = new ES3Settings().assemblyNames;
                    var assemblyList = new List<Assembly>();

                    /* We only use a try/catch block for UWP because exceptions can be disabled on some other platforms (e.g. WebGL), but the non-try/catch method doesn't work on UWP */
#if NETFX_CORE
                    for (int i = 0; i < assemblyNames.Length; i++)
                    {
                        try
                        {
                            var assembly = Assembly.Load(new AssemblyName(assemblyNames[i]));
                            if (assembly != null)
                                assemblyList.Add(assembly);
                        }
                        catch { }
                    }

#else
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                        // This try/catch block is here to catch errors such as assemblies containing double-byte characters in their path.
                        // This obviously won't work if exceptions are disabled.
                        try
                        {
                            if (assemblyNames.Contains(assembly.GetName().Name)) assemblyList.Add(assembly);
                        }
                        catch
                        {
                        }
#endif
                    _assemblies = assemblyList.ToArray();
                }

                return _assemblies;
            }
        }

        public static ConstructorInfo GetConstructor(Type type, Type[] parameters)
        {
            return type.GetTypeInfo().GetConstructor(parameters);
        }

        /*
         * 	Gets the element type of a collection or array.
         * 	Returns null if type is not a collection type.
         */
        public static Type[] GetElementTypes(Type type)
        {
            if (IsGenericType(type))
                return GetGenericArguments(type);
            if (type.IsArray)
                return new[] { GetElementType(type) };
            return null;
        }

        public static List<FieldInfo> GetSerializableFields(Type type, List<FieldInfo> serializableFields = null,
            bool safe = true, string[] memberNames = null,
            BindingFlags bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                    BindingFlags.Static | BindingFlags.DeclaredOnly)
        {
            if (type == null)
                return new List<FieldInfo>();

            var fields = type.GetFields(bindings);

            if (serializableFields == null)
                serializableFields = new List<FieldInfo>();

            foreach (var field in fields)
            {
                var fieldName = field.Name;

                // If a members array was provided as a parameter, only include the field if it's in the array.
                if (memberNames != null)
                    if (!memberNames.Contains(fieldName))
                        continue;

                var fieldType = field.FieldType;

                if (AttributeIsDefined(field, es3SerializableAttributeType))
                {
                    serializableFields.Add(field);
                    continue;
                }

                if (AttributeIsDefined(field, es3NonSerializableAttributeType))
                    continue;

                if (safe)
                    // If the field is private, only serialize it if it's explicitly marked as serializable.
                    if (!field.IsPublic && !AttributeIsDefined(field, serializeFieldAttributeType))
                        continue;

                // Exclude const or readonly fields.
                if (field.IsLiteral || field.IsInitOnly)
                    continue;

                // Don't store fields whose type is the same as the class the field is housed in unless it's stored by reference (to prevent cyclic references)
                if (fieldType == type && !IsAssignableFrom(typeof(Object), fieldType))
                    continue;

                // If property is marked as obsolete or non-serialized, don't serialize it.
                if (AttributeIsDefined(field, nonSerializedAttributeType) ||
                    AttributeIsDefined(field, obsoleteAttributeType))
                    continue;

                if (!TypeIsSerializable(field.FieldType))
                    continue;

                // Don't serialize member fields of Unity classes unless they have the SerializeField attribute.
                if (safe && field.DeclaringType.Namespace != null && fieldName.StartsWith(memberFieldPrefix) &&
                    !AttributeIsDefined(field, serializeFieldAttributeType) &&
                    field.DeclaringType.Namespace.Contains("UnityEngine"))
                    continue;

                serializableFields.Add(field);
            }

            var baseType = BaseType(type);
            if (baseType != null && baseType != typeof(object) && baseType != typeof(Object))
                GetSerializableFields(BaseType(type), serializableFields, safe, memberNames);

            return serializableFields;
        }

        public static List<PropertyInfo> GetSerializableProperties(Type type,
            List<PropertyInfo> serializableProperties = null, bool safe = true, string[] memberNames = null,
            BindingFlags bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                    BindingFlags.Static | BindingFlags.DeclaredOnly)
        {
            var isComponent = IsAssignableFrom(typeof(Component), type);

            // Only get private properties if we're not getting properties safely.
            if (!safe)
                bindings = bindings | BindingFlags.NonPublic;

            var properties = type.GetProperties(bindings);

            if (serializableProperties == null)
                serializableProperties = new List<PropertyInfo>();

            foreach (var p in properties)
            {
                if (AttributeIsDefined(p, es3SerializableAttributeType))
                {
                    serializableProperties.Add(p);
                    continue;
                }

                if (AttributeIsDefined(p, es3NonSerializableAttributeType))
                    continue;

                var propertyName = p.Name;

                if (excludedPropertyNames.Contains(propertyName))
                    continue;

                // If a members array was provided as a parameter, only include the property if it's in the array.
                if (memberNames != null)
                    if (!memberNames.Contains(propertyName))
                        continue;

                if (safe)
                    // If safe serialization is enabled, only get properties which are explicitly marked as serializable.
                    if (!AttributeIsDefined(p, serializeFieldAttributeType) &&
                        !AttributeIsDefined(p, es3SerializableAttributeType))
                        continue;

                var propertyType = p.PropertyType;

                // Don't store properties whose type is the same as the class the property is housed in unless it's stored by reference (to prevent cyclic references)
                if (propertyType == type && !IsAssignableFrom(typeof(Object), propertyType))
                    continue;

                if (!p.CanRead || !p.CanWrite)
                    continue;

                // Only support properties with indexing if they're an array.
                if (p.GetIndexParameters().Length != 0 && !propertyType.IsArray)
                    continue;

                // Check that the type of the property is one which we can serialize.
                // Also check whether an ES3Type exists for it.
                if (!TypeIsSerializable(propertyType))
                    continue;

                // Ignore certain properties on components.
                if (isComponent)
                    // Ignore properties which are accessors for GameObject fields.
                    if (propertyName == componentTagFieldName || propertyName == componentNameFieldName)
                        continue;

                // If property is marked as obsolete or non-serialized, don't serialize it.
                if (AttributeIsDefined(p, obsoleteAttributeType) || AttributeIsDefined(p, nonSerializedAttributeType))
                    continue;

                serializableProperties.Add(p);
            }

            var baseType = BaseType(type);
            if (baseType != null && baseType != typeof(object))
                GetSerializableProperties(baseType, serializableProperties, safe, memberNames);

            return serializableProperties;
        }

        public static bool TypeIsSerializable(Type type)
        {
            if (type == null)
                return false;

            if (AttributeIsDefined(type, es3NonSerializableAttributeType))
                return false;

            if (IsPrimitive(type) || IsValueType(type) || IsAssignableFrom(typeof(Component), type) ||
                IsAssignableFrom(typeof(ScriptableObject), type))
                return true;

            var es3Type = ES3TypeMgr.GetOrCreateES3Type(type, false);

            if (es3Type != null && !es3Type.isUnsupported)
                return true;

            if (TypeIsArray(type))
            {
                if (TypeIsSerializable(type.GetElementType()))
                    return true;
                return false;
            }

            var genericArgs = type.GetGenericArguments();
            for (var i = 0; i < genericArgs.Length; i++)
                if (!TypeIsSerializable(genericArgs[i]))
                    return false;

            /*if (HasParameterlessConstructor(type))
                return true;*/
            return false;
        }

        public static object CreateInstance(Type type)
        {
            if (IsAssignableFrom(typeof(Component), type))
                return ES3ComponentType.CreateComponent(type);
            if (IsAssignableFrom(typeof(ScriptableObject), type))
                return ScriptableObject.CreateInstance(type);
            if (HasParameterlessConstructor(type))
                return Activator.CreateInstance(type);
#if NETFX_CORE
                throw new NotSupportedException($"Cannot create an instance of {type} because it does not have a parameterless constructor, which is required on Universal Windows platform.");
#else
            return FormatterServices.GetUninitializedObject(type);
#endif
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            if (IsAssignableFrom(typeof(Component), type))
                return ES3ComponentType.CreateComponent(type);
            if (IsAssignableFrom(typeof(ScriptableObject), type))
                return ScriptableObject.CreateInstance(type);
            return Activator.CreateInstance(type, args);
        }

        public static Array ArrayCreateInstance(Type type, int length)
        {
            return Array.CreateInstance(type, new[] { length });
        }

        public static Array ArrayCreateInstance(Type type, int[] dimensions)
        {
            return Array.CreateInstance(type, dimensions);
        }

        public static Type MakeGenericType(Type type, Type genericParam)
        {
            return type.MakeGenericType(genericParam);
        }

        public static ES3ReflectedMember[] GetSerializableMembers(Type type, bool safe = true,
            string[] memberNames = null)
        {
            if (type == null)
                return new ES3ReflectedMember[0];

            var fieldInfos = GetSerializableFields(type, new List<FieldInfo>(), safe, memberNames);
            var propertyInfos = GetSerializableProperties(type, new List<PropertyInfo>(), safe, memberNames);
            var reflectedFields = new ES3ReflectedMember[fieldInfos.Count + propertyInfos.Count];

            for (var i = 0; i < fieldInfos.Count; i++)
                reflectedFields[i] = new ES3ReflectedMember(fieldInfos[i]);
            for (var i = 0; i < propertyInfos.Count; i++)
                reflectedFields[i + fieldInfos.Count] = new ES3ReflectedMember(propertyInfos[i]);

            return reflectedFields;
        }

        public static ES3ReflectedMember GetES3ReflectedProperty(Type type, string propertyName)
        {
            var propertyInfo = GetProperty(type, propertyName);
            return new ES3ReflectedMember(propertyInfo);
        }

        public static ES3ReflectedMember GetES3ReflectedMember(Type type, string fieldName)
        {
            var fieldInfo = GetField(type, fieldName);
            return new ES3ReflectedMember(fieldInfo);
        }

        /*
         * 	Finds all classes of a specific type, and then returns an instance of each.
         * 	Ignores classes which can't be instantiated (i.e. abstract classes, those without parameterless constructors).
         */
        public static IList<T> GetInstances<T>()
        {
            var instances = new List<T>();
            foreach (var assembly in Assemblies)
            foreach (var type in assembly.GetTypes())
                if (IsAssignableFrom(typeof(T), type) && HasParameterlessConstructor(type) && !IsAbstract(type))
                    instances.Add((T)Activator.CreateInstance(type));
            return instances;
        }

        public static IList<Type> GetDerivedTypes(Type derivedType)
        {
            return
            (
                from assembly in Assemblies
                from type in assembly.GetTypes()
                where IsAssignableFrom(derivedType, type)
                select type
            ).ToList();
        }

        public static bool IsAssignableFrom(Type a, Type b)
        {
            return a.IsAssignableFrom(b);
        }

        public static Type GetGenericTypeDefinition(Type type)
        {
            return type.GetGenericTypeDefinition();
        }

        public static Type[] GetGenericArguments(Type type)
        {
            return type.GetGenericArguments();
        }

        public static int GetArrayRank(Type type)
        {
            return type.GetArrayRank();
        }

        public static string GetAssemblyQualifiedName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public static ES3ReflectedMethod GetMethod(Type type, string methodName, Type[] genericParameters,
            Type[] parameterTypes)
        {
            return new ES3ReflectedMethod(type, methodName, genericParameters, parameterTypes);
        }

        public static bool TypeIsArray(Type type)
        {
            return type.IsArray;
        }

        public static Type GetElementType(Type type)
        {
            return type.GetElementType();
        }

#if NETFX_CORE
		public static bool IsAbstract(Type type)
		{
		return type.GetTypeInfo().IsAbstract;
		}

		public static bool IsInterface(Type type)
		{
		return type.GetTypeInfo().IsInterface;
		}

		public static bool IsGenericType(Type type)
		{
		return type.GetTypeInfo().IsGenericType;
		}

		public static bool IsValueType(Type type)
		{
		return type.GetTypeInfo().IsValueType;
		}

		public static bool IsEnum(Type type)
		{
		return type.GetTypeInfo().IsEnum;
		}

		public static bool HasParameterlessConstructor(Type type)
		{
		    return GetParameterlessConstructor(type) != null;
		}

		public static ConstructorInfo GetParameterlessConstructor(Type type)
		{
		    foreach (var cInfo in type.GetTypeInfo().DeclaredConstructors)
		        if (!cInfo.IsStatic && cInfo.GetParameters().Length == 0)
		            return cInfo;
		    return null;
		}

		public static string GetShortAssemblyQualifiedName(Type type)
		{
		if (IsPrimitive (type))
		return type.ToString ();
		return type.FullName + "," + type.GetTypeInfo().Assembly.GetName().Name;
		}

		public static PropertyInfo GetProperty(Type type, string propertyName)
		{
        	var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null && type.BaseType != typeof(object))
                return GetProperty(type.BaseType, propertyName);
            return property;
		}

		public static FieldInfo GetField(Type type, string fieldName)
		{
		return type.GetTypeInfo().GetDeclaredField(fieldName);
		}

        public static MethodInfo[] GetMethods(Type type, string methodName)
        {
            return type.GetTypeInfo().GetDeclaredMethods(methodName);
        }

		public static bool IsPrimitive(Type type)
		{
		return (type.GetTypeInfo().IsPrimitive || type == typeof(string) || type == typeof(decimal));
		}

		public static bool AttributeIsDefined(MemberInfo info, Type attributeType)
		{
		var attributes = info.GetCustomAttributes(attributeType, true);
		foreach(var attribute in attributes)
		return true;
		return false;
		}

		public static bool AttributeIsDefined(Type type, Type attributeType)
		{
		var attributes = type.GetTypeInfo().GetCustomAttributes(attributeType, true);
		foreach(var attribute in attributes)
		return true;
		return false;
		}

		public static bool ImplementsInterface(Type type, Type interfaceType)
		{
		return type.GetTypeInfo().ImplementedInterfaces.Contains(interfaceType);
		}

        public static Type BaseType(Type type)
        {
            return type.GetTypeInfo().BaseType;
        }
#else
        public static bool IsAbstract(Type type)
        {
            return type.IsAbstract;
        }

        public static bool IsInterface(Type type)
        {
            return type.IsInterface;
        }

        public static bool IsGenericType(Type type)
        {
            return type.IsGenericType;
        }

        public static bool IsValueType(Type type)
        {
            return type.IsValueType;
        }

        public static bool IsEnum(Type type)
        {
            return type.IsEnum;
        }

        public static bool HasParameterlessConstructor(Type type)
        {
            if (IsValueType(type) || GetParameterlessConstructor(type) != null)
                return true;
            return false;
        }

        public static ConstructorInfo GetParameterlessConstructor(Type type)
        {
            var constructors =
                type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var constructor in constructors)
                if (constructor.GetParameters().Length == 0)
                    return constructor;
            return null;
        }

        public static string GetShortAssemblyQualifiedName(Type type)
        {
            if (IsPrimitive(type))
                return type.ToString();
            return type.FullName + "," + type.Assembly.GetName().Name;
        }

        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null && BaseType(type) != typeof(object))
                return GetProperty(BaseType(type), propertyName);
            return property;
        }

        public static FieldInfo GetField(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null && BaseType(type) != typeof(object))
                return GetField(BaseType(type), fieldName);
            return field;
        }

        public static MethodInfo[] GetMethods(Type type, string methodName)
        {
            return type.GetMethods().Where(t => t.Name == methodName).ToArray();
        }

        public static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
        }

        public static bool AttributeIsDefined(MemberInfo info, Type attributeType)
        {
            return Attribute.IsDefined(info, attributeType, true);
        }

        public static bool AttributeIsDefined(Type type, Type attributeType)
        {
            return type.IsDefined(attributeType, true);
        }

        public static bool ImplementsInterface(Type type, Type interfaceType)
        {
            return type.GetInterface(interfaceType.Name) != null;
        }

        public static Type BaseType(Type type)
        {
            return type.BaseType;
        }

        public static Type GetType(string typeString)
        {
            switch (typeString)
            {
                case "bool":
                    return typeof(bool);
                case "byte":
                    return typeof(byte);
                case "sbyte":
                    return typeof(sbyte);
                case "char":
                    return typeof(char);
                case "decimal":
                    return typeof(decimal);
                case "double":
                    return typeof(double);
                case "float":
                    return typeof(float);
                case "int":
                    return typeof(int);
                case "uint":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "string":
                    return typeof(string);
                case "Vector2":
                    return typeof(Vector2);
                case "Vector3":
                    return typeof(Vector3);
                case "Vector4":
                    return typeof(Vector4);
                case "Color":
                    return typeof(Color);
                case "Transform":
                    return typeof(Transform);
                case "Component":
                    return typeof(Component);
                case "GameObject":
                    return typeof(GameObject);
                case "MeshFilter":
                    return typeof(MeshFilter);
                case "Material":
                    return typeof(Material);
                case "Texture2D":
                    return typeof(Texture2D);
                case "UnityEngine.Object":
                    return typeof(Object);
                case "System.Object":
                    return typeof(object);
                default:
                    return Type.GetType(typeString);
            }
        }

        public static string GetTypeString(Type type)
        {
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(sbyte))
                return "sbyte";
            if (type == typeof(char))
                return "char";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(double))
                return "double";
            if (type == typeof(float))
                return "float";
            if (type == typeof(int))
                return "int";
            if (type == typeof(uint))
                return "uint";
            if (type == typeof(long))
                return "long";
            if (type == typeof(ulong))
                return "ulong";
            if (type == typeof(short))
                return "short";
            if (type == typeof(ushort))
                return "ushort";
            if (type == typeof(string))
                return "string";
            if (type == typeof(Vector2))
                return "Vector2";
            if (type == typeof(Vector3))
                return "Vector3";
            if (type == typeof(Vector4))
                return "Vector4";
            if (type == typeof(Color))
                return "Color";
            if (type == typeof(Transform))
                return "Transform";
            if (type == typeof(Component))
                return "Component";
            if (type == typeof(GameObject))
                return "GameObject";
            if (type == typeof(MeshFilter))
                return "MeshFilter";
            if (type == typeof(Material))
                return "Material";
            if (type == typeof(Texture2D))
                return "Texture2D";
            if (type == typeof(Object))
                return "UnityEngine.Object";
            if (type == typeof(object))
                return "System.Object";
            return GetShortAssemblyQualifiedName(type);
        }
#endif

        /*
         * 	Allows us to use FieldInfo and PropertyInfo interchangably.
         */
        public struct ES3ReflectedMember
        {
            // The FieldInfo or PropertyInfo for this field.
            private readonly FieldInfo fieldInfo;
            private readonly PropertyInfo propertyInfo;
            public bool isProperty;

            public bool IsNull => fieldInfo == null && propertyInfo == null;
            public string Name => isProperty ? propertyInfo.Name : fieldInfo.Name;
            public Type MemberType => isProperty ? propertyInfo.PropertyType : fieldInfo.FieldType;

            public bool IsPublic => isProperty
                ? propertyInfo.GetGetMethod(true).IsPublic && propertyInfo.GetSetMethod(true).IsPublic
                : fieldInfo.IsPublic;

            public bool IsProtected => isProperty ? propertyInfo.GetGetMethod(true).IsFamily : fieldInfo.IsFamily;
            public bool IsStatic => isProperty ? propertyInfo.GetGetMethod(true).IsStatic : fieldInfo.IsStatic;

            public ES3ReflectedMember(object fieldPropertyInfo)
            {
                if (fieldPropertyInfo == null)
                {
                    propertyInfo = null;
                    fieldInfo = null;
                    isProperty = false;
                    return;
                }

                isProperty = IsAssignableFrom(typeof(PropertyInfo), fieldPropertyInfo.GetType());
                if (isProperty)
                {
                    propertyInfo = (PropertyInfo)fieldPropertyInfo;
                    fieldInfo = null;
                }
                else
                {
                    fieldInfo = (FieldInfo)fieldPropertyInfo;
                    propertyInfo = null;
                }
            }

            public void SetValue(object obj, object value)
            {
                if (isProperty)
                    propertyInfo.SetValue(obj, value, null);
                else
                    fieldInfo.SetValue(obj, value);
            }

            public object GetValue(object obj)
            {
                if (isProperty)
                    return propertyInfo.GetValue(obj, null);
                return fieldInfo.GetValue(obj);
            }
        }

        public class ES3ReflectedMethod
        {
            private readonly MethodInfo method;

            public ES3ReflectedMethod(Type type, string methodName, Type[] genericParameters, Type[] parameterTypes)
            {
                var nonGenericMethod = type.GetMethod(methodName, parameterTypes);
                method = nonGenericMethod.MakeGenericMethod(genericParameters);
            }

            public ES3ReflectedMethod(Type type, string methodName, Type[] genericParameters, Type[] parameterTypes,
                BindingFlags bindingAttr)
            {
                var nonGenericMethod = type.GetMethod(methodName, bindingAttr, null, parameterTypes, null);
                method = nonGenericMethod.MakeGenericMethod(genericParameters);
            }

            public object Invoke(object obj, object[] parameters = null)
            {
                return method.Invoke(obj, parameters);
            }
        }
    }
}