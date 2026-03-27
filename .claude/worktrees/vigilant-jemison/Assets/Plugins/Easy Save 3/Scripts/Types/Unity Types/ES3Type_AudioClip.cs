using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("name", "samples", "channels", "frequency", "sampleData")]
    public class ES3Type_AudioClip : ES3UnityObjectType
    {
        public static ES3Type Instance;

        public ES3Type_AudioClip() : base(typeof(AudioClip))
        {
            Instance = this;
        }

        protected override void WriteUnityObject(object obj, ES3Writer writer)
        {
            var param = (AudioClip)obj;
            var samples = new float[param.samples * param.channels];
            param.GetData(samples, 0);
            writer.WriteProperty("name", param.name);
            writer.WriteProperty("samples", param.samples);
            writer.WriteProperty("channels", param.channels);
            writer.WriteProperty("frequency", param.frequency);
            writer.WriteProperty("sampleData", samples);
        }

        protected override void ReadUnityObject<T>(ES3Reader reader, object obj)
        {
            var clip = (AudioClip)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "sampleData":
                        clip.SetData(reader.Read<float[]>(ES3Type_floatArray.Instance), 0);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }

        protected override object ReadUnityObject<T>(ES3Reader reader)
        {
            var name = "";
            var samples = 0;
            var channels = 0;
            var frequency = 0;
            AudioClip clip = null;

            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "name":
                        name = reader.Read<string>(ES3Type_string.Instance);
                        break;
                    case "samples":
                        samples = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "channels":
                        channels = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "frequency":
                        frequency = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "sampleData":
                        clip = AudioClip.Create(name, samples, channels, frequency, false);
                        clip.SetData(reader.Read<float[]>(ES3Type_floatArray.Instance), 0);
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            return clip;
        }
    }

    public class ES3Type_AudioClipArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_AudioClipArray() : base(typeof(AudioClip[]), ES3Type_AudioClip.Instance)
        {
            Instance = this;
        }
    }
}