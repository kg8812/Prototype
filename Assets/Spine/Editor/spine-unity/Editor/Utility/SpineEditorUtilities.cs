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

#pragma warning disable 0219
#pragma warning disable 0618 // for 3.7 branch only. Avoids "PreferenceItem' is obsolete: '[PreferenceItem] is deprecated. Use [SettingsProvider] instead."

// Original contribution by: Mitch Thompson

#define SPINE_SKELETONMECANIM

#if UNITY_2017_2_OR_NEWER
#define NEWPLAYMODECALLBACKS
#endif

#if UNITY_2018 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEWHIERARCHYWINDOWCALLBACKS
#endif

#if UNITY_2018_3_OR_NEWER
#define NEW_PREFERENCES_SETTINGS_PROVIDER
#endif

#if UNITY_2017_1_OR_NEWER
#define BUILT_IN_SPRITE_MASK_COMPONENT
#endif

#if UNITY_2020_2_OR_NEWER
#define HAS_ON_POSTPROCESS_PREFAB
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spine.Unity.Editor
{
    // Analysis disable once ConvertToStaticType
    [InitializeOnLoad]
    public partial class SpineEditorUtilities : AssetPostprocessor
    {
        public static string editorPath = "";
        public static string editorGUIPath = "";
        public static bool initialized;
        private static readonly List<string> texturesWithoutMetaFile = new();

        // Auto-import post process entry point for all assets
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved,
            string[] movedFromAssetPaths)
        {
            if (imported.Length == 0)
                return;

            // we copy the list here to prevent nested calls to OnPostprocessAllAssets() triggering a Clear() of the list
            // in the middle of execution.
            var texturesWithoutMetaFileCopy = new List<string>(texturesWithoutMetaFile);
            AssetUtility.HandleOnPostprocessAllAssets(imported, texturesWithoutMetaFileCopy);
            texturesWithoutMetaFile.Clear();
        }

        // Auto-import entry point for textures
        private void OnPreprocessTexture()
        {
#if UNITY_2018_1_OR_NEWER
            var customTextureSettingsExist = !assetImporter.importSettingsMissing;
#else
			bool customTextureSettingsExist = System.IO.File.Exists(assetImporter.assetPath + ".meta");
#endif
            if (!customTextureSettingsExist) texturesWithoutMetaFile.Add(assetImporter.assetPath);
        }

        public static void OnTextureImportedFirstTime(string texturePath)
        {
            texturesWithoutMetaFile.Add(texturePath);
        }

        public static class HierarchyHandler
        {
            private static readonly Dictionary<int, GameObject> skeletonRendererTable = new();
            private static readonly Dictionary<int, SkeletonUtilityBone> skeletonUtilityBoneTable = new();
            private static readonly Dictionary<int, BoundingBoxFollower> boundingBoxFollowerTable = new();
            private static readonly Dictionary<int, BoundingBoxFollowerGraphic> boundingBoxFollowerGraphicTable = new();

#if NEWPLAYMODECALLBACKS
            internal static void IconsOnPlaymodeStateChanged(PlayModeStateChange stateChange)
            {
#else
			internal static void IconsOnPlaymodeStateChanged () {
#endif
                skeletonRendererTable.Clear();
                skeletonUtilityBoneTable.Clear();
                boundingBoxFollowerTable.Clear();
                boundingBoxFollowerGraphicTable.Clear();

#if NEWHIERARCHYWINDOWCALLBACKS
                EditorApplication.hierarchyChanged -= IconsOnChanged;
#else
				EditorApplication.hierarchyWindowChanged -= IconsOnChanged;
#endif
                EditorApplication.hierarchyWindowItemOnGUI -= IconsOnGUI;

                if (!Application.isPlaying && Preferences.showHierarchyIcons)
                {
#if NEWHIERARCHYWINDOWCALLBACKS
                    EditorApplication.hierarchyChanged += IconsOnChanged;
#else
					EditorApplication.hierarchyWindowChanged += IconsOnChanged;
#endif
                    EditorApplication.hierarchyWindowItemOnGUI += IconsOnGUI;
                    IconsOnChanged();
                }
            }

            internal static void IconsOnChanged()
            {
                skeletonRendererTable.Clear();
                skeletonUtilityBoneTable.Clear();
                boundingBoxFollowerTable.Clear();
                boundingBoxFollowerGraphicTable.Clear();

                var arr = Object.FindObjectsOfType<SkeletonRenderer>();
                foreach (var r in arr)
                    skeletonRendererTable[r.gameObject.GetInstanceID()] = r.gameObject;

                var boneArr = Object.FindObjectsOfType<SkeletonUtilityBone>();
                foreach (var b in boneArr)
                    skeletonUtilityBoneTable[b.gameObject.GetInstanceID()] = b;

                var bbfArr = Object.FindObjectsOfType<BoundingBoxFollower>();
                foreach (var bbf in bbfArr)
                    boundingBoxFollowerTable[bbf.gameObject.GetInstanceID()] = bbf;

                var bbfgArr = Object.FindObjectsOfType<BoundingBoxFollowerGraphic>();
                foreach (var bbf in bbfgArr)
                    boundingBoxFollowerGraphicTable[bbf.gameObject.GetInstanceID()] = bbf;
            }

            internal static void IconsOnGUI(int instanceId, Rect selectionRect)
            {
                var r = new Rect(selectionRect);
                if (skeletonRendererTable.ContainsKey(instanceId))
                {
                    r.x = r.width - 15;
                    r.width = 15;
                    GUI.Label(r, Icons.spine);
                }
                else if (skeletonUtilityBoneTable.ContainsKey(instanceId))
                {
                    r.x -= 26;
                    if (skeletonUtilityBoneTable[instanceId] != null)
                    {
                        if (skeletonUtilityBoneTable[instanceId].transform.childCount == 0)
                            r.x += 13;
                        r.y += 2;
                        r.width = 13;
                        r.height = 13;
                        if (skeletonUtilityBoneTable[instanceId].mode == SkeletonUtilityBone.Mode.Follow)
                            GUI.DrawTexture(r, Icons.bone);
                        else
                            GUI.DrawTexture(r, Icons.poseBones);
                    }
                }
                else if (boundingBoxFollowerTable.ContainsKey(instanceId))
                {
                    r.x -= 26;
                    if (boundingBoxFollowerTable[instanceId] != null)
                    {
                        if (boundingBoxFollowerTable[instanceId].transform.childCount == 0)
                            r.x += 13;
                        r.y += 2;
                        r.width = 13;
                        r.height = 13;
                        GUI.DrawTexture(r, Icons.boundingBox);
                    }
                }
                else if (boundingBoxFollowerGraphicTable.ContainsKey(instanceId))
                {
                    r.x -= 26;
                    if (boundingBoxFollowerGraphicTable[instanceId] != null)
                    {
                        if (boundingBoxFollowerGraphicTable[instanceId].transform.childCount == 0)
                            r.x += 13;
                        r.y += 2;
                        r.width = 13;
                        r.height = 13;
                        GUI.DrawTexture(r, Icons.boundingBox);
                    }
                }
            }

#if UNITY_2021_2_OR_NEWER
            internal static DragAndDropVisualMode HandleDragAndDrop(int dropTargetInstanceID,
                HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
            {
                var skeletonDataAsset = DragAndDrop.objectReferences.Length == 0
                    ? null
                    : DragAndDrop.objectReferences[0] as SkeletonDataAsset;
                if (skeletonDataAsset == null)
                    return DragAndDropVisualMode.None;
                if (!perform)
                    return DragAndDropVisualMode.Copy;

                var dropTargetObject = EditorUtility.InstanceIDToObject(dropTargetInstanceID) as GameObject;
                var dropTarget = dropTargetObject != null ? dropTargetObject.transform : null;
                var parent = dropTarget;
                var siblingIndex = 0;
                if (parent != null)
                {
                    if (dropMode == HierarchyDropFlags.DropBetween)
                    {
                        parent = dropTarget.parent;
                        siblingIndex = dropTarget ? dropTarget.GetSiblingIndex() + 1 : 0;
                    }
                    else if (dropMode == HierarchyDropFlags.DropAbove)
                    {
                        parent = dropTarget.parent;
                        siblingIndex = dropTarget ? dropTarget.GetSiblingIndex() : 0;
                    }
                }

                DragAndDropInstantiation.ShowInstantiateContextMenu(skeletonDataAsset, Vector3.zero, parent,
                    siblingIndex);
                return DragAndDropVisualMode.Copy;
            }
#else
			internal static void HandleDragAndDrop (int instanceId, Rect selectionRect) {
				// HACK: Uses EditorApplication.hierarchyWindowItemOnGUI.
				// Only works when there is at least one item in the scene.
				UnityEngine.Event current = UnityEngine.Event.current;
				EventType eventType = current.type;
				bool isDraggingEvent = eventType == EventType.DragUpdated;
				bool isDropEvent = eventType == EventType.DragPerform;
				UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (isDraggingEvent || isDropEvent) {
					EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
					if (mouseOverWindow != null) {

						// One, existing, valid SkeletonDataAsset
						Object[] references = UnityEditor.DragAndDrop.objectReferences;
						if (references.Length == 1) {
							SkeletonDataAsset skeletonDataAsset = references[0] as SkeletonDataAsset;
							if (skeletonDataAsset != null && skeletonDataAsset.GetSkeletonData(true) != null) {

								// Allow drag-and-dropping anywhere in the Hierarchy Window.
								// HACK: string-compare because we can't get its type via reflection.
								const string HierarchyWindow = "UnityEditor.SceneHierarchyWindow";
								const string GenericDataTargetID = "target";
								if (HierarchyWindow.Equals(mouseOverWindow.GetType().ToString(), System.StringComparison.Ordinal)) {
									if (isDraggingEvent) {
										UnityEngine.Object mouseOverTarget =
 UnityEditor.EditorUtility.InstanceIDToObject(instanceId);
										if (mouseOverTarget)
											DragAndDrop.SetGenericData(GenericDataTargetID, mouseOverTarget);
										// Note: do not call current.Use(), otherwise we get the wrong drop-target parent.
									} else if (isDropEvent) {
										GameObject parentGameObject =
 DragAndDrop.GetGenericData(GenericDataTargetID) as UnityEngine.GameObject;
										Transform parent = parentGameObject != null ? parentGameObject.transform : null;
										// when dragging into empty space in hierarchy below last node, last node would be parent.
										if (IsLastNodeInHierarchy(parent))
											parent = null;
										DragAndDropInstantiation.ShowInstantiateContextMenu(skeletonDataAsset, Vector3.zero, parent, 0);
										UnityEditor.DragAndDrop.AcceptDrag();
										current.Use();
										return;
									}
								}
							}
						}
					}
				}
			}

			internal static bool IsLastNodeInHierarchy (Transform node) {
				if (node == null)
					return false;

				while (node.parent != null) {
					if (node.GetSiblingIndex() != node.parent.childCount - 1)
						return false;
					node = node.parent;
				}

				GameObject[] rootNodes = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
				bool isLastNode = (rootNodes.Length > 0 && rootNodes[rootNodes.Length - 1].transform == node);
				return isLastNode;
			}
#endif
        }

#if HAS_ON_POSTPROCESS_PREFAB
        // Post process prefabs for setting the MeshFilter to not cause constant Prefab override changes.
        private void OnPostprocessPrefab(GameObject g)
        {
            if (SpineBuildProcessor.isBuilding)
                return;

            SetupSpinePrefabMesh(g, context);
        }

        public static bool SetupSpinePrefabMesh(GameObject g, AssetImportContext context)
        {
            var nameUsageCount = new Dictionary<string, int>();
            var wasModified = false;
            var skeletonRenderers = g.GetComponentsInChildren<SkeletonRenderer>(true);
            foreach (var renderer in skeletonRenderers)
            {
                wasModified = true;
                var meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = renderer.gameObject.AddComponent<MeshFilter>();

                renderer.EditorUpdateMeshFilterHideFlags();
                renderer.Initialize(true, true);
                renderer.LateUpdateMesh();
                var mesh = meshFilter.sharedMesh;
                if (mesh == null) continue;

                var meshName = string.Format("Skeleton Prefab Mesh [{0}]", renderer.name);
                if (nameUsageCount.ContainsKey(meshName))
                {
                    nameUsageCount[meshName]++;
                    meshName = string.Format("Skeleton Prefab Mesh [{0} ({1})]", renderer.name,
                        nameUsageCount[meshName]);
                }
                else
                {
                    nameUsageCount.Add(meshName, 0);
                }

                mesh.name = meshName;
                mesh.hideFlags = HideFlags.None;
                if (context != null)
                    context.AddObjectToAsset(meshFilter.sharedMesh.name, meshFilter.sharedMesh);
            }

            return wasModified;
        }

        public static bool CleanupSpinePrefabMesh(GameObject g)
        {
            var wasModified = false;
            var skeletonRenderers = g.GetComponentsInChildren<SkeletonRenderer>(true);
            foreach (var renderer in skeletonRenderers)
            {
                var meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter != null)
                    if (meshFilter.sharedMesh)
                    {
                        wasModified = true;
                        meshFilter.sharedMesh = null;
                        meshFilter.hideFlags = HideFlags.None;
                    }
            }

            return wasModified;
        }
#endif

        #region Initialization

        static SpineEditorUtilities()
        {
            EditorApplication.delayCall += Initialize; // delayed so that AssetDatabase is ready.
        }

        private static void Initialize()
        {
            // Note: Preferences need to be loaded when changing play mode
            // to initialize handle scale correctly.
#if !NEW_PREFERENCES_SETTINGS_PROVIDER
			Preferences.Load();
#else
            SpinePreferences.Load();
#endif

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            string[] assets;
            string assetPath;
            assets = AssetDatabase.FindAssets("t:texture icon-subMeshRenderer", null);
            if (assets.Length > 0)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                editorGUIPath = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            }

            assets = AssetDatabase.FindAssets("t:script SpineEditorUtilities", null);
            if (assets.Length > 0)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                editorPath = Path.GetDirectoryName(assetPath).Replace('\\', '/');
                if (string.IsNullOrEmpty(editorGUIPath))
                    editorGUIPath = editorPath.Replace("/Utility", "/GUI");
            }

            if (string.IsNullOrEmpty(editorGUIPath))
                return;
            Icons.Initialize();

            // Drag and Drop
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= DragAndDropInstantiation.SceneViewDragAndDrop;
            SceneView.duringSceneGui += DragAndDropInstantiation.SceneViewDragAndDrop;
#else
			SceneView.onSceneGUIDelegate -= DragAndDropInstantiation.SceneViewDragAndDrop;
			SceneView.onSceneGUIDelegate += DragAndDropInstantiation.SceneViewDragAndDrop;
#endif

#if UNITY_2021_2_OR_NEWER
            DragAndDrop.RemoveDropHandler(HierarchyHandler.HandleDragAndDrop);
            DragAndDrop.AddDropHandler(HierarchyHandler.HandleDragAndDrop);
#else
			EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHandler.HandleDragAndDrop;
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyHandler.HandleDragAndDrop;
#endif
            // Hierarchy Icons
#if NEWPLAYMODECALLBACKS
            EditorApplication.playModeStateChanged -= HierarchyHandler.IconsOnPlaymodeStateChanged;
            EditorApplication.playModeStateChanged += HierarchyHandler.IconsOnPlaymodeStateChanged;
            HierarchyHandler.IconsOnPlaymodeStateChanged(PlayModeStateChange.EnteredEditMode);
#else
			EditorApplication.playmodeStateChanged -= HierarchyHandler.IconsOnPlaymodeStateChanged;
			EditorApplication.playmodeStateChanged += HierarchyHandler.IconsOnPlaymodeStateChanged;
			HierarchyHandler.IconsOnPlaymodeStateChanged();
#endif

            // Data Refresh Edit Mode.
            // This prevents deserialized SkeletonData from persisting from play mode to edit mode.
#if NEWPLAYMODECALLBACKS
            EditorApplication.playModeStateChanged -= DataReloadHandler.OnPlaymodeStateChanged;
            EditorApplication.playModeStateChanged += DataReloadHandler.OnPlaymodeStateChanged;
            DataReloadHandler.OnPlaymodeStateChanged(PlayModeStateChange.EnteredEditMode);
#else
			EditorApplication.playmodeStateChanged -= DataReloadHandler.OnPlaymodeStateChanged;
			EditorApplication.playmodeStateChanged += DataReloadHandler.OnPlaymodeStateChanged;
			DataReloadHandler.OnPlaymodeStateChanged();
#endif

            if (Preferences.textureImporterWarning) IssueWarningsForUnrecommendedTextureSettings();

            initialized = true;
        }

        public static void ConfirmInitialization()
        {
            if (!initialized)
                Initialize();
        }

        public static void IssueWarningsForUnrecommendedTextureSettings()
        {
            var atlasDescriptionGUIDs =
                AssetDatabase.FindAssets(
                    "t:textasset .atlas"); // Note: finds ".atlas.txt" but also ".atlas 1.txt" files.
            for (var i = 0; i < atlasDescriptionGUIDs.Length; ++i)
            {
                var atlasDescriptionPath = AssetDatabase.GUIDToAssetPath(atlasDescriptionGUIDs[i]);
                if (!atlasDescriptionPath.EndsWith(".atlas.txt"))
                    continue;

                var texturePath = atlasDescriptionPath.Replace(".atlas.txt", ".png");

                var textureExists = IssueWarningsForUnrecommendedTextureSettings(texturePath);
                if (!textureExists)
                {
                    texturePath = texturePath.Replace(".png", ".jpg");
                    textureExists = IssueWarningsForUnrecommendedTextureSettings(texturePath);
                }

                if (!textureExists)
                {
                }
            }
        }

        public static void ReloadSkeletonDataAssetAndComponent(SkeletonRenderer component)
        {
            if (component == null) return;
            ReloadSkeletonDataAsset(component.skeletonDataAsset);
            ReinitializeComponent(component);
        }

        public static void ReloadSkeletonDataAssetAndComponent(SkeletonGraphic component)
        {
            if (component == null) return;
            ReloadSkeletonDataAsset(component.skeletonDataAsset);
            // Reinitialize.
            ReinitializeComponent(component);
        }

        public static void ClearSkeletonDataAsset(SkeletonDataAsset skeletonDataAsset)
        {
            skeletonDataAsset.Clear();
            DataReloadHandler.ClearAnimationReferenceAssets(skeletonDataAsset);
        }

        public static void ReloadSkeletonDataAsset(SkeletonDataAsset skeletonDataAsset, bool clearAtlasAssets = true)
        {
            if (skeletonDataAsset == null)
                return;

            if (clearAtlasAssets)
                foreach (var aa in skeletonDataAsset.atlasAssets)
                    if (aa != null)
                        aa.Clear();

            ClearSkeletonDataAsset(skeletonDataAsset);
            skeletonDataAsset.GetSkeletonData(true);
            DataReloadHandler.ReloadAnimationReferenceAssets(skeletonDataAsset);
        }

        public static void ReinitializeComponent(SkeletonRenderer component)
        {
            if (component == null) return;
            if (!SkeletonDataAssetIsValid(component.SkeletonDataAsset)) return;

            var stateComponent = component as IAnimationStateComponent;
            AnimationState oldAnimationState = null;
            if (stateComponent != null) oldAnimationState = stateComponent.AnimationState;

            component.Initialize(true); // implicitly clears any subscribers

            if (oldAnimationState != null) stateComponent.AnimationState.AssignEventSubscribersFrom(oldAnimationState);
            if (stateComponent != null)
            {
                // Any set animation needs to be applied as well since it might set attachments,
                // having an effect on generated SpriteMaskMaterials below.
                stateComponent.AnimationState.Apply(component.skeleton);
                component.LateUpdate();
            }

#if BUILT_IN_SPRITE_MASK_COMPONENT
            SpineMaskUtilities.EditorAssignSpriteMaskMaterials(component);
#endif
            component.LateUpdate();
        }

        public static void ReinitializeComponent(SkeletonGraphic component)
        {
            if (component == null) return;
            if (!SkeletonDataAssetIsValid(component.SkeletonDataAsset)) return;
            component.Initialize(true);
            component.LateUpdate();
        }

        public static bool SkeletonDataAssetIsValid(SkeletonDataAsset asset)
        {
            return asset != null && asset.GetSkeletonData(true) != null;
        }

        public static bool IssueWarningsForUnrecommendedTextureSettings(string texturePath)
        {
            var texImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
            if (texImporter == null) return false;

            var extensionPos = texturePath.LastIndexOf('.');
            var materialPath = texturePath.Substring(0, extensionPos) + "_Material.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            if (material == null)
                return true;

            string errorMessage = null;
            if (MaterialChecks.IsTextureSetupProblematic(material, PlayerSettings.colorSpace,
                    texImporter.sRGBTexture, texImporter.mipmapEnabled, texImporter.alphaIsTransparency,
                    texturePath, materialPath, ref errorMessage))
                Debug.LogWarning(errorMessage, material);
            return true;
        }

        #endregion
    }

    public class SpineAssetModificationProcessor : AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string assetName)
        {
            // Note: This method seems to be called from the main thread,
            // not from worker threads when Project Settings - Editor - Parallel Import is enabled.
            var endIndex = assetName.LastIndexOf(".meta");
            var assetPath = endIndex < 0 ? assetName : assetName.Substring(0, endIndex);
            SpineEditorUtilities.OnTextureImportedFirstTime(assetPath);
        }
    }

    public class TextureModificationWarningProcessor : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            if (SpineEditorUtilities.Preferences.textureImporterWarning)
                foreach (var path in paths)
                    if (path != null &&
                        (path.EndsWith(".png.meta", StringComparison.Ordinal) ||
                         path.EndsWith(".jpg.meta", StringComparison.Ordinal)))
                    {
                        var texturePath = Path.ChangeExtension(path, null); // .meta removed
                        var atlasPath = Path.ChangeExtension(texturePath, "atlas.txt");
                        if (File.Exists(atlasPath))
                            SpineEditorUtilities.IssueWarningsForUnrecommendedTextureSettings(texturePath);
                    }

            return paths;
        }
    }

    public class AnimationWindowPreview
    {
        private static Type animationWindowType;

        public static Type AnimationWindowType
        {
            get
            {
                if (animationWindowType == null)
                    animationWindowType = Type.GetType("UnityEditor.AnimationWindow,UnityEditor");
                return animationWindowType;
            }
        }

        public static Object GetOpenAnimationWindow()
        {
            var openAnimationWindows = Resources.FindObjectsOfTypeAll(AnimationWindowType);
            return openAnimationWindows.Length == 0 ? null : openAnimationWindows[0];
        }

        public static AnimationClip GetAnimationClip(Object animationWindow)
        {
            if (animationWindow == null)
                return null;

            const BindingFlags bindingFlagsInstance =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var animEditorField = AnimationWindowType.GetField("m_AnimEditor", bindingFlagsInstance);

            var selectionProperty = animEditorField.FieldType.GetProperty("selection", bindingFlagsInstance);
            var animEditor = animEditorField.GetValue(animationWindow);
            if (animEditor == null) return null;
            var selection = selectionProperty.GetValue(animEditor, null);
            if (selection == null) return null;

            var animationClipProperty = selection.GetType().GetProperty("animationClip");
            return animationClipProperty.GetValue(selection, null) as AnimationClip;
        }

        public static float GetAnimationTime(Object animationWindow)
        {
            if (animationWindow == null)
                return 0.0f;

            const BindingFlags bindingFlagsInstance =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var animEditorField = AnimationWindowType.GetField("m_AnimEditor", bindingFlagsInstance);
            var animEditor = animEditorField.GetValue(animationWindow);

            var animEditorFieldType = animEditorField.FieldType;
            var stateProperty = animEditorFieldType.GetProperty("state", bindingFlagsInstance);
            var animWindowStateType = stateProperty.PropertyType;
            var timeProperty = animWindowStateType.GetProperty("currentTime", bindingFlagsInstance);

            var state = stateProperty.GetValue(animEditor, null);
            return (float)timeProperty.GetValue(state, null);
        }
    }
}