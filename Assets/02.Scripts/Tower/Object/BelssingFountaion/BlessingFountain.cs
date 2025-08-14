using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlessingFountain : MonoBehaviour, IInteractable
{
    public int CurRerollPrice => _curRerollPrice;
    public bool CanInteract => !_isBlessed;
    
    [SerializeField] private BlessingFountainBlessDataSO blessData;

    [SerializeField] private ParticleSystem _buffParticle;
    
    [Space(10f)]
    [SerializeField] private int rerollDefaultPrice;
    [SerializeField] private float rerollPriceMultiplier;
    
    private TowerBlessingFountainUI _blessingFountainUI;
    private InteractGuideUI _interactGuideUI;
    private PlayerController _player;
    private SpriteRenderer _spriteRenderer;

    private bool _isBlessed;
    private int _curRerollCount;
    private int _curRerollPrice;

     private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.material.SetFloat("_Glow", 1.5f);
    }

    private void Start()
    {
        var uiManager = UIManager.Instance;

        _curRerollPrice = rerollDefaultPrice;

        _interactGuideUI = uiManager.GetUI<InteractGuideUI>();
        _blessingFountainUI = uiManager.GetUI<TowerBlessingFountainUI>();
    }

    public void Interact(PlayerController player)
    {
        _player = player;

        _isBlessed = true;
        _interactGuideUI.Disable();

        _blessingFountainUI.Enable();
        _blessingFountainUI.InitBless(this);

        _spriteRenderer.material.SetFloat("_Glow", 0f);
    }

    public void OnEnter()
    {
        if (!_isBlessed)
        {
            _interactGuideUI.InitTarget(transform);
            _interactGuideUI.Enable();
        }
    }

    public void OnExit()
    {
        _interactGuideUI.Disable();
    }

    /// <summary>
    /// 축복 목록 생성
    /// </summary>
    public List<BlessingFountainBlessInstance> GetModifier()
    {
        int blessCount = GetRandomBlessCount();
        int entryLength = blessData.ModifierEntries.Length;

        List<BlessingFountainBlessInstance> blessList = new();

        for (int i = 0; i < blessCount; i++)
        {
            var entry = blessData.ModifierEntries[Random.Range(0, entryLength)];

            BlessingFountainBlessInstance.Grade grade;
            float statValue = GetStatValue(entry, out grade);

            // 소수 둘째 자리 반올림
            statValue = Mathf.Round(statValue * 100) / 100f;

            StatModifier modifier = new StatModifier()
            {
                statType = entry.statType,
                modType = entry.modType,
                value = statValue,
                source = Constant.TowerSceneName
            };

            blessList.Add(new BlessingFountainBlessInstance(grade, modifier.modType, entry.entryName, entry.additionText, modifier));
        }

        // 정렬
        blessList = blessList
            .OrderBy(b => b.mod.statType)
            .ThenBy(b => b.grade)
            .ToList();

        return blessList;
    }

    public void Reroll()
    {
        if (_player.PlayerInstance.Soul >= _curRerollPrice)
        {
            AudioManager.Instance.Play("RerollUIClip");

            _player.PlayerInstance.ModifySoul(_curRerollPrice * -1);
            
            _curRerollCount++;
            _curRerollPrice = Mathf.RoundToInt(rerollDefaultPrice * (_curRerollCount * rerollPriceMultiplier));

            _blessingFountainUI.InitBless(this);
        }
    }

    public void PlayBuffParticle()
    {
        _buffParticle.transform.position = _player.transform.position;
        _buffParticle.Play();
    }

    /// <summary>
    /// 등급과 수치를 결정하는 로직
    /// Percent와 Flat에 따라 구간과 확률이 다름
    /// </summary>
    private float GetStatValue(BlessingFountainBlessEntry entry, out BlessingFountainBlessInstance.Grade grade)
    {
        // 1️⃣ Percent / Flat에 따른 구간 설정
        float lowMax, mediumMax;
        
        lowMax = entry.maxValue * 0.3f;
        mediumMax = entry.maxValue * 0.6f;

        // 2️⃣ 가중치(낮은 값이 잘 나옴)
        int lowWeight = 65;
        int mediumWeight = 30;
        int highWeight = 5;

        int totalWeight = lowWeight + mediumWeight + highWeight;
        int roll = Random.Range(1, totalWeight + 1);

        float minValue, maxValue;
        if (roll <= lowWeight)
        {
            grade = BlessingFountainBlessInstance.Grade.Low;
            minValue = entry.minValue;
            maxValue = lowMax;
        }
        else if (roll <= lowWeight + mediumWeight)
        {
            grade = BlessingFountainBlessInstance.Grade.Medium;
            minValue = lowMax;
            maxValue = mediumMax;
        }
        else
        {
            grade = BlessingFountainBlessInstance.Grade.High;
            minValue = mediumMax;
            maxValue = entry.maxValue;
        }

        float newStat = Random.Range(minValue, maxValue);

        if (entry.modType == StatModType.PercentAdd)
        {
            if (newStat < 0.1f) grade = BlessingFountainBlessInstance.Grade.Low;
            else if (newStat < 0.2f) grade = BlessingFountainBlessInstance.Grade.Medium;
            else grade = BlessingFountainBlessInstance.Grade.High;
        }
        else
        {
            float ratio = newStat / entry.maxValue;
            if (ratio < 0.3f) grade = BlessingFountainBlessInstance.Grade.Low;
            else if (ratio < 0.6f) grade = BlessingFountainBlessInstance.Grade.Medium;
            else grade = BlessingFountainBlessInstance.Grade.High;

            newStat = Mathf.RoundToInt(newStat);
        }

        return newStat;
    }
    
    private int GetRandomBlessCount()
    {
        Dictionary<int, int> weights = new Dictionary<int, int>();

        weights[1] = 40; // 50% 확률
        weights[2] = 35; // 35% 확률
        weights[3] = 25; // 15% 확률

        // 총합
        int totalWeight = weights.Values.Sum();

        int roll = Random.Range(1, totalWeight + 1);
        int cumulative = 0;

        foreach (var pair in weights)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        // fallback
        return blessData.MinBlessCount;
    }
    
}
