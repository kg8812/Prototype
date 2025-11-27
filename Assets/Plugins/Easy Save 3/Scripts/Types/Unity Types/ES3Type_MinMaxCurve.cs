using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("mode", "curveMultiplier", "curveMax", "curveMin", "constantMax", "constantMin", "constant",
        "curve")]
    public class ES3Type_MinMaxCurve : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_MinMaxCurve() : base(typeof(ParticleSystem.MinMaxCurve))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.MinMaxCurve)obj;

            writer.WriteProperty("mode", instance.mode);
            writer.WriteProperty("curveMultiplier", instance.curveMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("curveMax", instance.curveMax, ES3Type_AnimationCurve.Instance);
            writer.WriteProperty("curveMin", instance.curveMin, ES3Type_AnimationCurve.Instance);
            writer.WriteProperty("constantMax", instance.constantMax, ES3Type_float.Instance);
            writer.WriteProperty("constantMin", instance.constantMin, ES3Type_float.Instance);
            writer.WriteProperty("constant", instance.constant, ES3Type_float.Instance);
            writer.WriteProperty("curve", instance.curve, ES3Type_AnimationCurve.Instance);
        }

        [Preserve]
        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.MinMaxCurve();
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "mode":
                        instance.mode = reader.Read<ParticleSystemCurveMode>();
                        break;
                    case "curveMultiplier":
                        instance.curveMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "curveMax":
                        instance.curveMax = reader.Read<AnimationCurve>(ES3Type_AnimationCurve.Instance);
                        break;
                    case "curveMin":
                        instance.curveMin = reader.Read<AnimationCurve>(ES3Type_AnimationCurve.Instance);
                        break;
                    case "constantMax":
                        instance.constantMax = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "constantMin":
                        instance.constantMin = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "constant":
                        instance.constant = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "curve":
                        instance.curve = reader.Read<AnimationCurve>(ES3Type_AnimationCurve.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            return instance;
        }

        [Preserve]
        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.MinMaxCurve)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "mode":
                        instance.mode = reader.Read<ParticleSystemCurveMode>();
                        break;
                    case "curveMultiplier":
                        instance.curveMultiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "curveMax":
                        instance.curveMax = reader.Read<AnimationCurve>(ES3Type_AnimationCurve.Instance);
                        break;
                    case "curveMin":
                        instance.curveMin = reader.Read<AnimationCurve>(ES3Type_AnimationCurve.Instance);
                        break;
                    case "constantMax":
                        instance.constantMax = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "constantMin":
                        instance.constantMin = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "constant":
                        instance.constant = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "curve":
                        instance.curve = reader.Read<AnimationCurve>(ES3Type_AnimationCurve.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}