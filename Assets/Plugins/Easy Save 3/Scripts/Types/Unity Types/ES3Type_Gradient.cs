using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("colorKeys", "alphaKeys", "mode")]
    public class ES3Type_Gradient : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_Gradient() : base(typeof(Gradient))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (Gradient)obj;
            writer.WriteProperty("colorKeys", instance.colorKeys, ES3Type_GradientColorKeyArray.Instance);
            writer.WriteProperty("alphaKeys", instance.alphaKeys, ES3Type_GradientAlphaKeyArray.Instance);
            writer.WriteProperty("mode", instance.mode);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new Gradient();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (Gradient)obj;
            instance.SetKeys(
                reader.ReadProperty<GradientColorKey[]>(ES3Type_GradientColorKeyArray.Instance),
                reader.ReadProperty<GradientAlphaKey[]>(ES3Type_GradientAlphaKeyArray.Instance)
            );

            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "mode":
                        instance.mode = reader.Read<GradientMode>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}