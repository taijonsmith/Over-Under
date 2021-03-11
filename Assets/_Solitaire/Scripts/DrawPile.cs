using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Draw pile script handles click action on empty draw pile.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DrawPile : MonoBehaviour
    {
        public DeckManager DeckManager = null;

        public void Start()
        {
            if (DeckManager == null)
                Debug.LogError("No DeckManager Defined!");
        }

        public void OnMouseDown()
        {
            DeckManager.FlipDiscard();
        }
    }
}
