using System;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3Properties]
    public class ES3Type_Type : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_Type() : base(typeof(Type))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var type = (Type)obj;
            writer.WriteProperty("assemblyQualifiedName", type.AssemblyQualifiedName);
        }

        public override object Read<T>(ES3Reader reader)
        {
            return Type.GetType(reader.ReadProperty<string>());
        }
    }
}