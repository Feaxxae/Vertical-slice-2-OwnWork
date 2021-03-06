﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the entire turn of the enemy
/// </summary>

public class Enemy_Turn : MonoBehaviour
{
    //lists
    [SerializeField]
    private List<GameObject> iFieldCards = new List<GameObject>();
    [SerializeField]
    private List<GameObject> jEnemyHand = new List<GameObject>();
    [SerializeField]
    private List<GameObject> kEnemyField = new List<GameObject>();

    //Scripts
    private OnFieldCards _cardsOnField;
    private Enemy_Hand _enemyHand;
    private Enemy_Mana _enemyMana;
    private Turn_Manager _turnManager;
    private Card_Stats _cStats;

    //Game objects
    private GameObject[] _EnemyCards;

    //Ints
    int indexLocation;

    //Bools
    private bool hardBreak = false;
    private bool switch1 = false;
    private bool switch2 = false;
    private bool switch3 = false;

    //Player stuff
    private bool pTauntCheck = false;
    private bool pFieldEmpty = false;

    //Enemy stuff
    private bool eChargeCheck = false;
    private bool eChargeOnField = false;
    private bool eSpellCheck = false;

    // Use this for initialization
    void Start()
    {
        if (!(_cardsOnField = this.GetComponent<OnFieldCards>()))
        {
            _cardsOnField = this.gameObject.AddComponent<OnFieldCards>();
        }
        if (!(_enemyHand = this.GetComponent<Enemy_Hand>()))
        {
            _enemyHand = this.gameObject.AddComponent<Enemy_Hand>();
        }
        if (!(_enemyMana = this.GetComponent<Enemy_Mana>()))
        {
            _enemyMana = this.gameObject.AddComponent<Enemy_Mana>();
        }
        if (!(_turnManager = this.GetComponent<Turn_Manager>()))
        {
            _turnManager = this.gameObject.AddComponent<Turn_Manager>();
        }
        //_EnemyCards = GameObject.FindGameObjectsWithTag("EnemyCard");
        //minion = _cardsOnField.onPlayerField.Find(minion => minion.tag == "Taunt");
    }

    // Update is called once per frame
    void Update()
    {
        if (_turnManager.turns == false)
        {
            //Place a While loop or If statement here to check if the Mana is
            //Greater then 0, or if all cards cost more mana then its current.
            //If so, end turn.

            if (_cardsOnField.onPlayerField.Count == 0)
            {
                pFieldEmpty = true;
            }

            if (pFieldEmpty != true && hardBreak == false)
            {
                for (int i = 0; i < _cardsOnField.onPlayerField.Count; i++)
                {
                    //Debug.Log("null check: "+i + " "+ _cardsOnField.onPlayerField[i]);
                    iFieldCards.Add(_cardsOnField.onPlayerField[i]);
                    GameObject eField = iFieldCards[i];
                    if (eField.GetComponent<Card_Stats>().cTaunt == true)
                    {
                        pTauntCheck = true;
                        GameObject pMinion = _cardsOnField.onPlayerField[i];
                        switch1 = true;
                    }
                    else
                    {
                        pTauntCheck = false;
                        switch1 = true;
                    }
                    iFieldCards.Clear();
                }
                for (int j = 0; j < _enemyHand.CardsInHand.Count; j++)
                {
                    jEnemyHand.Add(_enemyHand.CardsInHand[j]);
                    GameObject eHand = jEnemyHand[j];
                    Debug.Log("enemy Card in hand: " + eHand);
                    Debug.Log("Card in Hand: " + j);
                    if (jEnemyHand[j] != null)
                    {
                        if (eHand.GetComponent<Card_Stats>().cCharge != true)
                        {
                            //do nothing
                            switch2 = true;
                        }
                        else if (eHand.GetComponent<Card_Stats>().cCharge == true)
                        {
                            GameObject eMinion = _enemyHand.CardsInHand[j];
                            eChargeCheck = true;
                            indexLocation = j;
                            _enemyMana.currentMana -= eMinion.GetComponent<Card_Stats>().cMana;
                            _cardsOnField.onEnemyField.Add(eMinion);
                            jEnemyHand.Clear();
                            switch2 = true;
                        }
                        
                    }
                    
                    //else if (_enemyHand.CardsInHand[j].tag == "Spell")
                    //{
                    //    GameObject eSpell = _enemyHand.CardsInHand[j];
                    //    indexLocation = j;
                    //    eSpellCheck = true;
                    //}
                }
                if (switch1 == true && switch2 == true)
                {
                    hardBreak = true;
                }
            }

            if (pFieldEmpty == true && hardBreak == false)
            {
                for (int k = 0; k < _enemyHand.CardsInHand.Count; k++)
                {
                    if (_enemyHand.CardsInHand[k].tag == "Battlecry")
                    {
                        GameObject eMinion = _enemyHand.CardsInHand[k];
                        if (_enemyMana.currentMana >= eMinion.GetComponent<Card_Stats>().cMana && eMinion.GetComponent<Card_Stats>().canPlayCard == true)
                        {
                            indexLocation = k;
                            _enemyHand.CardsInHand.RemoveAt(indexLocation);
                            _enemyMana.currentMana -= eMinion.GetComponent<Card_Stats>().cMana;
                            _cardsOnField.onEnemyField.Add(eMinion);
                        }
                    }
                    if (_enemyHand.CardsInHand[k].tag == "Deathrattle")
                    {
                        GameObject eMinion = _enemyHand.CardsInHand[k];
                        if (_enemyMana.currentMana >= eMinion.GetComponent<Card_Stats>().cMana && eMinion.GetComponent<Card_Stats>().canPlayCard == true)
                        {
                            indexLocation = k;
                            _enemyHand.CardsInHand.RemoveAt(indexLocation);
                            _enemyMana.currentMana -= eMinion.GetComponent<Card_Stats>().cMana;
                            _cardsOnField.onEnemyField.Add(eMinion);
                        }
                    }
                    if (k >= _enemyHand.CardsInHand.Count)
                    {
                        switch3 = true;
                    }
                }
                if (switch3 == true)
                {
                    hardBreak = true;
                }
            }

            if (pTauntCheck == true)
            {
                if (eChargeCheck == true)
                {
                    Debug.Log("e Play Charge " + indexLocation);
                    _enemyHand.CardsInHand.RemoveAt(indexLocation);
                    eChargeCheck = false;
                    eChargeOnField = true;
                }
                else if (eSpellCheck == true)
                {
                    Debug.Log("e Play Spell" + indexLocation);
                    //_enemyHand.CardsInHand.RemoveAt(indexLocation);
                    //eSpellCheck = false;
                    //Use Spell on Taunt minion
                }
            }

            if (eChargeOnField == true)
            {
                //attack the taunt
            }
            else
            {
                //Attack face
            }
        }
    }
}
