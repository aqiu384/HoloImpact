// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that allows manipulation of an object with your hand on HoloLens.
    /// Manipulation is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public abstract class HandManipulation : MonoBehaviour,
                                                IFocusable,
                                                IInputHandler,
                                                ISourceStateHandler
    {
        /// <summary>
        /// Event triggered when manipulation starts.
        /// </summary>
        public event Action StartedManipulation;

        /// <summary>
        /// Event triggered when manipulation stops.
        /// </summary>
        public event Action StoppedManipulation;

        [Tooltip("Transform that will be manipulated. Defaults to the object of the component.")]
        public Transform TargetTransform;

        [Tooltip("Scale by which hand movement in z is multipled to move the dragged object.")]
        public float DistanceScale = 2f;

        public bool IsManipulationEnabled = true;

        protected Camera m_mainCamera;

        protected bool m_isManipulating;
        protected bool m_isGazed;

        protected float m_objRefDistance;
        protected float m_handRefDistance;

        // World Space Offset
        protected Vector3 m_objRefGrabPoint;

        // World Space
        protected Vector3 m_objRefForward;
        protected Vector3 m_draggingPosition;

        // World Space
        protected Quaternion m_gazeAngularOffset;
        protected Quaternion m_draggingRotation;

        protected IInputSource m_currentInputSource = null;
        protected uint m_currentInputSourceId;

        protected virtual void Start()
        {
            if (TargetTransform == null)
            {
                TargetTransform = transform;
            }

            m_mainCamera = Camera.main;
        }

        protected virtual void OnDestroy()
        {
            if (m_isManipulating)
            {
                StopManipulating();
            }

            if (m_isGazed)
            {
                OnFocusExit();
            }
        }

        protected virtual void Update()
        {
            if (IsManipulationEnabled && m_isManipulating)
            {
                UpdateManipulationPosition();
                ProcessManipulationPosition();
            }
        }

        protected abstract void ProcessManipulationPosition();

        /// <summary>
        /// Start manipulating the object.
        /// </summary>
        protected virtual void StartManipulating()
        {
            if (!IsManipulationEnabled)
            {
                return;
            }

            if (m_isManipulating)
            {
                return;
            }

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            m_isManipulating = true;
            //GazeCursor.Instance.SetState(GazeCursor.State.Move);
            //GazeCursor.Instance.SetTargetObject(HostTransform);

            Vector3 gazeHitPosition = GazeManager.Instance.HitInfo.point;
            Vector3 handPosition;
            m_currentInputSource.TryGetPosition(m_currentInputSourceId, out handPosition);

            Vector3 pivotPosition = GetHandPivotPosition();
            m_handRefDistance = Vector3.Magnitude(handPosition - pivotPosition);
            m_objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);

            Vector3 objForward = TargetTransform.forward;

            // Store where the object was grabbed from
            m_objRefGrabPoint = TargetTransform.position - gazeHitPosition;

            Vector3 objDirection = Vector3.Normalize(gazeHitPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(handPosition - pivotPosition);

            objForward = m_mainCamera.transform.InverseTransformDirection(objForward);       // in camera space
            objDirection = m_mainCamera.transform.InverseTransformDirection(objDirection);   // in camera space
            handDirection = m_mainCamera.transform.InverseTransformDirection(handDirection); // in camera space

            m_objRefForward = m_mainCamera.transform.TransformDirection(objForward); // in world space

            // Store the initial offset between the hand and the object, so that we can consider it when manipulating
            m_gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            m_draggingPosition = gazeHitPosition;

            StartedManipulation.RaiseEvent();
        }

        /// <summary>
        /// Update the position of the object being manipulated.
        /// </summary>
        protected virtual void UpdateManipulationPosition()
        {
            Vector3 newHandPosition;
            m_currentInputSource.TryGetPosition(m_currentInputSourceId, out newHandPosition);

            Vector3 pivotPosition = GetHandPivotPosition();

            Vector3 newHandDirection = Vector3.Normalize(newHandPosition - pivotPosition);

            newHandDirection = m_mainCamera.transform.InverseTransformDirection(newHandDirection); // in camera space
            Vector3 targetDirection = Vector3.Normalize(m_gazeAngularOffset * newHandDirection);
            targetDirection = m_mainCamera.transform.TransformDirection(targetDirection); // back to world space

            float currenthandDistance = Vector3.Magnitude(newHandPosition - pivotPosition);

            float distanceRatio = currenthandDistance / m_handRefDistance;
            float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
            float targetDistance = m_objRefDistance + distanceOffset;

            m_draggingPosition = pivotPosition + (targetDirection * targetDistance);
        }

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        public Vector3 GetHandPivotPosition()
        {
            Vector3 pivot = m_mainCamera.transform.position + new Vector3(0, -0.2f, 0) - m_mainCamera.transform.forward * 0.2f; // a bit lower and behind
            return pivot;
        }

        /// <summary>
        /// Enables or disables manipulation.
        /// </summary>
        /// <param name="isEnabled">Indicates whether manipulation should be enabled or disabled.</param>
        public void SetManipulation(bool isEnabled)
        {
            if (IsManipulationEnabled == isEnabled)
            {
                return;
            }

            IsManipulationEnabled = isEnabled;

            if (m_isManipulating)
            {
                StopManipulating();
            }
        }

        /// <summary>
        /// Stop manipulating the object.
        /// </summary>
        public void StopManipulating()
        {
            if (!m_isManipulating)
            {
                return;
            }

            // Remove self as a modal input handler
            InputManager.Instance.PopModalInputHandler();

            m_isManipulating = false;
            m_currentInputSource = null;
            StoppedManipulation.RaiseEvent();
        }

        public void OnFocusEnter()
        {
            if (!IsManipulationEnabled)
            {
                return;
            }

            if (m_isGazed)
            {
                return;
            }

            m_isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsManipulationEnabled)
            {
                return;
            }

            if (!m_isGazed)
            {
                return;
            }

            m_isGazed = false;
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (m_currentInputSource != null &&
                eventData.SourceId == m_currentInputSourceId)
            {
                StopManipulating();
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (m_isManipulating)
            {
                // We're already handling manipulation input, so we can't start a new manipulation operation.
                return;
            }

            if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
            {
                // The input source must provide positional data for this script to be usable
                return;
            }

            m_currentInputSource = eventData.InputSource;
            m_currentInputSourceId = eventData.SourceId;
            StartManipulating();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (m_currentInputSource != null && eventData.SourceId == m_currentInputSourceId)
            {
                StopManipulating();
            }
        }
    }
}