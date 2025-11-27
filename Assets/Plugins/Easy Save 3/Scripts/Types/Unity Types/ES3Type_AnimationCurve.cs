using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("keys", "preWrapMode", "postWrapMode")]
    public class ES3Type_AnimationCurve : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_AnimationCurve() : base(typeof(AnimationCurve))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (AnimationCurve)obj;

            writer.WriteProperty("keys", instance.keys, ES3Type_KeyframeArray.Instance);
            writer.WriteProperty("preWrapMode", instance.preWrapMode);
            writer.WriteProperty("postWrapMode", instance.postWrapMode);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new AnimationCurve();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (AnimationCurve)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "keys":
                        instance.keys = reader.Read<Keyframe[]>();
                        break;
                    case "preWrapMode":
                        instance.preWrapMode = reader.Read<WrapMode>();
                        break;
                    case "postWrapMode":
                        instance.postWrapMode = reader.Read<WrapMode>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}