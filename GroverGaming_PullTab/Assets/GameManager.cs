using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] TMP_Text _balanceText;
    [Space(10)]
    [SerializeField] TMP_Text _betText;
    [SerializeField] float[] _betAmounts;
    int _bet = 0;
    float _balance = 10.00f;

    [Header("Game")]
    [SerializeField] Button[] _buttons;
    [Space(10)]
    [SerializeField] AudioClip _goodClick;
    [SerializeField] AudioClip _badClick;
    [SerializeField] AudioClip _spinClick;


    public void Pull()
    {
        if (_bet == 0)
        {
            SFXManager.Instance.PlayOnShot(_badClick);
            return;
        }

        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = false;

        _balance -= _betAmounts[_bet];
        _balanceText.text = String.Format("{0:C}", _balance);
        SFXManager.Instance.PlayOnShot(_spinClick);
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
                SFXManager.Instance.PlayOnShot(_goodClick);
            }
            else
            {
                SFXManager.Instance.PlayOnShot(_badClick);
            }
        }
        else
        {
            if (_bet > 0)
            {
                _bet--;
                _betText.text = String.Format("{0:C}", _betAmounts[_bet]);
                SFXManager.Instance.PlayOnShot(_goodClick);
            }
            else
            {
                SFXManager.Instance.PlayOnShot(_badClick);
            }
        }
    }
}
