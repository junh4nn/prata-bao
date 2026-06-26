using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("Backend Configuration")]
    [SerializeField] private string inventoryUrl = "http://localhost:3000/api/inventory";

    [Header("Data")]
    [SerializeField] private ItemRegistry         itemRegistry;
    [SerializeField] private InventoryCardDisplay inventoryCardPrefab;

    [Header("UI - Filter/Sort")]
    [SerializeField] private TMP_Dropdown    categoryDropdown;
    [SerializeField] private TMP_Dropdown    sortDropdown;
    [SerializeField] private Button          sortDirectionButton;
    [SerializeField] private TextMeshProUGUI sortDirectionIcon;

    [Header("UI - Grid")]
    [SerializeField] private Transform       gridContent;
    [SerializeField] private TextMeshProUGUI collectedCountText;

    [Header("UI - Detail Modal")]
    [SerializeField] private ItemDetailModal detailModal;

    [Header("Navigation")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private Button     backButton;

    private InventoryItemRow[] allRows = Array.Empty<InventoryItemRow>();
    private bool sortAscending = true;

    void Start()
    {
        if (categoryDropdown    != null) categoryDropdown.onValueChanged.AddListener(_ => RefreshGrid());
        if (sortDropdown        != null) sortDropdown.onValueChanged.AddListener(_ => RefreshGrid());
        if (sortDirectionButton != null) sortDirectionButton.onClick.AddListener(OnSortDirectionClicked);
        if (backButton          != null) backButton.onClick.AddListener(OnBackClicked);
        if (sortDirectionIcon   != null) sortDirectionIcon.text = "▲";
    }

    void OnEnable()
    {
        StartCoroutine(FetchInventory());
    }

    private IEnumerator FetchInventory()
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{inventoryUrl}/{AuthManager.UserId}"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[InventoryManager] Failed to load inventory: {request.error}");
                yield break;
            }

            allRows = JsonUtility.FromJson<InventoryResponse>(request.downloadHandler.text).items;
            RefreshGrid();
        }
    }

    private void OnSortDirectionClicked()
    {
        sortAscending = !sortAscending;
        if (sortDirectionIcon != null) sortDirectionIcon.text = sortAscending ? "▲" : "▼";
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        IEnumerable<InventoryItemRow> rows = allRows;

        int categoryIndex = categoryDropdown != null ? categoryDropdown.value : 0;
        string typeFilter = categoryIndex switch { 1 => "Fauna", 2 => "Flora", _ => null };
        if (typeFilter != null)
            rows = rows.Where(r => r.type == typeFilter);

        int sortIndex = sortDropdown != null ? sortDropdown.value : 0;
        rows = sortIndex switch
        {
            1 => sortAscending ? rows.OrderBy(r => r.name) : rows.OrderByDescending(r => r.name),
            2 => sortAscending ? rows.OrderBy(r => r.firstObtainedAt) : rows.OrderByDescending(r => r.firstObtainedAt),
            _ => sortAscending
                ? rows.OrderBy(r => RarityVisuals.GetStarCount(Enum.Parse<Rarity>(r.rarity))).ThenBy(r => r.itemId)
                : rows.OrderByDescending(r => RarityVisuals.GetStarCount(Enum.Parse<Rarity>(r.rarity))).ThenBy(r => r.itemId)
        };

        List<InventoryItemRow> visibleRows = rows.ToList();

        if (gridContent != null)
        {
            foreach (Transform child in gridContent)
                Destroy(child.gameObject);

            if (inventoryCardPrefab != null)
            {
                foreach (InventoryItemRow row in visibleRows)
                {
                    InventoryCardDisplay card = Instantiate(inventoryCardPrefab, gridContent);
                    card.OnClicked = OnCardClicked;
                    card.Setup(row, itemRegistry != null ? itemRegistry.FindById(row.itemId) : null);
                }
            }
        }

        if (collectedCountText != null)
            collectedCountText.text = $"COLLECTED · {visibleRows.Count} CARDS";
    }

    private void OnCardClicked(InventoryItemRow row, ItemDefinition visuals)
    {
        if (detailModal != null) detailModal.Show(row, visuals);
    }

    private void OnBackClicked()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (hubPanel       != null) hubPanel.SetActive(true);
    }
}
