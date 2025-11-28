using UnityEngine;

public class ParticleRotate : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] trainParticles;

    public void OnEnable()
    {
        GameManager.instance.Player.ActorMovement.WhenSlope.AddListener(AddKey);
    }

    public void OnDisable()
    {
        GameManager.instance.Player.ActorMovement.WhenSlope.RemoveListener(AddKey);
        ParticleReset();
    }

    public void AddKey(ActorMovement.DashInfo info)
    {
        var curve = new AnimationCurve();
        var duration = 0.2f - info.duration;
        var SpeedValue = (info.endPos - info.startPos).y * 15f / Mathf.Abs((info.endPos - info.startPos).x);
        Keyframe startkey = new(0f, 0f, 0f, 0f);
        Keyframe addkey = new(duration / 0.2f, SpeedValue, SpeedValue > 0f ? 1f : -1f, 0f);
        Keyframe endkey = new(1, SpeedValue, SpeedValue > 0f ? 1f : -1f, 0f);
        curve.AddKey(startkey);
        curve.AddKey(addkey);
        curve.AddKey(endkey);
        //AnimationUtility.SetKeyLeftTangentMode(curve, 0, AnimationUtility.TangentMode.Constant);
        //AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);
        //AnimationUtility.SetKeyLeftTangentMode(curve, 2, AnimationUtility.TangentMode.Constant);
        for (var i = 0; i < trainParticles.Length; i++)
        {
            var ver = trainParticles[i].velocityOverLifetime;
            ver.y = new ParticleSystem.MinMaxCurve(1f, curve);
        }
    }


    public void ParticleReset()
    {
        var curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(1.0f, 0.0f);
        for (var i = 0; i < trainParticles.Length; i++)
        {
            var ver = trainParticles[i].velocityOverLifetime;
            ver.y = new ParticleSystem.MinMaxCurve(1f, curve);
            ver.orbitalX = new ParticleSystem.MinMaxCurve(0f);
        }
    }
}