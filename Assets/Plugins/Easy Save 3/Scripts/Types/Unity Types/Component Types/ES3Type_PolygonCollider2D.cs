using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("points", "pathCount", "paths", "density", "isTrigger", "usedByEffector", "offset",
        "sharedMaterial", "enabled")]
    public class ES3Type_PolygonCollider2D : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_PolygonCollider2D() : base(typeof(PolygonCollider2D))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (PolygonCollider2D)obj;

            writer.WriteProperty("points", instance.points, ES3Type_Vector2Array.Instance);
            writer.WriteProperty("pathCount", instance.pathCount, ES3Type_int.Instance);

            for (var i = 0; i < instance.pathCount; i++)
                writer.WriteProperty("path" + i, instance.GetPath(i), ES3Type_Vector2Array.Instance);

            if (instance.attachedRigidbody != null && instance.attachedRigidbody.useAutoMass)
                writer.WriteProperty("density", instance.density, ES3Type_float.Instance);
            writer.WriteProperty("isTrigger", instance.isTrigger, ES3Type_bool.Instance);
            writer.WriteProperty("usedByEffector", instance.usedByEffector, ES3Type_bool.Instance);
            writer.WriteProperty("offset", instance.offset, ES3Type_Vector2.Instance);
            writer.WriteProperty("sharedMaterial", instance.sharedMaterial);
            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (PolygonCollider2D)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "points":
                        instance.points = reader.Read<Vector2[]>(ES3Type_Vector2Array.Instance);
                        break;
                    case "pathCount":
                        var pathCount = reader.Read<int>(ES3Type_int.Instance);
                        for (var i = 0; i < pathCount; i++)
                            instance.SetPath(i, reader.ReadProperty<Vector2[]>(ES3Type_Vector2Array.Instance));
                        break;
                    case "density":
                        instance.density = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "isTrigger":
                        instance.isTrigger = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "usedByEffector":
                        instance.usedByEffector = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "offset":
                        instance.offset = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "sharedMaterial":
                        instance.sharedMaterial = reader.Read<PhysicsMaterial2D>(ES3Type_PhysicsMaterial2D.Instance);
                        break;
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }

    public class ES3Type_PolygonCollider2DArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_PolygonCollider2DArray() : base(typeof(PolygonCollider2D[]), ES3Type_PolygonCollider2D.Instance)
        {
            Instance = this;
        }
    }
}