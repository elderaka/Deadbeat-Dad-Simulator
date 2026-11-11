using UnityEngine;
using UnityEngine.EventSystems;

namespace BapakSimulator
{
    public sealed class TouchDragSurface : MonoBehaviour, IPointerDownHandler, IDragHandler,
        IPointerUpHandler, IInitializePotentialDragHandler
    {
        private int? pointer;
        private Vector2 origin;
        private Vector2 previous;
        private Vector2 delta;

        public Vector2 Displacement { get; private set; }

        public void OnInitializePotentialDrag(PointerEventData eventData) => eventData.useDragThreshold = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pointer.HasValue || eventData.button != PointerEventData.InputButton.Left) return;
            pointer = eventData.pointerId;
            origin = previous = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (pointer != eventData.pointerId) return;
            delta += eventData.position - previous;
            previous = eventData.position;
            Displacement = previous - origin;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pointer != eventData.pointerId) return;
            pointer = null;
            Displacement = Vector2.zero;
        }

        internal Vector2 ConsumeDelta()
        {
            Vector2 result = delta;
            delta = Vector2.zero;
            return result;
        }

        private void OnDisable()
        {
            pointer = null;
            Displacement = delta = Vector2.zero;
        }
    }
}
