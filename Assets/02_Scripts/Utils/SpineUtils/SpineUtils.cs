using Default;
using Spine.Unity;
using UnityEngine;

namespace Apis
{
    public static class SpineUtils
    {
        public static void AddBoneFollower(SkeletonRenderer skeleton, string boneName, GameObject target)
        {
            if (skeleton == null) return;

            BoneFollower follower = target.GetOrAddComponent<BoneFollower>();

            follower.skeletonRenderer = skeleton;
            follower.boneName = boneName;

            follower.followBoneRotation = false;
            follower.followXYPosition = true;
            follower.followZPosition = false;
            follower.followLocalScale = false;
            follower.followParentWorldScale = false;
            follower.followSkeletonFlip = false;

            follower.Initialize();

            target.transform.rotation = Quaternion.identity;
            target.transform.localRotation = Quaternion.identity;

        }

        public static CustomBoneFollower AddCustomBoneFollower(SkeletonRenderer skeleton, string boneName, GameObject target)
        {
            if (skeleton == null) return null;

            CustomBoneFollower follower = target.GetOrAddComponent<CustomBoneFollower>();

            follower.skeletonRenderer = skeleton;
            follower.boneName = boneName;

            follower.followBoneRotation = false;
            follower.followXYPosition = true;
            follower.followZPosition = false;
            follower.followLocalScale = false;
            follower.followParentWorldScale = false;
            follower.followSkeletonFlip = false;

            follower.Initialize();

            target.transform.rotation = Quaternion.identity;
            target.transform.localRotation = Quaternion.identity;

            return follower;
        }
    }
}