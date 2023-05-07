using UnityEngine;
using ZenvaSurvival.Player;

namespace ZenvaSurvival.Items
{
    public class ItemObject : MonoBehaviour, IInteractable
    {
        public ItemData item;
        
        public string GetInteractPrompt()
        {
            return string.Format("Pickup {0}", item.displayName);
        }

        public void OnInteract()
        {
            Inventory.instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}