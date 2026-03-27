using System.Collections;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("_items", "_size", "_version")]
    public class ES3Type_ArrayList : ES3ObjectType
    {
        public static ES3Type Instance;

        public ES3Type_ArrayList() : base(typeof(ArrayList))
        {
            Instance = this;
        }


        protected override void WriteObject(object obj, ES3Writer writer)
        {
            var instance = (ArrayList)obj;

            writer.WritePrivateField("_items", instance);
            writer.WritePrivateField("_size", instance);
            writer.WritePrivateField("_version", instance);
        }

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var instance = (ArrayList)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "_items":
                        instance = (ArrayList)reader.SetPrivateField("_items", reader.Read<object[]>(), instance);
                        break;
                    case "_size":
                        instance = (ArrayList)reader.SetPrivateField("_size", reader.Read<int>(), instance);
                        break;
                    case "_version":
                        instance = (ArrayList)reader.SetPrivateField("_version", reader.Read<int>(), instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }

        protected override object ReadObject<T>(ES3Reader reader)
        {
            var instance = new ArrayList();
            ReadObject<T>(reader, instance);
            return instance;
        }
    }


    public class ES3UserType_ArrayListArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3UserType_ArrayListArray() : base(typeof(ArrayList[]), ES3Type_ArrayList.Instance)
        {
            Instance = this;
        }
    }
}