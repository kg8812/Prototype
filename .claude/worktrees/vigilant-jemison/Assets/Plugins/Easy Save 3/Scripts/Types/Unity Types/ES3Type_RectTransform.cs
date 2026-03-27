using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("anchorMin", "anchorMax", "anchoredPosition", "sizeDelta", "pivot", "offsetMin",
        "offsetMax", "localPosition", "localRotation", "localScale", "parent", "hideFlags")]
    public class ES3Type_RectTransform : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_RectTransform() : base(typeof(RectTransform))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (RectTransform)obj;

            writer.WritePropertyByRef("parent", instance.parent);
            writer.WriteProperty("anchorMin", instance.anchorMin, ES3Type_Vector2.Instance);
            writer.WriteProperty("anchorMax", instance.anchorMax, ES3Type_Vector2.Instance);
            writer.WriteProperty("anchoredPosition", instance.anchoredPosition, ES3Type_Vector2.Instance);
            writer.WriteProperty("sizeDelta", instance.sizeDelta, ES3Type_Vector2.Instance);
            writer.WriteProperty("pivot", instance.pivot, ES3Type_Vector2.Instance);
            writer.WriteProperty("offsetMin", instance.offsetMin, ES3Type_Vector2.Instance);
            writer.WriteProperty("offsetMax", instance.offsetMax, ES3Type_Vector2.Instance);
            writer.WriteProperty("localPosition", instance.localPosition, ES3Type_Vector3.Instance);
            writer.WriteProperty("localRotation", instance.localRotation, ES3Type_Quaternion.Instance);
            writer.WriteProperty("localScale", instance.localScale, ES3Type_Vector3.Instance);
            writer.WriteProperty("hideFlags", instance.hideFlags);
            writer.WriteProperty("siblingIndex", instance.GetSiblingIndex());
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            if (obj.GetType() == typeof(Transform))
                obj = ((Transform)obj).gameObject.AddComponent<RectTransform>();

            var instance = (RectTransform)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "anchorMin":
                        instance.anchorMin = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "anchorMax":
                        instance.anchorMax = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "anchoredPosition":
                        instance.anchoredPosition = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "sizeDelta":
                        instance.sizeDelta = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "pivot":
                        instance.pivot = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "offsetMin":
                        instance.offsetMin = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "offsetMax":
                        instance.offsetMax = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "localPosition":
                        instance.localPosition = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "localRotation":
                        instance.localRotation = reader.Read<Quaternion>(ES3Type_Quaternion.Instance);
                        break;
                    case "localScale":
                        instance.localScale = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "parent":
                        instance.SetParent(reader.Read<Transform>(ES3Type_Transform.Instance));
                        break;
                    case "hierarchyCapacity":
                        instance.hierarchyCapacity = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "hideFlags":
                        instance.hideFlags = reader.Read<HideFlags>();
                        break;
                    case "siblingIndex":
                        instance.SetSiblingIndex(reader.Read<int>());
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}