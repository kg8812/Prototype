using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("time", "count", "minCount", "maxCount", "cycleCount", "repeatInterval", "probability")]
    public class ES3Type_Burst : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_Burst() : base(typeof(ParticleSystem.Burst))
        {
            Instance = this;
        }


        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.Burst)obj;

            writer.WriteProperty("time", instance.time, ES3Type_float.Instance);
            writer.WriteProperty("count", instance.count, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("minCount", instance.minCount, ES3Type_short.Instance);
            writer.WriteProperty("maxCount", instance.maxCount, ES3Type_short.Instance);
            writer.WriteProperty("cycleCount", instance.cycleCount, ES3Type_int.Instance);
            writer.WriteProperty("repeatInterval", instance.repeatInterval, ES3Type_float.Instance);
            writer.WriteProperty("probability", instance.probability, ES3Type_float.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.Burst();
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "time":
                        instance.time = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "count":
                        instance.count = reader.Read<ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "minCount":
                        instance.minCount = reader.Read<short>(ES3Type_short.Instance);
                        break;
                    case "maxCount":
                        instance.maxCount = reader.Read<short>(ES3Type_short.Instance);
                        break;
                    case "cycleCount":
                        instance.cycleCount = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "repeatInterval":
                        instance.repeatInterval = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "probability":
                        instance.probability = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            return instance;
        }
    }


    public class ES3Type_BurstArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_BurstArray() : base(typeof(ParticleSystem.Burst[]), ES3Type_Burst.Instance)
        {
            Instance = this;
        }
    }
}