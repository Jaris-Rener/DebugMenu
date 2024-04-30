﻿namespace DebugMenu
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class WindowDragger : PointerManipulator
    {
        private Vector3 _start;
        private bool _active;
        private int _pointerId;
        private Vector2 _startSize;
        private readonly VisualElement _dragElement;

        public WindowDragger(VisualElement dragElement)
        {
            _dragElement = dragElement;
            _pointerId = -1;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            _active = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected void OnPointerDown(PointerDownEvent e)
        {
            if (_active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e) && e.target == _dragElement)
            {
                _start = e.localPosition;
                _pointerId = e.pointerId;

                _active = true;
                target.CapturePointer(_pointerId);
                e.StopPropagation();
            }
        }

        protected void OnPointerMove(PointerMoveEvent e)
        {
            if (!_active || !target.HasPointerCapture(_pointerId))
                return;

            Vector2 diff = e.localPosition - _start;

            target.style.top = target.layout.y + diff.y;
            target.style.left = target.layout.x + diff.x;

            e.StopPropagation();
        }

        protected void OnPointerUp(PointerUpEvent e)
        {
            if (!_active || !target.HasPointerCapture(_pointerId) || !CanStopManipulation(e))
                return;

            _active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }
    }
}