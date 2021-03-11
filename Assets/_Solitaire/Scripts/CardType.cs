using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public enum Suit
    {
        Clubs,
        Spades,
        Hearts,
        Diamonds
    }

    public enum Value
    {
        Ace,
        Card2,
        Card3,
        Card4,
        Card5,
        Card6,
        Card7,
        Card8,
        Card9,
        Card10,
        Jack,
        Queen,
        King
    }

    public struct CardType
    {
        public Suit Suit { get; set; }
        public Value Value { get; set; }

        #region Conversions

        /// <summary>
        /// Parse a string into a CardType.
        /// </summary>
        /// <param name="input">string</param>
        /// <exception cref="ArgumentException">Throws on invalid argument.</exception>
        /// <returns>CardType</returns>
        public static CardType Parse(string input)
        {
            var result = new CardType();
            var str = input.ToLower();

            if (str.Contains("clubs"))
                result.Suit = Suit.Clubs;
            else if (str.Contains("spades"))
                result.Suit = Suit.Spades;
            else if (str.Contains("hearts"))
                result.Suit = Suit.Hearts;
            else if (str.Contains("diamonds"))
                result.Suit = Suit.Diamonds;
            else throw new ArgumentException("No valid suit.");

            if (str.Contains("ace"))
                result.Value = Value.Ace;
            else if (str.Contains("2"))
                result.Value = Value.Card2;
            else if (str.Contains("3"))
                result.Value = Value.Card3;
            else if (str.Contains("4"))
                result.Value = Value.Card4;
            else if (str.Contains("5"))
                result.Value = Value.Card5;
            else if (str.Contains("6"))
                result.Value = Value.Card6;
            else if (str.Contains("7"))
                result.Value = Value.Card7;
            else if (str.Contains("8"))
                result.Value = Value.Card8;
            else if (str.Contains("9"))
                result.Value = Value.Card9;
            else if (str.Contains("10"))
                result.Value = Value.Card10;
            else if (str.Contains("jack"))
                result.Value = Value.Jack;
            else if (str.Contains("queen"))
                result.Value = Value.Queen;
            else if (str.Contains("king"))
                result.Value = Value.King;
            else throw new ArgumentException("No valid value.");

            return result;
        }

        /// <summary>
        /// Parse a string into a CardType.
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="result">CardType</param>
        /// <returns>Success</returns>
        public static bool TryParse(string input, out CardType result)
        {
            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                result = default(CardType);
                return false;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var val = Value.ToString().ToLower();
            if (val.Contains("card"))
                val = val.Substring(4);
            sb.Append(val);
            sb.Append("_of_");
            var suit = Suit.ToString().ToLower();
            sb.Append(suit);
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// Iterate through all combinations of suit and value
        /// </summary>
        /// <returns>All CardTypes</returns>
        public static IEnumerable<CardType> GetAllTypes()
        {
            return from suit in Enum.GetValues(typeof(Suit)).Cast<Suit>()
                   from value in Enum.GetValues(typeof(Value)).Cast<Value>()
                   select new CardType
            {
                Suit = suit,
                Value = value
            };
        }
    }
}
