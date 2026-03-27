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

#if UNITY_2019_2_OR_NEWER
#define HINGE_JOINT_NEW_BEHAVIOUR
#endif

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Editor
{
    using Icons = SpineEditorUtilities.Icons;

    [CustomEditor(typeof(SkeletonUtilityBone))]
    [CanEditMultipleObjects]
    public class SkeletonUtilityBoneInspector : UnityEditor.Editor
    {
        private readonly Dictionary<Slot, List<BoundingBoxAttachment>> boundingBoxTable = new();
        private bool canCreateHingeChain;

        //multi selected flags
        private bool containsFollows, containsOverrides, multiObject;
        private GUIContent hierarchyLabel;

        private SerializedProperty mode,
            boneName,
            zPosition,
            position,
            rotation,
            scale,
            overrideAlpha,
            hierarchy,
            parentReference;

        private SkeletonUtility skeletonUtility;

        //single selected helpers
        private SkeletonUtilityBone utilityBone;

        private void OnEnable()
        {
            mode = serializedObject.FindProperty("mode");
            boneName = serializedObject.FindProperty("boneName");
            zPosition = serializedObject.FindProperty("zPosition");
            position = serializedObject.FindProperty("position");
            rotation = serializedObject.FindProperty("rotation");
            scale = serializedObject.FindProperty("scale");
            overrideAlpha = serializedObject.FindProperty("overrideAlpha");
            hierarchy = serializedObject.FindProperty("hierarchy");
            hierarchyLabel = new GUIContent("Skeleton Utility Parent");
            parentReference = serializedObject.FindProperty("parentReference");

            utilityBone = (SkeletonUtilityBone)target;
            skeletonUtility = utilityBone.hierarchy;
            EvaluateFlags();

            if (!utilityBone.valid && skeletonUtility != null)
            {
                if (skeletonUtility.skeletonRenderer != null)
                    skeletonUtility.skeletonRenderer.Initialize(false);
                if (skeletonUtility.skeletonGraphic != null)
                    skeletonUtility.skeletonGraphic.Initialize(false);
            }

            canCreateHingeChain = CanCreateHingeChain();
            boundingBoxTable.Clear();

            if (multiObject) return;
            if (utilityBone.bone == null) return;

            var skeleton = utilityBone.bone.Skeleton;
            var slotCount = skeleton.Slots.Count;
            var skin = skeleton.Skin;
            if (skeleton.Skin == null)
                skin = skeleton.Data.DefaultSkin;

            for (var i = 0; i < slotCount; i++)
            {
                var slot = skeletonUtility.Skeleton.Slots.Items[i];
                if (slot.Bone == utilityBone.bone)
                {
                    var slotAttachments = new List<Skin.SkinEntry>();
                    var slotIndex = skeleton.Data.FindSlot(slot.Data.Name).Index;
                    skin.GetAttachments(slotIndex, slotAttachments);

                    var boundingBoxes = new List<BoundingBoxAttachment>();
                    foreach (var entry in slotAttachments)
                    {
                        var boundingBoxAttachment = entry.Attachment as BoundingBoxAttachment;
                        if (boundingBoxAttachment != null)
                            boundingBoxes.Add(boundingBoxAttachment);
                    }

                    if (boundingBoxes.Count > 0)
                        boundingBoxTable.Add(slot, boundingBoxes);
                }
            }
        }

        private void EvaluateFlags()
        {
            if (Selection.objects.Length == 1)
            {
                containsFollows = utilityBone.mode == SkeletonUtilityBone.Mode.Follow;
                containsOverrides = utilityBone.mode == SkeletonUtilityBone.Mode.Override;
            }
            else
            {
                var boneCount = 0;
                foreach (var o in Selection.objects)
                {
                    var go = o as GameObject;
                    if (go != null)
                    {
                        var sub = go.GetComponent<SkeletonUtilityBone>();
                        if (sub != null)
                        {
                            boneCount++;
                            containsFollows |= sub.mode == SkeletonUtilityBone.Mode.Follow;
                            containsOverrides |= sub.mode == SkeletonUtilityBone.Mode.Override;
                        }
                    }
                }

                multiObject |= boneCount > 1;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mode);
            if (EditorGUI.EndChangeCheck())
            {
                containsOverrides = mode.enumValueIndex == 1;
                containsFollows = mode.enumValueIndex == 0;
            }

            using (new EditorGUI.DisabledGroupScope(multiObject))
            {
                var str = boneName.stringValue;
                if (str == "")
                    str = "<None>";
                if (multiObject)
                    str = "<Multiple>";

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Bone");
                    if (GUILayout.Button(str, EditorStyles.popup))
                        BoneSelectorContextMenu(str, ((SkeletonUtilityBone)target).hierarchy.Skeleton.Bones, "<None>",
                            TargetBoneSelected);
                }
            }

            var isOverrideMode = mode.enumValueIndex == 1;
            using (new EditorGUI.DisabledGroupScope(isOverrideMode))
            {
                EditorGUILayout.PropertyField(zPosition);
            }

            EditorGUILayout.PropertyField(position, new GUIContent("XY Position"));
            EditorGUILayout.PropertyField(rotation);
            EditorGUILayout.PropertyField(scale);

            using (new EditorGUI.DisabledGroupScope(containsFollows))
            {
                EditorGUILayout.PropertyField(overrideAlpha);
                EditorGUILayout.PropertyField(parentReference);
                EditorGUILayout.PropertyField(hierarchy, hierarchyLabel);
            }

            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                using (new EditorGUI.DisabledGroupScope(multiObject || !utilityBone.valid || utilityBone.bone == null ||
                                                        utilityBone.bone.Children.Count == 0))
                {
                    if (GUILayout.Button(SpineInspectorUtility.TempContent("Add Child Bone", Icons.bone),
                            GUILayout.MinWidth(120), GUILayout.Height(24)))
                        BoneSelectorContextMenu("", utilityBone.bone.Children, "<Recursively>", SpawnChildBoneSelected);
                }

                using (new EditorGUI.DisabledGroupScope(multiObject || !utilityBone.valid || utilityBone.bone == null ||
                                                        containsOverrides))
                {
                    if (GUILayout.Button(SpineInspectorUtility.TempContent("Add Override", Icons.poseBones),
                            GUILayout.MinWidth(120), GUILayout.Height(24)))
                        SpawnOverride();
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                using (new EditorGUI.DisabledGroupScope(multiObject || !utilityBone.valid || !canCreateHingeChain))
                {
                    if (GUILayout.Button(SpineInspectorUtility.TempContent("Create 3D Hinge Chain", Icons.hingeChain),
                            GUILayout.MinWidth(120), GUILayout.Height(24)))
                        CreateHingeChain();
                    if (GUILayout.Button(SpineInspectorUtility.TempContent("Create 2D Hinge Chain", Icons.hingeChain),
                            GUILayout.MinWidth(120), GUILayout.Height(24)))
                        CreateHingeChain2D();
                }

                EditorGUILayout.Space();
            }

            using (new EditorGUI.DisabledGroupScope(multiObject || boundingBoxTable.Count == 0))
            {
                EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Bounding Boxes", Icons.boundingBox),
                    EditorStyles.boldLabel);

                foreach (var entry in boundingBoxTable)
                {
                    var slot = entry.Key;
                    var boundingBoxes = entry.Value;

                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(slot.Data.Name);
                    EditorGUI.indentLevel++;
                    {
                        foreach (var box in boundingBoxes)
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Space(30);
                                var buttonLabel = box.IsWeighted() ? box.Name + " (!)" : box.Name;
                                if (GUILayout.Button(buttonLabel, GUILayout.Width(200)))
                                {
                                    utilityBone.bone.Skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
                                    var bbTransform =
                                        utilityBone.transform.Find("[BoundingBox]" +
                                                                   box
                                                                       .Name); // Use FindChild in older versions of Unity.
                                    if (bbTransform != null)
                                    {
                                        var originalCollider = bbTransform.GetComponent<PolygonCollider2D>();
                                        if (originalCollider != null)
                                            SkeletonUtility.SetColliderPointsLocal(originalCollider, slot, box);
                                        else
                                            SkeletonUtility.AddBoundingBoxAsComponent(box, slot,
                                                bbTransform.gameObject);
                                    }
                                    else
                                    {
                                        var newPolygonCollider =
                                            SkeletonUtility.AddBoundingBoxGameObject(null, box, slot,
                                                utilityBone.transform);
                                        bbTransform = newPolygonCollider.transform;
                                    }

                                    EditorGUIUtility.PingObject(bbTransform);
                                }
                            }
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }

            BoneFollowerInspector.RecommendRigidbodyButton(utilityBone);

            serializedObject.ApplyModifiedProperties();
        }

        private static void BoneSelectorContextMenu(string current, ExposedList<Bone> bones, string topValue,
            GenericMenu.MenuFunction2 callback)
        {
            var menu = new GenericMenu();

            if (topValue != "")
                menu.AddItem(new GUIContent(topValue), current == topValue, callback, null);

            for (var i = 0; i < bones.Count; i++)
                menu.AddItem(new GUIContent(bones.Items[i].Data.Name), bones.Items[i].Data.Name == current, callback,
                    bones.Items[i]);

            menu.ShowAsContext();
        }

        private void TargetBoneSelected(object obj)
        {
            if (obj == null)
            {
                boneName.stringValue = "";
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                var bone = (Bone)obj;
                boneName.stringValue = bone.Data.Name;
                serializedObject.ApplyModifiedProperties();
                utilityBone.Reset();
            }
        }

        private void SpawnChildBoneSelected(object obj)
        {
            if (obj == null)
            {
                // Add recursively
                foreach (var bone in utilityBone.bone.Children)
                {
                    var go = skeletonUtility.SpawnBoneRecursively(bone, utilityBone.transform, utilityBone.mode,
                        utilityBone.position, utilityBone.rotation, utilityBone.scale);
                    var newUtilityBones = go.GetComponentsInChildren<SkeletonUtilityBone>();
                    foreach (var utilBone in newUtilityBones)
                        SkeletonUtilityInspector.AttachIcon(utilBone);
                }
            }
            else
            {
                var bone = (Bone)obj;
                var go = skeletonUtility.SpawnBone(bone, utilityBone.transform, utilityBone.mode, utilityBone.position,
                    utilityBone.rotation, utilityBone.scale);
                SkeletonUtilityInspector.AttachIcon(go.GetComponent<SkeletonUtilityBone>());
                Selection.activeGameObject = go;
                EditorGUIUtility.PingObject(go);
            }
        }

        private void SpawnOverride()
        {
            var go = skeletonUtility.SpawnBone(utilityBone.bone, utilityBone.transform.parent,
                SkeletonUtilityBone.Mode.Override, utilityBone.position, utilityBone.rotation, utilityBone.scale);
            go.name = go.name + " [Override]";
            SkeletonUtilityInspector.AttachIcon(go.GetComponent<SkeletonUtilityBone>());
            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
        }

        private bool CanCreateHingeChain()
        {
            if (utilityBone == null)
                return false;
            if (utilityBone.GetComponent<Rigidbody>() != null || utilityBone.GetComponent<Rigidbody2D>() != null)
                return false;
            if (utilityBone.bone != null && utilityBone.bone.Children.Count == 0)
                return false;

            var rigidbodies = utilityBone.GetComponentsInChildren<Rigidbody>();
            var rigidbodies2D = utilityBone.GetComponentsInChildren<Rigidbody2D>();
            return rigidbodies.Length <= 0 && rigidbodies2D.Length <= 0;
        }

        private void CreateHingeChain2D()
        {
            var kinematicParentUtilityBone = utilityBone.transform.parent.GetComponent<SkeletonUtilityBone>();
            if (kinematicParentUtilityBone == null)
            {
                EditorUtility.DisplayDialog("No parent SkeletonUtilityBone found!",
                    "Please select the first physically moving chain node, having a parent GameObject with a SkeletonUtilityBone component attached.",
                    "OK");
                return;
            }

            float mass = 10;
            const float rotationLimit = 20.0f;

            SetSkeletonUtilityToFlipByRotation();

            kinematicParentUtilityBone.mode = SkeletonUtilityBone.Mode.Follow;
            kinematicParentUtilityBone.position = kinematicParentUtilityBone.rotation =
                kinematicParentUtilityBone.scale = kinematicParentUtilityBone.zPosition = true;

            var commonParentObject = new GameObject(skeletonUtility.name + " HingeChain Parent " + utilityBone.name);
            var commonParentActivateOnFlip = commonParentObject.AddComponent<ActivateBasedOnFlipDirection>();
            commonParentActivateOnFlip.skeletonRenderer = skeletonUtility.skeletonRenderer;
            commonParentActivateOnFlip.skeletonGraphic = skeletonUtility.skeletonGraphic;

            // HingeChain Parent
            // Needs to be on top hierarchy level (not attached to the moving skeleton at least) for physics to apply proper momentum.
            var normalChainParentObject = new GameObject("HingeChain");
            normalChainParentObject.transform.SetParent(commonParentObject.transform);
            commonParentActivateOnFlip.activeOnNormalX = normalChainParentObject;

            //FollowSkeletonUtilityRootRotation followRotationComponent = normalChainParentObject.AddComponent<FollowSkeletonUtilityRootRotation>();
            //followRotationComponent.reference = skeletonUtility.boneRoot;

            // Follower Kinematic Rigidbody
            var rootFollowerKinematic = new GameObject(kinematicParentUtilityBone.name + " Follower");
            rootFollowerKinematic.transform.parent = normalChainParentObject.transform;
            var followerRigidbody = rootFollowerKinematic.AddComponent<Rigidbody2D>();
            followerRigidbody.mass = mass;
            followerRigidbody.isKinematic = true;
            rootFollowerKinematic.AddComponent<FollowLocationRigidbody2D>().reference =
                kinematicParentUtilityBone.transform;
            rootFollowerKinematic.transform.position = kinematicParentUtilityBone.transform.position;
            rootFollowerKinematic.transform.rotation = kinematicParentUtilityBone.transform.rotation;

            CreateHingeChain2D(utilityBone, mass, rotationLimit, normalChainParentObject.transform,
                rootFollowerKinematic.transform, kinematicParentUtilityBone.transform);

            Duplicate2DHierarchyForFlippedChains(normalChainParentObject, commonParentActivateOnFlip,
                skeletonUtility.transform, rotationLimit);
            Selection.activeGameObject = commonParentObject;
        }

        private void CreateHingeChain2D(SkeletonUtilityBone bone, float mass, float rotationLimit,
            Transform groupObject,
            Transform jointParent, Transform utilityParent)
        {
            mass *= 0.75f;
            bone.parentReference = utilityParent;
            bone.transform.SetParent(groupObject, true); // we need a flat hierarchy of all Joint objects in Unity.
            AttachRigidbodyAndCollider2D(bone);
            bone.mode = SkeletonUtilityBone.Mode.Override;
            bone.scale = bone.position = bone.zPosition = false;

            var joint = bone.gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = jointParent.GetComponent<Rigidbody2D>();
            joint.useLimits = true;
            ApplyJoint2DAngleLimits(joint, rotationLimit, jointParent, bone.transform);
            bone.GetComponent<Rigidbody2D>().mass = mass;

            var parent = bone.transform;
            var children = new List<SkeletonUtilityBone>();
            var utilityChildCount = 0;
            for (var i = 0; i < parent.childCount; ++i)
            {
                var childUtilityBone = parent.GetChild(i).GetComponent<SkeletonUtilityBone>();
                if (childUtilityBone != null)
                    children.Add(childUtilityBone);
            }

            mass /= Mathf.Max(1.0f, utilityChildCount);

            for (var i = 0; i < children.Count; ++i)
            {
                var childBone = children[i];
                if (childBone == null) continue;
                CreateHingeChain2D(childBone, mass, rotationLimit, groupObject, parent, parent);
            }
        }

        private void ApplyJoint2DAngleLimits(HingeJoint2D joint, float rotationLimit, Transform parentBone,
            Transform bone)
        {
#if HINGE_JOINT_NEW_BEHAVIOUR
            var referenceAngle = (parentBone.eulerAngles.z - bone.eulerAngles.z + 360f) % 360f;
            var minAngle = referenceAngle - rotationLimit;
            var maxAngle = referenceAngle + rotationLimit;
            if (maxAngle > 270f)
            {
                minAngle -= 360f;
                maxAngle -= 360f;
            }

            if (minAngle < -90f)
            {
                minAngle += 360f;
                maxAngle += 360f;
            }
#else
			float minAngle = -rotationLimit;
			float maxAngle = rotationLimit;
#endif
            joint.limits = new JointAngleLimits2D
            {
                min = minAngle,
                max = maxAngle
            };
        }

        private void Duplicate2DHierarchyForFlippedChains(GameObject normalChainParentObject,
            ActivateBasedOnFlipDirection commonParentActivateOnFlip,
            Transform skeletonUtilityRoot, float rotationLimit)
        {
            var mirroredChain = Instantiate(normalChainParentObject, normalChainParentObject.transform.position,
                normalChainParentObject.transform.rotation, commonParentActivateOnFlip.transform);
            mirroredChain.name = normalChainParentObject.name + " FlippedX";

            commonParentActivateOnFlip.activeOnFlippedX = mirroredChain;

            var followerKinematicObject = mirroredChain.GetComponentInChildren<FollowLocationRigidbody2D>();
            followerKinematicObject.followFlippedX = true;
            FlipBone2DHorizontal(followerKinematicObject.transform, skeletonUtilityRoot);

            var childBoneJoints = mirroredChain.GetComponentsInChildren<HingeJoint2D>();
            Transform prevRotatedChild = null;
            var parentTransformForAngles = followerKinematicObject.transform;
            for (var i = 0; i < childBoneJoints.Length; ++i)
            {
                var joint = childBoneJoints[i];
                FlipBone2DHorizontal(joint.transform, skeletonUtilityRoot);
                ApplyJoint2DAngleLimits(joint, rotationLimit, parentTransformForAngles, joint.transform);

                var rotatedChild = Instantiate(joint.gameObject, joint.transform, true);
                rotatedChild.name = joint.name + " rotated";
                var rotationEulerAngles = rotatedChild.transform.localEulerAngles;
                rotationEulerAngles.x = 180;
                rotatedChild.transform.localEulerAngles = rotationEulerAngles;
                DestroyImmediate(rotatedChild.GetComponent<HingeJoint2D>());
                DestroyImmediate(rotatedChild.GetComponent<BoxCollider2D>());
                DestroyImmediate(rotatedChild.GetComponent<Rigidbody2D>());

                DestroyImmediate(joint.gameObject.GetComponent<SkeletonUtilityBone>());

                if (i > 0)
                {
                    var utilityBone = rotatedChild.GetComponent<SkeletonUtilityBone>();
                    utilityBone.parentReference = prevRotatedChild;
                }

                prevRotatedChild = rotatedChild.transform;
                parentTransformForAngles = joint.transform;
            }

            mirroredChain.SetActive(false);
        }

        private void FlipBone2DHorizontal(Transform bone, Transform mirrorPosition)
        {
            var position = bone.position;
            position.x =
                2 * mirrorPosition.position.x - position.x; // = mirrorPosition + (mirrorPosition - bone.position)
            bone.position = position;

            var boneZ = bone.forward;
            var boneX = bone.right;
            boneX.x *= -1;

            bone.rotation = Quaternion.LookRotation(boneZ, Vector3.Cross(boneZ, boneX));
        }

        private void CreateHingeChain()
        {
            var kinematicParentUtilityBone = utilityBone.transform.parent.GetComponent<SkeletonUtilityBone>();
            if (kinematicParentUtilityBone == null)
            {
                EditorUtility.DisplayDialog("No parent SkeletonUtilityBone found!",
                    "Please select the first physically moving chain node, having a parent GameObject with a SkeletonUtilityBone component attached.",
                    "OK");
                return;
            }

            float mass = 10;
            const float rotationLimit = 20.0f;

            SetSkeletonUtilityToFlipByRotation();

            kinematicParentUtilityBone.mode = SkeletonUtilityBone.Mode.Follow;
            kinematicParentUtilityBone.position = kinematicParentUtilityBone.rotation =
                kinematicParentUtilityBone.scale = kinematicParentUtilityBone.zPosition = true;

            // HingeChain Parent
            // Needs to be on top hierarchy level (not attached to the moving skeleton at least) for physics to apply proper momentum.
            var chainParentObject = new GameObject(skeletonUtility.name + " HingeChain Parent " + utilityBone.name);
            var followRotationComponent = chainParentObject.AddComponent<FollowSkeletonUtilityRootRotation>();
            followRotationComponent.reference = skeletonUtility.boneRoot;

            // Follower Kinematic Rigidbody
            var rootFollowerKinematic = new GameObject(kinematicParentUtilityBone.name + " Follower");
            rootFollowerKinematic.transform.parent = chainParentObject.transform;
            var followerRigidbody = rootFollowerKinematic.AddComponent<Rigidbody>();
            followerRigidbody.mass = mass;
            followerRigidbody.isKinematic = true;
            rootFollowerKinematic.AddComponent<FollowLocationRigidbody>().reference =
                kinematicParentUtilityBone.transform;
            rootFollowerKinematic.transform.position = kinematicParentUtilityBone.transform.position;
            rootFollowerKinematic.transform.rotation = kinematicParentUtilityBone.transform.rotation;

            CreateHingeChain(utilityBone, mass, rotationLimit, chainParentObject.transform,
                rootFollowerKinematic.transform);

            Selection.activeGameObject = chainParentObject;
        }

        private void CreateHingeChain(SkeletonUtilityBone bone, float mass, float rotationLimit, Transform groupObject,
            Transform jointParent)
        {
            mass *= 0.75f;

            bone.parentReference = jointParent;
            bone.transform.SetParent(groupObject.transform,
                true); // we need a flat hierarchy of all Joint objects in Unity.
            AttachRigidbodyAndCollider(bone);
            bone.mode = SkeletonUtilityBone.Mode.Override;

            var joint = bone.gameObject.AddComponent<HingeJoint>();
            joint.axis = Vector3.forward;
            joint.connectedBody = jointParent.GetComponent<Rigidbody>();
            joint.useLimits = true;
            joint.limits = new JointLimits
            {
                min = -rotationLimit,
                max = rotationLimit
            };
            bone.GetComponent<Rigidbody>().mass = mass;

            var parent = bone.transform;
            var children = new List<SkeletonUtilityBone>();
            var utilityChildCount = 0;
            for (var i = 0; i < parent.childCount; ++i)
            {
                var childUtilityBone = parent.GetChild(i).GetComponent<SkeletonUtilityBone>();
                if (childUtilityBone != null)
                    children.Add(childUtilityBone);
            }

            mass /= Mathf.Max(1.0f, utilityChildCount);

            for (var i = 0; i < children.Count; ++i)
            {
                var childBone = children[i];
                if (childBone == null) continue;
                CreateHingeChain(childBone, mass, rotationLimit, groupObject, parent);
            }
        }

        private void SetSkeletonUtilityToFlipByRotation()
        {
            if (!skeletonUtility.flipBy180DegreeRotation)
            {
                skeletonUtility.flipBy180DegreeRotation = true;
                Debug.Log(
                    "Set SkeletonUtility " + skeletonUtility.name +
                    " to flip by rotation instead of negative scale (required).", skeletonUtility);
            }
        }

        private static void AttachRigidbodyAndCollider(SkeletonUtilityBone utilBone, bool enableCollider = false)
        {
            if (utilBone.GetComponent<Collider>() == null)
            {
                if (utilBone.bone.Data.Length == 0)
                {
                    var sphere = utilBone.gameObject.AddComponent<SphereCollider>();
                    sphere.radius = 0.1f;
                    sphere.enabled = enableCollider;
                }
                else
                {
                    var length = utilBone.bone.Data.Length;
                    var box = utilBone.gameObject.AddComponent<BoxCollider>();
                    box.size = new Vector3(length, length / 3f, 0.2f);
                    box.center = new Vector3(length / 2f, 0, 0);
                    box.enabled = enableCollider;
                }
            }

            utilBone.gameObject.AddComponent<Rigidbody>();
        }

        private static void AttachRigidbodyAndCollider2D(SkeletonUtilityBone utilBone, bool enableCollider = false)
        {
            if (utilBone.GetComponent<Collider2D>() == null)
            {
                if (utilBone.bone.Data.Length == 0)
                {
                    var sphere = utilBone.gameObject.AddComponent<CircleCollider2D>();
                    sphere.radius = 0.1f;
                    sphere.enabled = enableCollider;
                }
                else
                {
                    var length = utilBone.bone.Data.Length;
                    var box = utilBone.gameObject.AddComponent<BoxCollider2D>();
                    box.size = new Vector3(length, length / 3f, 0.2f);
                    box.offset = new Vector3(length / 2f, 0, 0);
                    box.enabled = enableCollider;
                }
            }

            utilBone.gameObject.AddComponent<Rigidbody2D>();
        }
    }
}