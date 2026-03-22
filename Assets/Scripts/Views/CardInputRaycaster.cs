using UnityEngine;
using UnityEngine.InputSystem;

namespace Views
{
    public class CardInputRaycaster : MonoBehaviour
    {
        [SerializeField] Camera cam;

        HandCardView currentHovered;

        void Update()
        {
            if (Mouse.current == null)
                return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, 100f))
            {
                var view = hit.collider.GetComponentInParent<HandCardView>();

                if (view != currentHovered)
                {
                    if (currentHovered != null)
                        currentHovered.SetHovered(false);

                    currentHovered = view;

                    if (currentHovered != null)
                        currentHovered.SetHovered(true);
                }

                if (view != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    view.OnClicked();
                }
            }
            else
            {
                if (currentHovered != null)
                {
                    currentHovered.SetHovered(false);
                    currentHovered = null;
                }
            }
        }
    }
}