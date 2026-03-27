using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("additionalVertexStreams", "enabled", "shadowCastingMode", "receiveShadows",
        "sharedMaterials", "lightmapIndex", "realtimeLightmapIndex", "lightmapScaleOffset",
        "motionVectorGenerationMode", "realtimeLightmapScaleOffset", "lightProbeUsage", "lightProbeProxyVolumeOverride",
        "probeAnchor", "reflectionProbeUsage", "sortingLayerName", "sortingLayerID", "sortingOrder")]
    public class ES3Type_MeshRenderer : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_MeshRenderer() : base(typeof(MeshRenderer))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (MeshRenderer)obj;

            writer.WriteProperty("additionalVertexStreams", instance.additionalVertexStreams, ES3Type_Mesh.Instance);
            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("shadowCastingMode", instance.shadowCastingMode);
            writer.WriteProperty("receiveShadows", instance.receiveShadows, ES3Type_bool.Instance);
            writer.WriteProperty("sharedMaterials", instance.sharedMaterials, ES3Type_MaterialArray.Instance);
            writer.WriteProperty("lightmapIndex", instance.lightmapIndex, ES3Type_int.Instance);
            writer.WriteProperty("realtimeLightmapIndex", instance.realtimeLightmapIndex, ES3Type_int.Instance);
            writer.WriteProperty("lightmapScaleOffset", instance.lightmapScaleOffset, ES3Type_Vector4.Instance);
            writer.WriteProperty("motionVectorGenerationMode", instance.motionVectorGenerationMode);
            writer.WriteProperty("realtimeLightmapScaleOffset", instance.realtimeLightmapScaleOffset,
                ES3Type_Vector4.Instance);
            writer.WriteProperty("lightProbeUsage", instance.lightProbeUsage);
            writer.WriteProperty("lightProbeProxyVolumeOverride", instance.lightProbeProxyVolumeOverride);
            writer.WriteProperty("probeAnchor", instance.probeAnchor, ES3Type_Transform.Instance);
            writer.WriteProperty("reflectionProbeUsage", instance.reflectionProbeUsage);
            writer.WriteProperty("sortingLayerName", instance.sortingLayerName, ES3Type_string.Instance);
            writer.WriteProperty("sortingLayerID", instance.sortingLayerID, ES3Type_int.Instance);
            writer.WriteProperty("sortingOrder", instance.sortingOrder, ES3Type_int.Instance);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (MeshRenderer)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "additionalVertexStreams":
                        instance.additionalVertexStreams = reader.Read<Mesh>(ES3Type_Mesh.Instance);
                        break;
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "shadowCastingMode":
                        instance.shadowCastingMode = reader.Read<ShadowCastingMode>();
                        break;
                    case "receiveShadows":
                        instance.receiveShadows = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "sharedMaterials":
                        instance.sharedMaterials = reader.Read<Material[]>();
                        break;
                    case "lightmapIndex":
                        instance.lightmapIndex = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "realtimeLightmapIndex":
                        instance.realtimeLightmapIndex = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "lightmapScaleOffset":
                        instance.lightmapScaleOffset = reader.Read<Vector4>(ES3Type_Vector4.Instance);
                        break;
                    case "motionVectorGenerationMode":
                        instance.motionVectorGenerationMode = reader.Read<MotionVectorGenerationMode>();
                        break;
                    case "realtimeLightmapScaleOffset":
                        instance.realtimeLightmapScaleOffset = reader.Read<Vector4>(ES3Type_Vector4.Instance);
                        break;
                    case "lightProbeUsage":
                        instance.lightProbeUsage = reader.Read<LightProbeUsage>();
                        break;
                    case "lightProbeProxyVolumeOverride":
                        instance.lightProbeProxyVolumeOverride = reader.Read<GameObject>(ES3Type_GameObject.Instance);
                        break;
                    case "probeAnchor":
                        instance.probeAnchor = reader.Read<Transform>(ES3Type_Transform.Instance);
                        break;
                    case "reflectionProbeUsage":
                        instance.reflectionProbeUsage = reader.Read<ReflectionProbeUsage>();
                        break;
                    case "sortingLayerName":
                        instance.sortingLayerName = reader.Read<string>(ES3Type_string.Instance);
                        break;
                    case "sortingLayerID":
                        instance.sortingLayerID = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "sortingOrder":
                        instance.sortingOrder = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }

    public class ES3Type_MeshRendererArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_MeshRendererArray() : base(typeof(MeshRenderer[]), ES3Type_MeshRenderer.Instance)
        {
            Instance = this;
        }
    }
}