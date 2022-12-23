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
    [SerializeField] GameObject[] _bars;
    [Space(20)]
    [SerializeField] GameObject[] _winLines;
    [SerializeField] float _flashTime = 0.2f;
    [SerializeField] int _flashAmount = 5;

    List<SymbolWeight> _hor0;
    List<SymbolWeight> _hor1;
    List<SymbolWeight> _hor2;
    List<SymbolWeight> _vert0;
    List<SymbolWeight> _vert1;

    int _resetHold = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;

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

    public void Pull()
    {
        if (_bet == 0)
        {
            SFXManager.Instance.BadClick();
            return;
        }

        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = false;

        for (int i = 0; i < _winLines.Length; i++)
            _winLines[i].SetActive(false);

        for (int i = 0; i < _bars.Length; i++)
            _bars[i].GetComponentInParent<InfiniteScroll>().Spin();

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
        if(_resetHold == 3)
        {
            _resetHold = 0;
            SFXManager.Instance.SpinStop();

            //reset the bars for checking
            _hor0 = new List<SymbolWeight>();
            _hor1 = new List<SymbolWeight>();
            _hor2 = new List<SymbolWeight>();
            _vert0 = new List<SymbolWeight>();
            _vert1 = new List<SymbolWeight>();

            for (int i = 0; i < _bars.Length; i++)
            {
                //Horizontal bars
                _hor0.Add(_bars[i].transform.GetChild(1).gameObject.GetComponent<Symbol>().symbolWeight);
                _hor1.Add(_bars[i].transform.GetChild(2).gameObject.GetComponent<Symbol>().symbolWeight);
                _hor2.Add(_bars[i].transform.GetChild(3).gameObject.GetComponent<Symbol>().symbolWeight);

                //Vertical bars
                _vert0.Add(_bars[i].transform.GetChild(i + 1).gameObject.GetComponent<Symbol>().symbolWeight);
                _vert1.Add(_bars[i].transform.GetChild(3 - i).gameObject.GetComponent<Symbol>().symbolWeight);
            }

            if(!CheckForWin())
            {
                //No win the buttons will come back instantly
                for (int i = 0; i < _buttons.Length; i++)
                    _buttons[i].interactable = true;
            }
        }
    }

    public int GetRandomWeightedIndex(List<int> weights)
    {
        int weightSum = 0;
        for (int i = 0; i < weights.Count; ++i)
            weightSum += weights[i];

        int index = 0;
        int lastIndex = weights.Count - 1;
        while (index < lastIndex)
        {
            if (UnityEngine.Random.Range(0, weightSum) < weights[index])
                return index;

            weightSum -= weights[index++];
        }

        return index;
    }

    //TODO
    //Really ugly and want to refactor this
    bool CheckForWin()
    {
        bool value = false;

        if (_hor0[0] == _hor0[1] && _hor0[0] == _hor0[2])
        {
            StartCoroutine(WinFlash(_winLines[0], _hor0[0]._muliplyer));
            value = true;
        }

        if (_hor1[0] == _hor1[1] && _hor1[0] == _hor1[2])
        {
            StartCoroutine(WinFlash(_winLines[1], _hor1[0]._muliplyer));
            value = true;
        }

        if (_hor2[0] == _hor2[1] && _hor2[0] == _hor2[2])
        {
            StartCoroutine(WinFlash(_winLines[2], _hor2[0]._muliplyer));
            value = true;
        }

        if (_vert0[0] == _vert0[1] && _vert0[0] == _vert0[2])
        {
            StartCoroutine(WinFlash(_winLines[3], _vert0[0]._muliplyer));
            value = true;
        }

        if (_vert1[0] == _vert1[1] && _vert1[0] == _vert1[2])
        {
            StartCoroutine(WinFlash(_winLines[4], _vert1[0]._muliplyer));
            value = true;
        }

        return value;
    }

    IEnumerator WinFlash(GameObject winLine, int multipliyer)
    {
        for(int i = 0; i < _flashAmount; i++)
        {
            winLine.SetActive(true);
            yield return new WaitForSeconds(_flashTime);
            winLine.SetActive(false);
            yield return new WaitForSeconds(_flashTime);
        }

        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = true;

        float winnings = _betAmounts[_bet] * multipliyer;

        _balance += winnings;
        _balanceText.text = String.Format("{0:C}", _balance);
    }
}
