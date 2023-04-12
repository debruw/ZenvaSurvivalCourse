using System;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using ZenvaSurvival.Items;
using ZenvaSurvival.Ui;

namespace ZenvaSurvival.Player
{
    public class Inventory : MonoBehaviour
    {
        public ItemSlotUI[] uiSlots;
        public ItemSlot[] slots;
        
        public GameObject inventoryWindow;
        public Transform dropPosition;
        
        [Header("Selected Item")]
        private ItemSlot selectedItem;
        private int selectedItemIndex;
        public TextMeshProUGUI selectedItemName;
        public TextMeshProUGUI selectedItemDescription;
        public TextMeshProUGUI selectedItemStatNames;
        public TextMeshProUGUI selectedItemStatValues;
        public GameObject useButton;
        public GameObject equipButton;
        public GameObject unEquipButton;
        public GameObject dropButton;
        
        private int curEquipIndex;
        
        // components
        private PlayerController controller;
        private PlayerNeeds needs;
        
        [Header("Events")]
        public UnityEvent onOpenInventory;
        public UnityEvent onCloseInventory;

        // singleton
        public static Inventory instance;
        
        void Awake ()
        {
            instance = this;
            controller = GetComponent<PlayerController>();
            needs = GetComponent<PlayerNeeds>();
        }

        private void Start()
        {
            inventoryWindow.SetActive(false);
            slots = new ItemSlot[uiSlots.Length];
            // initialize the slots
            for(int x = 0; x < slots.Length; x++)
            {
                slots[x] = new ItemSlot();
                uiSlots[x].index = x;
                uiSlots[x].Clear();
            }
            ClearSelectedItemWindow();
        }

        public void OnInventoryButton(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Toggle();
            }
        }
        
        // opens or closes the inventory
        public void Toggle ()
        {
            if(inventoryWindow.activeInHierarchy)
            {
                inventoryWindow.SetActive(false);
                onCloseInventory.Invoke();
                controller.ToggleCursor(false);
            }
            else
            {
                inventoryWindow.SetActive(true);
                onOpenInventory.Invoke();
                ClearSelectedItemWindow();
                controller.ToggleCursor(true);
            }
        }

        // is the inventory currently open?
        public bool IsOpen ()
        {
            return inventoryWindow.activeInHierarchy;
        }

        // adds the requested item to the player's inventory
        public void AddItem (ItemData item)
        {
            // does this item have a stack it can be added to?
            if(item.canStack)
            {
                ItemSlot slotToStackTo = GetItemStack(item);
                if(slotToStackTo != null)
                {
                    slotToStackTo.quantity++;
                    UpdateUI();
                    return;
                }
            }
            
            ItemSlot emptySlot = GetEmptySlot();
            // do we have an empty slot for the item?
            if(emptySlot != null)
            {
                emptySlot.item = item;
                emptySlot.quantity = 1;
                UpdateUI();
                return;
            }
            // if the item can't stack and there are no empty slots - throw it away
            ThrowItem(item);
        }
        
        // spawns the item infront of the player
        void ThrowItem (ItemData item)
        {
            Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360.0f));
        }
        
        // updates the UI slots
        void UpdateUI ()
        {
            for(int x = 0; x < slots.Length; x++)
            {
                if(slots[x].item != null)
                    uiSlots[x].Set(slots[x]);
                else
                    uiSlots[x].Clear();
            }
        }
        
        // returns the item slot that the requested item can be stacked on
        // returns null if there is no stack available
        ItemSlot GetItemStack (ItemData item)
        {
            for(int x = 0; x < slots.Length; x++)
            {
                if(slots[x].item == item && slots[x].quantity < item.maxStackAmount)
                    return slots[x];
            }
            return null;
        }
        
        // returns an empty slot in the inventory
        // if there are no empty slots - return null
        ItemSlot GetEmptySlot ()
        {
            for(int x = 0; x < slots.Length; x++)
            {
                if(slots[x].item == null)
                    return slots[x];
            }
            return null;
        }

        // called when we click on an item slot
        public void SelectItem (int index)
        {
            if (slots[index].item == null)
                return;

            selectedItem = slots[index];
            selectedItemIndex = index;

            selectedItemName.text = selectedItem.item.displayName;
            selectedItemDescription.text = selectedItem.item.description;
            
            //set stat values and stat names
            selectedItemStatNames.text = string.Empty;
            selectedItemStatValues.text = string.Empty;

            for (int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                selectedItemStatNames.text += selectedItem.item.consumables[x].type.ToString() + "\n";
                selectedItemStatValues.text += selectedItem.item.consumables[x].value.ToString() + "\n";
            }
            
            useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
            equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
            unEquipButton.SetActive(selectedItem.item.type == ItemType.Consumable && uiSlots[index].equipped);
            dropButton.SetActive(true);
        }
        
        // called when the inventory opens or the currently selected item has depleted
        void ClearSelectedItemWindow()
        {
            // clear th text elements
            selectedItem = null;
            selectedItemName.text = String.Empty;
            selectedItemDescription.text = String.Empty;
            selectedItemStatNames.text = String.Empty;
            selectedItemStatValues.text = String.Empty;
            
            //disable buttons
            useButton.SetActive(false);
            equipButton.SetActive(false);
            unEquipButton.SetActive(false);
            dropButton.SetActive(false);
        }
        
        // called when the "Use" button is pressed
        public void OnUseButton ()
        {
            if (selectedItem.item.type == ItemType.Consumable)
            {
                for (int x = 0; x < selectedItem.item.consumables.Length; x++)
                {
                    switch (selectedItem.item.consumables[x].type)
                    {
                        case ConsumableType.Hunger:
                            needs.Eat(selectedItem.item.consumables[x].value);
                            break;
                        case ConsumableType.Thirst:
                            needs.Drink(selectedItem.item.consumables[x].value);
                            break;
                        case ConsumableType.Health:
                            needs.Heal(selectedItem.item.consumables[x].value);
                            break;
                        case ConsumableType.Sleep:
                            needs.Sleep(selectedItem.item.consumables[x].value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            RemoveSelectedItem();
        }
        
        // called when the "Equip" button is pressed
        public void OnEquipButton ()
        {
        }
        
        // unequips the requested item
        void UnEquip (int index)
        {
        }
        
        // called when the "UnEquip" button is pressed
        public void OnUnEquipButton ()
        {
        }
        
        // called when the "Drop" button is pressed
        public void OnDropButton ()
        {
            ThrowItem(selectedItem.item);
            RemoveSelectedItem();
        }

        // removes the currently selected item
        void RemoveSelectedItem ()
        {
            selectedItem.quantity--;
            if (selectedItem.quantity == 0)
            {
                if (uiSlots[selectedItemIndex].equipped)
                    UnEquip(selectedItemIndex);
                
                selectedItem.item = null;
                ClearSelectedItemWindow();
            }
            
            UpdateUI();
        }
        
        public void RemoveItem (ItemData item)
        {
        }
        
        // does the player have "quantity" amount of "item"s?
        public bool HasItems (ItemData item, int quantity)
        {
            return false;
        }
    }
    
    // stores information about an item slot in the inventory
    public class ItemSlot
    {
        public ItemData item;
        public int quantity;
    }
}