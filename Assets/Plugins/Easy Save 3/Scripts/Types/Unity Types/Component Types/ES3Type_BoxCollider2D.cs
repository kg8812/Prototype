using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("size", "density", "isTrigger", "usedByEffector", "offset", "sharedMaterial", "enabled")]
    public class ES3Type_BoxCollider2D : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_BoxCollider2D() : base(typeof(BoxCollider2D))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (BoxCollider2D)obj;

            writer.WriteProperty("size", instance.size);
            if (instance.attachedRigidbody != null && instance.attachedRigidbody.useAutoMass)
                writer.WriteProperty("density", instance.density);
            writer.WriteProperty("isTrigger", instance.isTrigger);
            writer.WriteProperty("usedByEffector", instance.usedByEffector);
            writer.WriteProperty("offset", instance.offset);
            writer.WritePropertyByRef("sharedMaterial", instance.sharedMaterial);
            writer.WriteProperty("enabled", instance.enabled);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (BoxCollider2D)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "size":
                        instance.size = reader.Read<Vector2>();
                        break;
                    case "density":
                        instance.density = reader.Read<float>();
                        break;
                    case "isTrigger":
                        instance.isTrigger = reader.Read<bool>();
                        break;
                    case "usedByEffector":
                        instance.usedByEffector = reader.Read<bool>();
                        break;
                    case "offset":
                        instance.offset = reader.Read<Vector2>();
                        break;
                    case "sharedMaterial":
                        instance.sharedMaterial = reader.Read<PhysicsMaterial2D>();
                        break;
                    case "enabled":
                        instance.enabled = reader.Read<bool>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}