using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using ZenvaSurvival.Items;

namespace ZenvaSurvival.Player
{
    public class EquipManager : MonoBehaviour
    {
        public Equip curEquip;
        public Transform equipParent;

        private PlayerController controller;

        //singleton
        public static EquipManager instance;

        private void Awake()
        {
            instance = this;
            controller = GetComponent<PlayerController>();
        }

        // called when we press the Left Mouse Button - managed by the Input System
        public void OnAttackInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
            {
                curEquip.OnAttackInput();
            }
        }
        
        // called when we press the Right Mouse Button - managed by the Input System
        public void OnAltAttackInput (InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
            {
                curEquip.OnAltAttackInput();
            }
        }

        // called when we equip an item
        public void EquipNew(ItemData item)
        {
            UnEquip();
            curEquip = Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>();
        }

        // called when we un-equip an item
        public void UnEquip()
        {
            if(curEquip != null)
            {
                Destroy(curEquip.gameObject);
                curEquip = null;
            }
        }
    }
}