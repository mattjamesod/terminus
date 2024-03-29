﻿using Helpers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Controllers
{
    public class DialogueController : MonoBehaviour
    {
        #region Singleton
        public static DialogueController Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Editor

        public GameObject dialogueUI;
        public GameObject dialogueOption;
        public GameObject dialogueResponse;
        public GameObject scrollBar;

        #endregion

        #region UI

        private GameObject header;
        private GameObject optionsList;

        #endregion

        #region Logic

        public DialogueTree Conversation;

        private List<DialogueNode> currentOptions
            = new List<DialogueNode>();

        public IList<Action> onDialogueStart
            = new List<Action>();

        public IList<Action> onDialogueDecisionTime
            = new List<Action>();

        public IList<Action> onDialogueDecisionMade
            = new List<Action>();

        public IList<Action> onDialogueEnd
            = new List<Action>();

        #endregion

        private void Start()
        {
            header = dialogueUI.transform.GetChild(0).gameObject;
            optionsList = dialogueUI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        }

        public void BeginDialogue(DialogueTree conversation)
        {
            this.Conversation = conversation;

            header.GetComponentInChildren<TextMeshProUGUI>().text = conversation.Name;

            if (conversation?.Dialogue == null)
            {
                Debug.Log(conversation.NoDialogueGreeting);
                return;
            }

            onDialogueStart.InvokeAll();

            if (Conversation.Locking)
            {
                WaitingForDecision = WaitForDecisionPoint(3);
                StartCoroutine(WaitingForDecision); // generate delay from player stats
            }
            
            AddOptions(conversation.Dialogue);

            RefreshUI();

            dialogueUI.SetActive(true);
        }

        private IEnumerator WaitingForDecision;

        private IEnumerator WaitForDecisionPoint(float delay)
        {
            yield return new WaitForSeconds(delay);

            onDialogueDecisionTime.InvokeAll();
        }

        public void EndDialogue()
        {
            dialogueUI.SetActive(false);

            currentOptions.Clear();

            ClearDialogueHistory();
        }

        private void RefreshUI()
        {
            int i = 1;

            foreach (var option in currentOptions)
            {
                var optionUI = Instantiate(dialogueOption);

                optionUI.tag = "DialogueOption";
                optionUI.transform.SetParent(optionsList.transform);
                optionUI.GetComponentInChildren<TextMeshProUGUI>().text = $"{i++}. {option.Prompt}";
                optionUI.GetComponent<Button>().onClick.AddListener(() => EnterNode(option));
            }

            scrollBar.GetComponent<Scrollbar>().value = 0;
        }

        private void EnterNode(DialogueNode node)
        {
            StopCoroutine(WaitingForDecision);
            onDialogueDecisionMade.InvokeAll();

            foreach (Transform child in optionsList.transform)
            {
                if (child.gameObject.tag == "DialogueOption")
                    Destroy(child.gameObject);
            }

            var optionSelectedUI = Instantiate(dialogueResponse);

            optionSelectedUI.transform.SetParent(optionsList.transform);
            optionSelectedUI.GetComponentInChildren<TextMeshProUGUI>().text = node.Prompt;
            optionSelectedUI.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Italic;

            StartCoroutine(SpeakDialogue(node));
        }

        private IEnumerator SpeakDialogue(DialogueNode node)
        {
            foreach (var line in node.Lines)
            {
                scrollBar.GetComponent<Scrollbar>().value = 0;

                var responseUI = Instantiate(dialogueResponse);

                responseUI.transform.SetParent(optionsList.transform);
                responseUI.GetComponentInChildren<TextMeshProUGUI>().text = line.Text;

                yield return new WaitForSeconds(line.Length);
            }

            BranchDialogue(node);
        }

        private void BranchDialogue(DialogueNode node)
        {
            node.ExecuteAction(Conversation.Owner);

            if (node.Siblings != null)
                AddOptions(node.Siblings);

            if (node.Children != null)
            {
                currentOptions = currentOptions.Where(x => x.Persist && x.Display(Conversation.Owner)).ToList();
                AddOptions(node.Children);
            }

            RefreshUI();
        }

        private void ClearDialogueHistory()
        {
            foreach (Transform child in optionsList.transform)
                Destroy(child.gameObject);
        }

        private void AddOptions(IEnumerable<DialogueNode> options)
            => currentOptions.AddRange(options.Where(x => x.Display(Conversation.Owner)));
    }
}
