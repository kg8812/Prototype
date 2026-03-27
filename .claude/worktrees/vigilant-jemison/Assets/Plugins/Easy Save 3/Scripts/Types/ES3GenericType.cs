using System;
using ES3Internal;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    public abstract class ES3GenericType : ES3Type
    {
        public ES3Type[] genericArgumentES3Types;
        public Type[] genericArguments;

        public ES3GenericType(Type type) : base(type)
        {
            genericArguments = ES3Reflection.GetGenericArguments(type);
            genericArgumentES3Types = new ES3Type[genericArguments.Length];

            for (var i = 0; i < genericArguments.Length; i++)
            {
                genericArgumentES3Types[i] = ES3TypeMgr.GetOrCreateES3Type(genericArguments[i], false);
                if (genericArgumentES3Types[i] == null || genericArgumentES3Types[i].isUnsupported)
                    isUnsupported = true;
            }
        }

        /*public override void Write(object obj, ES3Writer writer)
        {
            var hasValue = (bool)hasValueProperty.GetValue(obj);
            writer.WriteProperty("HasValue", hasValue, ES3Type_bool.Instance);

            if (hasValue)
            {
                var value = valueProperty.GetValue(obj);
                writer.WriteProperty("Value", value, argumentES3Type);
            }
        }

        public override object Read<T>(ES3Reader reader)
        {
            var hasValue = reader.ReadProperty<bool>(ES3Type_bool.Instance);

            if (!hasValue)
            {
                // Call parameterless constructor to set it as null.
                var constructor = ES3Reflection.GetConstructor(type, new Type[0]);
                return constructor.Invoke(new object[0]);
            }
            else
            {
                var value = reader.ReadProperty<object>(argumentES3Type);
                var constructor = ES3Reflection.GetConstructor(type, new Type[] { genericArgument });
                return constructor.Invoke(new object[] { value });
            }
        }*/
    }
}