using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] GameObject _content;
    [SerializeField] RectTransform _startingPoint;
    [SerializeField] RectTransform _resetPoint;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 reset;

    private void Awake()
    {
        start = _startingPoint.position;
        reset = _resetPoint.position;
        //_content.GetComponent<VerticalLayoutGroup>().enabled = false;
    }

    public void Update()
    {
        for(int i = 0; i < _content.transform.childCount; i++)
        {
            RectTransform loc = _content.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            loc.localPosition += new Vector3(0, 0.5f, 0);

            if(loc.position.y > start.y)
            {
                loc.position = reset;
            }
        }
    }
}
