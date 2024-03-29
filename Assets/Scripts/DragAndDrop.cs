﻿using Models;
using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class DragAndDrop : DraggablePanel, IEndDragHandler
    {
        private CanvasGroup canvasGroup;
        private Transform slot;
        private int slotIndex;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            slot = transform.parent;
            slotIndex = slot.GetSiblingIndex();
        }

        public InventoryItem Item { get; set; }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;

            slot.SetAsLastSibling();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var maybeSlot = eventData.pointerCurrentRaycast.gameObject;
            rectTransform.position = initRectPos;

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            slot.SetSiblingIndex(slotIndex);

            if (!maybeSlot.TryGetComponent(out DragAndDropTarget dropTarget))
                return;

            Item.Inventory.Remove(Item);

            Item.Slot = dropTarget.slotNum;
            dropTarget.inventory.Add(Item);
        }
    }
}
