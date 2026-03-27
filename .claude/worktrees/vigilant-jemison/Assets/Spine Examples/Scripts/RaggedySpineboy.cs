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

using System.Collections;
using UnityEngine;

namespace Spine.Unity.Examples
{
    public class RaggedySpineboy : MonoBehaviour
    {
        public LayerMask groundMask;
        public float restoreDuration = 0.5f;
        public Vector2 launchVelocity = new(50, 100);
        private Collider2D naturalCollider;

        private SkeletonRagdoll2D ragdoll;

        private void Start()
        {
            ragdoll = GetComponent<SkeletonRagdoll2D>();
            naturalCollider = GetComponent<Collider2D>();
        }

        private void OnMouseUp()
        {
            if (naturalCollider.enabled)
                Launch();
        }

        private void AddRigidbody()
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            naturalCollider.enabled = true;
        }

        private void RemoveRigidbody()
        {
            Destroy(GetComponent<Rigidbody2D>());
            naturalCollider.enabled = false;
        }

        private void Launch()
        {
            RemoveRigidbody();
            ragdoll.Apply();
            ragdoll.RootRigidbody.linearVelocity =
                new Vector2(Random.Range(-launchVelocity.x, launchVelocity.x), launchVelocity.y);
            StartCoroutine(WaitUntilStopped());
        }

        private IEnumerator Restore()
        {
            var estimatedPos = ragdoll.EstimatedSkeletonPosition;
            Vector3 rbPosition = ragdoll.RootRigidbody.position;

            var skeletonPoint = estimatedPos;
            var hit = Physics2D.Raycast(rbPosition, estimatedPos - rbPosition,
                Vector3.Distance(estimatedPos, rbPosition), groundMask);
            if (hit.collider != null)
                skeletonPoint = hit.point;

            ragdoll.RootRigidbody.isKinematic = true;
            ragdoll.SetSkeletonPosition(skeletonPoint);

            yield return ragdoll.SmoothMix(0, restoreDuration);
            ragdoll.Remove();

            AddRigidbody();
        }

        private IEnumerator WaitUntilStopped()
        {
            yield return new WaitForSeconds(0.5f);

            float t = 0;
            while (t < 0.5f)
            {
                t = ragdoll.RootRigidbody.linearVelocity.magnitude > 0.09f ? 0 : t + Time.deltaTime;
                yield return null;
            }

            StartCoroutine(Restore());
        }
    }
}