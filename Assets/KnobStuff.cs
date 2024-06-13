using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KnobStuff : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 home;
    Vector2 eventDataForUpdate;
    public Movement player;

    [SerializeField]
    bool drag;

    void Start()
    {
        home = transform.position;
        StartCoroutine("waitasec");
    }

    IEnumerable waitasec()
    {
        yield return new WaitForSeconds(0.1f);
        home = transform.position;
    }

void Update()
    {
        if (drag)
        {
            transform.position = eventDataForUpdate;
            if (Vector3.Distance(home, transform.position) > 150)
            {
                transform.position -= home;
                transform.position = transform.position.normalized * 150;
                transform.position += home;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, home, Time.deltaTime * 20);
        }

        player.aim = ((transform.position - home) / 150).normalized;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        eventDataForUpdate = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        drag = false;
        player.LaunchInput();
    }

    public void OnDrag(PointerEventData eventData)
    {
        drag = true;
        eventDataForUpdate = eventData.position;
    }
}