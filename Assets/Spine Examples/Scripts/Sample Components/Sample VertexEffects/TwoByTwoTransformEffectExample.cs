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

using Spine.Unity.Examples;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Examples
{
    // This is a sample component for C# vertex effects for Spine rendering components.
    // Using shaders and materials to control vertex properties is still more performant
    // than using this API, but in cases where your vertex effect logic cannot be
    // expressed as shader code, these vertex effects can be useful.
    public class TwoByTwoTransformEffectExample : MonoBehaviour
    {
        public Vector2 xAxis = new(1, 0);
        public Vector2 yAxis = new(0, 1);

        private SkeletonRenderer skeletonRenderer;

        private void OnEnable()
        {
            skeletonRenderer = GetComponent<SkeletonRenderer>();
            if (skeletonRenderer == null) return;

            // Use the OnPostProcessVertices callback to modify the vertices at the correct time.
            skeletonRenderer.OnPostProcessVertices -= ProcessVertices;
            skeletonRenderer.OnPostProcessVertices += ProcessVertices;

            Debug.Log("2x2 Transform Effect Enabled.");
        }

        private void OnDisable()
        {
            if (skeletonRenderer == null) return;
            skeletonRenderer.OnPostProcessVertices -= ProcessVertices;
            Debug.Log("2x2 Transform Effect Disabled.");
        }

        private void ProcessVertices(MeshGeneratorBuffers buffers)
        {
            if (!enabled)
                return;

            var vertexCount =
                buffers.vertexCount; // For efficiency, limit your effect to the actual mesh vertex count using vertexCount

            // Modify vertex positions by accessing Vector3[] vertexBuffer
            var vertices = buffers.vertexBuffer;
            var transformedPos = default(Vector3);
            for (var i = 0; i < vertexCount; i++)
            {
                var originalPos = vertices[i];
                transformedPos.x = xAxis.x * originalPos.x + yAxis.x * originalPos.y;
                transformedPos.y = xAxis.y * originalPos.x + yAxis.y * originalPos.y;
                vertices[i] = transformedPos;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TwoByTwoTransformEffectExample))]
public class TwoByTwoTransformEffectExampleEditor : Editor
{
    private TwoByTwoTransformEffectExample Target => target as TwoByTwoTransformEffectExample;

    private void OnSceneGUI()
    {
        var transform = Target.transform;
        LocalVectorHandle(ref Target.xAxis, transform, Color.red);
        LocalVectorHandle(ref Target.yAxis, transform, Color.green);
    }

    private static void LocalVectorHandle(ref Vector2 v, Transform transform, Color color)
    {
        var originalColor = Handles.color;
        Handles.color = color;
        Handles.DrawLine(transform.position, transform.TransformPoint(v));
#if UNITY_2022_1_OR_NEWER
        v = transform.InverseTransformPoint(Handles.FreeMoveHandle(transform.TransformPoint(v), 0.3f, Vector3.zero,
            Handles.CubeHandleCap));
#else
		v =
 transform.InverseTransformPoint(UnityEditor.Handles.FreeMoveHandle(transform.TransformPoint(v), Quaternion.identity, 0.3f, Vector3.zero, UnityEditor.Handles.CubeHandleCap));
#endif
        Handles.color = originalColor;
    }
}
#endif