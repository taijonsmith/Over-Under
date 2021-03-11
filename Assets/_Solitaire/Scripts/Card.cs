using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Card script controls flipping and updating each card.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Card : MonoBehaviour
    {
        public bool IsFlipped = false;
        public GameObject FrontFace = null;
        public GameObject BackFace = null;

        #region Properties

        public SpriteRenderer FrontRenderer { get; set; }

        public SpriteRenderer BackRenderer { get; set; }

        public DeckManager DeckManager { get; set; }

        public CardType CardType { get; set; }

        public bool IsDragging { get; set; }

        public Vector3 DragOffset { get; set; }

        #endregion

        #region MonoBehavior Overrides

        public void Start()
        {
            if (DeckManager == null)
                Debug.LogError("No DeckManager Defined!");

            FrontRenderer = FrontFace.GetComponent<SpriteRenderer>();
            BackRenderer = BackFace.GetComponent<SpriteRenderer>();
            UpdateFacing();
        }

        public void OnMouseDown()
        {
            var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            DragOffset = transform.position - Camera.main.ScreenToWorldPoint(screenPoint);

            DeckManager.HandleClick(this);
        }

        public void OnMouseDrag()
        {
            if (!IsDragging) return;
            DeckManager.HandleDrag(this);
            UpdateDrag();
        }

        public void OnMouseUp()
        {
            if (IsDragging)
                DeckManager.HandleDragEnd(this);
            IsDragging = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Flip a card.
        /// </summary>
        /// <param name="isFlipped">Force flip up or down</param>
        public void Flip(bool? isFlipped = null)
        {
            if (isFlipped == null)
                IsFlipped = !IsFlipped;
            else
                IsFlipped = isFlipped.Value;

            UpdateFacing();
        }

        /// <summary>
        /// Begin a drag action, using a reference position and offset to render in a dragged stack.
        /// </summary>
        /// <param name="refPosition">Position of top card in stack</param>
        /// <param name="offset">Offset from top card</param>
        public void StartDrag(Vector3 refPosition, float offset)
        {
            var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            var dragOffset = refPosition - Camera.main.ScreenToWorldPoint(screenPoint);
            dragOffset.y -= offset;
            DragOffset = dragOffset;
            IsDragging = true;
        }

        /// <summary>
        /// Update the drag render based on the parameters set in <see cref="StartDrag"/>.
        /// </summary>
        public void UpdateDrag()
        {
            if (!IsDragging) return;
            var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            var newPosition = Camera.main.ScreenToWorldPoint(screenPoint) + DragOffset;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }

        /// <summary>
        /// Sets the card's front face sprite.
        /// </summary>
        /// <param name="sprite">Sprite</param>
        public void SetSprite(Sprite sprite)
        {
            var front = GetSpriteRenderer(true);
            front.sprite = sprite;
        }

        /// <summary>
        /// Sets the render order of the card.
        /// </summary>
        /// <param name="order">Sprite Sorting Order</param>
        public void SetSortingOrder(int order)
        {
            var front = GetSpriteRenderer(true);
            var back = GetSpriteRenderer(false);
            front.sortingOrder = order;
            back.sortingOrder = order;
        }

        private SpriteRenderer GetSpriteRenderer(bool isFront)
        {
            var index = isFront ? 0 : 1;
            var component = gameObject.transform.GetChild(index);
            return component.GetComponent<SpriteRenderer>();
        }

        public override string ToString()
        {
            return CardType.ToString();
        }

        private void UpdateFacing()
        {
            FrontRenderer.enabled = IsFlipped;
            BackRenderer.enabled = !IsFlipped;
        }

        #endregion
    }
}
