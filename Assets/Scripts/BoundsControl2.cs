#region Assembly MixedReality.Toolkit.SpatialManipulation, Version=3.3.1.0, Culture=neutral, PublicKeyToken=null
// D:\App_Dev\unity\WIP 3D Cutout\Library\ScriptAssemblies\MixedReality.Toolkit.SpatialManipulation.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MixedReality.Toolkit.SpatialManipulation
{
    [RequireComponent(typeof(ConstraintManager))]
    public class BoundsControl2 : MonoBehaviour
    {
        protected struct LogicImplementation
        {
            public ManipulationLogic<Vector3> moveLogic;

            public ManipulationLogic<Quaternion> rotateLogic;

            public ManipulationLogic<Vector3> scaleLogic;
        }

        [Header("Bounds")]
        [SerializeField]
        [Tooltip("This prefab will be instantiated as the bounds visuals. Consider making your own prefab to modify how the visuals are drawn.")]
        private GameObject boundsVisualsPrefab;

        [SerializeField]
        [Tooltip("How should the bounds be automatically calculated?")]
        private BoundsCalculator2.BoundsCalculationMethod boundsCalculationMethod;

        [SerializeField]
        [Tooltip("Should BoundsControl include inactive objects when it traverses the hierarchy to calculate bounds?")]
        private bool includeInactiveObjects;

        [SerializeField]
        [Tooltip("Should BoundsControl use a specific object to calculate bounds, instead of the entire hierarchy?")]
        private bool overrideBounds;

        [SerializeField]
        [DrawIf("overrideBounds", null, DrawIfAttribute.ComparisonType.Equal)]
        [Tooltip("The bounds will be calculated from this object and this object only, instead of the entire hierarchy.")]
        private Transform boundsOverride;

        [SerializeField]
        [Tooltip("How should this BoundsControl flatten?")]
        private FlattenMode flattenMode = FlattenMode.Auto;

        [SerializeField]
        [Tooltip("The bounds will be padded around the extent of the object by this amount, in world units.")]
        private float boundsPadding = 0.01f;

        [Header("Interactable Connection")]
        [SerializeField]
        [Tooltip("Reference to the interactable (such as ObjectManipulator) in charge of the wrapped object")]
        private StatefulInteractable interactable;

        [SerializeField]
        [Tooltip("Toggle the handles when the interactable is selected, not moved, and then released.")]
        private bool toggleHandlesOnClick = true;

        [SerializeField]
        [DrawIf("toggleHandlesOnClick", null, DrawIfAttribute.ComparisonType.Equal)]
        [Tooltip("During a selection of the associated interactable, if the interactable is dragged/moved a smaller distance than this value, the handles will be activated/deactivated.")]
        private float dragToggleThreshold = 0.02f;

        [Header("Manipulation")]
        [SerializeField]
        [Tooltip("The transform to be manipulated.")]
        private Transform target;

        [SerializeField]
        [Tooltip("Should any handles be visible?")]
        private bool handlesActive;

        [SerializeField]
        [Tooltip("Which type of handles should be visible?")]
        private HandleType enabledHandles = HandleType.Rotation | HandleType.Scale;

        [EnumFlags]
        [SerializeField]
        [Tooltip("Specifies whether the rotate handles will rotate the object around its origin, or the center of its calculated bounds.")]
        private RotateAnchorType rotateAnchor = RotateAnchorType.BoundsCenter;

        [EnumFlags]
        [SerializeField]
        [Tooltip("Specifies whether the scale handles will rotate the object around their opposing corner, or the center of its calculated bounds.")]
        private ScaleAnchorType scaleAnchor;

        [SerializeField]
        [Tooltip("Scale mode that is applied when interacting with scale handles - default is uniform scaling. Non uniform mode scales the control according to hand / controller movement in space.")]
        private HandleScaleMode scaleBehavior;

        [Header("Modifiers")]
        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing.")]
        private bool smoothingActive = true;

        [SerializeField]
        [DrawIf("smoothingActive", null, DrawIfAttribute.ComparisonType.Equal)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float rotateLerpTime = 1E-05f;

        [SerializeField]
        [DrawIf("smoothingActive", null, DrawIfAttribute.ComparisonType.Equal)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float scaleLerpTime = 1E-05f;

        [SerializeField]
        [DrawIf("smoothingActive", null, DrawIfAttribute.ComparisonType.Equal)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the translation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float translateLerpTime = 1E-05f;

        [SerializeField]
        [Tooltip("Enable or disable constraint support of this component. When enabled, transform changes will be post processed by the linked constraint manager.")]
        private bool enableConstraints = true;

        [SerializeField]
        [DrawIf("enableConstraints", null, DrawIfAttribute.ComparisonType.Equal)]
        [Tooltip("Constraint manager slot to enable constraints when manipulating the object.")]
        private ConstraintManager constraintsManager;

        [SerializeField]
        [Tooltip("The concrete types of ManipulationLogic<T> to use for manipulations.")]
        private ObjectManipulator.LogicType manipulationLogicTypes = new ObjectManipulator.LogicType
        {
            moveLogicType = typeof(BoundsControlMoveLogic),
            rotateLogicType = typeof(BoundsControlRotateLogic),
            scaleLogicType = typeof(BoundsControlScaleLogic)
        };

        [Header("Events")]
        [SerializeField]
        private SelectEnterEvent manipulationStarted = new SelectEnterEvent();

        [SerializeField]
        private SelectExitEvent manipulationEnded = new SelectExitEvent();

        private Bounds currentBounds;

        private GameObject boxInstance;

        private Vector3 startMovePosition;

        private BoundsHandleInteractable currentHandle;

        private Vector3 flattenVector;

        private MixedRealityTransform initialTransformOnGrabStart;

        private Vector3 oppositeCorner;

        private Vector3 initialAnchorOnGrabStart;

        private Vector3 initialAnchorDeltaOnGrabStart;

        private bool needsBoundsRecompute;

        private int waitForFrames = 1;

        private Vector3 minimumScale;

        private const float lowerAbsoluteClamp = 0.001f;

        private bool isHostSelected;

        private bool hasPassedToggleThreshold;

        private static readonly ProfilerMarker TransformTargetPerfMarker = new ProfilerMarker("[MRTK] BoundsControl.TransformTarget");

        public GameObject BoundsVisualsPrefab
        {
            get
            {
                return boundsVisualsPrefab;
            }
            set
            {
                if (value != boundsVisualsPrefab)
                {
                    boundsVisualsPrefab = value;
                    CreateBoundsVisuals();
                }
            }
        }

        public BoundsCalculator2.BoundsCalculationMethod BoundsCalculationMethod
        {
            get
            {
                return boundsCalculationMethod;
            }
            set
            {
                boundsCalculationMethod = value;
                needsBoundsRecompute = ComputeBounds();
            }
        }

        public bool IncludeInactiveObjects
        {
            get
            {
                return includeInactiveObjects;
            }
            set
            {
                if (includeInactiveObjects != value)
                {
                    includeInactiveObjects = value;
                    needsBoundsRecompute = ComputeBounds();
                }
            }
        }

        public bool OverrideBounds
        {
            get
            {
                return overrideBounds;
            }
            set
            {
                if (overrideBounds != value)
                {
                    overrideBounds = value;
                    needsBoundsRecompute = ComputeBounds();
                }
            }
        }

        public Transform BoundsOverride
        {
            get
            {
                return boundsOverride;
            }
            set
            {
                if (value != boundsOverride)
                {
                    boundsOverride = value;
                    needsBoundsRecompute = true;
                }
            }
        }

        public FlattenMode FlattenMode
        {
            get
            {
                return flattenMode;
            }
            set
            {
                flattenMode = value;
                needsBoundsRecompute = true;
            }
        }

        public float BoundsPadding
        {
            get
            {
                return boundsPadding;
            }
            set
            {
                boundsPadding = value;
                needsBoundsRecompute = true;
            }
        }

        public StatefulInteractable Interactable
        {
            get
            {
                return interactable;
            }
            set
            {
                if (interactable != value)
                {
                    UnsubscribeFromInteractable();
                    interactable = value;
                    SubscribeToInteractable();
                }
            }
        }

        public bool ToggleHandlesOnClick
        {
            get
            {
                return toggleHandlesOnClick;
            }
            set
            {
                toggleHandlesOnClick = value;
            }
        }

        public float DragToggleThreshold
        {
            get
            {
                return dragToggleThreshold;
            }
            set
            {
                dragToggleThreshold = value;
            }
        }

        public Transform Target
        {
            get
            {
                if (target == null)
                {
                    target = base.transform;
                }

                return target;
            }
            set
            {
                target = value;
            }
        }

        public bool HandlesActive
        {
            get
            {
                return handlesActive;
            }
            set
            {
                handlesActive = value;
            }
        }

        public HandleType EnabledHandles
        {
            get
            {
                return enabledHandles;
            }
            set
            {
                enabledHandles = value;
            }
        }

        public RotateAnchorType RotateAnchor
        {
            get
            {
                return rotateAnchor;
            }
            set
            {
                if (rotateAnchor != value)
                {
                    rotateAnchor = value;
                }
            }
        }

        public ScaleAnchorType ScaleAnchor
        {
            get
            {
                return scaleAnchor;
            }
            set
            {
                if (scaleAnchor != value)
                {
                    scaleAnchor = value;
                }
            }
        }

        public HandleScaleMode ScaleBehavior
        {
            get
            {
                return scaleBehavior;
            }
            set
            {
                if (scaleBehavior != value)
                {
                    scaleBehavior = value;
                }
            }
        }

        public bool SmoothingActive
        {
            get
            {
                return smoothingActive;
            }
            set
            {
                smoothingActive = value;
            }
        }

        public float RotateLerpTime
        {
            get
            {
                return rotateLerpTime;
            }
            set
            {
                rotateLerpTime = value;
            }
        }

        public float ScaleLerpTime
        {
            get
            {
                return scaleLerpTime;
            }
            set
            {
                scaleLerpTime = value;
            }
        }

        public float TranslateLerpTime
        {
            get
            {
                return translateLerpTime;
            }
            set
            {
                translateLerpTime = value;
            }
        }

        public bool EnableConstraints
        {
            get
            {
                return enableConstraints;
            }
            set
            {
                enableConstraints = value;
            }
        }

        public ConstraintManager ConstraintsManager
        {
            get
            {
                return constraintsManager;
            }
            set
            {
                constraintsManager = value;
            }
        }

        public ObjectManipulator.LogicType ManipulationLogicTypes
        {
            get
            {
                return manipulationLogicTypes;
            }
            set
            {
                manipulationLogicTypes = value;
                InstantiateManipulationLogic();
            }
        }

        public SelectEnterEvent ManipulationStarted
        {
            get
            {
                return manipulationStarted;
            }
            set
            {
                manipulationStarted = value;
            }
        }

        public SelectExitEvent ManipulationEnded
        {
            get
            {
                return manipulationEnded;
            }
            set
            {
                manipulationEnded = value;
            }
        }

        public bool IsManipulated => currentHandle != null;

        public bool IsFlat { get; protected set; }

        public Bounds CurrentBounds => currentBounds;

        public Vector3 OppositeCorner => oppositeCorner;

        protected LogicImplementation ManipulationLogicImplementations { get; private set; }

        private void InstantiateManipulationLogic()
        {
            ManipulationLogicImplementations = new LogicImplementation
            {
                moveLogic = (Activator.CreateInstance(ManipulationLogicTypes.moveLogicType) as ManipulationLogic<Vector3>),
                rotateLogic = (Activator.CreateInstance(ManipulationLogicTypes.rotateLogicType) as ManipulationLogic<Quaternion>),
                scaleLogic = (Activator.CreateInstance(ManipulationLogicTypes.scaleLogicType) as ManipulationLogic<Vector3>)
            };
        }

        private void Awake()
        {
            if (Interactable == null)
            {
                Interactable = GetComponentInParent<StatefulInteractable>();
            }
            else
            {
                SubscribeToInteractable();
            }

            minimumScale = Target.transform.localScale * 0.001f;
            CreateBoundsVisuals();
            if (constraintsManager == null)
            {
                constraintsManager = GetComponent<ConstraintManager>();
            }

            if (constraintsManager != null)
            {
                constraintsManager.Setup(new MixedRealityTransform(Target.transform));
            }

            ManipulationLogicTypes = manipulationLogicTypes;
        }

        protected virtual void Update()
        {
            if (needsBoundsRecompute && waitForFrames-- <= 0)
            {
                ComputeBounds(isSecondPass: true);
                needsBoundsRecompute = false;
            }

            TransformTarget();
            CheckToggleThreshold();
        }

        private void OnDestroy()
        {
            UnsubscribeFromInteractable();
        }

        private void OnHostSelected(SelectEnterEventArgs args)
        {
            isHostSelected = true;
            hasPassedToggleThreshold = false;
            startMovePosition = Target.localPosition;
        }

        private void OnHostDeselected(SelectExitEventArgs args)
        {
            if (!hasPassedToggleThreshold && toggleHandlesOnClick)
            {
                HandlesActive = !HandlesActive;
            }

            hasPassedToggleThreshold = false;
            isHostSelected = false;
        }

        private void SubscribeToInteractable()
        {
            if (Interactable != null)
            {
                Interactable.firstSelectEntered.AddListener(OnHostSelected);
                Interactable.lastSelectExited.AddListener(OnHostDeselected);
            }
        }

        private void UnsubscribeFromInteractable()
        {
            if (Interactable != null)
            {
                Interactable.firstSelectEntered.RemoveListener(OnHostSelected);
                Interactable.lastSelectExited.RemoveListener(OnHostDeselected);
            }
        }

        private void CreateBoundsVisuals()
        {
            if (boxInstance != null)
            {
                UnityEngine.Object.Destroy(boxInstance);
                boxInstance = null;
            }

            if (boundsVisualsPrefab != null)
            {
                boxInstance = UnityEngine.Object.Instantiate(boundsVisualsPrefab, base.transform);
                needsBoundsRecompute = ComputeBounds();
            }
        }

        public void RecomputeBounds()
        {
            ComputeBounds(isSecondPass: true);
        }

        private bool ComputeBounds(bool isSecondPass = false)
        {
            Transform transform = ((overrideBounds && boundsOverride != null) ? boundsOverride : Target);
            currentBounds = BoundsCalculator2.CalculateBounds(Target, transform, boxInstance.transform, out var containsCanvas, boundsCalculationMethod, includeInactiveObjects, !isSecondPass);
            if (containsCanvas && !isSecondPass)
            {
                return containsCanvas;
            }

            Vector3 size = Vector3.Scale(currentBounds.size, Target.lossyScale);
            flattenVector = BoundsCalculator.CalculateFlattenVector(size);
            bool flag = size.x < 0.01f || size.y < 0.01f || size.z < 0.01f;
            IsFlat = (flag && FlattenMode == FlattenMode.Auto) || FlattenMode == FlattenMode.Always;
            Vector3 vector = Vector3.Scale(IsFlat ? Vector3.Scale(Vector3.one * BoundsPadding, Vector3.one - flattenVector) : (Vector3.one * BoundsPadding), new Vector3(1f / Target.lossyScale.x, 1f / Target.lossyScale.y, 1f / Target.lossyScale.z));
            currentBounds.size += vector;
            boxInstance.transform.localScale = currentBounds.size;
            boxInstance.transform.localPosition = currentBounds.center;
            return containsCanvas;
        }

        internal void OnHandleSelectExited(BoundsHandleInteractable handle, SelectExitEventArgs args)
        {
            Debug.LogFormat("OnHandleSelectExited. ");
            if (currentHandle == handle)
            {
                currentHandle = null;
                manipulationEnded?.Invoke(args);
            }
        }

        internal void OnHandleSelectEntered(BoundsHandleInteractable handle, SelectEnterEventArgs args)
        {
            Debug.LogFormat("OnHandleSelectEntered. ");
            if (!(currentHandle != null) && (handle.HandleType & EnabledHandles) == handle.HandleType)
            {
                manipulationStarted?.Invoke(args);
                currentHandle = handle;
                initialTransformOnGrabStart = new MixedRealityTransform(Target.transform);
                Vector3 vector = (initialAnchorOnGrabStart = ((RotateAnchor == RotateAnchorType.BoundsCenter) ? Target.transform.TransformPoint(currentBounds.center) : Target.transform.position));
                initialAnchorDeltaOnGrabStart = Target.transform.position - vector;
                if (currentHandle.HandleType == HandleType.Scale)
                {
                    oppositeCorner = boxInstance.transform.TransformPoint(-currentHandle.transform.localPosition);
                    ManipulationLogicImplementations.scaleLogic.Setup(currentHandle.interactorsSelecting, args.interactableObject, initialTransformOnGrabStart);
                }
                else if (currentHandle.HandleType == HandleType.Rotation)
                {
                    ManipulationLogicImplementations.rotateLogic.Setup(currentHandle.interactorsSelecting, args.interactableObject, initialTransformOnGrabStart);
                }
                else if (currentHandle.HandleType == HandleType.Translation)
                {
                    ManipulationLogicImplementations.moveLogic.Setup(currentHandle.interactorsSelecting, args.interactableObject, initialTransformOnGrabStart);
                }

                if (EnableConstraints && constraintsManager != null)
                {
                    constraintsManager.OnManipulationStarted(new MixedRealityTransform(Target.transform));
                }
            }
        }

        private void TransformTarget()
        {
            using (TransformTargetPerfMarker.Auto())
            {
                if (!(currentHandle != null))
                {
                    return;
                }

                TransformFlags a = TransformFlags.None;
                MixedRealityTransform currentTarget = new MixedRealityTransform(target.transform);
                if (currentHandle.HandleType == HandleType.Rotation)
                {
                    Quaternion quaternion = ManipulationLogicImplementations.rotateLogic.Update(currentHandle.interactorsSelecting, interactable, currentTarget, RotateAnchor == RotateAnchorType.BoundsCenter);
                    Quaternion quaternion2 = quaternion * Quaternion.Inverse(initialTransformOnGrabStart.Rotation);
                    Vector3 vector = initialAnchorOnGrabStart + quaternion2 * initialAnchorDeltaOnGrabStart;
                    MixedRealityTransform mixedRealityTransform = MixedRealityTransform.NewRotate(quaternion);
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyRotationConstraints(ref mixedRealityTransform, isOneHanded: true, currentHandle.IsGrabSelected);
                    }

                    if (!a.IsMaskSet(TransformFlags.Rotate))
                    {
                        Target.transform.SetPositionAndRotation(smoothingActive ? Smoothing.SmoothTo(Target.transform.position, vector, rotateLerpTime, Time.deltaTime) : vector, smoothingActive ? Smoothing.SmoothTo(Target.transform.rotation, mixedRealityTransform.Rotation, rotateLerpTime, Time.deltaTime) : mixedRealityTransform.Rotation);
                    }
                }
                else if (currentHandle.HandleType == HandleType.Scale)
                {
                    Vector3 vector2 = ((ScaleAnchor == ScaleAnchorType.BoundsCenter) ? Target.transform.TransformPoint(currentBounds.center) : oppositeCorner);
                    MixedRealityTransform mixedRealityTransform2 = MixedRealityTransform.NewScale(ManipulationLogicImplementations.scaleLogic.Update(currentHandle.interactorsSelecting, interactable, currentTarget, ScaleAnchor == ScaleAnchorType.BoundsCenter));
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyScaleConstraints(ref mixedRealityTransform2, isOneHanded: true, currentHandle.IsGrabSelected);
                    }

                    mixedRealityTransform2.Scale = Vector3.Max(mixedRealityTransform2.Scale, minimumScale);
                    if (!a.IsMaskSet(TransformFlags.Scale))
                    {
                        Target.transform.localScale = (smoothingActive ? Smoothing.SmoothTo(Target.transform.localScale, mixedRealityTransform2.Scale, scaleLerpTime, Time.deltaTime) : mixedRealityTransform2.Scale);
                    }

                    Vector3 value = Target.transform.InverseTransformDirection(initialTransformOnGrabStart.Position - vector2);
                    Vector3 vector3 = Target.transform.TransformDirection(value.Mul(mixedRealityTransform2.Scale.Div(initialTransformOnGrabStart.Scale))) + vector2;
                    Target.transform.position = (smoothingActive ? Smoothing.SmoothTo(Target.transform.position, vector3, scaleLerpTime, Time.deltaTime) : vector3);
                }
                else if (currentHandle.HandleType == HandleType.Translation)
                {
                    MixedRealityTransform mixedRealityTransform3 = MixedRealityTransform.NewTranslate(ManipulationLogicImplementations.moveLogic.Update(currentHandle.interactorsSelecting, interactable, currentTarget, centeredAnchor: true));
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyTranslationConstraints(ref mixedRealityTransform3, isOneHanded: true, currentHandle.IsGrabSelected);
                    }

                    if (!a.IsMaskSet(TransformFlags.Move))
                    {
                        Target.transform.position = (smoothingActive ? Smoothing.SmoothTo(Target.transform.position, mixedRealityTransform3.Position, translateLerpTime, Time.deltaTime) : mixedRealityTransform3.Position);
                    }
                }
            }
        }

        private void CheckToggleThreshold()
        {
            if (isHostSelected && !hasPassedToggleThreshold && Vector3.Distance(startMovePosition, Target.localPosition) >= dragToggleThreshold)
            {
                hasPassedToggleThreshold = true;
            }
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
