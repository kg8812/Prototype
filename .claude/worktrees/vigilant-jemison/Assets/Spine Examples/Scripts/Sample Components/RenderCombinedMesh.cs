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

#if UNITY_2019_3_OR_NEWER
#define SET_VERTICES_HAS_LENGTH_PARAMETER
#endif

using System;
using System.Linq;
using UnityEngine;

namespace Spine.Unity.Examples
{
#if NEW_PREFAB_SYSTEM
    [ExecuteAlways]
#else
	[ExecuteInEditMode]
#endif
    public class RenderCombinedMesh : MonoBehaviour
    {
        public SkeletonRenderer skeletonRenderer;
        public SkeletonRenderSeparator renderSeparator;
        public MeshRenderer[] referenceRenderers;

        private bool updateViaSkeletonCallback;
        private MeshFilter[] referenceMeshFilters;
        private MeshRenderer ownRenderer;
        private MeshFilter ownMeshFilter;

        protected DoubleBuffered<Mesh> doubleBufferedMesh;
        protected ExposedList<Vector3> positionBuffer;
        protected ExposedList<Color32> colorBuffer;
        protected ExposedList<Vector2> uvBuffer;
        protected ExposedList<int> indexBuffer;

#if UNITY_EDITOR
        private void Reset()
        {
            if (skeletonRenderer == null)
                skeletonRenderer = GetComponentInParent<SkeletonRenderer>();
            GatherRenderers();

            Awake();
            if (referenceRenderers.Length > 0)
                ownRenderer.sharedMaterial = referenceRenderers[0].sharedMaterial;

            LateUpdate();
        }
#endif
        protected void GatherRenderers()
        {
            referenceRenderers = GetComponentsInChildren<MeshRenderer>();
            if (referenceRenderers.Length == 0 ||
                (referenceRenderers.Length == 1 && referenceRenderers[0].gameObject == gameObject))
            {
                var parent = transform.parent;
                if (parent)
                    referenceRenderers = parent.GetComponentsInChildren<MeshRenderer>();
            }

            referenceRenderers = referenceRenderers.Where(
                (val, idx) => val.gameObject != gameObject && val.enabled).ToArray();
        }

        private void Awake()
        {
            if (skeletonRenderer == null)
                skeletonRenderer = GetComponentInParent<SkeletonRenderer>();
            if (referenceRenderers == null || referenceRenderers.Length == 0) GatherRenderers();

            if (renderSeparator == null)
            {
                if (skeletonRenderer)
                    renderSeparator = skeletonRenderer.GetComponent<SkeletonRenderSeparator>();
                else
                    renderSeparator = GetComponentInParent<SkeletonRenderSeparator>();
            }

            var count = referenceRenderers.Length;
            referenceMeshFilters = new MeshFilter[count];
            for (var i = 0; i < count; ++i) referenceMeshFilters[i] = referenceRenderers[i].GetComponent<MeshFilter>();

            ownRenderer = GetComponent<MeshRenderer>();
            if (ownRenderer == null)
                ownRenderer = gameObject.AddComponent<MeshRenderer>();
            ownMeshFilter = GetComponent<MeshFilter>();
            if (ownMeshFilter == null)
                ownMeshFilter = gameObject.AddComponent<MeshFilter>();
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Awake();
#endif
            if (skeletonRenderer)
            {
                skeletonRenderer.OnMeshAndMaterialsUpdated -= UpdateOnCallback;
                skeletonRenderer.OnMeshAndMaterialsUpdated += UpdateOnCallback;
                updateViaSkeletonCallback = true;
            }

            if (renderSeparator)
            {
                renderSeparator.OnMeshAndMaterialsUpdated -= UpdateOnCallback;
                renderSeparator.OnMeshAndMaterialsUpdated += UpdateOnCallback;
                updateViaSkeletonCallback = true;
            }
        }

        private void OnDisable()
        {
            if (skeletonRenderer)
                skeletonRenderer.OnMeshAndMaterialsUpdated -= UpdateOnCallback;
            if (renderSeparator)
                renderSeparator.OnMeshAndMaterialsUpdated -= UpdateOnCallback;
        }

        private void OnDestroy()
        {
            for (var i = 0; i < 2; ++i)
            {
                var mesh = doubleBufferedMesh.GetNext();
#if UNITY_EDITOR
                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(mesh);
                else
                    Destroy(mesh);
#else
				UnityEngine.Object.Destroy(mesh);
#endif
            }
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UpdateMesh();
                return;
            }
#endif

            if (updateViaSkeletonCallback)
                return;
            UpdateMesh();
        }

        private void UpdateOnCallback(SkeletonRenderer r)
        {
            UpdateMesh();
        }

        protected void EnsureBufferSizes(int combinedVertexCount, int combinedIndexCount)
        {
            if (positionBuffer == null)
            {
                positionBuffer = new ExposedList<Vector3>(combinedVertexCount);
                uvBuffer = new ExposedList<Vector2>(combinedVertexCount);
                colorBuffer = new ExposedList<Color32>(combinedVertexCount);
                indexBuffer = new ExposedList<int>(combinedIndexCount);
            }

            if (positionBuffer.Count != combinedVertexCount)
            {
                positionBuffer.Resize(combinedVertexCount);
                uvBuffer.Resize(combinedVertexCount);
                colorBuffer.Resize(combinedVertexCount);
            }

            if (indexBuffer.Count != combinedIndexCount) indexBuffer.Resize(combinedIndexCount);
        }

        private void InitMesh()
        {
            if (doubleBufferedMesh == null)
            {
                doubleBufferedMesh = new DoubleBuffered<Mesh>();
                for (var i = 0; i < 2; ++i)
                {
                    var combinedMesh = doubleBufferedMesh.GetNext();
                    combinedMesh.MarkDynamic();
                    combinedMesh.name = "RenderCombinedMesh" + i;
                    combinedMesh.subMeshCount = 1;
                }
            }
        }

        private void UpdateMesh()
        {
            InitMesh();
            var combinedVertexCount = 0;
            var combinedIndexCount = 0;
            GetCombinedMeshInfo(ref combinedVertexCount, ref combinedIndexCount);

            EnsureBufferSizes(combinedVertexCount, combinedIndexCount);

            var combinedV = 0;
            var combinedI = 0;
            for (int r = 0, rendererCount = referenceMeshFilters.Length; r < rendererCount; ++r)
            {
                var meshFilter = referenceMeshFilters[r];
                var mesh = meshFilter.sharedMesh;
                if (mesh == null) continue;

                var vertexCount = mesh.vertexCount;
                var positions = mesh.vertices;
                var uvs = mesh.uv;
                var colors = mesh.colors32;

                Array.Copy(positions, 0, positionBuffer.Items, combinedV, vertexCount);
                Array.Copy(uvs, 0, uvBuffer.Items, combinedV, vertexCount);
                Array.Copy(colors, 0, colorBuffer.Items, combinedV, vertexCount);

                for (int s = 0, submeshCount = mesh.subMeshCount; s < submeshCount; ++s)
                {
                    var submeshIndexCount = (int)mesh.GetIndexCount(s);
                    var submeshIndices = mesh.GetIndices(s);
                    var dstIndices = indexBuffer.Items;
                    for (var i = 0; i < submeshIndexCount; ++i)
                        dstIndices[i + combinedI] = submeshIndices[i] + combinedV;
                    combinedI += submeshIndexCount;
                }

                combinedV += vertexCount;
            }

            var combinedMesh = doubleBufferedMesh.GetNext();
            combinedMesh.Clear();
#if SET_VERTICES_HAS_LENGTH_PARAMETER
            combinedMesh.SetVertices(positionBuffer.Items, 0, positionBuffer.Count);
            combinedMesh.SetUVs(0, uvBuffer.Items, 0, uvBuffer.Count);
            combinedMesh.SetColors(colorBuffer.Items, 0, colorBuffer.Count);
            combinedMesh.SetTriangles(indexBuffer.Items, 0, indexBuffer.Count, 0);
#else
			// Note: excess already contains zero positions and indices after ExposedList.Resize().
			combinedMesh.vertices = this.positionBuffer.Items;
			combinedMesh.uv = this.uvBuffer.Items;
			combinedMesh.colors32 = this.colorBuffer.Items;
			combinedMesh.triangles = this.indexBuffer.Items;
#endif
            ownMeshFilter.sharedMesh = combinedMesh;
        }

        private void GetCombinedMeshInfo(ref int vertexCount, ref int indexCount)
        {
            for (int r = 0, rendererCount = referenceMeshFilters.Length; r < rendererCount; ++r)
            {
                var meshFilter = referenceMeshFilters[r];
                var mesh = meshFilter.sharedMesh;
                if (mesh == null) continue;

                vertexCount += mesh.vertexCount;
                for (int s = 0, submeshCount = mesh.subMeshCount; s < submeshCount; ++s)
                    indexCount += (int)mesh.GetIndexCount(s);
            }
        }
    }
}