/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated April 5, 2025. Replaces all prior versions.
 *
 * Copyright (c) 2013-2025, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
    using Animation = Animation;

    public class SkeletonAnimationMulti : MonoBehaviour
    {
        private const int MainTrackIndex = 0;

        public bool initialFlipX, initialFlipY;
        public string initialAnimation;
        public bool initialLoop;

        [Space] public List<SkeletonDataAsset> skeletonDataAssets = new();

        [Header("Settings")] public MeshGenerator.Settings meshGeneratorSettings = MeshGenerator.Settings.Default;

        //Stateful

        #region Lifecycle

        private void Awake()
        {
            Initialize(false);
        }

        #endregion

        private void Clear()
        {
            foreach (var skeletonAnimation in SkeletonAnimations)
                Destroy(skeletonAnimation.gameObject);

            SkeletonAnimations.Clear();
            AnimationNameTable.Clear();
            AnimationSkeletonTable.Clear();
        }

        private void SetActiveSkeleton(int index)
        {
            if (index < 0 || index >= SkeletonAnimations.Count)
                SetActiveSkeleton(null);
            else
                SetActiveSkeleton(SkeletonAnimations[index]);
        }

        private void SetActiveSkeleton(SkeletonAnimation skeletonAnimation)
        {
            foreach (var iter in SkeletonAnimations)
                iter.gameObject.SetActive(iter == skeletonAnimation);

            CurrentSkeletonAnimation = skeletonAnimation;
        }

        #region API

        public Dictionary<Animation, SkeletonAnimation> AnimationSkeletonTable { get; } = new();

        public Dictionary<string, Animation> AnimationNameTable { get; } = new();

        public SkeletonAnimation CurrentSkeletonAnimation { get; private set; }

        public List<SkeletonAnimation> SkeletonAnimations { get; } = new();

        public void Initialize(bool overwrite)
        {
            if (SkeletonAnimations.Count != 0 && !overwrite) return;

            Clear();

            var settings = meshGeneratorSettings;
            var thisTransform = transform;
            foreach (var dataAsset in skeletonDataAssets)
            {
                var newSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(dataAsset);
                newSkeletonAnimation.transform.SetParent(thisTransform, false);

                newSkeletonAnimation.SetMeshSettings(settings);
                newSkeletonAnimation.initialFlipX = initialFlipX;
                newSkeletonAnimation.initialFlipY = initialFlipY;
                var skeleton = newSkeletonAnimation.skeleton;
                skeleton.ScaleX = initialFlipX ? -1 : 1;
                skeleton.ScaleY = initialFlipY ? -1 : 1;

                newSkeletonAnimation.Initialize(false);
                SkeletonAnimations.Add(newSkeletonAnimation);
            }

            // Build cache
            var animationNameTable = this.AnimationNameTable;
            var animationSkeletonTable = this.AnimationSkeletonTable;
            foreach (var skeletonAnimation in SkeletonAnimations)
            foreach (var animationObject in skeletonAnimation.Skeleton.Data.Animations)
            {
                animationNameTable[animationObject.Name] = animationObject;
                animationSkeletonTable[animationObject] = skeletonAnimation;
            }

            SetActiveSkeleton(SkeletonAnimations[0]);
            SetAnimation(initialAnimation, initialLoop);
        }

        public Animation FindAnimation(string animationName)
        {
            Animation animation;
            AnimationNameTable.TryGetValue(animationName, out animation);
            return animation;
        }

        public TrackEntry SetAnimation(string animationName, bool loop)
        {
            return SetAnimation(FindAnimation(animationName), loop);
        }

        public TrackEntry SetAnimation(Animation animation, bool loop)
        {
            if (animation == null) return null;

            SkeletonAnimation skeletonAnimation;
            AnimationSkeletonTable.TryGetValue(animation, out skeletonAnimation);

            if (skeletonAnimation != null)
            {
                SetActiveSkeleton(skeletonAnimation);
                skeletonAnimation.skeleton.SetToSetupPose();
                var trackEntry = skeletonAnimation.state.SetAnimation(MainTrackIndex, animation, loop);
                skeletonAnimation.Update(0);
                return trackEntry;
            }

            return null;
        }

        public void SetEmptyAnimation(float mixDuration)
        {
            CurrentSkeletonAnimation.state.SetEmptyAnimation(MainTrackIndex, mixDuration);
        }

        public void ClearAnimation()
        {
            CurrentSkeletonAnimation.state.ClearTrack(MainTrackIndex);
        }

        public TrackEntry GetCurrent()
        {
            return CurrentSkeletonAnimation.state.GetCurrent(MainTrackIndex);
        }

        #endregion
    }
}