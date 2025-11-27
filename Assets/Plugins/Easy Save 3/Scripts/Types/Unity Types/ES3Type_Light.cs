using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("type", "color", "intensity", "bounceIntensity", "shadows", "shadowStrength",
        "shadowResolution", "shadowCustomResolution", "shadowBias", "shadowNormalBias", "shadowNearPlane", "range",
        "spotAngle", "cookieSize", "cookie", "flare", "renderMode", "cullingMask", "areaSize", "lightmappingMode",
        "enabled", "hideFlags")]
    public class ES3Type_Light : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_Light() : base(typeof(Light))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (Light)obj;

            writer.WriteProperty("type", instance.type);
            writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
            writer.WriteProperty("intensity", instance.intensity, ES3Type_float.Instance);
            writer.WriteProperty("bounceIntensity", instance.bounceIntensity, ES3Type_float.Instance);
            writer.WriteProperty("shadows", instance.shadows);
            writer.WriteProperty("shadowStrength", instance.shadowStrength, ES3Type_float.Instance);
            writer.WriteProperty("shadowResolution", instance.shadowResolution);
            writer.WriteProperty("shadowCustomResolution", instance.shadowCustomResolution, ES3Type_int.Instance);
            writer.WriteProperty("shadowBias", instance.shadowBias, ES3Type_float.Instance);
            writer.WriteProperty("shadowNormalBias", instance.shadowNormalBias, ES3Type_float.Instance);
            writer.WriteProperty("shadowNearPlane", instance.shadowNearPlane, ES3Type_float.Instance);
            writer.WriteProperty("range", instance.range, ES3Type_float.Instance);
            writer.WriteProperty("spotAngle", instance.spotAngle, ES3Type_float.Instance);
            writer.WriteProperty("cookieSize", instance.cookieSize, ES3Type_float.Instance);
            writer.WriteProperty("cookie", instance.cookie, ES3Type_Texture2D.Instance);
            writer.WriteProperty("flare", instance.flare, ES3Type_Texture2D.Instance);
            writer.WriteProperty("renderMode", instance.renderMode);
            writer.WriteProperty("cullingMask", instance.cullingMask, ES3Type_int.Instance);
            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("hideFlags", instance.hideFlags);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (Light)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "type":
                        instance.type = reader.Read<LightType>();
                        break;
                    case "color":
                        instance.color = reader.Read<Color>(ES3Type_Color.Instance);
                        break;
                    case "intensity":
                        instance.intensity = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "bounceIntensity":
                        instance.bounceIntensity = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "shadows":
                        instance.shadows = reader.Read<LightShadows>();
                        break;
                    case "shadowStrength":
                        instance.shadowStrength = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "shadowResolution":
                        instance.shadowResolution = reader.Read<LightShadowResolution>();
                        break;
                    case "shadowCustomResolution":
                        instance.shadowCustomResolution = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "shadowBias":
                        instance.shadowBias = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "shadowNormalBias":
                        instance.shadowNormalBias = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "shadowNearPlane":
                        instance.shadowNearPlane = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "range":
                        instance.range = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "spotAngle":
                        instance.spotAngle = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "cookieSize":
                        instance.cookieSize = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "cookie":
                        instance.cookie = reader.Read<Texture>();
                        break;
                    case "flare":
                        instance.flare = reader.Read<Flare>();
                        break;
                    case "renderMode":
                        instance.renderMode = reader.Read<LightRenderMode>();
                        break;
                    case "cullingMask":
                        instance.cullingMask = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "hideFlags":
                        instance.hideFlags = reader.Read<HideFlags>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}