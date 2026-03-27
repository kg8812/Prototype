using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("properties", "systems", "types")]
    public class ES3Type_SubEmittersModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_SubEmittersModule() : base(typeof(ParticleSystem.SubEmittersModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.SubEmittersModule)obj;

            var seProperties = new ParticleSystemSubEmitterProperties[instance.subEmittersCount];
            var seSystems = new ParticleSystem[instance.subEmittersCount];
            var seTypes = new ParticleSystemSubEmitterType[instance.subEmittersCount];

            for (var i = 0; i < instance.subEmittersCount; i++)
            {
                seProperties[i] = instance.GetSubEmitterProperties(i);
                seSystems[i] = instance.GetSubEmitterSystem(i);
                seTypes[i] = instance.GetSubEmitterType(i);
            }

            writer.WriteProperty("properties", seProperties);
            writer.WriteProperty("systems", seSystems);
            writer.WriteProperty("types", seTypes);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.SubEmittersModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.SubEmittersModule)obj;

            ParticleSystemSubEmitterProperties[] seProperties = null;
            ParticleSystem[] seSystems = null;
            ParticleSystemSubEmitterType[] seTypes = null;

            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "properties":
                        seProperties =
                            reader.Read<ParticleSystemSubEmitterProperties[]>(
                                new ES3ArrayType(typeof(ParticleSystemSubEmitterProperties[])));
                        break;
                    case "systems":
                        seSystems = reader.Read<ParticleSystem[]>();
                        break;
                    case "types":
                        seTypes = reader.Read<ParticleSystemSubEmitterType[]>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            if (seProperties != null)
                for (var i = 0; i < seProperties.Length; i++)
                {
                    instance.RemoveSubEmitter(i);
                    instance.AddSubEmitter(seSystems[i], seTypes[i], seProperties[i]);
                }
        }
    }
}