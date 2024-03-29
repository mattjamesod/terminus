﻿using Enums;
using Models;
using Models.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class EquipmentRig : MonoBehaviour
    {
        private Dictionary<EquipmentType, Equipment> Equipment
            = new Dictionary<EquipmentType, Equipment>();

        public Action<InventoryItem> OnItemAdded;
        public Action<Equipment> OnItemRemoved;

        public void Add(InventoryItem invDetails)
        {
            var equipment = invDetails.Item as Equipment;
            var slot = equipment.Type;

            TryUnequip(slot);

            Equipment[slot] = equipment;
            OnItemAdded?.Invoke(invDetails);
            Debug.Log($"{name} equipped {equipment.Name}!");
        }

        private void TryUnequip(EquipmentType slot)
        {
            if (Equipment.TryGetValue(slot, out Equipment foundEquipped))
            {
                OnItemRemoved?.Invoke(foundEquipped);
                Equipment[slot] = null;
            }
        }
    }
}
