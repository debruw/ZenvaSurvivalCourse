using UnityEngine;
using ZenvaSurvival.Items;

namespace ZenvaSurvival.Crafting
{
    [CreateAssetMenu(fileName = "Crafting Recipe", menuName = "New Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public ItemData itemToCraft;
        public ResourceCost[] cost;
    }

    [System.Serializable]
    public class ResourceCost
    {
        public ItemData item;
        public int quantity;
    }
}