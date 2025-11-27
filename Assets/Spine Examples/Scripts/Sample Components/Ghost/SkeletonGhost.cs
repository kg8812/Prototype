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

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spine.Unity.Examples
{
    [RequireComponent(typeof(SkeletonRenderer))]
    public class SkeletonGhost : MonoBehaviour
    {
        // Internal Settings
        private const HideFlags GhostHideFlags = HideFlags.HideInHierarchy;
        private const string GhostingShaderName = "Spine/Special/SkeletonGhost";

        [Header("Animation")] public bool ghostingEnabled = true;

        [Tooltip("The time between invididual ghost pieces being spawned.")] [FormerlySerializedAs("spawnRate")]
        public float spawnInterval = 1f / 30f;

        [Tooltip(
            "Maximum number of ghosts that can exist at a time. If the fade speed is not fast enough, the oldest ghost will immediately disappear to enforce the maximum number.")]
        public int maximumGhosts = 10;

        public float fadeSpeed = 10;

        [Header("Rendering")] public Shader ghostShader;

        public Color32 color = new(0xFF, 0xFF, 0xFF, 0x00); // default for additive.

        [Tooltip("Remember to set color alpha to 0 if Additive is true")]
        public bool additive = true;

        [Tooltip("0 is Color and Alpha, 1 is Alpha only.")] [Range(0, 1)]
        public float textureFade = 1;

        [Header("Sorting")] public bool sortWithDistanceOnly;

        public float zOffset;

        private readonly Dictionary<Material, Material> materialTable = new();
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private float nextSpawnTime;
        private SkeletonGhostRenderer[] pool;
        private int poolIndex;
        private SkeletonRenderer skeletonRenderer;

        private void Start()
        {
            Initialize(false);
        }

        private void Update()
        {
            if (!ghostingEnabled || poolIndex >= pool.Length)
                return;

            if (Time.time >= nextSpawnTime)
            {
                var go = pool[poolIndex].gameObject;

                var materials = meshRenderer.sharedMaterials;
                for (var i = 0; i < materials.Length; i++)
                {
                    var originalMat = materials[i];
                    Material ghostMat;
                    if (!materialTable.ContainsKey(originalMat))
                    {
                        ghostMat = new Material(originalMat)
                        {
                            shader = ghostShader,
                            color = Color.white
                        };

                        if (ghostMat.HasProperty("_TextureFade"))
                            ghostMat.SetFloat("_TextureFade", textureFade);

                        materialTable.Add(originalMat, ghostMat);
                    }
                    else
                    {
                        ghostMat = materialTable[originalMat];
                    }

                    materials[i] = ghostMat;
                }

                var goTransform = go.transform;
                goTransform.parent = transform;

                pool[poolIndex].Initialize(meshFilter.sharedMesh, materials, color, additive, fadeSpeed,
                    meshRenderer.sortingLayerID,
                    sortWithDistanceOnly ? meshRenderer.sortingOrder : meshRenderer.sortingOrder - 1);

                goTransform.localPosition = new Vector3(0f, 0f, zOffset);
                goTransform.localRotation = Quaternion.identity;
                goTransform.localScale = Vector3.one;

                goTransform.parent = null;

                poolIndex++;

                if (poolIndex == pool.Length)
                    poolIndex = 0;

                nextSpawnTime = Time.time + spawnInterval;
            }
        }

        private void OnDestroy()
        {
            if (pool != null)
                for (var i = 0; i < maximumGhosts; i++)
                    if (pool[i] != null)
                        pool[i].Cleanup();

            foreach (var mat in materialTable.Values)
                Destroy(mat);
        }

        public void Initialize(bool overwrite)
        {
            if (pool == null || overwrite)
            {
                if (ghostShader == null)
                    ghostShader = Shader.Find(GhostingShaderName);

                skeletonRenderer = GetComponent<SkeletonRenderer>();
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();
                nextSpawnTime = Time.time + spawnInterval;
                pool = new SkeletonGhostRenderer[maximumGhosts];
                for (var i = 0; i < maximumGhosts; i++)
                {
                    var go = new GameObject(gameObject.name + " Ghost", typeof(SkeletonGhostRenderer));
                    pool[i] = go.GetComponent<SkeletonGhostRenderer>();
                    go.SetActive(false);
                    go.hideFlags = GhostHideFlags;
                }

                var skeletonAnimation = skeletonRenderer as IAnimationStateComponent;
                if (skeletonAnimation != null)
                    skeletonAnimation.AnimationState.Event += OnEvent;
            }
        }

        //SkeletonAnimation
        /*
         *	Int Value:		0 sets ghostingEnabled to false, 1 sets ghostingEnabled to true
         *	Float Value:	Values greater than 0 set the spawnRate equal the float value
         *	String Value:	Pass RGBA hex color values in to set the color property. IE: "A0FF8BFF"
         */
        private void OnEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("Ghosting", StringComparison.Ordinal))
            {
                ghostingEnabled = e.Int > 0;
                if (e.Float > 0)
                    spawnInterval = e.Float;

                if (!string.IsNullOrEmpty(e.String))
                    color = HexToColor(e.String);
            }
        }

        //SkeletonAnimator
        //SkeletonAnimator or Mecanim based animations only support toggling ghostingEnabled. Be sure not to set anything other than the Int param in Spine or String will take priority.
        private void Ghosting(float val)
        {
            ghostingEnabled = val > 0;
        }

        // based on UnifyWiki http://wiki.unity3d.com/index.php?title=HexConverter
        private static Color32 HexToColor(string hex)
        {
            const NumberStyles HexNumber = NumberStyles.HexNumber;

            if (hex.Length < 6)
                return Color.magenta;

            hex = hex.Replace("#", "");
            var r = byte.Parse(hex.Substring(0, 2), HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), HexNumber);
            byte a = 0xFF;
            if (hex.Length == 8)
                a = byte.Parse(hex.Substring(6, 2), HexNumber);

            return new Color32(r, g, b, a);
        }
    }
}