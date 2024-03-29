﻿using Models;
using Models.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Scripts.Executor;

namespace Scripts.Interactables
{
    public class Pickup : Interactable
    {
        public override string ActionName => $"Pick up {name}";

        public override bool IsAccessible(Transform InterestedParty)
            => InterestedParty.TryGetComponent(out userInventory);

        public string itemID;
        public TextAsset itemJson;
        public Item item;

        private Inventory userInventory;

        private void Start()
        {
            item = itemRepository.Get(itemID);
        }

        protected override void OnInteract(Transform interestedParty)
        {
            if (userInventory != null || interestedParty.TryGetComponent(out userInventory))
            {
                Debug.Log($"Adding {item.Name} to inventory");
                userInventory.Add(item);
                Destroy(gameObject);
            }
        }

        protected override void OnStopInteract() { }

    }
}
