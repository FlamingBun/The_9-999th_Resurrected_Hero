using System;
using UnityEngine;

/// <summary>
/// 상점에서 판매되는 아이템 데이터를 정의하는 클래스입니다.
/// - 기본 아이템 정보 (이름, 설명, 가격 등)
/// - 상점에서의 판매 관련 정보 포함 (재고, 할인율, 장착 가능 여부 등)
/// </summary>
[Serializable]
public class TownShopItemData
{
    [Header("기본 아이템 참조")]
    public int key;

    [Header("상점 판매 정보")]
    public int price;
    public int maxStock;             // 판매 수량 (0이면 품절)
    public float discountRate;    // 할인율 (예: 0.2f = 20% 할인)
}
