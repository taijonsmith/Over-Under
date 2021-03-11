using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Deck Manager manages the deck of cards.
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        #region Fields

        public GameObject CardPrefab;

        // Used to edit position of elements in the Unity editor
        public GameObject DrawPile;
        public GameObject DiscardPile;
        public GameObject ClubsPile;
        public GameObject SpadesPile;
        public GameObject HeartsPile;
        public GameObject DiamondsPile;
        public GameObject Stack1;
        public GameObject Stack2;
        public GameObject Stack3;
        public GameObject Stack4;
        public GameObject Stack5;
        public GameObject Stack6;
        public GameObject Stack7;
        public float StackOffset = 1.0f;

        // Static members used for loading cards
        private static readonly IList<Sprite> Sprites = new List<Sprite>();
        private static readonly IList<CardType> CardTypes = CardType.GetAllTypes().ToList();

        private const int CardCount = 52;

        // Read-only collection of all cards
        private readonly IList<Card> _deck = new List<Card>();

        // Ordered collections of cards for each screen elements
        private IDictionary<GameObject, IList<Card>> Elements { get; set; }

        // Ordered collectinos of card for stacks only
        private IDictionary<GameObject, IList<Card>> Stacks
        {
            get
            {
                return Elements.Where(x => x.Key == Stack1 ||
                    x.Key == Stack2 || x.Key == Stack3 || x.Key == Stack4 ||
                    x.Key == Stack5 || x.Key == Stack6 || x.Key == Stack7)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            Elements = new Dictionary<GameObject, IList<Card>>
            {
                {DrawPile, new List<Card>()},
                {DiscardPile, new List<Card>()},
                {ClubsPile, new List<Card>()},
                {SpadesPile, new List<Card>()},
                {HeartsPile, new List<Card>()},
                {DiamondsPile, new List<Card>()},
                {Stack1, new List<Card>()},
                {Stack2, new List<Card>()},
                {Stack3, new List<Card>()},
                {Stack4, new List<Card>()},
                {Stack5, new List<Card>()},
                {Stack6, new List<Card>()},
                {Stack7, new List<Card>()}
            };

            LoadSprites();
            LoadCards();
            ShuffleCards();
            DealCards();

            // Some initialization requires the game to be fully initialized
            // We delay by a few milliseconds to allow the engine to load everything
            StartCoroutine(StartDelay(0.01f));
        }

        public void HandleClick(Card card)
        {
            if (!card.IsFlipped && IsBottomOfStack(card))
            {
                // Flip bottom card in stack
                card.Flip();
            }
            else if (card.IsFlipped && IsInStack(card) && !IsBottomOfStack(card))
            {
                // Dragging stack
                card.IsDragging = true;
                var theRest = GetDragStack(card);
                foreach (var dragInfo in theRest)
                {
                    dragInfo.Card.StartDrag(card.transform.position, dragInfo.CardOffset * StackOffset);
                }
            }
            else if (card.IsFlipped)
            {
                // Dragging single card
                card.IsDragging = true;
            }
            else if (IsInDraw(card))
            {
                DeckDraw(card);
            }
        }

        public void HandleDrag(Card card)
        {
            var zindex = -1;
            var sortOrder = 50;
            var position = card.transform.position;
            position.z = zindex;
            card.transform.position = position;
            card.SetSortingOrder(sortOrder);

            var stackCards = GetDragStack(card).Select(x => x.Card);
            foreach (var c in stackCards)
            {
                if (c == card) continue;

                zindex--;
                sortOrder++;
                var position2 = c.transform.position;
                position2.z = zindex;
                c.transform.position = position2;
                c.SetSortingOrder(sortOrder);

                c.UpdateDrag();
            }
        }

        public void HandleDragEnd(Card card)
        {
            // 1. Find closest destination (Magnitude of difference in position vectors)
            var closestSnap = ClosestSnapPosition(card);

            // 2. Check for validity
            bool isMoveValid;
            var snapCards = GetCardCollection(closestSnap);

            // First if the card hasn't moved, just reset
            if (snapCards.Contains(card))
            {
                isMoveValid = false;
            }
            else if (closestSnap == ClubsPile ||
                closestSnap == SpadesPile ||
                closestSnap == HeartsPile ||
                closestSnap == DiamondsPile)
            {
                if ((closestSnap == ClubsPile && card.CardType.Suit != Suit.Clubs) ||
                    (closestSnap == SpadesPile && card.CardType.Suit != Suit.Spades) ||
                    (closestSnap == HeartsPile && card.CardType.Suit != Suit.Hearts) ||
                    (closestSnap == DiamondsPile && card.CardType.Suit != Suit.Diamonds))
                {
                    // Suit doesn't match
                    isMoveValid = false;
                }
                else if (!snapCards.Any())
                {
                    // Adding ace to empty pile
                    isMoveValid = card.CardType.Value == Value.Ace;
                }
                else
                {
                    // Check that card is the next highest value
                    var highestCard = snapCards.OrderByDescending(x => x.CardType.Value).First();
                    var nextVal = Enum.GetValues(typeof(Value)).Cast<Value>()
                        .SkipWhile(x => x != highestCard.CardType.Value).Skip(1).First();

                    isMoveValid = card.CardType.Value == nextVal;
                }
            }
            else
            {
                // Empty stack
                if (!snapCards.Any())
                {
                    // Accept only king
                    isMoveValid = card.CardType.Value == Value.King;
                }
                else if (snapCards.Last().IsFlipped == false || snapCards.Last().CardType.Value == Value.Ace)
                {
                    // Reject if face down or ace
                    isMoveValid = false;
                }
                else
                {
                    // Building stack - alternate color and next lowest
                    var targetCard = snapCards.Last();

                    var isTargetBlack = targetCard.CardType.Suit == Suit.Spades ||
                                        targetCard.CardType.Suit == Suit.Clubs;
                    var isCardBlack = card.CardType.Suit == Suit.Spades ||
                                      card.CardType.Suit == Suit.Clubs;
                    var isDifferentColor = isTargetBlack ^ isCardBlack;

                    var prevVal = Enum.GetValues(typeof(Value)).Cast<Value>().Reverse()
                        .SkipWhile(x => x != targetCard.CardType.Value).Skip(1).First();

                    isMoveValid = isDifferentColor && card.CardType.Value == prevVal;
                }
            }

            if (isMoveValid)
            {
                // Move to new stack
                var currentLocation = GetCardLocation(card);
                currentLocation.Remove(card);
                snapCards.Add(card);
            }

            // If move wasn't valid, card will snap back
            UpdateAll();
        }

        /// <summary>
        /// Flips all cards in the discard pile back to the draw pile.
        /// </summary>
        public void FlipDiscard()
        {
            var drawPile = Elements[DrawPile];
            var discardPile = Elements[DiscardPile];
            foreach (var card in discardPile)
            {
                drawPile.Add(card);
            }

            discardPile.Clear();
            UpdateDraw();
            UpdateDiscard();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Set position of all cards on a delayed start.
        /// </summary>
        /// <param name="delaySeconds">Start delay in seconds</param>
        private IEnumerator StartDelay(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);

            foreach (var stack in Stacks.Select(x => x.Value))
            {
                InitializeStack(stack);
            }

            UpdateDraw();
        }

        /// <summary>
        /// Load all 52 card sprites in 'Resources/Sprites/'.
        /// </summary>
        private static void LoadSprites()
        {
            if (Sprites.Any()) return;

            foreach (var spritePath in CardTypes.Select(t => "Sprites/" + t.ToString()))
            {
                Sprites.Add(Resources.Load<Sprite>(spritePath));
            }
        }

        /// <summary>
        /// Create all card prefabs using loaded sprites.
        /// </summary>
        private void LoadCards()
        {
            for (int i = 0; i < CardCount; i++)
            {
                var prefab = Instantiate(CardPrefab);
                var card = prefab.GetComponent<Card>();
                card.DeckManager = this;
                card.CardType = CardTypes[i];
                card.SetSprite(Sprites[i]);

                _deck.Add(card);
            }
        }

        /// <summary>
        /// Shuffle the cards by swapping each card with another random card.
        /// </summary>
        private void ShuffleCards()
        {
            for (int i = 0; i < _deck.Count - 1; i++)
            {
                var rand = UnityEngine.Random.Range(i, _deck.Count);
                var temp = _deck[i];
                _deck[i] = _deck[rand];
                _deck[rand] = temp;
            }
        }

        /// <summary>
        /// Deal cards into their initial positions.
        /// </summary>
        private void DealCards()
        {
            for (int i = 0; i < CardCount; i++)
            {
                if (i < 1)
                    AddToStack(_deck[i], Elements[Stack1]);
                else if (i < 3)
                    AddToStack(_deck[i], Elements[Stack2]);
                else if (i < 6)
                    AddToStack(_deck[i], Elements[Stack3]);
                else if (i < 10)
                    AddToStack(_deck[i], Elements[Stack4]);
                else if (i < 15)
                    AddToStack(_deck[i], Elements[Stack5]);
                else if (i < 21)
                    AddToStack(_deck[i], Elements[Stack6]);
                else if (i < 28)
                    AddToStack(_deck[i], Elements[Stack7]);
                else
                    AddToDraw(_deck[i]);
            }
        }

        #endregion

        #region Card Manipulation

        /// <summary>
        /// Initialize the display of all cards in a stack.
        /// </summary>
        /// <param name="stack">Stack</param>
        private void InitializeStack(IList<Card> stack)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].Flip(i == stack.Count - 1);
            }

            UpdateStack(stack);
        }

        /// <summary>
        /// Fix the display of all cards in a stack.
        /// </summary>
        /// <param name="stack">Stack</param>
        private void UpdateStack(IList<Card> stack)
        {
            var positionRef = GetCardReference(stack);

            for (int i = 0; i < stack.Count; i++)
            {
                var revi = stack.Count - i;
                stack[i].SetSortingOrder(i);

                var position = positionRef.transform.position;
                position.y -= i * StackOffset;
                position.z = revi;
                stack[i].transform.position = position;
            }
        }

        /// <summary>
        /// Fix the display of all cards in a suit pile.
        /// </summary>
        /// <param name="stack">Pile</param>
        private void UpdateSuitPile(IList<Card> stack)
        {
            var positionRef = GetCardReference(stack);

            for (int i = 0; i < stack.Count; i++)
            {
                var revi = stack.Count - i;
                stack[i].SetSortingOrder(i);

                var position = positionRef.transform.position;
                position.z = revi;
                stack[i].transform.position = position;
            }
        }

        /// <summary>
        /// Fix the display of all cards in the draw pile.
        /// </summary>
        private void UpdateDraw()
        {
            var drawPile = Elements[DrawPile];
            for (int i = 0; i < drawPile.Count; i++)
            {
                var card = drawPile[i];
                var revi = drawPile.Count - i;

                // All cards should be flipped down
                card.Flip(false);

                // Render order should be low index -> lower on table
                card.SetSortingOrder(revi);

                // Collision tied to Z-index, which should be inverse of sprite order
                var position = DrawPile.transform.position;
                position.z = i;
                card.transform.position = position;
            }
        }

        /// <summary>
        /// Fix the display of all cards in the discard pile.
        /// </summary>
        private void UpdateDiscard()
        {
            var discardPile = Elements[DiscardPile];
            for (int i = 0; i < discardPile.Count; i++)
            {
                var card = discardPile[i];
                var revi = discardPile.Count - i;

                // All cards should be flipped up
                card.Flip(true);

                // Render order should be low index -> lower on table
                card.SetSortingOrder(i);

                // Collision tied to Z-index, which should be inverse of sprite order
                var position = DiscardPile.transform.position;
                position.z = revi;
                card.transform.position = position;
            }
        }

        /// <summary>
        /// Fix the display of all cards.
        /// </summary>
        private void UpdateAll()
        {
            UpdateDraw();
            UpdateDiscard();
            UpdateStack(Elements[Stack1]);
            UpdateStack(Elements[Stack2]);
            UpdateStack(Elements[Stack3]);
            UpdateStack(Elements[Stack4]);
            UpdateStack(Elements[Stack5]);
            UpdateStack(Elements[Stack6]);
            UpdateStack(Elements[Stack7]);
            UpdateSuitPile(Elements[ClubsPile]);
            UpdateSuitPile(Elements[SpadesPile]);
            UpdateSuitPile(Elements[HeartsPile]);
            UpdateSuitPile(Elements[DiamondsPile]);
        }

        /// <summary>
        /// Add a card to a stack
        /// </summary>
        /// <param name="card">Card</param>
        /// <param name="stackCards">Stack collection</param>
        private void AddToStack(Card card, ICollection<Card> stackCards)
        {
            stackCards.Add(card);
        }

        /// <summary>
        /// Add a card to the draw pile.
        /// </summary>
        /// <param name="card">Card</param>
        private void AddToDraw(Card card)
        {
            Elements[DrawPile].Add(card);
        }

        /// <summary>
        /// Draw a card from the draw pile, placing it in the discard pile.
        /// </summary>
        /// <param name="card">Card</param>
        private void DeckDraw(Card card)
        {
            Elements[DrawPile].Remove(card);
            Elements[DiscardPile].Add(card);

            UpdateDraw();
            UpdateDiscard();
        }

        #endregion

        #region Information Helpers

        /// <summary>
        /// Determines if a card is the last card in a stack.
        /// </summary>
        /// <param name="card">Card</param>
        /// <returns>Boolean</returns>
        private bool IsBottomOfStack(Card card)
        {
            return Stacks.Any(x => x.Value.Any() && x.Value.Last() == card);
        }

        /// <summary>
        /// Determines if a card is in a stack.
        /// </summary>
        /// <param name="card">Card</param>
        /// <returns>Boolean</returns>
        private bool IsInStack(Card card)
        {
            return Stacks.Any(x => x.Value.Contains(card));
        }

        /// <summary>
        /// Determines if a card is in the draw pile.
        /// </summary>
        /// <param name="card">Card</param>
        /// <returns>Boolean</returns>
        private bool IsInDraw(Card card)
        {
            return Elements[DrawPile].Contains(card);
        }

        /// <summary>
        /// Gets the cards in the same stack and under a card.
        /// </summary>
        /// <param name="card">Card</param>
        /// <returns>Stack Cards with Offset</returns>
        private IEnumerable<DragInfo> GetDragStack(Card card)
        {
            var result = new List<DragInfo>();
            var stack = Stacks.FirstOrDefault(x => x.Value.Contains(card)).Value;
            if (stack == null) return result;

            int count = 0;
            bool found = false;
            foreach (var c in stack)
            {
                if (!found && c != card) continue;
                if (c == card)
                {
                    found = true;
                }
                else
                {
                    count++;
                    result.Add(new DragInfo { Card = c, CardOffset = count });
                }
            }

            return result;
        }

        private IList<Card> GetCardLocation(Card card)
        {
            return Elements.FirstOrDefault(x => x.Value.Contains(card)).Value;
        }

        /// <summary>
        /// Gets the closest destination for dragging a card.
        /// </summary>
        /// <param name="card">Card</param>
        /// <returns>GameObject</returns>
        private GameObject ClosestSnapPosition(Card card)
        {
            var snapPositions = new List<GameObject>
            {
                Stack1,
                Stack2,
                Stack3,
                Stack4,
                Stack5,
                Stack6,
                Stack7,
                ClubsPile,
                SpadesPile,
                HeartsPile,
                DiamondsPile
            };

            var snapDistances = snapPositions.Select(x => new
            {
                obj = x,
                distance = x.transform.position - card.transform.position
            });

            return snapDistances.OrderBy(x => x.distance, new Vector3Comparer()).First().obj;
        }

        /// <summary>
        /// Gets the collection associated with a position reference
        /// </summary>
        /// <param name="objectRef">GameObject</param>
        /// <returns>Card collection</returns>
        private IList<Card> GetCardCollection(GameObject objectRef)
        {
            return Elements[objectRef];
        }

        /// <summary>
        /// Gets the position reference associated with a card collection
        /// </summary>
        /// <param name="cardCollection">Card collection</param>
        /// <returns>GameObject</returns>
        private GameObject GetCardReference(IList<Card> cardCollection)
        {
            return Elements.FirstOrDefault(x => x.Value == cardCollection).Key;
        }

        #endregion

        #region Helper Classes

        // Because C# 4 doesn't have Tuple, ValueTuple, or anything else to make my life easier
        private struct DragInfo
        {
            public Card Card { get; set; }
            public int CardOffset { get; set; }
        }

        // Compares Vector3 based on X-Y distance
        private class Vector3Comparer : IComparer<Vector3>
        {
            public int Compare(Vector3 x, Vector3 y)
            {
                var flatx = x;
                flatx.z = 0;
                var magx = flatx.magnitude;

                var flaty = y;
                flaty.z = 0;
                var magy = flaty.magnitude;

                return (int) (256 * (magx - magy));
            }
        }

        #endregion
    }
}
