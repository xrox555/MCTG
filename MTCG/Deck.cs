﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class Deck
    {
        public List<Card> _deck = new List<Card>();

        public void AddCard(Card card)
        {
            _deck.Add(card);
        }

        public void PrintDeck()
        {
            /*
            foreach (Card card in _deck)
            {
                card.PrintCard();

            }
            */

            for(int i = 0; i < _deck.Count; i++)
            {
                _deck[i].PrintCard();
            }
        }

        public int GetDeckSize()
        {
            return _deck.Count;  
        }

    }
}
