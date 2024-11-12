using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public delegate bool TrySummonHandler(int mana);
    public static event TrySummonHandler OnTrySummon;

    public delegate void SummonHandler(int mana);
    public static event SummonHandler OnSummon;

    private Vector2 originalPosition;

    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private GameObject _gameObject;

    private bool canSummon = true;

    private Vector2 canvasSize;

    private Game _game;

    void Start()
    {
        canvasSize = canvasRectTransform.sizeDelta;
        _game = FindObjectOfType<Game>();
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
            OnSummon?.Invoke(_gameObject.GetComponent<Avatar>().manaCost);
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canSummon = OnTrySummon?.Invoke(_gameObject.GetComponent<Avatar>().manaCost) ?? true;
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

    void OnEnable()
    {
        Game.OnHeroUpgradable += UpgradeHero; // Subscribe to the event
    }

    void OnDisable()
    {
        Game.OnHeroUpgradable -= UpgradeHero; // Unsubscribe to avoid memory leaks
    }

    public bool UpgradeHero()
    {
        transform.GetComponentInChildren<Image>().gameObject.SetActive(true);
        return false;
    }
}
