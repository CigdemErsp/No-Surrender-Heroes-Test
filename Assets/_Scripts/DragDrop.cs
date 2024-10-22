using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Vector2 originalPosition;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _gameObject;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        transform.position = originalPosition;

        Vector2 canvasPos = eventData.position;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log("OnDrop");
    }
}
