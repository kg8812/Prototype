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

#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif

#if UNITY_2018_2_OR_NEWER
#define HAS_CULL_TRANSPARENT_MESH
#endif

#define SPINE_OPTIONAL_ON_DEMAND_LOADING

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spine.Unity
{
#if NEW_PREFAB_SYSTEM
    [ExecuteAlways]
#else
	[ExecuteInEditMode]
#endif
    [RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Spine/SkeletonGraphic (Unity UI Canvas)")]
    [HelpURL("http://esotericsoftware.com/spine-unity#SkeletonGraphic-Component")]
    public class SkeletonGraphic : MaskableGraphic, ISkeletonComponent, IAnimationStateComponent, ISkeletonAnimation,
        IHasSkeletonDataAsset
    {
        #region Inspector

        public SkeletonDataAsset skeletonDataAsset;
        public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

        public Material additiveMaterial;
        public Material multiplyMaterial;
        public Material screenMaterial;

        /// <summary>Own color to replace <c>Graphic.m_Color</c>.</summary>
        [FormerlySerializedAs("m_Color")] [SerializeField]
        protected Color m_SkeletonColor = Color.white;

        /// <summary>
        ///     Sets the color of the skeleton. Does not call <see cref="Rebuild" /> and <see cref="UpdateMesh" />
        ///     unnecessarily as <c>Graphic.color</c> would otherwise do.
        /// </summary>
        public override Color color
        {
            get => m_SkeletonColor;
            set => m_SkeletonColor = value;
        }

        [SpineSkin(dataField: "skeletonDataAsset", defaultAsEmptyString: true)]
        public string initialSkinName;

        public bool initialFlipX, initialFlipY;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string startingAnimation;

        public bool startingLoop;
        public float timeScale = 1f;
        public bool freeze;
        protected float meshScale = 1f;
        protected Vector2 meshOffset = Vector2.zero;
        public float MeshScale => meshScale;
        public Vector2 MeshOffset => meshOffset;

        public enum LayoutMode
        {
            None = 0,
            WidthControlsHeight,
            HeightControlsWidth,
            FitInParent,
            EnvelopeParent
        }

        public LayoutMode layoutScaleMode = LayoutMode.None;
        [SerializeField] protected Vector2 referenceSize = Vector2.one;

        /// <summary>Offset relative to the pivot position, before potential layout scale is applied.</summary>
        [SerializeField] protected Vector2 pivotOffset = Vector2.zero;

        [SerializeField] protected float referenceScale = 1f;
        [SerializeField] protected float layoutScale = 1f;
#if UNITY_EDITOR
        protected LayoutMode previousLayoutScaleMode = LayoutMode.None;
        [SerializeField] protected Vector2 rectTransformSize = Vector2.zero;
        [SerializeField] protected bool editReferenceRect;
        protected bool previousEditReferenceRect;

        public bool EditReferenceRect
        {
            get => editReferenceRect;
            set => editReferenceRect = value;
        }

        public Vector2 RectTransformSize => rectTransformSize;
#else
		protected const bool EditReferenceRect = false;
#endif
        /// <summary>Update mode to optionally limit updates to e.g. only apply animations but not update the mesh.</summary>
        public UpdateMode UpdateMode
        {
            get => updateMode;
            set => updateMode = value;
        }

        protected UpdateMode updateMode = UpdateMode.FullUpdate;

        /// <summary>
        ///     Update mode used when the MeshRenderer becomes invisible
        ///     (when <c>OnBecameInvisible()</c> is called). Update mode is automatically
        ///     reset to <c>UpdateMode.FullUpdate</c> when the mesh becomes visible again.
        /// </summary>
        public UpdateMode updateWhenInvisible = UpdateMode.FullUpdate;

        public bool allowMultipleCanvasRenderers;
        public List<CanvasRenderer> canvasRenderers = new();
        protected List<SkeletonSubmeshGraphic> submeshGraphics = new();
        protected int usedRenderersCount;

        // Submesh Separation
        public const string SeparatorPartGameObjectName = "Part";

        /// <summary>
        ///     Slot names used to populate separatorSlots list when the Skeleton is initialized. Changing this after
        ///     initialization does nothing.
        /// </summary>
        [SerializeField] [SpineSlot] protected string[] separatorSlotNames = new string[0];

        /// <summary>
        ///     Slots that determine where the render is split. This is used by components such as SkeletonRenderSeparator so
        ///     that the skeleton can be rendered by two separate renderers on different GameObjects.
        /// </summary>
        [NonSerialized] public readonly List<Slot> separatorSlots = new();

        public bool enableSeparatorSlots;
        [SerializeField] protected List<Transform> separatorParts = new();
        public List<Transform> SeparatorParts => separatorParts;
        public bool updateSeparatorPartLocation = true;
        public bool updateSeparatorPartScale;

        private bool wasUpdatedAfterInit = true;
        private Texture baseTexture;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            // This handles Scene View preview.
            base.OnValidate();
            if (IsValid)
            {
                if (skeletonDataAsset == null)
                {
                    Clear();
                }
                else if (skeletonDataAsset.skeletonJSON == null)
                {
                    Clear();
                }
                else if (skeletonDataAsset.GetSkeletonData(true) != skeleton.Data)
                {
                    Clear();
                    Initialize(true);
                    if (!allowMultipleCanvasRenderers && (skeletonDataAsset.atlasAssets.Length > 1 ||
                                                          skeletonDataAsset.atlasAssets[0].MaterialCount > 1))
                        Debug.LogError(
                            "Unity UI does not support multiple textures per Renderer. Please enable 'Advanced - Multiple CanvasRenderers' to generate the required CanvasRenderer GameObjects. Otherwise your skeleton will not be rendered correctly.",
                            this);
                }
                else
                {
                    if (freeze) return;

                    if (!Application.isPlaying)
                    {
                        Initialize(true);
                        return;
                    }

                    if (!string.IsNullOrEmpty(initialSkinName))
                    {
                        var skin = skeleton.Data.FindSkin(initialSkinName);
                        if (skin != null)
                        {
                            if (skin == skeleton.Data.DefaultSkin)
                                skeleton.SetSkin((Skin)null);
                            else
                                skeleton.SetSkin(skin);
                        }
                    }
                }
            }
            else
            {
                // Under some circumstances (e.g. sometimes on the first import) OnValidate is called
                // before SpineEditorUtilities.ImportSpineContent, causing an unnecessary exception.
                // The (skeletonDataAsset.skeletonJSON != null) condition serves to prevent this exception.
                if (skeletonDataAsset != null && skeletonDataAsset.skeletonJSON != null)
                    Initialize(true);
            }
        }

        protected override void Reset()
        {
            base.Reset();
            if (material == null || material.shader != Shader.Find("Spine/SkeletonGraphic"))
                Debug.LogWarning("SkeletonGraphic works best with the SkeletonGraphic material.");
        }
#endif

        #endregion

        #region Runtime Instantiation

        /// <summary>Create a new GameObject with a SkeletonGraphic component.</summary>
        /// <param name="material">
        ///     Material for the canvas renderer to use. Usually, the default SkeletonGraphic material will
        ///     work.
        /// </param>
        public static SkeletonGraphic NewSkeletonGraphicGameObject(SkeletonDataAsset skeletonDataAsset,
            Transform parent, Material material)
        {
            var sg = AddSkeletonGraphicComponent(new GameObject("New Spine GameObject"), skeletonDataAsset, material);
            if (parent != null) sg.transform.SetParent(parent, false);
            return sg;
        }

        /// <summary>Add a SkeletonGraphic component to a GameObject.</summary>
        /// <param name="material">
        ///     Material for the canvas renderer to use. Usually, the default SkeletonGraphic material will
        ///     work.
        /// </param>
        public static SkeletonGraphic AddSkeletonGraphicComponent(GameObject gameObject,
            SkeletonDataAsset skeletonDataAsset, Material material)
        {
            var skeletonGraphic = gameObject.AddComponent<SkeletonGraphic>();
            if (skeletonDataAsset != null)
            {
                skeletonGraphic.material = material;
                skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
                skeletonGraphic.Initialize(false);
            }
#if HAS_CULL_TRANSPARENT_MESH
            var canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
            if (canvasRenderer) canvasRenderer.cullTransparentMesh = false;
#endif
            return skeletonGraphic;
        }

        #endregion

        #region Overrides

        // API for taking over rendering.
        /// <summary>
        ///     When true, no meshes and materials are assigned at CanvasRenderers if the used override
        ///     AssignMeshOverrideSingleRenderer or AssignMeshOverrideMultipleRenderers is non-null.
        /// </summary>
        public bool disableMeshAssignmentOnOverride = true;

        /// <summary>
        ///     Delegate type for overriding mesh and material assignment,
        ///     used when <c>allowMultipleCanvasRenderers</c> is false.
        /// </summary>
        /// <param name="mesh">Mesh normally assigned at the main CanvasRenderer.</param>
        /// <param name="graphicMaterial">Material normally assigned at the main CanvasRenderer.</param>
        /// <param name="texture">Texture normally assigned at the main CanvasRenderer.</param>
        public delegate void MeshAssignmentDelegateSingle(Mesh mesh, Material graphicMaterial, Texture texture);

        /// <param name="meshCount">
        ///     Number of meshes. Don't use <c>meshes.Length</c> as this might be higher
        ///     due to pre-allocated entries.
        /// </param>
        /// <param name="meshes">Mesh array where each element is normally assigned to one of the <c>canvasRenderers</c>.</param>
        /// <param name="graphicMaterials">
        ///     Material array where each element is normally assigned to one of the
        ///     <c>canvasRenderers</c>.
        /// </param>
        /// <param name="textures">Texture array where each element is normally assigned to one of the <c>canvasRenderers</c>.</param>
        public delegate void MeshAssignmentDelegateMultiple(int meshCount, Mesh[] meshes, Material[] graphicMaterials,
            Texture[] textures);

        private event MeshAssignmentDelegateSingle assignMeshOverrideSingle;
        private event MeshAssignmentDelegateMultiple assignMeshOverrideMultiple;

        /// <summary>
        ///     Allows separate code to take over mesh and material assignment for this SkeletonGraphic component.
        ///     Used when <c>allowMultipleCanvasRenderers</c> is false.
        /// </summary>
        public event MeshAssignmentDelegateSingle AssignMeshOverrideSingleRenderer
        {
            add
            {
                assignMeshOverrideSingle += value;
                if (disableMeshAssignmentOnOverride && assignMeshOverrideSingle != null) Initialize(false);
            }
            remove
            {
                assignMeshOverrideSingle -= value;
                if (disableMeshAssignmentOnOverride && assignMeshOverrideSingle == null) Initialize(false);
            }
        }

        /// <summary>
        ///     Allows separate code to take over mesh and material assignment for this SkeletonGraphic component.
        ///     Used when <c>allowMultipleCanvasRenderers</c> is true.
        /// </summary>
        public event MeshAssignmentDelegateMultiple AssignMeshOverrideMultipleRenderers
        {
            add
            {
                assignMeshOverrideMultiple += value;
                if (disableMeshAssignmentOnOverride && assignMeshOverrideMultiple != null) Initialize(false);
            }
            remove
            {
                assignMeshOverrideMultiple -= value;
                if (disableMeshAssignmentOnOverride && assignMeshOverrideMultiple == null) Initialize(false);
            }
        }


        /// <summary>Use this Dictionary to override a Texture with a different Texture.</summary>
        [field: NonSerialized]
        public Dictionary<Texture, Texture> CustomTextureOverride { get; } = new();

        /// <summary>Use this Dictionary to override the Material where the Texture was used at the original atlas.</summary>
        [field: NonSerialized]
        public Dictionary<Texture, Material> CustomMaterialOverride { get; } = new();

        // This is used by the UI system to determine what to put in the MaterialPropertyBlock.
        private Texture overrideTexture;

        public Texture OverrideTexture
        {
            get => overrideTexture;
            set
            {
                overrideTexture = value;
                canvasRenderer.SetTexture(mainTexture); // Refresh canvasRenderer's texture. Make sure it handles null.
            }
        }

        #endregion

        #region Internals

        public override Texture mainTexture
        {
            get
            {
                if (overrideTexture != null) return overrideTexture;
                return baseTexture;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            onCullStateChanged.AddListener(OnCullStateChanged);

            SyncSubmeshGraphicsWithCanvasRenderers();
            if (!IsValid)
            {
#if UNITY_EDITOR
                // workaround for special import case of open scene where OnValidate and Awake are
                // called in wrong order, before setup of Spine assets.
                if (!Application.isPlaying)
                    if (skeletonDataAsset != null && skeletonDataAsset.skeletonJSON == null)
                        return;
#endif
                Initialize(false);
                if (IsValid) Rebuild(CanvasUpdate.PreRender);
            }

#if UNITY_EDITOR
            InitLayoutScaleParameters();
#endif
        }

        protected override void OnDestroy()
        {
            Clear();
            base.OnDestroy();
        }

        public override void Rebuild(CanvasUpdate update)
        {
            base.Rebuild(update);
            if (!IsValid) return;
            if (canvasRenderer.cull) return;
            if (update == CanvasUpdate.PreRender)
            {
                PrepareInstructionsAndRenderers(true);
                UpdateMeshToInstructions();
            }

            if (allowMultipleCanvasRenderers) canvasRenderer.Clear();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (var canvasRenderer in canvasRenderers) canvasRenderer.Clear();
        }

        public virtual void Update()
        {
#if UNITY_EDITOR
            UpdateReferenceRectSizes();
            if (!Application.isPlaying)
            {
                Update(0f);
                return;
            }
#endif
            if (freeze || updateTiming != UpdateTiming.InUpdate) return;
            Update(unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            if (freeze || updateTiming != UpdateTiming.InFixedUpdate) return;
            Update(unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        public virtual void Update(float deltaTime)
        {
            if (!IsValid) return;

            wasUpdatedAfterInit = true;
            if (updateMode < UpdateMode.OnlyAnimationStatus)
                return;
            UpdateAnimationStatus(deltaTime);

            if (updateMode == UpdateMode.OnlyAnimationStatus)
                return;
            ApplyAnimation();
        }

        protected void SyncSubmeshGraphicsWithCanvasRenderers()
        {
            submeshGraphics.Clear();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyOldRawImages();
#endif
            foreach (var canvasRenderer in canvasRenderers)
            {
                var submeshGraphic = canvasRenderer.GetComponent<SkeletonSubmeshGraphic>();
                if (submeshGraphic == null)
                {
                    submeshGraphic = canvasRenderer.gameObject.AddComponent<SkeletonSubmeshGraphic>();
                    submeshGraphic.maskable = maskable;
                    submeshGraphic.raycastTarget = false;
                }

                submeshGraphics.Add(submeshGraphic);
            }
        }

        protected void UpdateAnimationStatus(float deltaTime)
        {
            deltaTime *= timeScale;
            state.Update(deltaTime);
            skeleton.Update(deltaTime);

            ApplyTransformMovementToPhysics();

            if (updateMode == UpdateMode.OnlyAnimationStatus) state.ApplyEventTimelinesOnly(skeleton, false);
        }

        public virtual void ApplyTransformMovementToPhysics()
        {
            if (Application.isPlaying)
            {
                if (physicsPositionInheritanceFactor != Vector2.zero)
                {
                    var position = GetPhysicsTransformPosition();
                    var positionDelta = (position - lastPosition) / meshScale;

                    positionDelta = transform.InverseTransformVector(positionDelta);
                    if (physicsMovementRelativeTo != null)
                        positionDelta = physicsMovementRelativeTo.TransformVector(positionDelta);
                    positionDelta.x *= physicsPositionInheritanceFactor.x;
                    positionDelta.y *= physicsPositionInheritanceFactor.y;

                    skeleton.PhysicsTranslate(positionDelta.x, positionDelta.y);
                    lastPosition = position;
                }

                if (physicsRotationInheritanceFactor != 0f)
                {
                    var rotation = GetPhysicsTransformRotation();
                    skeleton.PhysicsRotate(0, 0, physicsRotationInheritanceFactor * (rotation - lastRotation));
                    lastRotation = rotation;
                }
            }
        }

        protected Vector2 GetPhysicsTransformPosition()
        {
            if (physicsMovementRelativeTo == null) return transform.position;

            if (physicsMovementRelativeTo == transform.parent)
                return transform.localPosition;
            return physicsMovementRelativeTo.InverseTransformPoint(transform.position);
        }

        protected float GetPhysicsTransformRotation()
        {
            if (physicsMovementRelativeTo == null) return transform.rotation.eulerAngles.z;

            if (physicsMovementRelativeTo == transform.parent)
                return transform.localRotation.eulerAngles.z;
            var relative = Quaternion.Inverse(physicsMovementRelativeTo.rotation) * transform.rotation;
            return relative.eulerAngles.z;
        }

        public virtual void ApplyAnimation()
        {
            if (BeforeApply != null)
                BeforeApply(this);

            if (updateMode != UpdateMode.OnlyEventTimelines)
                state.Apply(skeleton);
            else
                state.ApplyEventTimelinesOnly(skeleton);

            AfterAnimationApplied();
        }

        public virtual void AfterAnimationApplied()
        {
            if (UpdateLocal != null)
                UpdateLocal(this);

            if (UpdateWorld == null)
            {
                UpdateWorldTransform(Skeleton.Physics.Update);
            }
            else
            {
                UpdateWorldTransform(Skeleton.Physics.Pose);
                UpdateWorld(this);
                UpdateWorldTransform(Skeleton.Physics.Update);
            }

            if (UpdateComplete != null)
                UpdateComplete(this);
        }

        protected void UpdateWorldTransform(Skeleton.Physics physics)
        {
            skeleton.UpdateWorldTransform(physics);
        }

        public void LateUpdate()
        {
            if (!IsValid) return;
            // instantiation can happen from Update() after this component, leading to a missing Update() call.
            if (!wasUpdatedAfterInit) Update(0);
            if (freeze) return;
            if (updateMode != UpdateMode.FullUpdate) return;

            if (updateTiming == UpdateTiming.InLateUpdate)
                Update(unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);

            UpdateMesh();
        }

        protected void OnCullStateChanged(bool culled)
        {
            if (culled)
                OnBecameInvisible();
            else
                OnBecameVisible();
        }

        public void OnBecameVisible()
        {
            updateMode = UpdateMode.FullUpdate;
        }

        public void OnBecameInvisible()
        {
            updateMode = updateWhenInvisible;
        }

        public void ReapplySeparatorSlotNames()
        {
            if (!IsValid)
                return;

            separatorSlots.Clear();
            for (int i = 0, n = separatorSlotNames.Length; i < n; i++)
            {
                var slotName = separatorSlotNames[i];
                if (slotName == "")
                    continue;
                var slot = skeleton.FindSlot(slotName);
                if (slot != null)
                    separatorSlots.Add(slot);
#if UNITY_EDITOR
                else
                    Debug.LogWarning(slotName + " is not a slot in " + skeletonDataAsset.skeletonJSON.name);
#endif
            }

            UpdateSeparatorPartParents();
        }

        #endregion

        #region API

        protected Skeleton skeleton;

        public Skeleton Skeleton
        {
            get
            {
                Initialize(false);
                return skeleton;
            }
            set => skeleton = value;
        }

        public SkeletonData SkeletonData
        {
            get
            {
                Initialize(false);
                return skeleton == null ? null : skeleton.Data;
            }
        }

        public bool IsValid => skeleton != null;

        public delegate void SkeletonRendererDelegate(SkeletonGraphic skeletonGraphic);

        public delegate void InstructionDelegate(SkeletonRendererInstruction instruction);

        /// <summary>OnRebuild is raised after the Skeleton is successfully initialized.</summary>
        public event SkeletonRendererDelegate OnRebuild;

        /// <summary>
        ///     OnInstructionsPrepared is raised at the end of <c>LateUpdate</c> after render instructions
        ///     are done, target renderers are prepared, and the mesh is ready to be generated.
        /// </summary>
        public event InstructionDelegate OnInstructionsPrepared;

        /// <summary>
        ///     OnMeshAndMaterialsUpdated is raised at the end of <c>Rebuild</c> after the Mesh and
        ///     all materials have been updated. Note that some Unity API calls are not permitted to be issued from
        ///     <c>Rebuild</c>, so you may want to subscribe to <see cref="OnInstructionsPrepared" /> instead
        ///     from where you can issue such preparation calls.
        /// </summary>
        public event SkeletonRendererDelegate OnMeshAndMaterialsUpdated;

        protected AnimationState state;

        public AnimationState AnimationState
        {
            get
            {
                Initialize(false);
                return state;
            }
        }

        /// <seealso cref="PhysicsPositionInheritanceFactor" />
        [SerializeField] protected Vector2 physicsPositionInheritanceFactor = Vector2.one;

        /// <seealso cref="PhysicsRotationInheritanceFactor" />
        [SerializeField] protected float physicsRotationInheritanceFactor = 1.0f;

        /// <summary>Reference transform relative to which physics movement will be calculated, or null to use world location.</summary>
        [SerializeField] protected Transform physicsMovementRelativeTo;

        /// <summary>Used for applying Transform translation to skeleton PhysicsConstraints.</summary>
        protected Vector2 lastPosition;

        /// <summary>Used for applying Transform rotation to skeleton PhysicsConstraints.</summary>
        protected float lastRotation;

        /// <summary>
        ///     When set to non-zero, Transform position movement in X and Y direction
        ///     is applied to skeleton PhysicsConstraints, multiplied by this scale factor.
        ///     Typical values are <c>Vector2.one</c> to apply XY movement 1:1,
        ///     <c>Vector2(2f, 2f)</c> to apply movement with double intensity,
        ///     <c>Vector2(1f, 0f)</c> to apply only horizontal movement, or
        ///     <c>Vector2.zero</c> to not apply any Transform position movement at all.
        /// </summary>
        public Vector2 PhysicsPositionInheritanceFactor
        {
            get => physicsPositionInheritanceFactor;
            set
            {
                if (physicsPositionInheritanceFactor == Vector2.zero && value != Vector2.zero) ResetLastPosition();
                physicsPositionInheritanceFactor = value;
            }
        }

        /// <summary>
        ///     When set to non-zero, Transform rotation movement is applied to skeleton PhysicsConstraints,
        ///     multiplied by this scale factor. Typical values are <c>1</c> to apply movement 1:1,
        ///     <c>2</c> to apply movement with double intensity, or
        ///     <c>0</c> to not apply any Transform rotation movement at all.
        /// </summary>
        public float PhysicsRotationInheritanceFactor
        {
            get => physicsRotationInheritanceFactor;
            set
            {
                if (physicsRotationInheritanceFactor == 0f && value != 0f) ResetLastRotation();
                physicsRotationInheritanceFactor = value;
            }
        }

        /// <summary>Reference transform relative to which physics movement will be calculated, or null to use world location.</summary>
        public Transform PhysicsMovementRelativeTo
        {
            get => physicsMovementRelativeTo;
            set
            {
                physicsMovementRelativeTo = value;
                if (physicsPositionInheritanceFactor != Vector2.zero) ResetLastPosition();
                if (physicsRotationInheritanceFactor != 0f) ResetLastRotation();
            }
        }

        public void ResetLastPosition()
        {
            lastPosition = GetPhysicsTransformPosition();
        }

        public void ResetLastRotation()
        {
            lastRotation = GetPhysicsTransformRotation();
        }

        public void ResetLastPositionAndRotation()
        {
            lastPosition = GetPhysicsTransformPosition();
            lastRotation = GetPhysicsTransformRotation();
        }

        [SerializeField] protected MeshGenerator meshGenerator = new();
        public MeshGenerator MeshGenerator => meshGenerator;
        private DoubleBuffered<MeshRendererBuffers.SmartMesh> meshBuffers;
        private readonly SkeletonRendererInstruction currentInstructions = new();

        /// <summary>
        ///     Returns the <see cref="SkeletonClipping" /> used by this renderer for use with e.g.
        ///     <see cref="Skeleton.GetBounds(out float, out float, out float, out float, ref float[], SkeletonClipping)" />
        /// </summary>
        public SkeletonClipping SkeletonClipping => meshGenerator.SkeletonClipping;

        public ExposedList<Mesh> MeshesMultipleCanvasRenderers { get; } = new();

        public ExposedList<Material> MaterialsMultipleCanvasRenderers { get; } = new();

        public ExposedList<Texture> TexturesMultipleCanvasRenderers { get; } = new();

        public Mesh GetLastMesh()
        {
            return meshBuffers.GetCurrent().mesh;
        }

        public bool MatchRectTransformWithBounds()
        {
            if (!wasUpdatedAfterInit) Update(0);
            UpdateMesh();

            if (!allowMultipleCanvasRenderers)
                return MatchRectTransformSingleRenderer();
            return MatchRectTransformMultipleRenderers();
        }

        protected bool MatchRectTransformSingleRenderer()
        {
            var mesh = GetLastMesh();
            if (mesh == null) return false;
            if (mesh.vertexCount == 0 || mesh.bounds.size == Vector3.zero)
            {
                rectTransform.sizeDelta = new Vector2(50f, 50f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                return false;
            }

            mesh.RecalculateBounds();
            SetRectTransformBounds(mesh.bounds);
            return true;
        }

        protected bool MatchRectTransformMultipleRenderers()
        {
            var anyBoundsAdded = false;
            var combinedBounds = new Bounds();
            for (var i = 0; i < canvasRenderers.Count; ++i)
            {
                var canvasRenderer = canvasRenderers[i];
                if (!canvasRenderer.gameObject.activeSelf)
                    continue;

                var mesh = MeshesMultipleCanvasRenderers.Items[i];
                if (mesh == null || mesh.vertexCount == 0)
                    continue;

                mesh.RecalculateBounds();
                var bounds = mesh.bounds;
                if (anyBoundsAdded)
                {
                    combinedBounds.Encapsulate(bounds);
                }
                else
                {
                    anyBoundsAdded = true;
                    combinedBounds = bounds;
                }
            }

            if (!anyBoundsAdded || combinedBounds.size == Vector3.zero)
            {
                rectTransform.sizeDelta = new Vector2(50f, 50f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                return false;
            }

            SetRectTransformBounds(combinedBounds);
            return true;
        }

        private void SetRectTransformBounds(Bounds combinedBounds)
        {
            var size = combinedBounds.size;
            var center = combinedBounds.center;
            var p = new Vector2(
                0.5f - center.x / size.x,
                0.5f - center.y / size.y
            );

            SetRectTransformSize(this, size);
            rectTransform.pivot = p;

            foreach (var separatorPart in separatorParts)
            {
                var separatorTransform = separatorPart.GetComponent<RectTransform>();
                if (separatorTransform)
                {
                    SetRectTransformSize(separatorTransform, size);
                    separatorTransform.pivot = p;
                }
            }

            foreach (var submeshGraphic in submeshGraphics)
            {
                SetRectTransformSize(submeshGraphic, size);
                submeshGraphic.rectTransform.pivot = p;
            }

            referenceSize = size;
            referenceScale = referenceScale * layoutScale;
            layoutScale = 1f;
        }

        public static void SetRectTransformSize(Graphic target, Vector2 size)
        {
            SetRectTransformSize(target.rectTransform, size);
        }

        public static void SetRectTransformSize(RectTransform targetRectTransform, Vector2 size)
        {
            var parentSize = Vector2.zero;
            if (targetRectTransform.parent != null)
            {
                var parentTransform = targetRectTransform.parent.GetComponent<RectTransform>();
                if (parentTransform)
                    parentSize = parentTransform.rect.size;
            }

            var anchorAreaSize =
                Vector2.Scale(targetRectTransform.anchorMax - targetRectTransform.anchorMin, parentSize);
            targetRectTransform.sizeDelta = size - anchorAreaSize;
        }

        /// <summary>OnAnimationRebuild is raised after the SkeletonAnimation component is successfully initialized.</summary>
        public event ISkeletonAnimationDelegate OnAnimationRebuild;

        public event UpdateBonesDelegate BeforeApply;
        public event UpdateBonesDelegate UpdateLocal;
        public event UpdateBonesDelegate UpdateWorld;
        public event UpdateBonesDelegate UpdateComplete;

        [SerializeField] protected UpdateTiming updateTiming = UpdateTiming.InUpdate;

        public UpdateTiming UpdateTiming
        {
            get => updateTiming;
            set => updateTiming = value;
        }

        [SerializeField] protected bool unscaledTime;

        public bool UnscaledTime
        {
            get => unscaledTime;
            set => unscaledTime = value;
        }

        /// <summary> Occurs after the vertex data populated every frame, before the vertices are pushed into the mesh.</summary>
        public event MeshGeneratorDelegate OnPostProcessVertices;

        public void Clear()
        {
            skeleton = null;
            canvasRenderer.Clear();

            for (var i = 0; i < canvasRenderers.Count; ++i)
                canvasRenderers[i].Clear();
            DestroyMeshes();
            MaterialsMultipleCanvasRenderers.Clear();
            TexturesMultipleCanvasRenderers.Clear();
            DisposeMeshBuffers();
        }

        public void TrimRenderers()
        {
            var newList = new List<CanvasRenderer>();
            foreach (var canvasRenderer in canvasRenderers)
                if (canvasRenderer.gameObject.activeSelf)
                {
                    newList.Add(canvasRenderer);
                }
                else
                {
                    if (Application.isEditor && !Application.isPlaying)
                        DestroyImmediate(canvasRenderer.gameObject);
                    else
                        Destroy(canvasRenderer.gameObject);
                }

            canvasRenderers = newList;
            SyncSubmeshGraphicsWithCanvasRenderers();
        }

        public void Initialize(bool overwrite)
        {
            if (IsValid && !overwrite) return;
#if UNITY_EDITOR
            if (BuildUtilities.IsInSkeletonAssetBuildPreProcessing)
                return;
#endif
            if (skeletonDataAsset == null) return;
            var skeletonData = skeletonDataAsset.GetSkeletonData(false);
            if (skeletonData == null) return;

            if (skeletonDataAsset.atlasAssets.Length <= 0 ||
                skeletonDataAsset.atlasAssets[0].MaterialCount <= 0) return;

            skeleton = new Skeleton(skeletonData)
            {
                ScaleX = initialFlipX ? -1 : 1,
                ScaleY = initialFlipY ? -1 : 1
            };

            InitMeshBuffers();
            baseTexture = skeletonDataAsset.atlasAssets[0].PrimaryMaterial.mainTexture;
            canvasRenderer.SetTexture(mainTexture); // Needed for overwriting initializations.

            ResetLastPositionAndRotation();

            // Set the initial Skin and Animation
            if (!string.IsNullOrEmpty(initialSkinName))
                skeleton.SetSkin(initialSkinName);

            separatorSlots.Clear();
            for (var i = 0; i < separatorSlotNames.Length; i++)
                separatorSlots.Add(skeleton.FindSlot(separatorSlotNames[i]));

            if (OnRebuild != null)
                OnRebuild(this);

            wasUpdatedAfterInit = false;
            state = new AnimationState(skeletonDataAsset.GetAnimationStateData());
            if (state == null)
            {
                Clear();
                return;
            }

            if (!string.IsNullOrEmpty(startingAnimation))
            {
                var animationObject = skeletonDataAsset.GetSkeletonData(false).FindAnimation(startingAnimation);
                if (animationObject != null)
                {
                    state.SetAnimation(0, animationObject, startingLoop);
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        Update(0f);
#endif
                }
            }

            if (OnAnimationRebuild != null)
                OnAnimationRebuild(this);
        }

        public void PrepareInstructionsAndRenderers(bool isInRebuild = false)
        {
            if (!allowMultipleCanvasRenderers)
            {
                MeshGenerator.GenerateSingleSubmeshInstruction(currentInstructions, skeleton, null);
                if (canvasRenderers.Count > 0)
                    DisableUnusedCanvasRenderers(0, isInRebuild);
                usedRenderersCount = 0;
            }
            else
            {
                MeshGenerator.GenerateSkeletonRendererInstruction(currentInstructions, skeleton, null,
                    enableSeparatorSlots ? separatorSlots : null,
                    enableSeparatorSlots ? separatorSlots.Count > 0 : false);

                var submeshCount = currentInstructions.submeshInstructions.Count;
                EnsureCanvasRendererCount(submeshCount);
                EnsureMeshesCount(submeshCount);
                EnsureUsedTexturesAndMaterialsCount(submeshCount);
                EnsureSeparatorPartCount();
                PrepareRendererGameObjects(currentInstructions, isInRebuild);
            }

            if (OnInstructionsPrepared != null)
                OnInstructionsPrepared(currentInstructions);
        }

        public void UpdateMesh()
        {
            PrepareInstructionsAndRenderers();
            UpdateMeshToInstructions();
        }

        public void UpdateMeshToInstructions()
        {
            if (!IsValid || currentInstructions.rawVertexCount < 0) return;
            skeleton.SetColor(color);

            if (!allowMultipleCanvasRenderers)
            {
                UpdateMeshSingleCanvasRenderer(currentInstructions);
            }
            else
            {
                UpdateMaterialsMultipleCanvasRenderers(currentInstructions);
                UpdateMeshMultipleCanvasRenderers(currentInstructions);
            }

            if (OnMeshAndMaterialsUpdated != null)
                OnMeshAndMaterialsUpdated(this);
        }

        public bool HasMultipleSubmeshInstructions()
        {
            if (!IsValid)
                return false;
            return MeshGenerator.RequiresMultipleSubmeshesByDrawOrder(skeleton);
        }

        #endregion

        protected void InitMeshBuffers()
        {
            if (meshBuffers != null)
            {
                meshBuffers.GetNext().Clear();
                meshBuffers.GetNext().Clear();
            }
            else
            {
                meshBuffers = new DoubleBuffered<MeshRendererBuffers.SmartMesh>();
            }
        }

        protected void DisposeMeshBuffers()
        {
            if (meshBuffers != null)
            {
                meshBuffers.GetNext().Dispose();
                meshBuffers.GetNext().Dispose();
                meshBuffers = null;
            }
        }

        protected void UpdateMeshSingleCanvasRenderer(SkeletonRendererInstruction currentInstructions)
        {
            var smartMesh = meshBuffers.GetNext();
            var updateTriangles =
                SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, smartMesh.instructionUsed);
            meshGenerator.Begin();

            var useAddSubmesh = currentInstructions.hasActiveClipping &&
                                currentInstructions.submeshInstructions.Count > 0;
            if (useAddSubmesh)
                meshGenerator.AddSubmesh(currentInstructions.submeshInstructions.Items[0], updateTriangles);
            else
                meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);

            meshScale = canvas == null ? 100 : canvas.referencePixelsPerUnit;
            if (layoutScaleMode != LayoutMode.None)
            {
                meshScale *= referenceScale;
                layoutScale = GetLayoutScale(layoutScaleMode);
                if (!EditReferenceRect) meshScale *= layoutScale;
                meshOffset = pivotOffset * layoutScale;
            }
            else
            {
                meshOffset = pivotOffset;
            }

            if (meshOffset == Vector2.zero)
                meshGenerator.ScaleVertexData(meshScale);
            else
                meshGenerator.ScaleAndOffsetVertexData(meshScale, meshOffset);

            if (OnPostProcessVertices != null) OnPostProcessVertices.Invoke(meshGenerator.Buffers);

            var mesh = smartMesh.mesh;
            meshGenerator.FillVertexData(mesh);
            if (updateTriangles) meshGenerator.FillTriangles(mesh);
            meshGenerator.FillLateVertexData(mesh);

            smartMesh.instructionUsed.Set(currentInstructions);
            if (assignMeshOverrideSingle != null)
                assignMeshOverrideSingle(mesh, canvasRenderer.GetMaterial(), mainTexture);

            var assignAtCanvasRenderer = assignMeshOverrideSingle == null || !disableMeshAssignmentOnOverride;
            if (assignAtCanvasRenderer)
                canvasRenderer.SetMesh(mesh);
            else
                canvasRenderer.SetMesh(null);

            var assignTexture = false;
            if (currentInstructions.submeshInstructions.Count > 0)
            {
                var material = currentInstructions.submeshInstructions.Items[0].material;
                if (material != null && baseTexture != material.mainTexture)
                {
                    baseTexture = material.mainTexture;
                    if (overrideTexture == null && assignAtCanvasRenderer)
                        assignTexture = true;
                }
            }

#if SPINE_OPTIONAL_ON_DEMAND_LOADING
            if (Application.isPlaying)
                HandleOnDemandLoading();
#endif
            if (assignTexture)
                canvasRenderer.SetTexture(mainTexture);
        }

        protected void UpdateMaterialsMultipleCanvasRenderers(SkeletonRendererInstruction currentInstructions)
        {
            var submeshCount = currentInstructions.submeshInstructions.Count;
            var useOriginalTextureAndMaterial = CustomMaterialOverride.Count == 0 && CustomTextureOverride.Count == 0;

            var blendModeMaterials = skeletonDataAsset.blendModeMaterials;
            var hasBlendModeMaterials = blendModeMaterials.RequiresBlendModeMaterials;

            var pmaVertexColors = meshGenerator.settings.pmaVertexColors;
            var usedMaterialItems = MaterialsMultipleCanvasRenderers.Items;
            var usedTextureItems = TexturesMultipleCanvasRenderers.Items;
            for (var i = 0; i < submeshCount; i++)
            {
                var submeshInstructionItem = currentInstructions.submeshInstructions.Items[i];
                var submeshMaterial = submeshInstructionItem.material;
                if (useOriginalTextureAndMaterial)
                {
                    if (submeshMaterial == null)
                    {
                        usedMaterialItems[i] = null;
                        usedTextureItems[i] = null;
                        continue;
                    }

                    usedTextureItems[i] = submeshMaterial.mainTexture;
                    if (!hasBlendModeMaterials)
                    {
                        usedMaterialItems[i] = materialForRendering;
                    }
                    else
                    {
                        var blendMode = blendModeMaterials.BlendModeForMaterial(submeshMaterial);
                        var usedMaterial = materialForRendering;
                        if (blendMode == BlendMode.Additive && !pmaVertexColors && additiveMaterial)
                            usedMaterial = additiveMaterial;
                        else if (blendMode == BlendMode.Multiply && multiplyMaterial)
                            usedMaterial = multiplyMaterial;
                        else if (blendMode == BlendMode.Screen && screenMaterial)
                            usedMaterial = screenMaterial;
                        usedMaterialItems[i] = submeshGraphics[i].GetModifiedMaterial(usedMaterial);
                    }
                }
                else
                {
                    var originalTexture = submeshMaterial.mainTexture;
                    Material usedMaterial;
                    Texture usedTexture;
                    if (!CustomMaterialOverride.TryGetValue(originalTexture, out usedMaterial))
                        usedMaterial = material;
                    if (!CustomTextureOverride.TryGetValue(originalTexture, out usedTexture))
                        usedTexture = originalTexture;

                    usedMaterialItems[i] = submeshGraphics[i].GetModifiedMaterial(usedMaterial);
                    usedTextureItems[i] = usedTexture;
                }
            }
        }

        protected void UpdateMeshMultipleCanvasRenderers(SkeletonRendererInstruction currentInstructions)
        {
            meshScale = canvas == null ? 100 : canvas.referencePixelsPerUnit;
            if (layoutScaleMode != LayoutMode.None)
            {
                meshScale *= referenceScale;
                layoutScale = GetLayoutScale(layoutScaleMode);
                if (!EditReferenceRect) meshScale *= layoutScale;
                meshOffset = pivotOffset * layoutScale;
            }
            else
            {
                meshOffset = pivotOffset;
            }

            // Generate meshes.
            var submeshCount = currentInstructions.submeshInstructions.Count;
            var meshesItems = MeshesMultipleCanvasRenderers.Items;
            var useOriginalTextureAndMaterial = CustomMaterialOverride.Count == 0 && CustomTextureOverride.Count == 0;

            var blendModeMaterials = skeletonDataAsset.blendModeMaterials;
            var hasBlendModeMaterials = blendModeMaterials.RequiresBlendModeMaterials;
#if HAS_CULL_TRANSPARENT_MESH
            var mainCullTransparentMesh = this.canvasRenderer.cullTransparentMesh;
#endif
            var pmaVertexColors = meshGenerator.settings.pmaVertexColors;
            var usedMaterialItems = MaterialsMultipleCanvasRenderers.Items;
            var usedTextureItems = TexturesMultipleCanvasRenderers.Items;
            for (var i = 0; i < submeshCount; i++)
            {
                var submeshInstructionItem = currentInstructions.submeshInstructions.Items[i];
                meshGenerator.Begin();
                meshGenerator.AddSubmesh(submeshInstructionItem);

                var targetMesh = meshesItems[i];
                if (meshOffset == Vector2.zero)
                    meshGenerator.ScaleVertexData(meshScale);
                else
                    meshGenerator.ScaleAndOffsetVertexData(meshScale, meshOffset);
                if (OnPostProcessVertices != null) OnPostProcessVertices.Invoke(meshGenerator.Buffers);
                meshGenerator.FillVertexData(targetMesh);
                meshGenerator.FillTriangles(targetMesh);
                meshGenerator.FillLateVertexData(targetMesh);

                var canvasRenderer = canvasRenderers[i];
                if (assignMeshOverrideSingle == null || !disableMeshAssignmentOnOverride)
                    canvasRenderer.SetMesh(targetMesh);
                else
                    canvasRenderer.SetMesh(null);

                var submeshGraphic = submeshGraphics[i];
                if (useOriginalTextureAndMaterial && hasBlendModeMaterials)
                {
                    var allowCullTransparentMesh = true;
                    var materialBlendMode = blendModeMaterials.BlendModeForMaterial(usedMaterialItems[i]);
                    if ((materialBlendMode == BlendMode.Normal && submeshInstructionItem.hasPMAAdditiveSlot) ||
                        (materialBlendMode == BlendMode.Additive && pmaVertexColors))
                        allowCullTransparentMesh = false;
#if HAS_CULL_TRANSPARENT_MESH
                    canvasRenderer.cullTransparentMesh = allowCullTransparentMesh ? mainCullTransparentMesh : false;
#endif
                }

                canvasRenderer.materialCount = 1;
            }

#if SPINE_OPTIONAL_ON_DEMAND_LOADING
            if (Application.isPlaying)
                HandleOnDemandLoading();
#endif
            var assignAtCanvasRenderer = assignMeshOverrideSingle == null || !disableMeshAssignmentOnOverride;
            if (assignAtCanvasRenderer)
                for (var i = 0; i < submeshCount; i++)
                {
                    var canvasRenderer = canvasRenderers[i];
                    canvasRenderer.SetMaterial(usedMaterialItems[i], usedTextureItems[i]);
                }

            if (assignMeshOverrideMultiple != null)
                assignMeshOverrideMultiple(submeshCount, meshesItems, usedMaterialItems, usedTextureItems);
        }

#if SPINE_OPTIONAL_ON_DEMAND_LOADING
        private void HandleOnDemandLoading()
        {
            foreach (var atlasAsset in skeletonDataAsset.atlasAssets)
                if (atlasAsset.TextureLoadingMode != AtlasAssetBase.LoadingMode.Normal)
                {
                    atlasAsset.BeginCustomTextureLoading();

                    if (!allowMultipleCanvasRenderers)
                    {
                        Texture loadedTexture = null;
                        atlasAsset.RequireTextureLoaded(mainTexture, ref loadedTexture, null);
                        if (loadedTexture)
                            baseTexture = loadedTexture;
                    }
                    else
                    {
                        var textureItems = TexturesMultipleCanvasRenderers.Items;
                        for (int i = 0, count = TexturesMultipleCanvasRenderers.Count; i < count; ++i)
                        {
                            Texture loadedTexture = null;
                            atlasAsset.RequireTextureLoaded(textureItems[i], ref loadedTexture, null);
                            if (loadedTexture)
                                TexturesMultipleCanvasRenderers.Items[i] = loadedTexture;
                        }
                    }

                    atlasAsset.EndCustomTextureLoading();
                }
        }
#endif

        protected void EnsureCanvasRendererCount(int targetCount)
        {
#if UNITY_EDITOR
            RemoveNullCanvasRenderers();
#endif
            var currentCount = canvasRenderers.Count;
            for (var i = currentCount; i < targetCount; ++i)
            {
                var go = new GameObject(string.Format("Renderer{0}", i), typeof(RectTransform));
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;
                var canvasRenderer = go.AddComponent<CanvasRenderer>();
                canvasRenderers.Add(canvasRenderer);
                var submeshGraphic = go.AddComponent<SkeletonSubmeshGraphic>();
                submeshGraphic.maskable = maskable;
                submeshGraphic.raycastTarget = false;
                submeshGraphic.rectTransform.pivot = rectTransform.pivot;
                submeshGraphic.rectTransform.anchorMin = Vector2.zero;
                submeshGraphic.rectTransform.anchorMax = Vector2.one;
                submeshGraphic.rectTransform.sizeDelta = Vector2.zero;
                submeshGraphics.Add(submeshGraphic);
            }
        }

        protected void PrepareRendererGameObjects(SkeletonRendererInstruction currentInstructions,
            bool isInRebuild = false)
        {
            var submeshCount = currentInstructions.submeshInstructions.Count;
            DisableUnusedCanvasRenderers(submeshCount, isInRebuild);

            var parent = separatorParts.Count == 0 ? transform : separatorParts[0];
            if (updateSeparatorPartLocation)
                for (var p = 0; p < separatorParts.Count; ++p)
                {
                    var separatorPart = separatorParts[p];
                    if (separatorPart == null) continue;
                    separatorPart.position = transform.position;
                    separatorPart.rotation = transform.rotation;
                }

            if (updateSeparatorPartScale)
            {
                var targetScale = transform.lossyScale;
                for (var p = 0; p < separatorParts.Count; ++p)
                {
                    var separatorPart = separatorParts[p];
                    if (separatorPart == null) continue;
                    var partParent = separatorPart.parent;
                    var parentScale = partParent == null ? Vector3.one : partParent.lossyScale;
                    separatorPart.localScale = new Vector3(
                        parentScale.x == 0f ? 1f : targetScale.x / parentScale.x,
                        parentScale.y == 0f ? 1f : targetScale.y / parentScale.y,
                        parentScale.z == 0f ? 1f : targetScale.z / parentScale.z);
                }
            }

            var separatorSlotGroupIndex = 0;
            var targetSiblingIndex = 0;
            for (var i = 0; i < submeshCount; i++)
            {
                var canvasRenderer = canvasRenderers[i];
                if (canvasRenderer != null)
                {
                    if (i >= usedRenderersCount)
                        canvasRenderer.gameObject.SetActive(true);

                    if (canvasRenderer.transform.parent != parent.transform && !isInRebuild)
                        canvasRenderer.transform.SetParent(parent.transform, false);

                    canvasRenderer.transform.SetSiblingIndex(targetSiblingIndex++);
                }

                var submeshGraphic = submeshGraphics[i];
                if (submeshGraphic != null)
                {
                    var dstTransform = submeshGraphic.rectTransform;
                    dstTransform.localPosition = Vector3.zero;
                    dstTransform.pivot = rectTransform.pivot;
                    dstTransform.anchorMin = Vector2.zero;
                    dstTransform.anchorMax = Vector2.one;
                    dstTransform.sizeDelta = Vector2.zero;
                }

                var submeshInstructionItem = currentInstructions.submeshInstructions.Items[i];
                if (submeshInstructionItem.forceSeparate)
                {
                    targetSiblingIndex = 0;
                    parent = separatorParts[++separatorSlotGroupIndex];
                }
            }

            usedRenderersCount = submeshCount;
        }

        protected void DisableUnusedCanvasRenderers(int usedCount, bool isInRebuild = false)
        {
#if UNITY_EDITOR
            RemoveNullCanvasRenderers();
#endif
            for (var i = usedCount; i < canvasRenderers.Count; i++)
            {
                canvasRenderers[i].Clear();
                if (!isInRebuild) // rebuild does not allow disabling Graphic and thus removing it from rebuild list.
                    canvasRenderers[i].gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        private void RemoveNullCanvasRenderers()
        {
            if (Application.isEditor && !Application.isPlaying)
                for (var i = canvasRenderers.Count - 1; i >= 0; --i)
                    if (canvasRenderers[i] == null)
                    {
                        canvasRenderers.RemoveAt(i);
                        submeshGraphics.RemoveAt(i);
                    }
        }

        private void DestroyOldRawImages()
        {
            foreach (var canvasRenderer in canvasRenderers)
            {
                var oldRawImage = canvasRenderer.GetComponent<RawImage>();
                if (oldRawImage != null) DestroyImmediate(oldRawImage);
            }
        }
#endif

        protected void EnsureMeshesCount(int targetCount)
        {
            var oldCount = MeshesMultipleCanvasRenderers.Count;
            MeshesMultipleCanvasRenderers.EnsureCapacity(targetCount);
            for (var i = oldCount; i < targetCount; i++)
                MeshesMultipleCanvasRenderers.Add(SpineMesh.NewSkeletonMesh());
        }

        protected void EnsureUsedTexturesAndMaterialsCount(int targetCount)
        {
            var oldCount = MaterialsMultipleCanvasRenderers.Count;
            MaterialsMultipleCanvasRenderers.EnsureCapacity(targetCount);
            TexturesMultipleCanvasRenderers.EnsureCapacity(targetCount);
            for (var i = oldCount; i < targetCount; i++)
            {
                MaterialsMultipleCanvasRenderers.Add(null);
                TexturesMultipleCanvasRenderers.Add(null);
            }
        }

        protected void DestroyMeshes()
        {
            foreach (var mesh in MeshesMultipleCanvasRenderers)
            {
#if UNITY_EDITOR
                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(mesh);
                else
                    Destroy(mesh);
#else
				UnityEngine.Object.Destroy(mesh);
#endif
            }

            MeshesMultipleCanvasRenderers.Clear();
        }

        protected void EnsureSeparatorPartCount()
        {
#if UNITY_EDITOR
            RemoveNullSeparatorParts();
#endif
            var targetCount = separatorSlots.Count + 1;
            if (targetCount == 1)
                return;

#if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
                for (var i = separatorParts.Count - 1; i >= 0; --i)
                    if (separatorParts[i] == null)
                        separatorParts.RemoveAt(i);
#endif
            var currentCount = separatorParts.Count;
            for (var i = currentCount; i < targetCount; ++i)
            {
                var go = new GameObject(string.Format("{0}[{1}]", SeparatorPartGameObjectName, i),
                    typeof(RectTransform));
                go.transform.SetParent(transform, false);

                var dstTransform = go.transform.GetComponent<RectTransform>();
                dstTransform.localPosition = Vector3.zero;
                dstTransform.pivot = rectTransform.pivot;
                dstTransform.anchorMin = Vector2.zero;
                dstTransform.anchorMax = Vector2.one;
                dstTransform.sizeDelta = Vector2.zero;

                separatorParts.Add(go.transform);
            }
        }

        protected void UpdateSeparatorPartParents()
        {
            var usedCount = separatorSlots.Count + 1;
            if (usedCount == 1)
            {
                usedCount = 0; // placed directly at the SkeletonGraphic parent
                for (var i = 0; i < canvasRenderers.Count; ++i)
                {
                    var canvasRenderer = canvasRenderers[i];
                    if (canvasRenderer.transform.parent.name.Contains(SeparatorPartGameObjectName))
                    {
                        canvasRenderer.transform.SetParent(transform, false);
                        canvasRenderer.transform.localPosition = Vector3.zero;
                    }
                }
            }

            for (var i = 0; i < separatorParts.Count; ++i)
            {
                var isUsed = i < usedCount;
                separatorParts[i].gameObject.SetActive(isUsed);
            }
        }

#if UNITY_EDITOR
        private void RemoveNullSeparatorParts()
        {
            if (Application.isEditor && !Application.isPlaying)
                for (var i = separatorParts.Count - 1; i >= 0; --i)
                    if (separatorParts[i] == null)
                        separatorParts.RemoveAt(i);
        }

        protected void InitLayoutScaleParameters()
        {
            previousLayoutScaleMode = layoutScaleMode;
        }

        protected void UpdateReferenceRectSizes()
        {
            if (rectTransformSize == Vector2.zero)
                rectTransformSize = GetCurrentRectSize();

            HandleChangedEditReferenceRect();

            if (layoutScaleMode != previousLayoutScaleMode)
            {
                if (layoutScaleMode != LayoutMode.None)
                {
                    SetRectTransformSize(this, rectTransformSize);
                }
                else
                {
                    rectTransformSize = referenceSize / referenceScale;
                    referenceScale = 1f;
                    SetRectTransformSize(this, rectTransformSize);
                }
            }

            if (editReferenceRect || layoutScaleMode == LayoutMode.None)
                referenceSize = GetCurrentRectSize();

            previousLayoutScaleMode = layoutScaleMode;
        }

        protected void HandleChangedEditReferenceRect()
        {
            if (editReferenceRect == previousEditReferenceRect) return;
            previousEditReferenceRect = editReferenceRect;

            if (editReferenceRect)
            {
                rectTransformSize = GetCurrentRectSize();
                ResetRectToReferenceRectSize();
            }
            else
            {
                SetRectTransformSize(this, rectTransformSize);
            }
        }

        public void ResetRectToReferenceRectSize()
        {
            referenceScale = referenceScale * GetLayoutScale(previousLayoutScaleMode);
            var referenceAspect = referenceSize.x / referenceSize.y;
            var newSize = GetCurrentRectSize();

            var mode = GetEffectiveLayoutMode(previousLayoutScaleMode);
            if (mode == LayoutMode.WidthControlsHeight)
                newSize.y = newSize.x / referenceAspect;
            else if (mode == LayoutMode.HeightControlsWidth)
                newSize.x = newSize.y * referenceAspect;
            SetRectTransformSize(this, newSize);
        }

        public Vector2 GetReferenceRectSize()
        {
            return referenceSize * GetLayoutScale(layoutScaleMode);
        }

        public Vector2 GetPivotOffset()
        {
            return pivotOffset;
        }

        public Vector2 GetScaledPivotOffset()
        {
            return pivotOffset * GetLayoutScale(layoutScaleMode);
        }
#endif
        public void SetScaledPivotOffset(Vector2 pivotOffsetScaled)
        {
            pivotOffset = pivotOffsetScaled / GetLayoutScale(layoutScaleMode);
        }

        protected float GetLayoutScale(LayoutMode mode)
        {
            var currentSize = GetCurrentRectSize();
            mode = GetEffectiveLayoutMode(mode);
            if (mode == LayoutMode.WidthControlsHeight) return currentSize.x / referenceSize.x;

            if (mode == LayoutMode.HeightControlsWidth) return currentSize.y / referenceSize.y;
            return 1f;
        }

        /// <summary>
        ///     <c>LayoutMode FitInParent</c> and <c>EnvelopeParent</c> actually result in
        ///     <c>HeightControlsWidth</c> or <c>WidthControlsHeight</c> depending on the actual vs reference aspect ratio.
        ///     This method returns the respective <c>LayoutMode</c> of the two for any given input <c>mode</c>.
        /// </summary>
        protected LayoutMode GetEffectiveLayoutMode(LayoutMode mode)
        {
            var currentSize = GetCurrentRectSize();
            var referenceAspect = referenceSize.x / referenceSize.y;
            var frameAspect = currentSize.x / currentSize.y;
            if (mode == LayoutMode.FitInParent)
                mode = frameAspect > referenceAspect ? LayoutMode.HeightControlsWidth : LayoutMode.WidthControlsHeight;
            else if (mode == LayoutMode.EnvelopeParent)
                mode = frameAspect > referenceAspect ? LayoutMode.WidthControlsHeight : LayoutMode.HeightControlsWidth;
            return mode;
        }

        private Vector2 GetCurrentRectSize()
        {
            return rectTransform.rect.size;
        }
    }
}