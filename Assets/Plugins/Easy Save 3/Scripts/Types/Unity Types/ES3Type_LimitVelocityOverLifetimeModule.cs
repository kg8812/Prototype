using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("enabled", "limitX", "limitXMultiplier", "limitY", "limitYMultiplier", "limitZ",
        "limitZMultiplier", "limit", "limitMultiplier", "dampen", "separateAxes", "space")]
    public class ES3Type_LimitVelocityOverLifetimeModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_LimitVelocityOverLifetimeModule() : base(typeof(ParticleSystem.LimitVelocityOverLifetimeModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.LimitVelocityOverLifetimeModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("limitX", instance.limitX, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("limitXMultiplier", instance.limitXMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("limitY", instance.limitY, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("limitYMultiplier", instance.limitYMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("limitZ", instance.limitZ, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("limitZMultiplier", instance.limitZMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("limit", instance.limit, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("limitMultiplier", instance.limitMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("dampen", instance.dampen, ES3Type_float.Instance);
            writer.WriteProperty("separateAxes", instance.separateAxes, ES3Type_bool.Instance);
            writer.WriteProperty("space", instance.space);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.LimitVelocityOverLifetimeModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.LimitVelocityOverLifetimeModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "limitX":
                        instance.limitX = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "limitXMultiplier":
                        instance.limitXMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "limitY":
                        instance.limitY = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "limitYMultiplier":
                        instance.limitYMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "limitZ":
                        instance.limitZ = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "limitZMultiplier":
                        instance.limitZMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "limit":
                        instance.limit = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "limitMultiplier":
                        instance.limitMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "dampen":
                        instance.dampen = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "separateAxes":
                        instance.separateAxes = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "space":
                        instance.space = reader.Read<ParticleSystemSimulationSpace>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}