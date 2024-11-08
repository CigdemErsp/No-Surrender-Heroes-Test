using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public delegate bool SummonHandler(int mana);
    public static event SummonHandler OnSummon;

    private Vector2 originalPosition;

    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private GameObject _gameObject;

    private bool canSummon = true;

    private Vector2 canvasSize;

    void Start()
    {
        canvasSize = canvasRectTransform.sizeDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canSummon)
            return;
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canSummon)
            return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canSummon)
        {
            // Debug.Log("OnEndDrag");
            transform.position = originalPosition;
            
            Vector2 canvasPos = eventData.position;

            if (canvasPos.y > ((canvasSize.y / 2) + 100))
                canvasPos.y = (canvasSize.y / 2) + 100;

            Ray ray = Camera.main.ScreenPointToRay(canvasPos);
            RaycastHit hit;

            // Perform a raycast to see if we hit the cube
            if (Physics.Raycast(ray, out hit))
            {
                // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f);
                // Check if the hit object is the cube
                if (hit.collider.CompareTag("Table")) // Ensure your cube has the correct tag
                {
                    // Get the hit point
                    Vector3 dropPositionWorld = hit.point;

                    // Place your item at this position
                    GameObject newCharacter = Instantiate(_gameObject, dropPositionWorld, Quaternion.identity);
                    newCharacter.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    newCharacter.tag = "Team 2";
                }
            }
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canSummon = OnSummon?.Invoke(_gameObject.GetComponent<Avatar>().manaCost) ?? true;
        if(!canSummon)
        {
            // Debug.Log("Not enough mana");
            return;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log("OnDrop");
    }
}
