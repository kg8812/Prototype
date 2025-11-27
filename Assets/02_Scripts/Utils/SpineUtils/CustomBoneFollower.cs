using Spine.Unity;
using UnityEngine;

public class CustomBoneFollower : BoneFollower
{
    public Vector3 offset;

    public new void LateUpdate()
    {
        base.LateUpdate();
        transform.localPosition += offset;
    }
}