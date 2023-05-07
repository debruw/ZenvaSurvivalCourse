﻿using System;
using UnityEngine;
using ZenvaSurvival.Player;

namespace ZenvaSurvival.Crafting
{
    public class CraftingTable : Building, IInteractable
    {
        private CraftingWindow craftingWindow;
        private PlayerController player;

        private void Start()
        {
            craftingWindow = FindObjectOfType<CraftingWindow>(true);
            player = FindObjectOfType<PlayerController>(true);
        }

        public string GetInteractPrompt()
        {
            return "Craft";
        }

        public void OnInteract()
        {
            craftingWindow.gameObject.SetActive(true);
            player.ToggleCursor(true);
        }
    }
}