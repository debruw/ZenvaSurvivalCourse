using System;
using UnityEngine;

namespace ZenvaSurvival.Player
{
    public class EquipTool : Equip
    {
        public float attactRate;
        private bool attacking;
        public float attackDistance;

        [Header("Resource Gathering")] 
        public bool doesGatherResources;

        [Header("Combat")] 
        public bool doesDealDamage;
        public int damage;
        
        //components
        private Animator anim;
        private Camera cam;

        private void Awake()
        {
            //get our components
            anim = GetComponent<Animator>();
            cam= Camera.main;
        }

        // called when we press the attack input
        public override void OnAttackInput()
        {
            base.OnAttackInput();
        }
    }
}