﻿using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Scripts.Controllers
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(CharacterBehaviour))]
    public class PlayerController : MonoBehaviour
    {
        #region Singleton
        public static PlayerController Instance;

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance);

            Instance = this;
        }
        #endregion

        private Character character;
        public LayerMask moveablePlaces;

        void Start()
        {
            character = GetComponent<Character>();
        }

        void Update()
        {
            if (!character.Ai.isResponsive)
                return;

            if (Input.GetMouseButtonDown(0))
                HandleLeftClick();

            if (Input.GetMouseButtonDown(1))
                HandleRightClick();

            if (Input.GetKeyDown(KeyCode.T))
                character.Ai.BeginTeleport();

            if (Input.GetKeyDown(KeyCode.Space))
                ToggleGamePlayPause();
        }

        private void ToggleGamePlayPause()
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0;
            else if (Time.timeScale == 0)
                Time.timeScale = 1;
        }

        private void HandleLeftClick()
            => InputHelper.MouseClick(hit =>
                {
                    character.RemoveFocus();
                    character.Ai.MoveTo(hit.point);
                    character.Ai.StopInteracting();
                },
                mask: moveablePlaces
            );

        private void HandleRightClick()
            => InputHelper.MouseClick(hit =>
                {
                    if (hit.collider.TryGetComponent(out Interactable focus))
                    {
                        character.SetFocus(focus);
                        character.Ai.Interact(focus);
                    }
                }
            );

        public void Migrate(GameObject newPlayer)
        {
            var successor = newPlayer.AddComponent<PlayerController>();
            successor.moveablePlaces = this.moveablePlaces;
        }
    }
}