using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SelectCursor : MonoBehaviour
{
    [SerializeField] float m_MoveTime;

    GameObject lastSelectedGameObject;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        transform.SetParent(Transition.instance.fadeCanvasGroup.transform.parent);
        transform.SetAsFirstSibling();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current == null) return;

        var current = EventSystem.current.currentSelectedGameObject;
        if (current == null) return;

        // カーソルの移動処理
        if (lastSelectedGameObject != current)
        {
            Vector3 goal = current.transform.position;

            transform.DOMove(goal, m_MoveTime).SetEase(Ease.OutCubic);
        }

        lastSelectedGameObject = current;
    }
}
