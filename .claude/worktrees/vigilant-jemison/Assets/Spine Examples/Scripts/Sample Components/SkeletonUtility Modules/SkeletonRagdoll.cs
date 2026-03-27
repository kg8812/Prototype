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

// Contributed by: Mitch Thompson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Examples
{
    [RequireComponent(typeof(SkeletonRenderer))]
    public class SkeletonRagdoll : MonoBehaviour
    {
        private static Transform parentSpaceHelper;
        private readonly Dictionary<Bone, BoneFlipEntry> boneFlipTable = new();
        private readonly Dictionary<Bone, Transform> boneTable = new();
        private Transform ragdollRoot;
        private Skeleton skeleton;

        private ISkeletonAnimation targetSkeletonComponent;
        public Rigidbody RootRigidbody { get; private set; }
        public Bone StartingBone { get; private set; }
        public Vector3 RootOffset { get; private set; }

        public bool IsActive { get; private set; }

        private IEnumerator Start()
        {
            if (parentSpaceHelper == null)
            {
                parentSpaceHelper = new GameObject("Parent Space Helper").transform;
                parentSpaceHelper.hideFlags = HideFlags.HideInHierarchy;
            }

            targetSkeletonComponent = GetComponent<SkeletonRenderer>() as ISkeletonAnimation;
            if (targetSkeletonComponent == null)
                Debug.LogError(
                    "Attached Spine component does not implement ISkeletonAnimation. This script is not compatible.");
            skeleton = targetSkeletonComponent.Skeleton;

            if (applyOnStart)
            {
                yield return null;
                Apply();
            }
        }

        private void RecursivelyCreateBoneProxies(Bone b)
        {
            var boneName = b.Data.Name;
            if (stopBoneNames.Contains(boneName))
                return;

            var boneGameObject = new GameObject(boneName);
            boneGameObject.layer = colliderLayer;
            var t = boneGameObject.transform;
            boneTable.Add(b, t);

            t.parent = transform;
            t.localPosition = new Vector3(b.WorldX, b.WorldY, 0);
            t.localRotation = Quaternion.Euler(0, 0, b.WorldRotationX - b.ShearX);
            t.localScale = new Vector3(b.WorldScaleX, b.WorldScaleY, 1);

            var colliders = AttachBoundingBoxRagdollColliders(b);
            if (colliders.Count == 0)
            {
                var length = b.Data.Length;
                if (length == 0)
                {
                    var ball = boneGameObject.AddComponent<SphereCollider>();
                    ball.radius = thickness * 0.5f;
                }
                else
                {
                    var box = boneGameObject.AddComponent<BoxCollider>();
                    box.size = new Vector3(length, thickness, thickness);
                    box.center = new Vector3(length * 0.5f, 0);
                }
            }

            var rb = boneGameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ;

            foreach (var child in b.Children)
                RecursivelyCreateBoneProxies(child);
        }

        private void UpdateSpineSkeleton(ISkeletonAnimation skeletonRenderer)
        {
            bool parentFlipX;
            bool parentFlipY;
            GetStartBoneParentFlipState(out parentFlipX, out parentFlipY);

            foreach (var pair in boneTable)
            {
                var b = pair.Key;
                var t = pair.Value;
                var isStartingBone = b == StartingBone;
                var parentBone = b.Parent;
                var parentTransform = isStartingBone ? ragdollRoot : boneTable[parentBone];
                if (!isStartingBone)
                {
                    var parentBoneFlip = boneFlipTable[parentBone];
                    parentFlipX = parentBoneFlip.flipX;
                    parentFlipY = parentBoneFlip.flipY;
                }

                var flipX = parentFlipX ^ (b.ScaleX < 0);
                var flipY = parentFlipY ^ (b.ScaleY < 0);

                BoneFlipEntry boneFlip;
                boneFlipTable.TryGetValue(b, out boneFlip);
                boneFlip.flipX = flipX;
                boneFlip.flipY = flipY;
                boneFlipTable[b] = boneFlip;

                var flipXOR = flipX ^ flipY;
                var parentFlipXOR = parentFlipX ^ parentFlipY;

                if (!oldRagdollBehaviour && isStartingBone)
                    if (b != skeleton.RootBone)
                    {
                        // RagdollRoot is not skeleton root.
                        ragdollRoot.localPosition = new Vector3(parentBone.WorldX, parentBone.WorldY, 0);
                        ragdollRoot.localRotation =
                            Quaternion.Euler(0, 0, parentBone.WorldRotationX - parentBone.ShearX);
                        ragdollRoot.localScale = new Vector3(parentBone.WorldScaleX, parentBone.WorldScaleY, 1);
                    }

                var parentTransformWorldPosition = parentTransform.position;
                var parentTransformWorldRotation = parentTransform.rotation;

                parentSpaceHelper.position = parentTransformWorldPosition;
                parentSpaceHelper.rotation = parentTransformWorldRotation;
                parentSpaceHelper.localScale = parentTransform.lossyScale;

                if (oldRagdollBehaviour)
                    if (isStartingBone && b != skeleton.RootBone)
                    {
                        var localPosition = new Vector3(b.Parent.WorldX, b.Parent.WorldY, 0);
                        parentSpaceHelper.position = ragdollRoot.TransformPoint(localPosition);
                        parentSpaceHelper.localRotation =
                            Quaternion.Euler(0, 0, parentBone.WorldRotationX - parentBone.ShearX);
                        parentSpaceHelper.localScale = new Vector3(parentBone.WorldScaleX, parentBone.WorldScaleY, 1);
                    }

                var boneWorldPosition = t.position;
                var right = parentSpaceHelper.InverseTransformDirection(t.right);

                var boneLocalPosition = parentSpaceHelper.InverseTransformPoint(boneWorldPosition);
                var boneLocalRotation = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;

                if (flipXOR) boneLocalPosition.y *= -1f;
                if (parentFlipXOR != flipXOR) boneLocalPosition.y *= -1f;

                if (parentFlipXOR) boneLocalRotation *= -1f;
                if (parentFlipX != flipX) boneLocalRotation += 180;

                b.X = Mathf.Lerp(b.X, boneLocalPosition.x, mix);
                b.Y = Mathf.Lerp(b.Y, boneLocalPosition.y, mix);
                b.Rotation = Mathf.Lerp(b.Rotation, boneLocalRotation, mix);
                //b.AppliedRotation = Mathf.Lerp(b.AppliedRotation, boneLocalRotation, mix);
            }
        }

        private void GetStartBoneParentFlipState(out bool parentFlipX, out bool parentFlipY)
        {
            parentFlipX = skeleton.ScaleX < 0;
            parentFlipY = skeleton.ScaleY < 0;
            var parent = StartingBone == null ? null : StartingBone.Parent;
            while (parent != null)
            {
                parentFlipX ^= parent.ScaleX < 0;
                parentFlipY ^= parent.ScaleY < 0;
                parent = parent.Parent;
            }
        }

        private List<Collider> AttachBoundingBoxRagdollColliders(Bone b)
        {
            const string AttachmentNameMarker = "ragdoll";
            var colliders = new List<Collider>();

            var t = boneTable[b];
            var go = t.gameObject;
            var skin = skeleton.Skin ?? skeleton.Data.DefaultSkin;

            var skinEntries = new List<Skin.SkinEntry>();
            foreach (var s in skeleton.Slots)
                if (s.Bone == b)
                {
                    skin.GetAttachments(skeleton.Slots.IndexOf(s), skinEntries);

                    foreach (var entry in skinEntries)
                    {
                        var bbAttachment = entry.Attachment as BoundingBoxAttachment;
                        if (bbAttachment != null)
                        {
                            if (!entry.Name.ToLower().Contains(AttachmentNameMarker))
                                continue;

                            var bbCollider = go.AddComponent<BoxCollider>();
                            var bounds = SkeletonUtility.GetBoundingBoxBounds(bbAttachment, thickness);
                            bbCollider.center = bounds.center;
                            bbCollider.size = bounds.size;
                            colliders.Add(bbCollider);
                        }
                    }
                }

            return colliders;
        }

        private struct BoneFlipEntry
        {
            public BoneFlipEntry(bool flipX, bool flipY)
            {
                this.flipX = flipX;
                this.flipY = flipY;
            }

            public bool flipX;
            public bool flipY;
        }

        public class LayerFieldAttribute : PropertyAttribute
        {
        }

        #region Inspector

        [Header("Hierarchy")] [SpineBone] public string startingBoneName = "";

        [SpineBone] public List<string> stopBoneNames = new();

        [Header("Parameters")] public bool applyOnStart;

        [Tooltip(
            "Warning! You will have to re-enable and tune mix values manually if attempting to remove the ragdoll system.")]
        public bool disableIK = true;

        public bool disableOtherConstraints;

        [Space(18)] [Tooltip("Set RootRigidbody IsKinematic to true when Apply is called.")]
        public bool pinStartBone;

        [Tooltip("Enable Collision between adjacent ragdoll elements (IE: Neck and Head)")]
        public bool enableJointCollision;

        public bool useGravity = true;

        [Tooltip(
            "If no BoundingBox Attachment is attached to a bone, this becomes the default Width or Radius of a Bone's ragdoll Rigidbody")]
        public float thickness = 0.125f;

        [Tooltip("Default rotational limit value. Min is negative this value, Max is this value.")]
        public float rotationLimit = 20;

        public float rootMass = 20;

        [Tooltip("If your ragdoll seems unstable or uneffected by limits, try lowering this value.")] [Range(0.01f, 1f)]
        public float massFalloffFactor = 0.4f;

        [Tooltip("The layer assigned to all of the rigidbody parts.")]
        public int colliderLayer;

        [Range(0, 1)] public float mix = 1;

        public bool oldRagdollBehaviour;

        #endregion

        #region API

        public Rigidbody[] RigidbodyArray
        {
            get
            {
                if (!IsActive)
                    return new Rigidbody[0];

                var rigidBodies = new Rigidbody[boneTable.Count];
                var i = 0;
                foreach (var t in boneTable.Values)
                {
                    rigidBodies[i] = t.GetComponent<Rigidbody>();
                    i++;
                }

                return rigidBodies;
            }
        }

        public Vector3 EstimatedSkeletonPosition => RootRigidbody.position - RootOffset;

        /// <summary>Instantiates the ragdoll simulation and applies its transforms to the skeleton.</summary>
        public void Apply()
        {
            IsActive = true;
            mix = 1;

            StartingBone = skeleton.FindBone(startingBoneName);
            RecursivelyCreateBoneProxies(StartingBone);

            RootRigidbody = boneTable[StartingBone].GetComponent<Rigidbody>();
            RootRigidbody.isKinematic = pinStartBone;
            RootRigidbody.mass = rootMass;
            var boneColliders = new List<Collider>();
            foreach (var pair in boneTable)
            {
                var b = pair.Key;
                var t = pair.Value;
                Transform parentTransform;
                boneColliders.Add(t.GetComponent<Collider>());
                if (b == StartingBone)
                {
                    ragdollRoot = new GameObject("RagdollRoot").transform;
                    ragdollRoot.SetParent(transform, false);
                    if (b == skeleton.RootBone)
                    {
                        // RagdollRoot is skeleton root's parent, thus the skeleton's scale and position.
                        ragdollRoot.localPosition = new Vector3(skeleton.X, skeleton.Y, 0);
                        ragdollRoot.localRotation =
                            skeleton.ScaleX < 0 ? Quaternion.Euler(0, 0, 180.0f) : Quaternion.identity;
                    }
                    else
                    {
                        ragdollRoot.localPosition = new Vector3(b.Parent.WorldX, b.Parent.WorldY, 0);
                        ragdollRoot.localRotation = Quaternion.Euler(0, 0, b.Parent.WorldRotationX - b.Parent.ShearX);
                    }

                    parentTransform = ragdollRoot;
                    RootOffset = t.position - transform.position;
                }
                else
                {
                    parentTransform = boneTable[b.Parent];
                }

                // Add joint and attach to parent.
                var rbParent = parentTransform.GetComponent<Rigidbody>();
                if (rbParent != null)
                {
                    var joint = t.gameObject.AddComponent<HingeJoint>();
                    joint.connectedBody = rbParent;
                    var localPos = parentTransform.InverseTransformPoint(t.position);
                    localPos.x *= 1;
                    joint.connectedAnchor = localPos;
                    joint.axis = Vector3.forward;

                    joint.GetComponent<Rigidbody>().mass = joint.connectedBody.mass * massFalloffFactor;
                    joint.limits = new JointLimits
                    {
                        min = -rotationLimit,
                        max = rotationLimit
                    };
                    joint.useLimits = true;
                    joint.enableCollision = enableJointCollision;
                }
            }

            // Ignore collisions among bones.
            for (var x = 0; x < boneColliders.Count; x++)
            for (var y = 0; y < boneColliders.Count; y++)
            {
                if (x == y) continue;
                Physics.IgnoreCollision(boneColliders[x], boneColliders[y]);
            }

            // Destroy existing override-mode SkeletonUtilityBones.
            var utilityBones = GetComponentsInChildren<SkeletonUtilityBone>();
            if (utilityBones.Length > 0)
            {
                var destroyedUtilityBoneNames = new List<string>();
                foreach (var ub in utilityBones)
                    if (ub.mode == SkeletonUtilityBone.Mode.Override)
                    {
                        destroyedUtilityBoneNames.Add(ub.gameObject.name);
                        Destroy(ub.gameObject);
                    }

                if (destroyedUtilityBoneNames.Count > 0)
                {
                    var msg = "Destroyed Utility Bones: ";
                    for (var i = 0; i < destroyedUtilityBoneNames.Count; i++)
                    {
                        msg += destroyedUtilityBoneNames[i];
                        if (i != destroyedUtilityBoneNames.Count - 1) msg += ",";
                    }

                    Debug.LogWarning(msg);
                }
            }

            // Disable skeleton constraints.
            if (disableIK)
            {
                var ikConstraints = skeleton.IkConstraints;
                for (int i = 0, n = ikConstraints.Count; i < n; i++)
                    ikConstraints.Items[i].Mix = 0;
            }

            if (disableOtherConstraints)
            {
                var transformConstraints = skeleton.TransformConstraints;
                for (int i = 0, n = transformConstraints.Count; i < n; i++)
                {
                    transformConstraints.Items[i].MixRotate = 0;
                    transformConstraints.Items[i].MixScaleX = 0;
                    transformConstraints.Items[i].MixScaleY = 0;
                    transformConstraints.Items[i].MixShearY = 0;
                    transformConstraints.Items[i].MixX = 0;
                    transformConstraints.Items[i].MixY = 0;
                }

                var pathConstraints = skeleton.PathConstraints;
                for (int i = 0, n = pathConstraints.Count; i < n; i++)
                {
                    pathConstraints.Items[i].MixRotate = 0;
                    pathConstraints.Items[i].MixX = 0;
                    pathConstraints.Items[i].MixY = 0;
                }
            }

            targetSkeletonComponent.UpdateWorld += UpdateSpineSkeleton;
        }

        /// <summary>Transitions the mix value from the current value to a target value.</summary>
        public Coroutine SmoothMix(float target, float duration)
        {
            return StartCoroutine(SmoothMixCoroutine(target, duration));
        }

        private IEnumerator SmoothMixCoroutine(float target, float duration)
        {
            var startTime = Time.time;
            var startMix = mix;
            while (mix > 0)
            {
                skeleton.SetBonesToSetupPose();
                mix = Mathf.SmoothStep(startMix, target, (Time.time - startTime) / duration);
                yield return null;
            }
        }

        /// <summary>Set the transform world position while preserving the ragdoll parts world position.</summary>
        public void SetSkeletonPosition(Vector3 worldPosition)
        {
            if (!IsActive)
            {
                Debug.LogWarning("Can't call SetSkeletonPosition while Ragdoll is not active!");
                return;
            }

            var offset = worldPosition - transform.position;
            transform.position = worldPosition;
            foreach (var t in boneTable.Values)
                t.position -= offset;

            UpdateSpineSkeleton(null);
            skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
        }

        /// <summary>Removes the ragdoll instance and effect from the animated skeleton.</summary>
        public void Remove()
        {
            IsActive = false;
            foreach (var t in boneTable.Values)
                Destroy(t.gameObject);

            Destroy(ragdollRoot.gameObject);

            boneTable.Clear();
            targetSkeletonComponent.UpdateWorld -= UpdateSpineSkeleton;
        }

        public Rigidbody GetRigidbody(string boneName)
        {
            var bone = skeleton.FindBone(boneName);
            return bone != null && boneTable.ContainsKey(bone) ? boneTable[bone].GetComponent<Rigidbody>() : null;
        }

        #endregion
    }
}