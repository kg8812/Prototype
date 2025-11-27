using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("velocity", "angularVelocity", "drag", "angularDrag", "mass", "useGravity",
        "maxDepenetrationVelocity", "isKinematic", "freezeRotation", "constraints", "collisionDetectionMode",
        "centerOfMass", "inertiaTensorRotation", "inertiaTensor", "detectCollisions", "position", "rotation",
        "interpolation", "solverIterations", "sleepThreshold", "maxAngularVelocity", "solverVelocityIterations")]
    public class ES3Type_Rigidbody : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_Rigidbody() : base(typeof(Rigidbody))
        {
            Instance = this;
        }


        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (Rigidbody)obj;

#if UNITY_6000_0_OR_NEWER
            writer.WriteProperty("velocity", instance.linearVelocity, ES3Type_Vector3.Instance);
            writer.WriteProperty("drag", instance.linearDamping, ES3Type_float.Instance);
            writer.WriteProperty("angularDrag", instance.angularDamping, ES3Type_float.Instance);
#else
            writer.WriteProperty("velocity", instance.velocity, ES3Type_Vector3.Instance);
            writer.WriteProperty("drag", instance.drag, ES3Type_float.Instance);
            writer.WriteProperty("angularDrag", instance.angularDrag, ES3Type_float.Instance);
#endif
            writer.WriteProperty("angularVelocity", instance.angularVelocity, ES3Type_Vector3.Instance);
            writer.WriteProperty("mass", instance.mass, ES3Type_float.Instance);
            writer.WriteProperty("useGravity", instance.useGravity, ES3Type_bool.Instance);
            writer.WriteProperty("maxDepenetrationVelocity", instance.maxDepenetrationVelocity, ES3Type_float.Instance);
            writer.WriteProperty("isKinematic", instance.isKinematic, ES3Type_bool.Instance);
            writer.WriteProperty("freezeRotation", instance.freezeRotation, ES3Type_bool.Instance);
            writer.WriteProperty("constraints", instance.constraints);
            writer.WriteProperty("collisionDetectionMode", instance.collisionDetectionMode);
            writer.WriteProperty("centerOfMass", instance.centerOfMass, ES3Type_Vector3.Instance);
            writer.WriteProperty("detectCollisions", instance.detectCollisions, ES3Type_bool.Instance);
            writer.WriteProperty("position", instance.position, ES3Type_Vector3.Instance);
            writer.WriteProperty("rotation", instance.rotation, ES3Type_Quaternion.Instance);
            writer.WriteProperty("interpolation", instance.interpolation);
            writer.WriteProperty("solverIterations", instance.solverIterations, ES3Type_int.Instance);
            writer.WriteProperty("sleepThreshold", instance.sleepThreshold, ES3Type_float.Instance);
            writer.WriteProperty("maxAngularVelocity", instance.maxAngularVelocity, ES3Type_float.Instance);
            writer.WriteProperty("solverVelocityIterations", instance.solverVelocityIterations, ES3Type_int.Instance);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (Rigidbody)obj;
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
#if UNITY_6000_0_OR_NEWER
                    case "velocity":
                        instance.linearVelocity = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "drag":
                        instance.linearDamping = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "angularDrag":
                        instance.angularDamping = reader.Read<float>(ES3Type_float.Instance);
                        break;
#else
                    case "velocity":
                        instance.velocity = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "drag":
                        instance.drag = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;
                    case "angularDrag":
                        instance.angularDrag = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;
#endif
                    case "angularVelocity":
                        instance.angularVelocity = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "mass":
                        instance.mass = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "useGravity":
                        instance.useGravity = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "maxDepenetrationVelocity":
                        instance.maxDepenetrationVelocity = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "isKinematic":
                        instance.isKinematic = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "freezeRotation":
                        instance.freezeRotation = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "constraints":
                        instance.constraints = reader.Read<RigidbodyConstraints>();
                        break;
                    case "collisionDetectionMode":
                        instance.collisionDetectionMode = reader.Read<CollisionDetectionMode>();
                        break;
                    case "centerOfMass":
                        instance.centerOfMass = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "inertiaTensorRotation":
                        instance.inertiaTensorRotation = reader.Read<Quaternion>(ES3Type_Quaternion.Instance);
                        break;
                    case "inertiaTensor":
                        var inertiaTensor = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        // Check that the inertia tensor isn't zero, as it will throw an error if we try to set it.
                        if (inertiaTensor != Vector3.zero)
                            instance.inertiaTensor = inertiaTensor;
                        break;
                    case "detectCollisions":
                        instance.detectCollisions = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "position":
                        instance.position = reader.Read<Vector3>(ES3Type_Vector3.Instance);
                        break;
                    case "rotation":
                        instance.rotation = reader.Read<Quaternion>(ES3Type_Quaternion.Instance);
                        break;
                    case "interpolation":
                        instance.interpolation = reader.Read<RigidbodyInterpolation>();
                        break;
                    case "solverIterations":
                        instance.solverIterations = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    case "sleepThreshold":
                        instance.sleepThreshold = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "maxAngularVelocity":
                        instance.maxAngularVelocity = reader.Read<float>(ES3Type_float.Instance);
                        break;
                    case "solverVelocityIterations":
                        instance.solverVelocityIterations = reader.Read<int>(ES3Type_int.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }


    public class ES3UserType_RigidbodyArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3UserType_RigidbodyArray() : base(typeof(Rigidbody[]), ES3Type_Rigidbody.Instance)
        {
            Instance = this;
        }
    }
}