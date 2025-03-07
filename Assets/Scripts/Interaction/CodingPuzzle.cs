﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interaction;
using UnityEngine;

public class CodingPuzzle : InteractableObject
{
    private CodingUIHandler _codingUIHandler;
    private AnimationTrigger _animationTrigger;
    public Dialog DialogFirst;
    public Dialog DialogHasAnswer;
    public Dialog DialogPuzzleCompleted;
    private DialogManager dialogManager;
    private PlayerInteraction _playerIntercation;
    private bool PuzzleComplete;
    public PuzzleBlock RewardBlock;
    public SpriteChanger spriteChanger;
    public List<CodingPuzzle> RequiredPuzzles = new List<CodingPuzzle>();
    public List<PuzzleBlock> RequiredPuzzleBlockRewards = new List<PuzzleBlock>();

    public List<PuzzleBlock> Answer;
    [TextArea(4, 8)]
    public string PuzzleDescription;

    private void Awake()
    {
        _codingUIHandler = FindObjectOfType<CodingUIHandler>();
        _animationTrigger = GetComponentInChildren<AnimationTrigger>();
        dialogManager = DialogManager.Instance;

        foreach (PuzzleBlock block in Answer)
        {
            if (block.Type == PuzzleBlockType.Undefined)
            {
                Debug.LogWarning("Puzzle '" + name + "' has answer puzzle block with type Undefined: " + block.Content);
            }
        }

        if (RewardBlock.Type == PuzzleBlockType.Undefined && RewardBlock.Content != "")
        {
            Debug.LogWarning("Puzzle '" + name + "' has answer puzzle block with type Undefined: " + RewardBlock.Content);
        }
    }

    protected override void OnInteractWithPlayer(PlayerInteraction playerInteraction)
    {
        _playerIntercation = playerInteraction;
        StartCoroutine(OnInteract());
    }

    public IEnumerator OnInteract()
    {
        if (dialogManager.DialogActive || _codingUIHandler.CodingUIContainer.activeSelf)
            yield break;

        bool hasAnswer = true;
        foreach (PuzzleBlock block in Answer)
        {
            if (block.Type != PuzzleBlockType.None && !_playerIntercation.Inventory.Contains(block))
                hasAnswer = false;
        }
        if (PuzzleComplete)
        {
            // if the puzzle is finished
            yield return dialogManager.StartDialog(DialogPuzzleCompleted);
        }
        else if (hasAnswer || CheckRequiredPuzzles())
        {
            // if Hedy got the answer before the puzzle
            yield return dialogManager.StartDialog(DialogHasAnswer);
            _codingUIHandler.ShowCodingUI(_playerIntercation.Inventory, Answer, PuzzleDescription, OnPuzzleCompleteCallback, OnPuzzleWrongCallback);
        }
        else
        {
            // if Hedy doesn't have the answer before the puzzle
            yield return dialogManager.StartDialog(DialogFirst);
        }
    }

    public void OnPuzzleCompleteCallback()
    {
        if (spriteChanger != null)
        {
            spriteChanger.ChangeSprite();
        }
        _animationTrigger.StartAnimation();
        if (RewardBlock != null && !string.IsNullOrEmpty(RewardBlock.Content))
        {
            _player.Inventory.Add(RewardBlock);
        }
        Debug.Log("Correct answer");
        PuzzleComplete = true;
    }

    // Used for puzzles with limited amount of tries or similar
    public void OnPuzzleWrongCallback()
    {
    }

    public bool CheckRequiredPuzzles()
    {
        if (RequiredPuzzles.Count > 0)
        {
            for (int i = 0; i < RequiredPuzzles.Count; i++)
            {
                if (!RequiredPuzzles[i].PuzzleComplete)
                {
                    return false;
                }
            }
            foreach (PuzzleBlock pb in RequiredPuzzleBlockRewards)
            {
                _player.Inventory.Add(pb);
            }
            return true;
        }
        return false;
    }
}
