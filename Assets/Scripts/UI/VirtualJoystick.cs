using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectTrinity.UI
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private RectTransform background;

        [SerializeField]
        private RectTransform handle;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Image handleImage;

        [SerializeField]
        private bool moveWithFinger;

        public float Horizontal { get { return inputVector.x; } }
        public float Vertical { get { return inputVector.y; } }
        public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

        private Vector2 lastDirection = Vector2.zero;
        private Vector2 startBackgroundPosition;
        private Vector2 inputVector = Vector2.zero;

        private Color backgroundImageColor;
        private Color handleImageColor;

        public bool JoystickActive { get; protected set; }

        private void Start()
        {
            startBackgroundPosition = background.position;
            background.gameObject.SetActive(true);

            backgroundImageColor = backgroundImage.color;
            handleImageColor = handleImage.color;

            SetAlpha(0.5f);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            lastDirection = eventData.position - ((Vector2)background.position);
            inputVector = lastDirection.magnitude > background.sizeDelta.x / 2f ? lastDirection.normalized : lastDirection / (background.sizeDelta.x / 2f);
            handle.anchoredPosition = inputVector * background.sizeDelta.x / 2f;

            if (moveWithFinger)
            {
                background.position = eventData.position - handle.anchoredPosition;
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            JoystickActive = true;

            background.position = eventData.position;
            handle.anchoredPosition = Vector2.zero;

            SetAlpha(1f);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            JoystickActive = false;

            inputVector = Vector2.zero;
            background.position = startBackgroundPosition;
            handle.anchoredPosition = Vector2.zero;

            SetAlpha(0.5f);
        }

        private void SetAlpha(float alpha)
        {
            Color tmpBackgroundColor = backgroundImageColor;
            tmpBackgroundColor.a = alpha;
            backgroundImage.color = tmpBackgroundColor;

            Color tmpHandleColor = handleImageColor;
            tmpHandleColor.a = alpha;
            handleImage.color = tmpHandleColor;
        }
    }
}