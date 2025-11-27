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
using UnityEngine;

namespace Spine.Unity.Examples
{
    public class SkeletonGhostRenderer : MonoBehaviour
    {
        private const string colorPropertyName = "_Color";
        private static readonly Color32 TransparentBlack = new(0, 0, 0, 0);
        private int colorId;

        private float fadeSpeed = 10;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private MaterialPropertyBlock mpb;
        private Color32 startColor;

        private void Awake()
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshFilter = gameObject.AddComponent<MeshFilter>();

            colorId = Shader.PropertyToID(colorPropertyName);
            mpb = new MaterialPropertyBlock();
        }

        public void Initialize(Mesh mesh, Material[] materials, Color32 color, bool additive, float speed,
            int sortingLayerID, int sortingOrder)
        {
            StopAllCoroutines();

            gameObject.SetActive(true);
            meshRenderer.sharedMaterials = materials;
            meshRenderer.sortingLayerID = sortingLayerID;
            meshRenderer.sortingOrder = sortingOrder;
            meshFilter.sharedMesh = Instantiate(mesh);
            startColor = color;
            mpb.SetColor(colorId, color);
            meshRenderer.SetPropertyBlock(mpb);

            fadeSpeed = speed;

            if (additive)
                StartCoroutine(FadeAdditive());
            else
                StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            var c = startColor;
            var black = TransparentBlack;

            var t = 1f;
            for (var hardTimeLimit = 5f; hardTimeLimit > 0; hardTimeLimit -= Time.deltaTime)
            {
                c = Color32.Lerp(black, startColor, t);
                mpb.SetColor(colorId, c);
                meshRenderer.SetPropertyBlock(mpb);

                t = Mathf.Lerp(t, 0, Time.deltaTime * fadeSpeed);
                if (t <= 0)
                    break;

                yield return null;
            }

            Destroy(meshFilter.sharedMesh);
            gameObject.SetActive(false);
        }

        private IEnumerator FadeAdditive()
        {
            var c = startColor;
            var black = TransparentBlack;

            var t = 1f;

            for (var hardTimeLimit = 5f; hardTimeLimit > 0; hardTimeLimit -= Time.deltaTime)
            {
                c = Color32.Lerp(black, startColor, t);
                mpb.SetColor(colorId, c);
                meshRenderer.SetPropertyBlock(mpb);

                t = Mathf.Lerp(t, 0, Time.deltaTime * fadeSpeed);
                if (t <= 0)
                    break;

                yield return null;
            }

            Destroy(meshFilter.sharedMesh);

            gameObject.SetActive(false);
        }

        public void Cleanup()
        {
            if (meshFilter != null && meshFilter.sharedMesh != null)
                Destroy(meshFilter.sharedMesh);

            Destroy(gameObject);
        }
    }
}