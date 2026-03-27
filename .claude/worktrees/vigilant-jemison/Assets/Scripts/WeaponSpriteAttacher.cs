/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated July 28, 2023. Replaces all prior versions.
 *
 * Copyright (c) 2013-2023, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software or
 * otherwise create derivative works of the Spine Runtimes (collectively,
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
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE
 * SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

// Original Contribution by: Mitch Thompson

using Spine.Unity.AttachmentTools;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Examples
{
    public class WeaponSpriteAttacher : MonoBehaviour
    {
        private bool applyPMA;

        private RegionAttachment[] attachments;
        private Slot[] spineSlots;

        //static Dictionary<Texture, AtlasPage> atlasPageCache;
        //static AtlasPage GetPageFor(Texture texture, Material newMaterial) {
        //    if (atlasPageCache == null) atlasPageCache = new Dictionary<Texture, AtlasPage>();
        //    AtlasPage atlasPage;
        //    atlasPageCache.TryGetValue(texture, out atlasPage);
        //    if (atlasPage == null) 
        //    {
        //        atlasPage = newMaterial.ToSpineAtlasPage();
        //        atlasPageCache[texture] = atlasPage;
        //    }
        //    return atlasPage;
        //}

        private void Start()
        {
            // Initialize slot and attachment references.
            Initialize(false);

            if (attachOnStart)
                Attach();
        }

        private void OnDestroy()
        {
            var animatedSkeleton = GetComponent<ISkeletonAnimation>();
            if (animatedSkeleton != null)
                animatedSkeleton.UpdateComplete -= AnimationOverrideSpriteAttach;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var skeletonComponent = GetComponent<ISkeletonComponent>();
            var skeletonRenderer = skeletonComponent as SkeletonRenderer;
            bool applyPMA;

            if (skeletonRenderer != null)
            {
                applyPMA = skeletonRenderer.pmaVertexColors;
            }
            else
            {
                var skeletonGraphic = skeletonComponent as SkeletonGraphic;
                applyPMA = skeletonGraphic != null && skeletonGraphic.MeshGenerator.settings.pmaVertexColors;
            }

            if (applyPMA)
            {
                if (sprites == null) return;
                for (var i = 0; i < sprites.Length; i++)
                    try
                    {
                        if (sprites[i] == null)
                            return;
                        sprites[i].texture.GetPixel(0, 0);
                    }
                    catch (UnityException e)
                    {
                        Debug.LogFormat(
                            "Texture of {0} ({1}) is not read/write enabled. SpriteAttacher requires this in order to work with a SkeletonRenderer that renders premultiplied alpha. Please check the texture settings.",
                            sprites[i].name, sprites[i].texture.name);
                        EditorGUIUtility.PingObject(sprites[i].texture);
                        throw e;
                    }
            }
        }
#endif

        private void AnimationOverrideSpriteAttach(ISkeletonAnimation animated)
        {
            if (overrideAnimation && isActiveAndEnabled)
                Attach();
        }

        public void Initialize(bool overwrite = true)
        {
            if (overwrite || attachments == null)
            {
                // Get the applyPMA value.
                var skeletonComponent = GetComponent<ISkeletonComponent>();
                var skeletonRenderer = skeletonComponent as SkeletonRenderer;
                if (skeletonRenderer != null)
                {
                    applyPMA = skeletonRenderer.pmaVertexColors;
                }
                else
                {
                    var skeletonGraphic = skeletonComponent as SkeletonGraphic;
                    if (skeletonGraphic != null)
                        applyPMA = skeletonGraphic.MeshGenerator.settings.pmaVertexColors;
                }

                // Subscribe to UpdateComplete to override animation keys.
                if (overrideAnimation)
                {
                    var animatedSkeleton = skeletonComponent as ISkeletonAnimation;
                    if (animatedSkeleton != null)
                    {
                        animatedSkeleton.UpdateComplete -= AnimationOverrideSpriteAttach;
                        animatedSkeleton.UpdateComplete += AnimationOverrideSpriteAttach;
                    }
                }

                if (slots == null || sprites == null)
                {
                    spineSlots = null;
                    attachments = null;
                    return;
                }

                spineSlots = new Slot[slots.Length];
                attachments = new RegionAttachment[sprites.Length];

                for (var i = 0; i < slots.Length; i++)
                {
                    spineSlots[i] ??= skeletonComponent.Skeleton.FindSlot(slots[i]);
                    if (sprites[i] == null)
                        attachments[i] = null;
                    else
                        attachments[i] = sprites[i].ToRegionAttachment(materials[i]);
                }
            }
        }

        public void RemoveAll()
        {
            if (spineSlots == null) return;
            foreach (var t in spineSlots)
                if (t != null)
                    t.Attachment = null;

            sprites = null;
            materials = null;
            slots = null;
            spineSlots = null;
            attachments = null;
        }

        /// <summary>Update the slot's attachment to the Attachment generated from the sprite.</summary>
        public void Attach()
        {
            if (spineSlots == null) return;

            for (var i = 0; i < spineSlots.Length; i++)
                if (spineSlots[i] != null)
                    spineSlots[i].Attachment = attachments[i];
        }

        #region Inspector

        public bool attachOnStart = true;
        public bool overrideAnimation = true;
        public Sprite[] sprites;
        public Material[] materials;
        [SpineSlot] public string[] slots;

        #endregion
    }
}