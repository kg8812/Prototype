using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("time", "hideFlags", "collision", "colorBySpeed", "colorOverLifetime", "emission",
        "externalForces", "forceOverLifetime", "inheritVelocity",
        "lights", "limitVelocityOverLifetime", "main", "noise", "rotatonBySpeed", "rotationOverLifetime", "shape",
        "sizeBySpeed", "sizeOverLifetime",
        "subEmitters", "textureSheetAnimation", "trails", "trigger", "useAutoRandomSeed", "velocityOverLifetime",
        "isPaused", "isPlaying", "isStopped")]
    public class ES3Type_ParticleSystem : ES3ComponentType
    {
        public static ES3Type Instance;

        public ES3Type_ParticleSystem() : base(typeof(ParticleSystem))
        {
            Instance = this;
        }

        protected override void WriteComponent(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem)obj;

            writer.WriteProperty("time", instance.time);
            writer.WriteProperty("hideFlags", instance.hideFlags);
            writer.WriteProperty("collision", instance.collision);
            writer.WriteProperty("colorBySpeed", instance.colorBySpeed);
            writer.WriteProperty("colorOverLifetime", instance.colorOverLifetime);
            writer.WriteProperty("emission", instance.emission);
            writer.WriteProperty("externalForces", instance.externalForces);
            writer.WriteProperty("forceOverLifetime", instance.forceOverLifetime);
            writer.WriteProperty("inheritVelocity", instance.inheritVelocity);
            writer.WriteProperty("lights", instance.lights);
            writer.WriteProperty("limitVelocityOverLifetime", instance.limitVelocityOverLifetime);
            writer.WriteProperty("main", instance.main);
            writer.WriteProperty("noise", instance.noise);
            writer.WriteProperty("rotationBySpeed", instance.rotationBySpeed);
            writer.WriteProperty("rotationOverLifetime", instance.rotationOverLifetime);
            writer.WriteProperty("shape", instance.shape);
            writer.WriteProperty("sizeBySpeed", instance.sizeBySpeed);
            writer.WriteProperty("sizeOverLifetime", instance.sizeOverLifetime);
            writer.WriteProperty("subEmitters", instance.subEmitters);
            writer.WriteProperty("textureSheetAnimation", instance.textureSheetAnimation);
            writer.WriteProperty("trails", instance.trails);
            writer.WriteProperty("trigger", instance.trigger);
            writer.WriteProperty("useAutoRandomSeed", instance.useAutoRandomSeed);
            writer.WriteProperty("velocityOverLifetime", instance.velocityOverLifetime);
            writer.WriteProperty("isPaused", instance.isPaused);
            writer.WriteProperty("isPlaying", instance.isPlaying);
            writer.WriteProperty("isStopped", instance.isStopped);
        }

        protected override void ReadComponent<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem)obj;
            // Stop particle system as some properties require it to not be playing to be set.
            instance.Stop();
            foreach (string propertyName in reader.Properties)
                switch (propertyName)
                {
                    case "time":
                        instance.time = reader.Read<float>();
                        break;
                    case "hideFlags":
                        instance.hideFlags = reader.Read<HideFlags>();
                        break;
                    case "collision":
                        reader.ReadInto<ParticleSystem.CollisionModule>(instance.collision,
                            ES3Type_CollisionModule.Instance);
                        break;
                    case "colorBySpeed":
                        reader.ReadInto<ParticleSystem.ColorBySpeedModule>(instance.colorBySpeed,
                            ES3Type_ColorBySpeedModule.Instance);
                        break;
                    case "colorOverLifetime":
                        reader.ReadInto<ParticleSystem.ColorOverLifetimeModule>(instance.colorOverLifetime,
                            ES3Type_ColorOverLifetimeModule.Instance);
                        break;
                    case "sizeOverLifetime":
                        reader.ReadInto<ParticleSystem.SizeOverLifetimeModule>(instance.sizeOverLifetime,
                            ES3Type_SizeOverLifetimeModule.Instance);
                        break;
                    case "shape":
                        reader.ReadInto<ParticleSystem.ShapeModule>(instance.shape, ES3Type_ShapeModule.Instance);
                        break;
                    case "emission":
                        reader.ReadInto<ParticleSystem.EmissionModule>(instance.emission,
                            ES3Type_EmissionModule.Instance);
                        break;
                    case "externalForces":
                        reader.ReadInto<ParticleSystem.ExternalForcesModule>(instance.externalForces,
                            ES3Type_ExternalForcesModule.Instance);
                        break;
                    case "forceOverLifetime":
                        reader.ReadInto<ParticleSystem.ForceOverLifetimeModule>(instance.forceOverLifetime,
                            ES3Type_ForceOverLifetimeModule.Instance);
                        break;
                    case "inheritVelocity":
                        reader.ReadInto<ParticleSystem.InheritVelocityModule>(instance.inheritVelocity,
                            ES3Type_InheritVelocityModule.Instance);
                        break;
                    case "lights":
                        reader.ReadInto<ParticleSystem.LightsModule>(instance.lights, ES3Type_LightsModule.Instance);
                        break;
                    case "limitVelocityOverLifetime":
                        reader.ReadInto<ParticleSystem.LimitVelocityOverLifetimeModule>(
                            instance.limitVelocityOverLifetime, ES3Type_LimitVelocityOverLifetimeModule.Instance);
                        break;
                    case "main":
                        reader.ReadInto<ParticleSystem.MainModule>(instance.main, ES3Type_MainModule.Instance);
                        break;
                    case "noise":
                        reader.ReadInto<ParticleSystem.NoiseModule>(instance.noise, ES3Type_NoiseModule.Instance);
                        break;
                    case "sizeBySpeed":
                        reader.ReadInto<ParticleSystem.SizeBySpeedModule>(instance.sizeBySpeed,
                            ES3Type_SizeBySpeedModule.Instance);
                        break;
                    case "rotationBySpeed":
                        reader.ReadInto<ParticleSystem.RotationBySpeedModule>(instance.rotationBySpeed,
                            ES3Type_RotationBySpeedModule.Instance);
                        break;
                    case "rotationOverLifetime":
                        reader.ReadInto<ParticleSystem.RotationOverLifetimeModule>(instance.rotationOverLifetime,
                            ES3Type_RotationOverLifetimeModule.Instance);
                        break;
                    case "subEmitters":
                        reader.ReadInto<ParticleSystem.SubEmittersModule>(instance.subEmitters,
                            ES3Type_SubEmittersModule.Instance);
                        break;
                    case "textureSheetAnimation":
                        reader.ReadInto<ParticleSystem.TextureSheetAnimationModule>(instance.textureSheetAnimation,
                            ES3Type_TextureSheetAnimationModule.Instance);
                        break;
                    case "trails":
                        reader.ReadInto<ParticleSystem.TrailModule>(instance.trails, ES3Type_TrailModule.Instance);
                        break;
                    case "trigger":
                        reader.ReadInto<ParticleSystem.TriggerModule>(instance.trigger, ES3Type_TriggerModule.Instance);
                        break;
                    case "useAutoRandomSeed":
                        instance.useAutoRandomSeed = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "velocityOverLifetime":
                        reader.ReadInto<ParticleSystem.VelocityOverLifetimeModule>(instance.velocityOverLifetime,
                            ES3Type_VelocityOverLifetimeModule.Instance);
                        break;
                    case "isPaused":
                        if (reader.Read<bool>(ES3Type_bool.Instance)) instance.Pause();
                        break;
                    case "isPlaying":
                        if (reader.Read<bool>(ES3Type_bool.Instance)) instance.Play();
                        break;
                    case "isStopped":
                        if (reader.Read<bool>(ES3Type_bool.Instance)) instance.Stop();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}