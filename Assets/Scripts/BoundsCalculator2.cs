#region Assembly MixedReality.Toolkit.SpatialManipulation, Version=3.3.1.0, Culture=neutral, PublicKeyToken=null
// D:\App_Dev\unity\WIP 3D Cutout\Library\ScriptAssemblies\MixedReality.Toolkit.SpatialManipulation.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System.Collections.Generic;
using UnityEngine;

namespace MixedReality.Toolkit.SpatialManipulation
{
    public static class BoundsCalculator2
    {
        public enum BoundsCalculationMethod
        {
            RendererOverCollider,
            ColliderOverRenderer,
            ColliderOnly,
            RendererOnly
        }

        private static List<Vector3> totalBoundsCorners = new List<Vector3>(8);

        private static List<Transform> childTransforms = new List<Transform>();

        private static Vector3[] cornersToWorld = new Vector3[8];

        internal static Bounds CalculateBounds(Transform root, Transform target, Transform exclude, out bool containsCanvas, BoundsCalculationMethod boundsCalculationMethod = BoundsCalculationMethod.RendererOverCollider, bool includeInactiveObjects = false, bool abortOnCanvas = false)
        {
            totalBoundsCorners.Clear();
            childTransforms.Clear();
            containsCanvas = false;
            target.GetComponentsInChildren(includeInactiveObjects, childTransforms);
            foreach (Transform childTransform in childTransforms)
            {
                if (!childTransform.IsChildOf(exclude))
                {
                    containsCanvas |= childTransform is RectTransform;
                    if (containsCanvas && abortOnCanvas)
                    {
                        break;
                    }

                    ExtractBoundsCorners(childTransform, boundsCalculationMethod);
                }
            }

            if (totalBoundsCorners.Count == 0)
            {
                return default(Bounds);
            }

            Bounds result = new Bounds(root.InverseTransformPoint(totalBoundsCorners[0]), Vector3.zero);
            for (int i = 1; i < totalBoundsCorners.Count; i++)
            {
                result.Encapsulate(root.InverseTransformPoint(totalBoundsCorners[i]));
            }

            return result;
        }

        public static Vector3 CalculateFlattenVector(Vector3 size)
        {
            if (size.x < size.y && size.x < size.z)
            {
                return Vector3.right;
            }

            if (size.y < size.x && size.y < size.z)
            {
                return Vector3.up;
            }

            return Vector3.forward;
        }

        private static void ExtractBoundsCorners(Transform childTransform, BoundsCalculationMethod boundsCalculationMethod)
        {
            KeyValuePair<Transform, Collider> colliderByTransform = default(KeyValuePair<Transform, Collider>);
            KeyValuePair<Transform, Bounds> rendererBoundsByTarget = default(KeyValuePair<Transform, Bounds>);
            if (boundsCalculationMethod != BoundsCalculationMethod.RendererOnly)
            {
                colliderByTransform = ((!childTransform.TryGetComponent<Collider>(out var component)) ? default(KeyValuePair<Transform, Collider>) : new KeyValuePair<Transform, Collider>(childTransform, component));
            }

            if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
            {
                MeshFilter component2 = childTransform.GetComponent<MeshFilter>();
                rendererBoundsByTarget = ((component2 != null && component2.sharedMesh != null) ? new KeyValuePair<Transform, Bounds>(childTransform, component2.sharedMesh.bounds) : ((!(childTransform is RectTransform rectTransform)) ? default(KeyValuePair<Transform, Bounds>) : new KeyValuePair<Transform, Bounds>(childTransform, new Bounds(rectTransform.rect.center, new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f)))));
            }

            if (((boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly && boundsCalculationMethod != BoundsCalculationMethod.ColliderOverRenderer) || ((!AddColliderBoundsCornersToTarget(colliderByTransform) || boundsCalculationMethod != BoundsCalculationMethod.ColliderOverRenderer) && boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)) && (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly || ((!AddRendererBoundsCornersToTarget(rendererBoundsByTarget) || boundsCalculationMethod != 0) && boundsCalculationMethod != BoundsCalculationMethod.RendererOnly)))
            {
                AddColliderBoundsCornersToTarget(colliderByTransform);
            }
        }

        private static bool AddRendererBoundsCornersToTarget(KeyValuePair<Transform, Bounds> rendererBoundsByTarget)
        {
            if (rendererBoundsByTarget.Key == null)
            {
                return false;
            }

            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            totalBoundsCorners.AddRange(cornersToWorld);
            return true;
        }

        private static bool AddColliderBoundsCornersToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            if (colliderByTransform.Key != null)
            {
                BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, totalBoundsCorners, 0);
            }

            return colliderByTransform.Key != null;
        }
    }
#if false // Decompilation log
    '287' items in cache
    ------------------
    Resolve: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
    Found single assembly: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
    Load from: 'C:\Program Files\Unity\Hub\Editor\2022.3.31f1\Editor\Data\NetStandard\ref\2.1.0\netstandard.dll'
    ------------------
    Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Load from: 'C:\Program Files\Unity\Hub\Editor\2022.3.31f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll'
    ------------------
    Resolve: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Load from: 'C:\Program Files\Unity\Hub\Editor\2022.3.31f1\Editor\Data\Managed\UnityEngine\UnityEngine.PhysicsModule.dll'
    ------------------
    Resolve: 'MixedReality.Toolkit.Core, Version=3.2.2.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'MixedReality.Toolkit.Core, Version=3.2.2.0, Culture=neutral, PublicKeyToken=null'
    Load from: 'D:\App_Dev\unity\WIP 3D Cutout\Library\ScriptAssemblies\MixedReality.Toolkit.Core.dll'
    ------------------
    Resolve: 'Unity.XR.Interaction.Toolkit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'Unity.XR.Interaction.Toolkit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Load from: 'D:\App_Dev\unity\WIP 3D Cutout\Library\ScriptAssemblies\Unity.XR.Interaction.Toolkit.dll'
    ------------------
    Resolve: 'Unity.InputSystem, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'Unity.InputSystem, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null'
    Load from: 'D:\App_Dev\unity\WIP 3D Cutout\Library\ScriptAssemblies\Unity.InputSystem.dll'
    ------------------
    Resolve: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
    Load from: 'C:\Program Files\Unity\Hub\Editor\2022.3.31f1\Editor\Data\Managed\UnityEngine\UnityEngine.XRModule.dll'
    ------------------
    Resolve: 'System.Runtime.InteropServices, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
    Found single assembly: 'System.Runtime.InteropServices, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
    WARN: Version mismatch. Expected: '2.1.0.0', Got: '4.1.2.0'
    Load from: 'C:\Program Files\Unity\Hub\Editor\2022.3.31f1\Editor\Data\NetStandard\compat\2.1.0\shims\netstandard\System.Runtime.InteropServices.dll'
    ------------------
    Resolve: 'System.Runtime.CompilerServices.Unsafe, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
    Could not find by name: 'System.Runtime.CompilerServices.Unsafe, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
#endif
}
