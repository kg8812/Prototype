using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("sharedMesh")]
    public class ES3Type_MeshFilter : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_MeshFilter() : base(typeof(MeshFilter))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (MeshFilter)obj;
            writer.WritePropertyByRef("sharedMesh", instance.sharedMesh);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (MeshFilter)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "sharedMesh":
                        instance.sharedMesh = reader.Read<Mesh>(ES3Type_Mesh.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }

    public class ES3Type_MeshFilterArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_MeshFilterArray() : base(typeof(MeshFilter[]), ES3Type_MeshFilter.Instance)
        {
            Instance = this;
        }
    }
}