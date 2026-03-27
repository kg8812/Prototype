using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("mode", "gradientMax", "gradientMin", "colorMax", "colorMin", "color", "gradient")]
    public class ES3Type_MinMaxGradient : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_MinMaxGradient() : base(typeof(ParticleSystem.MinMaxGradient))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.MinMaxGradient)obj;

            writer.WriteProperty("mode", instance.mode);
            writer.WriteProperty("gradientMax", instance.gradientMax, ES3Type_Gradient.Instance);
            writer.WriteProperty("gradientMin", instance.gradientMin, ES3Type_Gradient.Instance);
            writer.WriteProperty("colorMax", instance.colorMax, ES3Type_Color.Instance);
            writer.WriteProperty("colorMin", instance.colorMin, ES3Type_Color.Instance);
            writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
            writer.WriteProperty("gradient", instance.gradient, ES3Type_Gradient.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.MinMaxGradient();
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "mode":
                        instance.mode = reader.Read<ParticleSystemGradientMode>();
                        break;
                    case "gradientMax":
                        instance.gradientMax = reader.Read<Gradient>(ES3Type_Gradient.Instance);
                        break;
                    case "gradientMin":
                        instance.gradientMin = reader.Read<Gradient>(ES3Type_Gradient.Instance);
                        break;
                    case "colorMax":
                        instance.colorMax = reader.Read<Color>(ES3Type_Color.Instance);
                        break;
                    case "colorMin":
                        instance.colorMin = reader.Read<Color>(ES3Type_Color.Instance);
                        break;
                    case "color":
                        instance.color = reader.Read<Color>(ES3Type_Color.Instance);
                        break;
                    case "gradient":
                        instance.gradient = reader.Read<Gradient>(ES3Type_Gradient.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            return instance;
        }
    }
}