using UnityEngine;

public class ParticleRotate : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[] trainParticles;

    public void OnEnable() {
        GameManager.instance.Player.ActorMovement.WhenSlope.AddListener(AddKey);
    }

    public void AddKey(ActorMovement.DashInfo info) {
        
        AnimationCurve curve = new AnimationCurve();
        float duration = 0.2f - info.duration;
        float SpeedValue =(info.endPos - info.startPos).y * 15f / Mathf.Abs((info.endPos - info.startPos).x);
        Keyframe startkey = new(0f, 0f, 0f, 0f);
        Keyframe addkey = new(duration / 0.2f, SpeedValue, SpeedValue > 0f ? 1f : -1f, 0f);
        Keyframe endkey = new(1, SpeedValue, SpeedValue > 0f ? 1f : -1f, 0f);
        curve.AddKey(startkey);
        curve.AddKey(addkey);
        curve.AddKey(endkey);
        //AnimationUtility.SetKeyLeftTangentMode(curve, 0, AnimationUtility.TangentMode.Constant);
        //AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);
        //AnimationUtility.SetKeyLeftTangentMode(curve, 2, AnimationUtility.TangentMode.Constant);
        for (int i = 0; i < trainParticles.Length; i++) {
            var ver = trainParticles[i].velocityOverLifetime;
            ver.y = new ParticleSystem.MinMaxCurve(1f, curve);

        }
    }


    public void ParticleReset() {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(1.0f, 0.0f);
        for (int i = 0; i < trainParticles.Length; i++) {
            var ver = trainParticles[i].velocityOverLifetime;
            ver.y = new ParticleSystem.MinMaxCurve(1f, curve);
            ver.orbitalX = new ParticleSystem.MinMaxCurve(0f);
        }
    }

    public void OnDisable() {
        GameManager.instance.Player.ActorMovement.WhenSlope.RemoveListener(AddKey);
        ParticleReset();
    }

}
