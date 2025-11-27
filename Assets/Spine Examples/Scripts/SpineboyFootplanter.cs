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

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spine.Unity.Examples
{
    public class SpineboyFootplanter : MonoBehaviour
    {
        public float timeScale = 0.5f;
        [SpineBone] public string nearBoneName, farBoneName;

        [Header("Settings")] public Vector2 footSize;

        public float footRayRaise = 2f;
        public float comfyDistance = 1f;
        public float centerOfGravityXOffset = -0.25f;
        public float feetTooFarApartThreshold = 3f;
        public float offBalanceThreshold = 1.4f;
        public float minimumSpaceBetweenFeet = 0.5f;
        public float maxNewStepDisplacement = 2f;
        public float shuffleDistance = 1f;
        public float baseLerpSpeed = 3.5f;
        public FootMovement forward, backward;

        [Header("Debug")] [SerializeField] private float balance;

        [SerializeField] private float distanceBetweenFeet;
        [SerializeField] protected Foot nearFoot, farFoot;

        private readonly RaycastHit2D[] hits = new RaycastHit2D[1];
        private Bone nearFootBone, farFootBone;

        private Skeleton skeleton;

        public float Balance => balance;

        private void Start()
        {
            Time.timeScale = timeScale;
            var tpos = transform.position;

            // Default starting positions.
            nearFoot.worldPos = tpos;
            nearFoot.worldPos.x -= comfyDistance;
            nearFoot.worldPosPrev = nearFoot.worldPosNext = nearFoot.worldPos;

            farFoot.worldPos = tpos;
            farFoot.worldPos.x += comfyDistance;
            farFoot.worldPosPrev = farFoot.worldPosNext = farFoot.worldPos;

            var skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeleton = skeletonAnimation.Skeleton;

            skeletonAnimation.UpdateLocal += UpdateLocal;

            nearFootBone = skeleton.FindBone(nearBoneName);
            farFootBone = skeleton.FindBone(farBoneName);

            nearFoot.lerp = 1f;
            farFoot.lerp = 1f;
        }


        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                const float Radius = 0.15f;

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(nearFoot.worldPos, Radius);
                Gizmos.DrawWireSphere(nearFoot.worldPosNext, Radius);

                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(farFoot.worldPos, Radius);
                Gizmos.DrawWireSphere(farFoot.worldPosNext, Radius);
            }
        }

        private void UpdateLocal(ISkeletonAnimation animated)
        {
            var thisTransform = transform;

            Vector2 thisTransformPosition = thisTransform.position;
            var centerOfGravityX = thisTransformPosition.x + centerOfGravityXOffset;

            nearFoot.UpdateDistance(centerOfGravityX);
            farFoot.UpdateDistance(centerOfGravityX);
            balance = nearFoot.displacementFromCenter + farFoot.displacementFromCenter;
            distanceBetweenFeet = Mathf.Abs(nearFoot.worldPos.x - farFoot.worldPos.x);

            // Detect time to make a new step
            var isTooOffBalance = Mathf.Abs(balance) > offBalanceThreshold;
            var isFeetTooFarApart = distanceBetweenFeet > feetTooFarApartThreshold;
            var timeForNewStep = isFeetTooFarApart || isTooOffBalance;
            if (timeForNewStep)
            {
                // Choose which foot to use for next step.
                Foot stepFoot, otherFoot;
                var stepLegIsNearLeg = nearFoot.distanceFromCenter > farFoot.distanceFromCenter;
                if (stepLegIsNearLeg)
                {
                    stepFoot = nearFoot;
                    otherFoot = farFoot;
                }
                else
                {
                    stepFoot = farFoot;
                    otherFoot = nearFoot;
                }

                // Start a new step.
                if (!stepFoot.IsStepInProgress && otherFoot.IsPrettyMuchDoneStepping)
                {
                    var newDisplacement = Foot.GetNewDisplacement(otherFoot.displacementFromCenter, comfyDistance,
                        minimumSpaceBetweenFeet, maxNewStepDisplacement, forward, backward);
                    stepFoot.StartNewStep(newDisplacement, centerOfGravityX, thisTransformPosition.y, footRayRaise,
                        hits, footSize);
                }
            }


            var deltaTime = Time.deltaTime;
            var stepSpeed = baseLerpSpeed;
            stepSpeed += (Mathf.Abs(balance) - 0.6f) * 2.5f;

            // Animate steps that are in progress.
            nearFoot.UpdateStepProgress(deltaTime, stepSpeed, shuffleDistance, forward, backward);
            farFoot.UpdateStepProgress(deltaTime, stepSpeed, shuffleDistance, forward, backward);

            nearFootBone.SetLocalPosition(thisTransform.InverseTransformPoint(nearFoot.worldPos));
            farFootBone.SetLocalPosition(thisTransform.InverseTransformPoint(farFoot.worldPos));
        }

        [Serializable]
        public class FootMovement
        {
            public AnimationCurve xMoveCurve;
            public AnimationCurve raiseCurve;
            public float maxRaise;
            public float minDistanceCompensate;
            public float maxDistanceCompensate;
        }

        [Serializable]
        public class Foot
        {
            public Vector2 worldPos;
            public float displacementFromCenter;
            public float distanceFromCenter;

            [Space] public float lerp;

            public Vector2 worldPosPrev;
            public Vector2 worldPosNext;

            public bool IsStepInProgress => lerp < 1f;
            public bool IsPrettyMuchDoneStepping => lerp > 0.7f;

            public void UpdateDistance(float centerOfGravityX)
            {
                displacementFromCenter = worldPos.x - centerOfGravityX;
                distanceFromCenter = Mathf.Abs(displacementFromCenter);
            }

            public void StartNewStep(float newDistance, float centerOfGravityX, float tentativeY, float footRayRaise,
                RaycastHit2D[] hits, Vector2 footSize)
            {
                lerp = 0f;
                worldPosPrev = worldPos;
                var newX = centerOfGravityX - newDistance;
                var origin = new Vector2(newX, tentativeY + footRayRaise);
                //int hitCount = Physics2D.BoxCastNonAlloc(origin, footSize, 0f, Vector2.down, hits);
                var hitCount = Physics2D.BoxCast(origin, footSize, 0f, Vector2.down,
                    new ContactFilter2D { useTriggers = false }, hits);
                worldPosNext = hitCount > 0 ? hits[0].point : new Vector2(newX, tentativeY);
            }

            public void UpdateStepProgress(float deltaTime, float stepSpeed, float shuffleDistance,
                FootMovement forwardMovement, FootMovement backwardMovement)
            {
                if (!IsStepInProgress)
                    return;

                lerp += deltaTime * stepSpeed;

                var strideSignedSize = worldPosNext.x - worldPosPrev.x;
                var strideSign = Mathf.Sign(strideSignedSize);
                var strideSize = Mathf.Abs(strideSignedSize);

                var movement = strideSign > 0 ? forwardMovement : backwardMovement;

                worldPos.x = Mathf.Lerp(worldPosPrev.x, worldPosNext.x, movement.xMoveCurve.Evaluate(lerp));
                var groundLevel = Mathf.Lerp(worldPosPrev.y, worldPosNext.y, lerp);

                if (strideSize > shuffleDistance)
                {
                    var strideSizeFootRaise = Mathf.Clamp(strideSize * 0.5f, 1f, 2f);
                    worldPos.y = groundLevel +
                                 movement.raiseCurve.Evaluate(lerp) * movement.maxRaise * strideSizeFootRaise;
                }
                else
                {
                    lerp += Time.deltaTime;
                    worldPos.y = groundLevel;
                }

                if (lerp > 1f)
                    lerp = 1f;
            }

            public static float GetNewDisplacement(float otherLegDisplacementFromCenter, float comfyDistance,
                float minimumFootDistanceX, float maxNewStepDisplacement, FootMovement forwardMovement,
                FootMovement backwardMovement)
            {
                var movement = Mathf.Sign(otherLegDisplacementFromCenter) < 0 ? forwardMovement : backwardMovement;
                var randomCompensate = Random.Range(movement.minDistanceCompensate, movement.maxDistanceCompensate);

                var newDisplacement = otherLegDisplacementFromCenter * randomCompensate;
                if (Mathf.Abs(newDisplacement) > maxNewStepDisplacement ||
                    Mathf.Abs(otherLegDisplacementFromCenter) < minimumFootDistanceX)
                    newDisplacement = comfyDistance * Mathf.Sign(newDisplacement) * randomCompensate;

                return newDisplacement;
            }
        }
    }
}