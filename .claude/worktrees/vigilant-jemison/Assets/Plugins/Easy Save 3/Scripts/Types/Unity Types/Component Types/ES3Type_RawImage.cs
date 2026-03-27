#if ES3_UGUI

using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("texture", "uvRect", "onCullStateChanged", "maskable", "color", "raycastTarget",
        "useLegacyMeshGeneration", "material", "useGUILayout", "enabled", "hideFlags")]
    public class ES3Type_RawImage : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_RawImage() : base(typeof(RawImage))
        {
            Instance = this;
        }


        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (RawImage)obj;

            writer.WritePropertyByRef("texture", instance.texture);
            writer.WriteProperty("uvRect", instance.uvRect, ES3Type_Rect.Instance);
            writer.WriteProperty("onCullStateChanged", instance.onCullStateChanged);
            writer.WriteProperty("maskable", instance.maskable, ES3Type_bool.Instance);
            writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
            writer.WriteProperty("raycastTarget", instance.raycastTarget, ES3Type_bool.Instance);
            writer.WritePrivateProperty("useLegacyMeshGeneration", instance);
            // Unity automatically sets the default material if it's set to null.
            // This prevents missing reference warnings.
            if (instance.material.name.Contains("Default"))
                writer.WriteProperty("material", null);
            else
                writer.WriteProperty("material", instance.material);
            writer.WriteProperty("useGUILayout", instance.useGUILayout, ES3Type_bool.Instance);
            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("hideFlags", instance.hideFlags);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (RawImage)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "texture":
                        instance.texture = reader.Read<Texture>(ES3Type_Texture.Instance);
                        break;
                    case "uvRect":
                        instance.uvRect = reader.Read<Rect>(ES3Type_Rect.Instance);
                        break;
                    case "onCullStateChanged":
                        instance.onCullStateChanged = reader.Read<MaskableGraphic.CullStateChangedEvent>();
                        break;
                    case "maskable":
                        instance.maskable = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "color":
                        instance.color = reader.Read<Color>(ES3Type_Color.Instance);
                        break;
                    case "raycastTarget":
                        instance.raycastTarget = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "useLegacyMeshGeneration":
                        reader.SetPrivateProperty("useLegacyMeshGeneration", reader.Read<bool>(), instance);
                        break;
                    case "material":
                        instance.material = reader.Read<Material>(ES3Type_Material.Instance);
                        break;
                    case "useGUILayout":
                        instance.useGUILayout = reader.Read<bool>(ES3Type_bool.Instance);
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


    public class ES3Type_RawImageArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_RawImageArray() : base(typeof(RawImage[]), ES3Type_RawImage.Instance)
        {
            Instance = this;
        }
    }
}

#endif