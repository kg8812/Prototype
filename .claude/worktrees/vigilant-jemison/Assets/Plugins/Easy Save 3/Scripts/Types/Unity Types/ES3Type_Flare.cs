using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("hideFlags")]
    public class ES3Type_Flare : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_Flare() : base(typeof(Flare))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (Flare)obj;

            writer.WriteProperty("hideFlags", instance.hideFlags);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new Flare();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (Flare)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "hideFlags":
                        instance.hideFlags = reader.Read<HideFlags>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }

    public class ES3Type_FlareArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_FlareArray() : base(typeof(Flare[]), ES3Type_Flare.Instance)
        {
            Instance = this;
        }
    }
}