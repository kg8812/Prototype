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

// Original Contribution by: Mitch Thompson

using System;
using System.Collections.Generic;
using Spine.Unity.AttachmentTools;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Examples
{
    public class SpriteAttacher : MonoBehaviour
    {
        public const string DefaultPMAShader = "Spine/Skeleton";
        public const string DefaultStraightAlphaShader = "Sprites/Default";

        private static Dictionary<Texture, AtlasPage> atlasPageCache;
        private bool applyPMA;

        private RegionAttachment attachment;
        private Slot spineSlot;

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
                try
                {
                    if (sprite == null)
                        return;
                    sprite.texture.GetPixel(0, 0);
                }
                catch (UnityException e)
                {
                    Debug.LogFormat(
                        "Texture of {0} ({1}) is not read/write enabled. SpriteAttacher requires this in order to work with a SkeletonRenderer that renders premultiplied alpha. Please check the texture settings.",
                        sprite.name, sprite.texture.name);
                    EditorGUIUtility.PingObject(sprite.texture);
                    throw e;
                }
        }
#endif
        private static AtlasPage GetPageFor(Texture texture, Shader shader)
        {
            if (atlasPageCache == null) atlasPageCache = new Dictionary<Texture, AtlasPage>();
            AtlasPage atlasPage;
            atlasPageCache.TryGetValue(texture, out atlasPage);
            if (atlasPage == null)
            {
                var newMaterial = new Material(shader);
                atlasPage = newMaterial.ToSpineAtlasPage();
                atlasPageCache[texture] = atlasPage;
            }

            return atlasPage;
        }

        private void AnimationOverrideSpriteAttach(ISkeletonAnimation animated)
        {
            if (overrideAnimation && isActiveAndEnabled)
                Attach();
        }

        public void Initialize(bool overwrite = true)
        {
            if (overwrite || attachment == null)
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

                spineSlot = spineSlot ?? skeletonComponent.Skeleton.FindSlot(slot);
                var attachmentShader =
                    applyPMA ? Shader.Find(DefaultPMAShader) : Shader.Find(DefaultStraightAlphaShader);
                if (sprite == null)
                    attachment = null;
                else
                    attachment = applyPMA
                        ? sprite.ToRegionAttachmentPMAClone(attachmentShader)
                        : sprite.ToRegionAttachment(GetPageFor(sprite.texture, attachmentShader));
            }
        }

        /// <summary>Update the slot's attachment to the Attachment generated from the sprite.</summary>
        public void Attach()
        {
            if (spineSlot != null)
                spineSlot.Attachment = attachment;
        }

        #region Inspector

        public bool attachOnStart = true;
        public bool overrideAnimation = true;
        public Sprite sprite;
        [SpineSlot] public string slot;

        #endregion
    }


    public static class SpriteAttachmentExtensions
    {
        [Obsolete]
        public static RegionAttachment AttachUnitySprite(this Skeleton skeleton, string slotName, Sprite sprite,
            string shaderName = SpriteAttacher.DefaultPMAShader, bool applyPMA = true, float rotation = 0f)
        {
            return skeleton.AttachUnitySprite(slotName, sprite, Shader.Find(shaderName), applyPMA, rotation);
        }

        [Obsolete]
        public static RegionAttachment AddUnitySprite(this SkeletonData skeletonData, string slotName, Sprite sprite,
            string skinName = "", string shaderName = SpriteAttacher.DefaultPMAShader, bool applyPMA = true,
            float rotation = 0f)
        {
            return skeletonData.AddUnitySprite(slotName, sprite, skinName, Shader.Find(shaderName), applyPMA, rotation);
        }

        [Obsolete]
        public static RegionAttachment AttachUnitySprite(this Skeleton skeleton, string slotName, Sprite sprite,
            Shader shader, bool applyPMA, float rotation = 0f)
        {
            var att = applyPMA
                ? sprite.ToRegionAttachmentPMAClone(shader, rotation: rotation)
                : sprite.ToRegionAttachment(new Material(shader), rotation);
            skeleton.FindSlot(slotName).Attachment = att;
            return att;
        }

        [Obsolete]
        public static RegionAttachment AddUnitySprite(this SkeletonData skeletonData, string slotName, Sprite sprite,
            string skinName, Shader shader, bool applyPMA, float rotation = 0f)
        {
            var att = applyPMA
                ? sprite.ToRegionAttachmentPMAClone(shader, rotation: rotation)
                : sprite.ToRegionAttachment(new Material(shader), rotation);

            var slotIndex = skeletonData.FindSlot(slotName).Index;
            var skin = skeletonData.DefaultSkin;
            if (skinName != "")
                skin = skeletonData.FindSkin(skinName);

            if (skin != null)
                skin.SetAttachment(slotIndex, att.Name, att);

            return att;
        }
    }
}