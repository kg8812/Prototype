using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("width", "height", "dimension", "graphicsFormat", "useMipMap", "vrUsage", "memorylessMode",
        "format", "stencilFormat", "autoGenerateMips", "volumeDepth", "antiAliasing", "bindTextureMS",
        "enableRandomWrite", "useDynamicScale", "isPowerOfTwo", "depth", "descriptor", "masterTextureLimit",
        "anisotropicFiltering", "wrapMode", "wrapModeU", "wrapModeV", "wrapModeW", "filterMode", "anisoLevel",
        "mipMapBias", "imageContentsHash", "streamingTextureForceLoadAll", "streamingTextureDiscardUnusedMips",
        "allowThreadedTextureCreation", "name")]
    public class ES3Type_RenderTexture : ES3ObjectType
    {
        public static ES3Type Instance;

        public ES3Type_RenderTexture() : base(typeof(RenderTexture))
        {
            Instance = this;
        }


        protected override void WriteObject(object obj, ES3Writer writer)
        {
            var instance = (RenderTexture)obj;

            writer.WriteProperty("descriptor", instance.descriptor);
            writer.WriteProperty("antiAliasing", instance.antiAliasing, ES3Type_int.Instance);
            writer.WriteProperty("isPowerOfTwo", instance.isPowerOfTwo, ES3Type_bool.Instance);
            writer.WriteProperty("anisotropicFiltering", RenderTexture.anisotropicFiltering);
            writer.WriteProperty("wrapMode", instance.wrapMode);
            writer.WriteProperty("wrapModeU", instance.wrapModeU);
            writer.WriteProperty("wrapModeV", instance.wrapModeV);
            writer.WriteProperty("wrapModeW", instance.wrapModeW);
            writer.WriteProperty("filterMode", instance.filterMode);
            writer.WriteProperty("anisoLevel", instance.anisoLevel, ES3Type_int.Instance);
            writer.WriteProperty("mipMapBias", instance.mipMapBias, ES3Type_float.Instance);

#if UNITY_2020_1_OR_NEWER
            writer.WriteProperty("streamingTextureForceLoadAll", RenderTexture.streamingTextureForceLoadAll,
                ES3Type_bool.Instance);
            writer.WriteProperty("streamingTextureDiscardUnusedMips", RenderTexture.streamingTextureDiscardUnusedMips,
                ES3Type_bool.Instance);
            writer.WriteProperty("allowThreadedTextureCreation", RenderTexture.allowThreadedTextureCreation,
                ES3Type_bool.Instance);
#endif
        }

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var instance = (RenderTexture)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "width":
                        instance.width = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "height":
                        instance.height = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "dimension":
                        instance.dimension = reader.Read<TextureDimension>();
                        break;
                    case "useMipMap":
                        instance.useMipMap = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "memorylessMode":
                        instance.memorylessMode = reader.Read<RenderTextureMemoryless>();
                        break;
                    case "format":
                        instance.format = reader.Read<RenderTextureFormat>();
                        break;
                    case "autoGenerateMips":
                        instance.autoGenerateMips = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "volumeDepth":
                        instance.volumeDepth = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "antiAliasing":
                        instance.antiAliasing = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "enableRandomWrite":
                        instance.enableRandomWrite = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "isPowerOfTwo":
                        instance.isPowerOfTwo = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "depth":
                        instance.depth = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "descriptor":
                        instance.descriptor = reader.Read<RenderTextureDescriptor>();
                        break;
                    case "anisotropicFiltering":
                        RenderTexture.anisotropicFiltering = reader.Read<AnisotropicFiltering>();
                        break;
                    case "wrapMode":
                        instance.wrapMode = reader.Read<TextureWrapMode>();
                        break;
                    case "wrapModeU":
                        instance.wrapModeU = reader.Read<TextureWrapMode>();
                        break;
                    case "wrapModeV":
                        instance.wrapModeV = reader.Read<TextureWrapMode>();
                        break;
                    case "wrapModeW":
                        instance.wrapModeW = reader.Read<TextureWrapMode>();
                        break;
                    case "filterMode":
                        instance.filterMode = reader.Read<FilterMode>();
                        break;
                    case "anisoLevel":
                        instance.anisoLevel = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "mipMapBias":
                        instance.mipMapBias = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "name":
                        instance.name = reader.Read<string>(ES3Type_string.Instance);
                        break;

#if UNITY_2020_1_OR_NEWER
                    case "vrUsage":
                        instance.vrUsage = reader.Read<VRTextureUsage>();
                        break;
                    case "graphicsFormat":
                        instance.graphicsFormat = reader.Read<GraphicsFormat>();
                        break;
                    case "stencilFormat":
                        instance.stencilFormat = reader.Read<GraphicsFormat>();
                        break;
                    case "bindTextureMS":
                        instance.bindTextureMS = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "useDynamicScale":
                        instance.useDynamicScale = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "streamingTextureForceLoadAll":
                        RenderTexture.streamingTextureForceLoadAll = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "streamingTextureDiscardUnusedMips":
                        RenderTexture.streamingTextureDiscardUnusedMips = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "allowThreadedTextureCreation":
                        RenderTexture.allowThreadedTextureCreation = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
#endif

                    default:
                        reader.Skip();
                        break;
                }
        }

        protected override object ReadObject<T>(ES3Reader reader)
        {
            var descriptor = reader.ReadProperty<RenderTextureDescriptor>();
            var instance = new RenderTexture(descriptor);
            ReadObject<T>(reader, instance);
            return instance;
        }
    }


    public class ES3Type_RenderTextureArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_RenderTextureArray() : base(typeof(RenderTexture[]), ES3Type_RenderTexture.Instance)
        {
            Instance = this;
        }
    }
}