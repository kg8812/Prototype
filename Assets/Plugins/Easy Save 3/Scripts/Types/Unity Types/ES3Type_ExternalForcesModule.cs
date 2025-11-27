using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("enabled", "multiplier")]
    public class ES3Type_ExternalForcesModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_ExternalForcesModule() : base(typeof(ParticleSystem.ExternalForcesModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.ExternalForcesModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("multiplier", instance.multiplier, ES3Type_float.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.ExternalForcesModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.ExternalForcesModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "multiplier":
                        instance.multiplier = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}