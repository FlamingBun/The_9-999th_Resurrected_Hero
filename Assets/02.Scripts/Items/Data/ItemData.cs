using System;

/// <summary>
/// 아이템의 기본 정보를 담는 데이터 클래스입니다.
/// 직렬화되어 저장 및 로드, 인스펙터 활용 등에 사용됩니다.
/// </summary>
[Serializable]
public class ItemData
{
    public int key;
    public string name;
    public ItemType type;
    public string description;
    public int stat1;
    public int stat2;
    public int stat3;
}
