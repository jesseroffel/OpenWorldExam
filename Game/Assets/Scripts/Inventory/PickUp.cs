﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PickUp : MonoBehaviour
{
    public int itemId;
    Inventory inventory;
    private bool active = true;
    [Header("[HUD] Item Unlocked")]
    public GameObject ItemFoundPanel;
    public GameObject ItemFoundPrefab;


    void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        //ItemFoundPanel = GameObject.FindGameObjectWithTag("ItemFoundPanel");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && active)
        {
            //inventory.AddItem(itemId);
            if (inventory == null)
            {
                GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>().AddItem(itemId);
            }
            else
            {
                inventory.AddItem(itemId);
            }
            active = false;
            QuestList.QuestListObject.RegisterItemID(itemId);
            ShowPickUp(itemId);

            Destroy(this.gameObject);
        }
    }


    bool ShowPickUp(int id)
    {
        if (ItemFoundPrefab && ItemFoundPanel)
        {
            GameObject Unlock = Instantiate(ItemFoundPrefab);
            Unlock.transform.SetParent(ItemFoundPanel.transform);
            Unlock.transform.localScale = new Vector3(1, 1, 1);
            Unlock.transform.position = new Vector3(0, 0, 0);


            string ItemText = GameObject.FindGameObjectWithTag("Inventory").GetComponent<ItemDatabase>().FetchItemNameByID(id);
            Transform Title = Unlock.transform.GetChild(1);
            Title.GetComponent<Text>().text = ItemText;

            Debug.Log("[QUESTLOG] Displaying Item Found [" + ItemText + "]");
            return true;
        }
        else
        {
            return false;
        }
    }
}
