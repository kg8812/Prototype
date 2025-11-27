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

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Spine.Unity
{
    [RequireComponent(typeof(Animator))]
    [HelpURL("http://esotericsoftware.com/spine-unity#SkeletonMecanim-Component")]
    public class SkeletonMecanim : SkeletonRenderer, ISkeletonAnimation
    {
        [SerializeField] protected MecanimTranslator translator;
        private bool wasUpdatedAfterInit = true;
        public MecanimTranslator Translator => translator;

        public virtual void Update()
        {
            if (!valid || updateTiming != UpdateTiming.InUpdate) return;
            UpdateAnimation(Time.deltaTime);
        }

        /// <summary>Manual animation update. Required when <c>updateTiming</c> is set to <c>ManualUpdate</c>.</summary>
        /// <param name="deltaTime">Ignored parameter.</param>
        public virtual void Update(float deltaTime)
        {
            if (!valid) return;
            UpdateAnimation(deltaTime);
        }

        public virtual void FixedUpdate()
        {
            if (!valid || updateTiming != UpdateTiming.InFixedUpdate) return;
            UpdateAnimation(Time.deltaTime);
        }

        public override void LateUpdate()
        {
            if (updateTiming == UpdateTiming.InLateUpdate && valid && translator != null && translator.Animator != null)
                UpdateAnimation(Time.deltaTime);
            // instantiation can happen from Update() after this component, leading to a missing Update() call.
            if (!wasUpdatedAfterInit) Update();
            base.LateUpdate();
        }

        public override void OnBecameVisible()
        {
            var previousUpdateMode = updateMode;
            updateMode = UpdateMode.FullUpdate;

            // OnBecameVisible is called after LateUpdate()
            if (previousUpdateMode != UpdateMode.FullUpdate &&
                previousUpdateMode != UpdateMode.EverythingExceptMesh)
                Update();
            if (previousUpdateMode != UpdateMode.FullUpdate)
                LateUpdate();
        }

        public override void Initialize(bool overwrite, bool quiet = false)
        {
            if (valid && !overwrite)
                return;
#if UNITY_EDITOR
            if (BuildUtilities.IsInSkeletonAssetBuildPreProcessing)
                return;
#endif
            base.Initialize(overwrite, quiet);

            if (!valid)
                return;

            if (translator == null) translator = new MecanimTranslator();
            translator.Initialize(GetComponent<Animator>(), skeletonDataAsset);
            wasUpdatedAfterInit = false;

            if (_OnAnimationRebuild != null)
                _OnAnimationRebuild(this);
        }

        protected void UpdateAnimation(float deltaTime)
        {
            wasUpdatedAfterInit = true;

            // animation status is kept by Mecanim Animator component
            if (updateMode <= UpdateMode.OnlyAnimationStatus)
                return;

            skeleton.Update(deltaTime);

            ApplyTransformMovementToPhysics();

            ApplyAnimation();
        }

        public virtual void ApplyAnimation()
        {
            if (_BeforeApply != null)
                _BeforeApply(this);

#if UNITY_EDITOR
            var translatorAnimator = translator.Animator;
            if (translatorAnimator != null && !translatorAnimator.isInitialized)
                translatorAnimator.Rebind();

            if (Application.isPlaying)
            {
                translator.Apply(skeleton);
            }
            else
            {
                if (translatorAnimator != null && translatorAnimator.isInitialized &&
                    translatorAnimator.isActiveAndEnabled && translatorAnimator.runtimeAnimatorController != null)
                {
                    // Note: Rebind is required to prevent warning "Animator is not playing an AnimatorController" with prefabs
                    translatorAnimator.Rebind();
                    translator.Apply(skeleton);
                }
            }
#else
			translator.Apply(skeleton);
#endif
            AfterAnimationApplied();
        }

        public virtual void AfterAnimationApplied()
        {
            if (_UpdateLocal != null)
                _UpdateLocal(this);

            if (_UpdateWorld == null)
            {
                UpdateWorldTransform(Skeleton.Physics.Update);
            }
            else
            {
                UpdateWorldTransform(Skeleton.Physics.Pose);
                _UpdateWorld(this);
                UpdateWorldTransform(Skeleton.Physics.Update);
            }

            if (_UpdateComplete != null)
                _UpdateComplete(this);
        }

        [Serializable]
        public class MecanimTranslator
        {
            public delegate void OnClipAppliedDelegate(Animation clip, int layerIndex, float weight,
                float time, float lastTime, bool playsBackward);

            public enum MixMode
            {
                AlwaysMix,
                MixNext,
                Hard,
                Match
            }

            private const float WeightEpsilon = 0.0001f;

            private readonly Dictionary<int, Animation> animationTable = new(IntEqualityComparer.Instance);

            private readonly Dictionary<AnimationClip, int> clipNameHashCodeTable =
                new(AnimationClipEqualityComparer.Instance);

            private readonly List<Animation> previousAnimations = new();

            protected ClipInfos[] layerClipInfos = new ClipInfos[0];
            public Animator Animator { get; private set; }

            public int MecanimLayerCount
            {
                get
                {
                    if (!Animator)
                        return 0;
                    return Animator.layerCount;
                }
            }

            public string[] MecanimLayerNames
            {
                get
                {
                    if (!Animator)
                        return new string[0];
                    var layerNames = new string[Animator.layerCount];
                    for (var i = 0; i < Animator.layerCount; ++i) layerNames[i] = Animator.GetLayerName(i);
                    return layerNames;
                }
            }

            protected event OnClipAppliedDelegate _OnClipApplied;

            public event OnClipAppliedDelegate OnClipApplied
            {
                add => _OnClipApplied += value;
                remove => _OnClipApplied -= value;
            }

            public void Initialize(Animator animator, SkeletonDataAsset skeletonDataAsset)
            {
                this.Animator = animator;

                previousAnimations.Clear();

                animationTable.Clear();
                var data = skeletonDataAsset.GetSkeletonData(true);
                foreach (var a in data.Animations)
                    animationTable.Add(a.Name.GetHashCode(), a);

                clipNameHashCodeTable.Clear();
                ClearClipInfosForLayers();
            }

            private bool ApplyAnimation(Skeleton skeleton, AnimatorClipInfo info, AnimatorStateInfo stateInfo,
                int layerIndex, float layerWeight, MixBlend layerBlendMode,
                bool useCustomClipWeight = false, float customClipWeight = 1.0f)
            {
                var weight = info.weight * layerWeight;
                if (weight < WeightEpsilon)
                    return false;

                var clip = GetAnimation(info.clip);
                if (clip == null)
                    return false;
                var time = AnimationTime(stateInfo.normalizedTime, info.clip.length,
                    info.clip.isLooping, stateInfo.speed < 0);
                weight = useCustomClipWeight ? layerWeight * customClipWeight : weight;
                clip.Apply(skeleton, 0, time, info.clip.isLooping, null,
                    weight, layerBlendMode, MixDirection.In);
                if (_OnClipApplied != null)
                    OnClipAppliedCallback(clip, stateInfo, layerIndex, time, info.clip.isLooping, weight);
                return true;
            }

            private bool ApplyInterruptionAnimation(Skeleton skeleton,
                bool interpolateWeightTo1, AnimatorClipInfo info, AnimatorStateInfo stateInfo,
                int layerIndex, float layerWeight, MixBlend layerBlendMode, float interruptingClipTimeAddition,
                bool useCustomClipWeight = false, float customClipWeight = 1.0f)
            {
                var clipWeight = interpolateWeightTo1 ? (info.weight + 1.0f) * 0.5f : info.weight;
                var weight = clipWeight * layerWeight;
                if (weight < WeightEpsilon)
                    return false;

                var clip = GetAnimation(info.clip);
                if (clip == null)
                    return false;

                var time = AnimationTime(stateInfo.normalizedTime + interruptingClipTimeAddition,
                    info.clip.length, info.clip.isLooping, stateInfo.speed < 0);
                weight = useCustomClipWeight ? layerWeight * customClipWeight : weight;
                clip.Apply(skeleton, 0, time, info.clip.isLooping, null,
                    weight, layerBlendMode, MixDirection.In);
                if (_OnClipApplied != null)
                    OnClipAppliedCallback(clip, stateInfo, layerIndex, time, info.clip.isLooping, weight);
                return true;
            }

            private void OnClipAppliedCallback(Animation clip, AnimatorStateInfo stateInfo,
                int layerIndex, float time, bool isLooping, float weight)
            {
                var speedFactor = stateInfo.speedMultiplier * stateInfo.speed;
                var lastTime = time - Time.deltaTime * speedFactor;
                var clipDuration = clip.Duration;
                if (isLooping && clipDuration != 0)
                {
                    time %= clipDuration;
                    lastTime %= clipDuration;
                }

                _OnClipApplied(clip, layerIndex, weight, time, lastTime, speedFactor < 0);
            }

            public void Apply(Skeleton skeleton)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) GetLayerBlendModes();
#endif

                if (layerMixModes.Length < Animator.layerCount)
                {
                    var oldSize = layerMixModes.Length;
                    Array.Resize(ref layerMixModes, Animator.layerCount);
                    for (var layer = oldSize; layer < Animator.layerCount; ++layer)
                    {
                        var isAdditiveLayer = false;
                        if (layer < layerBlendModes.Length)
                            isAdditiveLayer = layerBlendModes[layer] == MixBlend.Add;
                        layerMixModes[layer] = isAdditiveLayer ? MixMode.AlwaysMix : MixMode.MixNext;
                    }
                }

                InitClipInfosForLayers();
                for (int layer = 0, n = Animator.layerCount; layer < n; layer++) GetStateUpdatesFromAnimator(layer);

                // Clear Previous
                if (autoReset)
                {
                    var previousAnimations = this.previousAnimations;
                    for (int i = 0, n = previousAnimations.Count; i < n; i++)
                        previousAnimations[i]
                            .Apply(skeleton, 0, 0, false, null, 0, MixBlend.Setup,
                                MixDirection.Out); // SetKeyedItemsToSetupPose

                    previousAnimations.Clear();
                    for (int layer = 0, n = Animator.layerCount; layer < n; layer++)
                    {
                        var layerWeight =
                            layer == 0
                                ? 1
                                : Animator.GetLayerWeight(
                                    layer); // Animator.GetLayerWeight always returns 0 on the first layer. Should be interpreted as 1.
                        if (layerWeight <= 0) continue;

                        var nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);

                        var hasNext = nextStateInfo.fullPathHash != 0;

                        int clipInfoCount, nextClipInfoCount, interruptingClipInfoCount;
                        IList<AnimatorClipInfo> clipInfo, nextClipInfo, interruptingClipInfo;
                        bool isInterruptionActive, shallInterpolateWeightTo1;
                        GetAnimatorClipInfos(layer, out isInterruptionActive, out clipInfoCount, out nextClipInfoCount,
                            out interruptingClipInfoCount,
                            out clipInfo, out nextClipInfo, out interruptingClipInfo, out shallInterpolateWeightTo1);

                        for (var c = 0; c < clipInfoCount; c++)
                        {
                            var info = clipInfo[c];
                            var weight = info.weight * layerWeight;
                            if (weight < WeightEpsilon) continue;
                            var clip = GetAnimation(info.clip);
                            if (clip != null)
                                previousAnimations.Add(clip);
                        }

                        if (hasNext)
                            for (var c = 0; c < nextClipInfoCount; c++)
                            {
                                var info = nextClipInfo[c];
                                var weight = info.weight * layerWeight;
                                if (weight < WeightEpsilon) continue;
                                var clip = GetAnimation(info.clip);
                                if (clip != null)
                                    previousAnimations.Add(clip);
                            }

                        if (isInterruptionActive)
                            for (var c = 0; c < interruptingClipInfoCount; c++)
                            {
                                var info = interruptingClipInfo[c];
                                var clipWeight = shallInterpolateWeightTo1 ? (info.weight + 1.0f) * 0.5f : info.weight;
                                var weight = clipWeight * layerWeight;
                                if (weight < WeightEpsilon) continue;
                                var clip = GetAnimation(info.clip);
                                if (clip != null)
                                    previousAnimations.Add(clip);
                            }
                    }
                }

                // Apply
                for (int layer = 0, n = Animator.layerCount; layer < n; layer++)
                {
                    var layerWeight =
                        layer == 0
                            ? 1
                            : Animator.GetLayerWeight(
                                layer); // Animator.GetLayerWeight always returns 0 on the first layer. Should be interpreted as 1.

                    bool isInterruptionActive;
                    AnimatorStateInfo stateInfo;
                    AnimatorStateInfo nextStateInfo;
                    AnimatorStateInfo interruptingStateInfo;
                    float interruptingClipTimeAddition;
                    GetAnimatorStateInfos(layer, out isInterruptionActive, out stateInfo, out nextStateInfo,
                        out interruptingStateInfo, out interruptingClipTimeAddition);

                    var hasNext = nextStateInfo.fullPathHash != 0;

                    int clipInfoCount, nextClipInfoCount, interruptingClipInfoCount;
                    IList<AnimatorClipInfo> clipInfo, nextClipInfo, interruptingClipInfo;
                    bool interpolateWeightTo1;
                    GetAnimatorClipInfos(layer, out isInterruptionActive, out clipInfoCount, out nextClipInfoCount,
                        out interruptingClipInfoCount,
                        out clipInfo, out nextClipInfo, out interruptingClipInfo, out interpolateWeightTo1);

                    var layerBlendMode = layer < layerBlendModes.Length ? layerBlendModes[layer] : MixBlend.Replace;
                    var mode = GetMixMode(layer, layerBlendMode);
                    if (mode == MixMode.AlwaysMix)
                    {
                        // Always use Mix instead of Applying the first non-zero weighted clip.
                        for (var c = 0; c < clipInfoCount; c++)
                            ApplyAnimation(skeleton, clipInfo[c], stateInfo, layer, layerWeight, layerBlendMode);
                        if (hasNext)
                            for (var c = 0; c < nextClipInfoCount; c++)
                                ApplyAnimation(skeleton, nextClipInfo[c], nextStateInfo, layer, layerWeight,
                                    layerBlendMode);

                        if (isInterruptionActive)
                            for (var c = 0; c < interruptingClipInfoCount; c++)
                                ApplyInterruptionAnimation(skeleton, interpolateWeightTo1,
                                    interruptingClipInfo[c], interruptingStateInfo,
                                    layer, layerWeight, layerBlendMode, interruptingClipTimeAddition);
                    }
                    else if (mode == MixMode.Match)
                    {
                        // Calculate matching Spine lerp(lerp(A, B, w2), C, w3) weights
                        // from Unity's absolute weights A*W1 + B*W2 + C*W3.
                        MatchWeights(layerClipInfos[layer], hasNext, isInterruptionActive, clipInfoCount,
                            nextClipInfoCount, interruptingClipInfoCount,
                            clipInfo, nextClipInfo, interruptingClipInfo);

                        var customWeights = layerClipInfos[layer].clipResolvedWeights;
                        for (var c = 0; c < clipInfoCount; c++)
                            ApplyAnimation(skeleton, clipInfo[c], stateInfo, layer, layerWeight, layerBlendMode,
                                true, customWeights[c]);
                        if (hasNext)
                        {
                            customWeights = layerClipInfos[layer].nextClipResolvedWeights;
                            for (var c = 0; c < nextClipInfoCount; c++)
                                ApplyAnimation(skeleton, nextClipInfo[c], nextStateInfo, layer, layerWeight,
                                    layerBlendMode,
                                    true, customWeights[c]);
                        }

                        if (isInterruptionActive)
                        {
                            customWeights = layerClipInfos[layer].interruptingClipResolvedWeights;
                            for (var c = 0; c < interruptingClipInfoCount; c++)
                                ApplyInterruptionAnimation(skeleton, interpolateWeightTo1,
                                    interruptingClipInfo[c], interruptingStateInfo,
                                    layer, layerWeight, layerBlendMode, interruptingClipTimeAddition,
                                    true, customWeights[c]);
                        }
                    }
                    else
                    {
                        // case MixNext || Hard
                        // Apply first non-zero weighted clip
                        var c = 0;
                        for (; c < clipInfoCount; c++)
                        {
                            if (!ApplyAnimation(skeleton, clipInfo[c], stateInfo, layer, layerWeight, layerBlendMode,
                                    true))
                                continue;
                            ++c;
                            break;
                        }

                        // Mix the rest
                        for (; c < clipInfoCount; c++)
                            ApplyAnimation(skeleton, clipInfo[c], stateInfo, layer, layerWeight, layerBlendMode);

                        c = 0;
                        if (hasNext)
                        {
                            // Apply next clip directly instead of mixing (ie: no crossfade, ignores mecanim transition weights)
                            if (mode == MixMode.Hard)
                                for (; c < nextClipInfoCount; c++)
                                {
                                    if (!ApplyAnimation(skeleton, nextClipInfo[c], nextStateInfo, layer, layerWeight,
                                            layerBlendMode,
                                            true))
                                        continue;
                                    ++c;
                                    break;
                                }

                            // Mix the rest
                            for (; c < nextClipInfoCount; c++)
                                if (!ApplyAnimation(skeleton, nextClipInfo[c], nextStateInfo, layer, layerWeight,
                                        layerBlendMode))
                                    continue;
                        }

                        c = 0;
                        if (isInterruptionActive)
                        {
                            // Apply next clip directly instead of mixing (ie: no crossfade, ignores mecanim transition weights)
                            if (mode == MixMode.Hard)
                                for (; c < interruptingClipInfoCount; c++)
                                    if (ApplyInterruptionAnimation(skeleton, interpolateWeightTo1,
                                            interruptingClipInfo[c], interruptingStateInfo,
                                            layer, layerWeight, layerBlendMode, interruptingClipTimeAddition, true))
                                    {
                                        ++c;
                                        break;
                                    }

                            // Mix the rest
                            for (; c < interruptingClipInfoCount; c++)
                                ApplyInterruptionAnimation(skeleton, interpolateWeightTo1,
                                    interruptingClipInfo[c], interruptingStateInfo,
                                    layer, layerWeight, layerBlendMode, interruptingClipTimeAddition);
                        }
                    }
                }
            }

            /// <summary>
            ///     Resolve matching weights from Unity's absolute weights A*w1 + B*w2 + C*w3 to
            ///     Spine's lerp(lerp(A, B, x), C, y) weights, in reverse order of clips.
            /// </summary>
            protected void MatchWeights(ClipInfos clipInfos, bool hasNext, bool isInterruptionActive,
                int clipInfoCount, int nextClipInfoCount, int interruptingClipInfoCount,
                IList<AnimatorClipInfo> clipInfo, IList<AnimatorClipInfo> nextClipInfo,
                IList<AnimatorClipInfo> interruptingClipInfo)
            {
                if (clipInfos.clipResolvedWeights.Length < clipInfoCount)
                    Array.Resize(ref clipInfos.clipResolvedWeights, clipInfoCount);
                if (hasNext && clipInfos.nextClipResolvedWeights.Length < nextClipInfoCount)
                    Array.Resize(ref clipInfos.nextClipResolvedWeights, nextClipInfoCount);
                if (isInterruptionActive &&
                    clipInfos.interruptingClipResolvedWeights.Length < interruptingClipInfoCount)
                    Array.Resize(ref clipInfos.interruptingClipResolvedWeights, interruptingClipInfoCount);

                var inverseWeight = 1.0f;
                if (isInterruptionActive)
                    for (var c = interruptingClipInfoCount - 1; c >= 0; c--)
                    {
                        var unityWeight = interruptingClipInfo[c].weight;
                        clipInfos.interruptingClipResolvedWeights[c] = interruptingClipInfo[c].weight * inverseWeight;
                        inverseWeight /= 1.0f - unityWeight;
                    }

                if (hasNext)
                    for (var c = nextClipInfoCount - 1; c >= 0; c--)
                    {
                        var unityWeight = nextClipInfo[c].weight;
                        clipInfos.nextClipResolvedWeights[c] = nextClipInfo[c].weight * inverseWeight;
                        inverseWeight /= 1.0f - unityWeight;
                    }

                for (var c = clipInfoCount - 1; c >= 0; c--)
                {
                    var unityWeight = clipInfo[c].weight;
                    clipInfos.clipResolvedWeights[c] = c == 0 ? 1f : clipInfo[c].weight * inverseWeight;
                    inverseWeight /= 1.0f - unityWeight;
                }
            }

            public KeyValuePair<Animation, float> GetActiveAnimationAndTime(int layer)
            {
                if (layer >= layerClipInfos.Length)
                    return new KeyValuePair<Animation, float>(null, 0);

                var layerInfos = layerClipInfos[layer];
                var isInterruptionActive = layerInfos.isInterruptionActive;
                AnimationClip clip = null;
                Animation animation = null;
                AnimatorStateInfo stateInfo;
                if (isInterruptionActive && layerInfos.interruptingClipInfoCount > 0)
                {
                    clip = layerInfos.interruptingClipInfos[0].clip;
                    stateInfo = layerInfos.interruptingStateInfo;
                }
                else
                {
                    clip = layerInfos.clipInfos[0].clip;
                    stateInfo = layerInfos.stateInfo;
                }

                animation = GetAnimation(clip);
                var time = AnimationTime(stateInfo.normalizedTime, clip.length,
                    clip.isLooping, stateInfo.speed < 0);
                return new KeyValuePair<Animation, float>(animation, time);
            }

            private static float AnimationTime(float normalizedTime, float clipLength, bool loop, bool reversed)
            {
                var time = ToSpineAnimationTime(normalizedTime, clipLength, loop, reversed);
                if (loop) return time;
                const float EndSnapEpsilon = 1f / 30f; // Workaround for end-duration keys not being applied.
                return clipLength - time < EndSnapEpsilon ? clipLength : time; // return a time snapped to clipLength;
            }

            private static float ToSpineAnimationTime(float normalizedTime, float clipLength, bool loop, bool reversed)
            {
                if (reversed)
                    normalizedTime = 1 - normalizedTime;
                if (normalizedTime < 0.0f)
                    normalizedTime = loop ? normalizedTime % 1.0f + 1.0f : 0.0f;
                return normalizedTime * clipLength;
            }

            private void InitClipInfosForLayers()
            {
                if (layerClipInfos.Length < Animator.layerCount)
                {
                    Array.Resize(ref layerClipInfos, Animator.layerCount);
                    for (int layer = 0, n = Animator.layerCount; layer < n; ++layer)
                        if (layerClipInfos[layer] == null)
                            layerClipInfos[layer] = new ClipInfos();
                }
            }

            private void ClearClipInfosForLayers()
            {
                for (int layer = 0, n = layerClipInfos.Length; layer < n; ++layer)
                    if (layerClipInfos[layer] == null)
                    {
                        layerClipInfos[layer] = new ClipInfos();
                    }
                    else
                    {
                        layerClipInfos[layer].isInterruptionActive = false;
                        layerClipInfos[layer].isLastFrameOfInterruption = false;
                        layerClipInfos[layer].clipInfos.Clear();
                        layerClipInfos[layer].nextClipInfos.Clear();
                        layerClipInfos[layer].interruptingClipInfos.Clear();
                    }
            }

            private MixMode GetMixMode(int layer, MixBlend layerBlendMode)
            {
                if (useCustomMixMode)
                {
                    var mode = layerMixModes[layer];
                    // Note: at additive blending it makes no sense to use constant weight 1 at a fadeout anim add1 as
                    // with override layers, so we use AlwaysMix instead to use the proper weights.
                    // AlwaysMix leads to the expected result = lower_layer + lerp(add1, add2, transition_weight).
                    if (layerBlendMode == MixBlend.Add && mode == MixMode.MixNext)
                    {
                        mode = MixMode.AlwaysMix;
                        layerMixModes[layer] = mode;
                    }

                    return mode;
                }

                return layerBlendMode == MixBlend.Add ? MixMode.AlwaysMix : MixMode.MixNext;
            }

#if UNITY_EDITOR
            private void GetLayerBlendModes()
            {
                if (layerBlendModes.Length < Animator.layerCount)
                    Array.Resize(ref layerBlendModes, Animator.layerCount);
                for (int layer = 0, n = Animator.layerCount; layer < n; ++layer)
                {
                    var controller = Animator.runtimeAnimatorController as AnimatorController;
                    if (controller != null)
                    {
                        layerBlendModes[layer] = MixBlend.First;
                        if (layer > 0)
                            layerBlendModes[layer] =
                                controller.layers[layer].blendingMode == AnimatorLayerBlendingMode.Additive
                                    ? MixBlend.Add
                                    : MixBlend.Replace;
                    }
                }
            }
#endif

            private void GetStateUpdatesFromAnimator(int layer)
            {
                var layerInfos = layerClipInfos[layer];
                var clipInfoCount = Animator.GetCurrentAnimatorClipInfoCount(layer);
                var nextClipInfoCount = Animator.GetNextAnimatorClipInfoCount(layer);

                var clipInfos = layerInfos.clipInfos;
                var nextClipInfos = layerInfos.nextClipInfos;
                var interruptingClipInfos = layerInfos.interruptingClipInfos;

                layerInfos.isInterruptionActive = clipInfoCount == 0 && clipInfos.Count != 0 &&
                                                  nextClipInfoCount == 0 && nextClipInfos.Count != 0;

                // Note: during interruption, GetCurrentAnimatorClipInfoCount and GetNextAnimatorClipInfoCount
                // are returning 0 in calls above. Therefore we keep previous clipInfos and nextClipInfos
                // until the interruption is over.
                if (layerInfos.isInterruptionActive)
                {
                    // Note: The last frame of a transition interruption
                    // will have fullPathHash set to 0, therefore we have to use previous
                    // frame's infos about interruption clips and correct some values
                    // accordingly (normalizedTime and weight).
                    var interruptingStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                    layerInfos.isLastFrameOfInterruption = interruptingStateInfo.fullPathHash == 0;
                    if (!layerInfos.isLastFrameOfInterruption)
                    {
                        Animator.GetNextAnimatorClipInfo(layer, interruptingClipInfos);
                        layerInfos.interruptingClipInfoCount = interruptingClipInfos.Count;
                        var oldTime = layerInfos.interruptingStateInfo.normalizedTime;
                        var newTime = interruptingStateInfo.normalizedTime;
                        layerInfos.interruptingClipTimeAddition = newTime - oldTime;
                        layerInfos.interruptingStateInfo = interruptingStateInfo;
                    }
                }
                else
                {
                    layerInfos.clipInfoCount = clipInfoCount;
                    layerInfos.nextClipInfoCount = nextClipInfoCount;
                    layerInfos.interruptingClipInfoCount = 0;
                    layerInfos.isLastFrameOfInterruption = false;

                    if (clipInfos.Capacity < clipInfoCount) clipInfos.Capacity = clipInfoCount;
                    if (nextClipInfos.Capacity < nextClipInfoCount) nextClipInfos.Capacity = nextClipInfoCount;

                    Animator.GetCurrentAnimatorClipInfo(layer, clipInfos);
                    Animator.GetNextAnimatorClipInfo(layer, nextClipInfos);

                    layerInfos.stateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
                    layerInfos.nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                }
            }

            private void GetAnimatorClipInfos(
                int layer,
                out bool isInterruptionActive,
                out int clipInfoCount,
                out int nextClipInfoCount,
                out int interruptingClipInfoCount,
                out IList<AnimatorClipInfo> clipInfo,
                out IList<AnimatorClipInfo> nextClipInfo,
                out IList<AnimatorClipInfo> interruptingClipInfo,
                out bool shallInterpolateWeightTo1)
            {
                var layerInfos = layerClipInfos[layer];
                isInterruptionActive = layerInfos.isInterruptionActive;

                clipInfoCount = layerInfos.clipInfoCount;
                nextClipInfoCount = layerInfos.nextClipInfoCount;
                interruptingClipInfoCount = layerInfos.interruptingClipInfoCount;

                clipInfo = layerInfos.clipInfos;
                nextClipInfo = layerInfos.nextClipInfos;
                interruptingClipInfo = isInterruptionActive ? layerInfos.interruptingClipInfos : null;
                shallInterpolateWeightTo1 = layerInfos.isLastFrameOfInterruption;
            }

            private void GetAnimatorStateInfos(
                int layer,
                out bool isInterruptionActive,
                out AnimatorStateInfo stateInfo,
                out AnimatorStateInfo nextStateInfo,
                out AnimatorStateInfo interruptingStateInfo,
                out float interruptingClipTimeAddition)
            {
                var layerInfos = layerClipInfos[layer];
                isInterruptionActive = layerInfos.isInterruptionActive;

                stateInfo = layerInfos.stateInfo;
                nextStateInfo = layerInfos.nextStateInfo;
                interruptingStateInfo = layerInfos.interruptingStateInfo;
                interruptingClipTimeAddition =
                    layerInfos.isLastFrameOfInterruption ? layerInfos.interruptingClipTimeAddition : 0;
            }

            private Animation GetAnimation(AnimationClip clip)
            {
                int clipNameHashCode;
                if (!clipNameHashCodeTable.TryGetValue(clip, out clipNameHashCode))
                {
                    clipNameHashCode = clip.name.GetHashCode();
                    clipNameHashCodeTable.Add(clip, clipNameHashCode);
                }

                Animation animation;
                animationTable.TryGetValue(clipNameHashCode, out animation);
                return animation;
            }

            protected class ClipInfos
            {
                public readonly List<AnimatorClipInfo> clipInfos = new();
                public readonly List<AnimatorClipInfo> interruptingClipInfos = new();
                public readonly List<AnimatorClipInfo> nextClipInfos = new();

                public int clipInfoCount;
                public float[] clipResolvedWeights = new float[0];
                public int interruptingClipInfoCount;
                public float[] interruptingClipResolvedWeights = new float[0];

                public float interruptingClipTimeAddition;
                public AnimatorStateInfo interruptingStateInfo;
                public bool isInterruptionActive;
                public bool isLastFrameOfInterruption;
                public int nextClipInfoCount;
                public float[] nextClipResolvedWeights = new float[0];
                public AnimatorStateInfo nextStateInfo;

                public AnimatorStateInfo stateInfo;
            }

            private class AnimationClipEqualityComparer : IEqualityComparer<AnimationClip>
            {
                internal static readonly IEqualityComparer<AnimationClip>
                    Instance = new AnimationClipEqualityComparer();

                public bool Equals(AnimationClip x, AnimationClip y)
                {
                    return x.GetInstanceID() == y.GetInstanceID();
                }

                public int GetHashCode(AnimationClip o)
                {
                    return o.GetInstanceID();
                }
            }

            private class IntEqualityComparer : IEqualityComparer<int>
            {
                internal static readonly IEqualityComparer<int> Instance = new IntEqualityComparer();

                public bool Equals(int x, int y)
                {
                    return x == y;
                }

                public int GetHashCode(int o)
                {
                    return o;
                }
            }

            #region Inspector

            public bool autoReset = true;
            public bool useCustomMixMode = true;
            public MixMode[] layerMixModes = new MixMode[0];
            public MixBlend[] layerBlendModes = new MixBlend[0];

            #endregion
        }

        #region Bone and Initialization Callbacks ISkeletonAnimation

        protected event ISkeletonAnimationDelegate _OnAnimationRebuild;
        protected event UpdateBonesDelegate _BeforeApply;
        protected event UpdateBonesDelegate _UpdateLocal;
        protected event UpdateBonesDelegate _UpdateWorld;
        protected event UpdateBonesDelegate _UpdateComplete;

        /// <summary>OnAnimationRebuild is raised after the SkeletonAnimation component is successfully initialized.</summary>
        public event ISkeletonAnimationDelegate OnAnimationRebuild
        {
            add => _OnAnimationRebuild += value;
            remove => _OnAnimationRebuild -= value;
        }

        /// <summary>
        ///     Occurs before the animations are applied.
        ///     Use this callback when you want to change the skeleton state before animations are applied on top.
        /// </summary>
        public event UpdateBonesDelegate BeforeApply
        {
            add => _BeforeApply += value;
            remove => _BeforeApply -= value;
        }

        /// <summary>
        ///     Occurs after the animations are applied and before world space values are resolved.
        ///     Use this callback when you want to set bone local values.
        /// </summary>
        public event UpdateBonesDelegate UpdateLocal
        {
            add => _UpdateLocal += value;
            remove => _UpdateLocal -= value;
        }

        /// <summary>
        ///     Occurs after the Skeleton's bone world space values are resolved (including all constraints).
        ///     Using this callback will cause the world space values to be solved an extra time.
        ///     Use this callback if want to use bone world space values, and also set bone local values.
        /// </summary>
        public event UpdateBonesDelegate UpdateWorld
        {
            add => _UpdateWorld += value;
            remove => _UpdateWorld -= value;
        }

        /// <summary>
        ///     Occurs after the Skeleton's bone world space values are resolved (including all constraints).
        ///     Use this callback if you want to use bone world space values, but don't intend to modify bone local values.
        ///     This callback can also be used when setting world position and the bone matrix.
        /// </summary>
        public event UpdateBonesDelegate UpdateComplete
        {
            add => _UpdateComplete += value;
            remove => _UpdateComplete -= value;
        }

        [SerializeField] protected UpdateTiming updateTiming = UpdateTiming.InUpdate;

        public UpdateTiming UpdateTiming
        {
            get => updateTiming;
            set => updateTiming = value;
        }

        #endregion
    }
}