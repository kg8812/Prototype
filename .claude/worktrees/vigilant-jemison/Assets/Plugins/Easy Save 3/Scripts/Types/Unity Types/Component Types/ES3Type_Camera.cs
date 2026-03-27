using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("fieldOfView", "nearClipPlane", "farClipPlane", "renderingPath", "allowHDR",
        "orthographicSize", "orthographic", "opaqueSortMode", "transparencySortMode", "depth", "aspect", "cullingMask",
        "eventMask", "backgroundColor", "rect", "pixelRect", "worldToCameraMatrix", "projectionMatrix",
        "nonJitteredProjectionMatrix", "useJitteredProjectionMatrixForTransparentRendering", "clearFlags",
        "stereoSeparation", "stereoConvergence", "cameraType", "stereoTargetEye", "targetDisplay",
        "useOcclusionCulling", "cullingMatrix", "layerCullSpherical", "depthTextureMode",
        "clearStencilAfterLightingPass", "enabled", "hideFlags")]
    public class ES3Type_Camera : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_Camera() : base(typeof(Camera))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (Camera)obj;

            writer.WriteProperty("fieldOfView", instance.fieldOfView);
            writer.WriteProperty("nearClipPlane", instance.nearClipPlane);
            writer.WriteProperty("farClipPlane", instance.farClipPlane);
            writer.WriteProperty("renderingPath", instance.renderingPath);
#if UNITY_5_6_OR_NEWER
            writer.WriteProperty("allowHDR", instance.allowHDR);
#endif
            writer.WriteProperty("orthographicSize", instance.orthographicSize);
            writer.WriteProperty("orthographic", instance.orthographic);
            writer.WriteProperty("opaqueSortMode", instance.opaqueSortMode);
            writer.WriteProperty("transparencySortMode", instance.transparencySortMode);
            writer.WriteProperty("depth", instance.depth);
            writer.WriteProperty("aspect", instance.aspect);
            writer.WriteProperty("cullingMask", instance.cullingMask);
            writer.WriteProperty("eventMask", instance.eventMask);
            writer.WriteProperty("backgroundColor", instance.backgroundColor);
            writer.WriteProperty("rect", instance.rect);
            writer.WriteProperty("pixelRect", instance.pixelRect);
            writer.WriteProperty("projectionMatrix", instance.projectionMatrix);
            writer.WriteProperty("nonJitteredProjectionMatrix", instance.nonJitteredProjectionMatrix);
            writer.WriteProperty("useJitteredProjectionMatrixForTransparentRendering",
                instance.useJitteredProjectionMatrixForTransparentRendering);
            writer.WriteProperty("clearFlags", instance.clearFlags);
            writer.WriteProperty("stereoSeparation", instance.stereoSeparation);
            writer.WriteProperty("stereoConvergence", instance.stereoConvergence);
            writer.WriteProperty("cameraType", instance.cameraType);
            writer.WriteProperty("stereoTargetEye", instance.stereoTargetEye);
            writer.WriteProperty("targetDisplay", instance.targetDisplay);
            writer.WriteProperty("useOcclusionCulling", instance.useOcclusionCulling);
            writer.WriteProperty("layerCullSpherical", instance.layerCullSpherical);
            writer.WriteProperty("depthTextureMode", instance.depthTextureMode);
            writer.WriteProperty("clearStencilAfterLightingPass", instance.clearStencilAfterLightingPass);
            writer.WriteProperty("enabled", instance.enabled);
            writer.WriteProperty("hideFlags", instance.hideFlags);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (Camera)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "fieldOfView":
                        instance.fieldOfView = reader.Read<float>();
                        break;
                    case "nearClipPlane":
                        instance.nearClipPlane = reader.Read<float>();
                        break;
                    case "farClipPlane":
                        instance.farClipPlane = reader.Read<float>();
                        break;
                    case "renderingPath":
                        instance.renderingPath = reader.Read<RenderingPath>();
                        break;
#if UNITY_5_6_OR_NEWER
                    case "allowHDR":
                        instance.allowHDR = reader.Read<bool>();
                        break;
#endif
                    case "orthographicSize":
                        instance.orthographicSize = reader.Read<float>();
                        break;
                    case "orthographic":
                        instance.orthographic = reader.Read<bool>();
                        break;
                    case "opaqueSortMode":
                        instance.opaqueSortMode = reader.Read<OpaqueSortMode>();
                        break;
                    case "transparencySortMode":
                        instance.transparencySortMode = reader.Read<TransparencySortMode>();
                        break;
                    case "depth":
                        instance.depth = reader.Read<float>();
                        break;
                    case "aspect":
                        instance.aspect = reader.Read<float>();
                        break;
                    case "cullingMask":
                        instance.cullingMask = reader.Read<int>();
                        break;
                    case "eventMask":
                        instance.eventMask = reader.Read<int>();
                        break;
                    case "backgroundColor":
                        instance.backgroundColor = reader.Read<Color>();
                        break;
                    case "rect":
                        instance.rect = reader.Read<Rect>();
                        break;
                    case "pixelRect":
                        instance.pixelRect = reader.Read<Rect>();
                        break;
                    case "projectionMatrix":
                        instance.projectionMatrix = reader.Read<Matrix4x4>();
                        break;
                    case "nonJitteredProjectionMatrix":
                        instance.nonJitteredProjectionMatrix = reader.Read<Matrix4x4>();
                        break;
                    case "useJitteredProjectionMatrixForTransparentRendering":
                        instance.useJitteredProjectionMatrixForTransparentRendering = reader.Read<bool>();
                        break;
                    case "clearFlags":
                        instance.clearFlags = reader.Read<CameraClearFlags>();
                        break;
                    case "stereoSeparation":
                        instance.stereoSeparation = reader.Read<float>();
                        break;
                    case "stereoConvergence":
                        instance.stereoConvergence = reader.Read<float>();
                        break;
                    case "cameraType":
                        instance.cameraType = reader.Read<CameraType>();
                        break;
                    case "stereoTargetEye":
                        instance.stereoTargetEye = reader.Read<StereoTargetEyeMask>();
                        break;
                    case "targetDisplay":
                        instance.targetDisplay = reader.Read<int>();
                        break;
                    case "useOcclusionCulling":
                        instance.useOcclusionCulling = reader.Read<bool>();
                        break;
                    case "layerCullSpherical":
                        instance.layerCullSpherical = reader.Read<bool>();
                        break;
                    case "depthTextureMode":
                        instance.depthTextureMode = reader.Read<DepthTextureMode>();
                        break;
                    case "clearStencilAfterLightingPass":
                        instance.clearStencilAfterLightingPass = reader.Read<bool>();
                        break;
                    case "enabled":
                        instance.enabled = reader.Read<bool>();
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