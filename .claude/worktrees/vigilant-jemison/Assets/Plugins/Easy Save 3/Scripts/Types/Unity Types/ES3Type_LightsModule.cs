using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("enabled", "ratio", "useRandomDistribution", "light", "useParticleColor",
        "sizeAffectsRange", "alphaAffectsIntensity", "range", "rangeMultiplier", "intensity", "intensityMultiplier",
        "maxLights")]
    public class ES3Type_LightsModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_LightsModule() : base(typeof(ParticleSystem.LightsModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.LightsModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("ratio", instance.ratio, ES3Type_float.Instance);
            writer.WriteProperty("useRandomDistribution", instance.useRandomDistribution, ES3Type_bool.Instance);
            writer.WritePropertyByRef("light", instance.light);
            writer.WriteProperty("useParticleColor", instance.useParticleColor, ES3Type_bool.Instance);
            writer.WriteProperty("sizeAffectsRange", instance.sizeAffectsRange, ES3Type_bool.Instance);
            writer.WriteProperty("alphaAffectsIntensity", instance.alphaAffectsIntensity, ES3Type_bool.Instance);
            writer.WriteProperty("range", instance.range, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("rangeMultiplier", instance.rangeMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("intensity", instance.intensity, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("intensityMultiplier", instance.intensityMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("maxLights", instance.maxLights, ES3Type_int.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.LightsModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.LightsModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "ratio":
                        instance.ratio = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "useRandomDistribution":
                        instance.useRandomDistribution = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "light":
                        instance.light = reader.Read<Light>(ES3Type_Light.Instance);
                        break;
                    case "useParticleColor":
                        instance.useParticleColor = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "sizeAffectsRange":
                        instance.sizeAffectsRange = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "alphaAffectsIntensity":
                        instance.alphaAffectsIntensity = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "range":
                        instance.range = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "rangeMultiplier":
                        instance.rangeMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "intensity":
                        instance.intensity = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "intensityMultiplier":
                        instance.intensityMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "maxLights":
                        instance.maxLights = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}