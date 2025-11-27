using System;
using ES3Internal;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    public abstract class ES3ObjectType : ES3Type
    {
        public ES3ObjectType(Type type) : base(type)
        {
        }

        protected abstract void WriteObject(object obj, ES3Writer writer);
        protected abstract object ReadObject<T>(ES3Reader reader);

        protected virtual void ReadObject<T>(ES3Reader reader, object obj)
        {
            throw new NotSupportedException("ReadInto is not supported for type " + type);
        }

        public override void Write(object obj, ES3Writer writer)
        {
            if (!WriteUsingDerivedType(obj, writer))
            {
                var baseType = ES3Reflection.BaseType(obj.GetType());
                if (baseType != typeof(object))
                {
                    var es3Type = ES3TypeMgr.GetOrCreateES3Type(baseType, false);
                    // If it's a Dictionary or Collection, we need to write it as a field with a property name.
                    if (es3Type != null && (es3Type.isDictionary || es3Type.isCollection))
                        writer.WriteProperty("_Values", obj, es3Type);
                }

                WriteObject(obj, writer);
            }
        }

        public override object Read<T>(ES3Reader reader)
        {
            string propertyName;
            while (true)
            {
                propertyName = ReadPropertyName(reader);

                if (propertyName == typeFieldName)
                    return ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).Read<T>(reader);
                reader.overridePropertiesName = propertyName;

                return ReadObject<T>(reader);
            }
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            string propertyName;
            while (true)
            {
                propertyName = ReadPropertyName(reader);

                if (propertyName == typeFieldName)
                {
                    ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).ReadInto<T>(reader, obj);
                    return;
                }
                // This is important we return if the enumerator returns null, otherwise we will encounter an endless cycle.

                if (propertyName == null)
                    return;
                reader.overridePropertiesName = propertyName;
                ReadObject<T>(reader, obj);
            }
        }
    }
}