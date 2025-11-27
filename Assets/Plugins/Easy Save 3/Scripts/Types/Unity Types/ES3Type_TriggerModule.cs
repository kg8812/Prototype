using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("enabled", "inside", "outside", "enter", "exit", "radiusScale")]
    public class ES3Type_TriggerModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_TriggerModule() : base(typeof(ParticleSystem.TriggerModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.TriggerModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("inside", instance.inside);
            writer.WriteProperty("outside", instance.outside);
            writer.WriteProperty("enter", instance.enter);
            writer.WriteProperty("exit", instance.exit);
            writer.WriteProperty("radiusScale", instance.radiusScale, ES3Type_float.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.TriggerModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.TriggerModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "inside":
                        instance.inside = reader.Read<ParticleSystemOverlapAction>();
                        break;
                    case "outside":
                        instance.outside = reader.Read<ParticleSystemOverlapAction>();
                        break;
                    case "enter":
                        instance.enter = reader.Read<ParticleSystemOverlapAction>();
                        break;
                    case "exit":
                        instance.exit = reader.Read<ParticleSystemOverlapAction>();
                        break;
                    case "radiusScale":
                        instance.radiusScale = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}