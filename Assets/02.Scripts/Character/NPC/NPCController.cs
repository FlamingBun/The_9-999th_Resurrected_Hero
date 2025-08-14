using System.Collections;
using System.Linq;
using UnityEngine;

public class NPCController : CharacterBaseController, IInteractable
{
    public bool CanInteract { get; set; } = true;
    
    
    [SerializeField] protected DialogueDataSO dialogueData;
    //[SerializeField] private QuestPresenter questPresenter;
    //[SerializeField] private string questIdToRegister = "tutorial_001";

    private CameraManager _cameraManager;
    private PlayerController _player;
    private SpriteRenderer _renderer;
    private DialogueHandler _dialogueHandler;
    private TownItemShopHandler _townItemShopHandler;
    private Coroutine _interactSequence;
    private InteractGuideUI _interactGuideUI;
    private LetterBoxUI _letterBoxUI;

    
    private Vector2 _defaultPos;
    
    private const float MaxMoveBackDist = 1.5f;
    

    protected override void Awake()
    {
        base.Awake();

        CanDamageable = false;
        
        _renderer = GetComponent<SpriteRenderer>();
        _dialogueHandler = GetComponent<DialogueHandler>();
        _townItemShopHandler = GetComponent<TownItemShopHandler>();

        _defaultPos = Rigid.position;
    }

    protected override void Start()
    {
        base.Start();
        
        _cameraManager = CameraManager.Instance;
        
        var uiManager = UIManager.Instance;
        
        _interactGuideUI = uiManager.GetUI<InteractGuideUI>();
        _letterBoxUI = uiManager.GetUI<LetterBoxUI>();

    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if ((_defaultPos - Rigid.position).sqrMagnitude > MaxMoveBackDist)
        {
            Vector3 targetPos = Vector3.MoveTowards(Rigid.position, _defaultPos, 1 * Time.fixedDeltaTime);
            Rigid.MovePosition(targetPos);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = player;
            LookAt(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            if (player == _player)
            {
                _player = null;
            }
        }
    }
    

    public void Interact(PlayerController player)
    {
        _player = player;

        if (_interactSequence != null)
        {
            StopCoroutine(_interactSequence);
        }

        _interactSequence = StartCoroutine(InteractSequence(player));
        
        _interactGuideUI.Disable();
    }

    public void OnEnter()
    {
        _interactGuideUI.InitTarget(transform);
        _interactGuideUI.Enable();
    }

    public void OnExit()
    {
        _interactGuideUI.Disable();

        CanInteract = true;
    }

    IEnumerator InteractSequence(PlayerController player)
    {
        CanInteract = false;
        
        _letterBoxUI.Enable();
        
        if (_dialogueHandler != null)
        {
            EnableDialogue(player);
            
            yield return new WaitUntil(() => _dialogueHandler.IsFinishDialogue);
        }
        
        if (_townItemShopHandler != null)
        {
            _townItemShopHandler.Enable();

            EnableShop();

            yield return new WaitUntil(() => !_townItemShopHandler.IsOpen);
        }
        
        
        _letterBoxUI.Disable();

        _cameraManager.DisableFocus();
        
        yield return new WaitUntil(() => !_letterBoxUI.IsEnabled);

        if (!CanInteract)
        {
            _interactGuideUI.Enable();
            
            CanInteract = true;
        }
    }

    private void EnableDialogue(PlayerController player)
    {
        /*string npcId = dialogueData.npcId;

        int friendship = NpcFriendshipManager.Instance.GetFriendship(npcId);*/
        AudioManager.Instance.Play(key: "InteractNPCClip");


        if (dialogueData.voiceKey != "")
        {
            AudioManager.Instance.Play(dialogueData.voiceKey);
        }
        
        _dialogueHandler.EnableDialogue(player, GetDialogueEntry());

        _cameraManager.EnableFocus(transform);
    }
    

    private void EnableShop()
    {
                
        // tutorial_005 퀘스트 클리어 처리
        /*var questManager = FindObjectOfType<QuestManager>();
        if (questManager != null && questManager.IsQuestAccepted("tutorial_005"))
        {
            questManager.ClearQuest("tutorial_005");
            Logger.Log("[퀘스트 클리어] tutorial_005 - 강해지는 방법");
        }*/
    }


    private void FinishInteract()
    {
        _interactGuideUI.Enable();


        /*if (questPresenter == null)
        {
            Logger.Log($"[NPCController] {name}에 연결된 QuestPresenter가 없습니다.");
            return;
        }

        // 현재 NPC가 줄 퀘스트 등록
        if (!string.IsNullOrEmpty(questIdToRegister))
        {
            questPresenter.RegisterQuestById(questIdToRegister);
        }*/

        /*var questManager = FindObjectOfType<QuestManager>();
        if (questManager == null) return;*/

        // ✅ 가장 마지막에 클리어한 퀘스트 기준으로 다음 퀘스트 찾아서 등록
        /*foreach (var quest in questManager.GetAllQuests())
        {
            if (quest.isCleared)
            {
                string nextQuestId = questManager.GetNextSequentialQuestId(quest.questId);
                if (!string.IsNullOrEmpty(nextQuestId))
                {
                    Logger.Log($"[NPCController] 다음 퀘스트 등록: {nextQuestId}");
                    questPresenter.RegisterQuestById(nextQuestId);
                    break; // 한 개만 등록하고 종료
                }
            }
        }*/
    }



    private void LookAt(PlayerController player)
    {
        Vector2 direction = player.transform.position - transform.position;
        _renderer.flipX =  direction.x < 0;
    }
    

    private DialogueEntry GetDialogueEntry()
    {
        // 높은 친밀도 조건이 먼저 체크되도록 정렬
        var sortedList = dialogueData.dialogEntries;
        sortedList = sortedList.OrderBy(_ => Random.value).ToList();
        
        /*sortedList.Sort((a, b) => b.friendshipThreshold.CompareTo(a.friendshipThreshold));

        foreach (var entry in sortedList)
        {
            if (friendship >= entry.friendshipThreshold)
            {
                return entry;
            }
        }

        Logger.LogError("[BaseNPCController] 조건에 맞는 대사를 찾지 못했습니다. 기본 대사를 출력합니다.");*/
        return sortedList.Count > 0 ? sortedList[0] : null;
    }
}
