using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("enabled", "size", "sizeMultiplier", "x", "xMultiplier", "y", "yMultiplier", "z",
        "zMultiplier", "separateAxes", "range")]
    public class ES3Type_SizeBySpeedModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_SizeBySpeedModule() : base(typeof(ParticleSystem.SizeBySpeedModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.SizeBySpeedModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("size", instance.size, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("sizeMultiplier", instance.sizeMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("x", instance.x, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("xMultiplier", instance.xMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("y", instance.y, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("yMultiplier", instance.yMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("z", instance.z, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("zMultiplier", instance.zMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("separateAxes", instance.separateAxes, ES3Type_bool.Instance);
            writer.WriteProperty("range", instance.range, ES3Type_Vector2.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.SizeBySpeedModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.SizeBySpeedModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "size":
                        instance.size = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "sizeMultiplier":
                        instance.sizeMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "x":
                        instance.x = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "xMultiplier":
                        instance.xMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "y":
                        instance.y = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "yMultiplier":
                        instance.yMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "z":
                        instance.z = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "zMultiplier":
                        instance.zMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "separateAxes":
                        instance.separateAxes = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "range":
                        instance.range = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}