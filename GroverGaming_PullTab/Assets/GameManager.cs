using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Money")]
    [SerializeField] TMP_Text _balanceText;
    [Space(10)]
    [SerializeField] TMP_Text _betText;
    [SerializeField] float[] _betAmounts;
    int _bet = 0;
    float _balance = 10.00f;

    [Header("Game")]
    [SerializeField] Button[] _buttons;
    [SerializeField] GameObject[] _wheels;

    [Header("Winnings")]
    [SerializeField] GameObject[] _winLines;
    [SerializeField] float _flashTime = 0.2f;
    [SerializeField] int _flashAmount = 5;
    [SerializeField] GameObject _winningScreen;
    [SerializeField] TMP_Text _winningText;
    float _winnings = 0.0f;


    //For win checking
    List<List<SymbolWeight>> _winCheck = new List<List<SymbolWeight>>();

    int _resetHold = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        for(int i = 0; i < 5; i++)
            _winCheck.Add(new List<SymbolWeight>());

        //Just to be sure
        Application.targetFrameRate = 60;
    }
    private void OnEnable()
    {
        //Delegate all buttons
        _buttons[0].onClick.AddListener(delegate { Pull(); });
        _buttons[1].onClick.AddListener(delegate { Bet(true); });
        _buttons[2].onClick.AddListener(delegate { Bet(false); });
    }
    private void OnDisable()
    {
        //Remove all listners
        _buttons[0].onClick.RemoveAllListeners();
        _buttons[1].onClick.RemoveAllListeners();
        _buttons[2].onClick.RemoveAllListeners();
    }

    private void Update()
    {
        //Because im lazy
        if(Input.GetKeyDown("space") && _buttons[0].interactable)
        {
            _bet = 1;
            _betText.text = String.Format("{0:C}", _betAmounts[_bet]);

            Pull();
        }
    }

    public void Pull()
    {
        if (_bet == 0 || _balance < _betAmounts[_bet])
        {
            SFXManager.Instance.BadClick();
            return;
        }

        _winnings = 0;

        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = false;

        for (int i = 0; i < _wheels.Length; i++)
            _wheels[i].GetComponentInParent<InfiniteScroll>().Spin();

        _balance -= _betAmounts[_bet];
        _balanceText.text = String.Format("{0:C}", _balance);
        SFXManager.Instance.SpinClick();
    }

    public void Bet(bool increase)
    {
        if(increase)
        {
            if (_bet < _betAmounts.Length - 1 &&
                _balance >= _betAmounts[_bet + 1])
            {
                _bet++;
                _betText.text = String.Format("{0:C}", _betAmounts[_bet]);
                SFXManager.Instance.GoodClick();
            }
            else
            {
                SFXManager.Instance.BadClick();
            }
        }
        else
        {
            if (_bet > 0)
            {
                _bet--;
                _betText.text = String.Format("{0:C}", _betAmounts[_bet]);
                SFXManager.Instance.GoodClick();
            }
            else
            {
                 SFXManager.Instance.BadClick();
            }
        }
    }

    public void Reset()
    {
        _resetHold++;
        SFXManager.Instance.SpinSoundReduce();

        //make sure all 3 wheels are done
        if (_resetHold != 3) return;

        _resetHold = 0;
        SFXManager.Instance.SpinStop();

        //reset the bars for checking
        for (int i = 0; i < _winCheck.Count; i++)
            _winCheck[i].Clear();

        for (int i = 0; i < _wheels.Length; i++)
        {
            //Horizontal bars
            _winCheck[0].Add(_wheels[i].transform.GetChild(1).gameObject.GetComponent<Symbol>().GetSymbolWeight);
            _winCheck[1].Add(_wheels[i].transform.GetChild(2).gameObject.GetComponent<Symbol>().GetSymbolWeight);
            _winCheck[2].Add(_wheels[i].transform.GetChild(3).gameObject.GetComponent<Symbol>().GetSymbolWeight);

            //Vertical bars
            _winCheck[3].Add(_wheels[i].transform.GetChild(i + 1).gameObject.GetComponent<Symbol>().GetSymbolWeight);
            _winCheck[4].Add(_wheels[i].transform.GetChild(3 - i).gameObject.GetComponent<Symbol>().GetSymbolWeight);
        }

        if(!CheckForWin())
            for (int i = 0; i < _buttons.Length; i++) //No win the buttons will come back instantly
                _buttons[i].interactable = true;
    }

    public int GetRandomWeightedIndex(List<int> weights)
    {
        int weightSum = 0;
        //Get total sum of all weights
        for (int i = 0; i < weights.Count; i++)
            weightSum += weights[i];

        int index = 0;

        int lastIndex = weights.Count - 1;

        //Loop over 'all' weights
        while (index < lastIndex)
        {
            //If random number is less than the current index we return that index
            if (UnityEngine.Random.Range(0, weightSum) < weights[index] * Mathf.CeilToInt((_bet + 1) / 2))
                return index;

            //Remove weight from total sum
            weightSum -= weights[index++];
        }

        return index;
    }

    bool CheckForWin()
    {
        bool value = false;

        for(int i = 0; i < _winCheck.Count; i++)
        {
            if(_winCheck[i][0] == _winCheck[i][1] && _winCheck[i][0] == _winCheck[i][2])
            {
                value = true;
                StartCoroutine(WinFlash(_winLines[i], _winCheck[i][0]._muliplyer));
            }
        }

        if(value) StartCoroutine(ShowWinings());

        return value;
    }

    IEnumerator WinFlash(GameObject winLine, int multipliyer)
    {
        _winnings += _betAmounts[_bet] * multipliyer;

        for(int i = 0; i < _flashAmount; i++)
        {
            winLine.SetActive(true);
            yield return new WaitForSeconds(_flashTime);
            winLine.SetActive(false);
            yield return new WaitForSeconds(_flashTime);
        }
    }

    IEnumerator ShowWinings()
    {
        //Wait for win lines to finish
        yield return new WaitForSeconds(_flashAmount * (_flashTime * 2));
        
        _winningScreen.SetActive(true);
        _winningText.text = String.Format("{0:C}", _winnings);

        //Temp 2 second wait
        yield return new WaitForSeconds(2.0f);

        _balance += _winnings;
        _balanceText.text = String.Format("{0:C}", _balance);

        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = true;

        _winningScreen.SetActive(false);
    }
}
