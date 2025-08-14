using System;
using TMPro;
using UnityEngine;


public class DialogueHandler : MonoBehaviour
{
    public bool IsFinishDialogue => !dialogueUIRootObj.activeSelf;
    
    [SerializeField] private GameObject dialogueUIRootObj;
    [SerializeField] private TMP_Text dialogueText;

    private PlayerInputController _inputController;
    private DialogueEntry _dialogueEntry;

    private void Awake()
    {
        dialogueUIRootObj.SetActive(false);
    }

    public void EnableDialogue(PlayerController player, DialogueEntry dialogueEntry)
    {
        _dialogueEntry = dialogueEntry;
        
        if (_dialogueEntry == null)
        {
            Logger.LogError("[DialogueController] _dialogueEntry가 null입니다. 대화 데이터를 확인하세요.");
            return;
        }
        
        dialogueText.text = _dialogueEntry.dialogText;
        dialogueUIRootObj.SetActive(true);

        _inputController = player.InputController;
        _inputController.EnableDialogueUIInputs();
        _inputController.OnNextDialogue += FinishDialogue; // 임시 - 대화 바로종료
        _inputController.OnClosePopupUI += FinishDialogue;
    }

    private void FinishDialogue()
    {
        dialogueUIRootObj.SetActive(false);
        
        _inputController.EnablePlayerInputs();
        _inputController.OnNextDialogue -= FinishDialogue;
        _inputController.OnClosePopupUI -= FinishDialogue;
    }
}
