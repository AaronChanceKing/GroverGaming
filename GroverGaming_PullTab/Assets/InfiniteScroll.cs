using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] [Tooltip("Parent object holding all slots")] 
    GameObject _content;

    [SerializeField] [Tooltip("Where the wheel needs to start looping")]
    RectTransform _endPoint;

    [SerializeField] [Tooltip("Where the panel will loop back to")] 
    RectTransform _resetPoint; 

    [SerializeField] [Tooltip("To stop the wheel in view and keep all 3 on track")]
    RectTransform _stopPoint;

    [SerializeField] [Tooltip("All possible symbols to pick")]
    List<SymbolWeight> _symbols;

    [Header("Rotation")]
    [SerializeField] [Tooltip("How fast the bars will spin")]
    float _speed;
    [SerializeField] [Tooltip("How long the bars will spin")]
    int _rotations;

    float _start;
    float _reset;

    private void Awake()
    {
        //Keep track of the y postion since object only move up
        _start = _endPoint.localPosition.y;
        _reset = _resetPoint.localPosition.y;

        _content.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    public void Spin() => StartCoroutine(SpinCoroutine());

    IEnumerator SpinCoroutine()
    {
        _content.GetComponent<VerticalLayoutGroup>().enabled = false;

        SetSymbols();

        List<RectTransform> children = new List<RectTransform>();
        for (int i = 1; i < _content.transform.childCount - 1; i++)
            children.Add(_content.transform.GetChild(i).gameObject.GetComponent<RectTransform>());

        int rotations = children.Count * _rotations;
        //Spin the wheel!!
        while (rotations > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].localPosition += new Vector3(0, (rotations < _content.transform.childCount - 2 ? _speed / 2 : _speed) * Time.fixedDeltaTime, 0);

                if (children[i].localPosition.y > _start)
                {
                    children[i].localPosition = new Vector3(children[i].localPosition.x, _reset, 0);
                    children[i].transform.SetSiblingIndex(9);
                    rotations--;
                }
            }
            yield return null;
        }

        GameManager.Instance.Reset();

        _content.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    void SetSymbols()
    {
        //Set symbols based on weighed randomness
        List<int> rand = new List<int>();
        List<SymbolWeight> symbols = new List<SymbolWeight>(_symbols);

        for (int i = 1; i < _content.transform.childCount - 1; i++)
        {
            for (int j = 0; j < symbols.Count; j++)
                rand.Add(symbols[j]._weight);

            int pick = GameManager.Instance.GetRandomWeightedIndex(rand);
            _content.transform.GetChild(i).gameObject.GetComponent<Symbol>().SetSymbol(symbols[pick]);

            symbols.RemoveAt(pick);
            rand.Clear();
        }

        symbols.Clear();
    }
}
