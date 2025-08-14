# The_9-999th_Resurrected_Hero

<img width="1584" height="1125" alt="projectcover" src="https://github.com/user-attachments/assets/f7893e09-d5fe-43b7-b7c1-d2a3558063f1" />

2D 로그라이크 Unity 프로젝트 (2022.3.17f1)  
**Infinite-power**는 절차적 맵 생성, 플레이어 FSM, 적 AI, NPC, UI, SO 데이터 구조 등  
핵심 시스템을 모두 포함하는 팀 기반 로그라이크 RPG입니다.

---

## 💡 프로젝트 소개

- **장르**: 2D 탑다운 로그라이크 RPG
- **주요 목표**  
  - 매번 달라지는 던전, 다양한 적/보스, 수집·성장 요소  
  - 빠른 반복 테스트 및 프로토타이핑  
  - 핵심 시스템 SO화(생산성/유지보수↑)  
  - 팀원별 책임 파트, 특히 UI 구조 최적화

- **시연 영상**
<br>

[![Video Label](http://img.youtube.com/vi/yomrICic4Ng/0.jpg)](https://youtu.be/yomrICic4Ng)

- **기술 스택**  
  - Unity 2022.3.17f1  
  - Git, iterm2, zshrc, Git Bash  
  - Jira, Notion, 마일스톤 관리


- **기능**
  - NPC 대화 / 상호작용 시스템
    ![녹화_2025_08_12_12_14_13_822](https://github.com/user-attachments/assets/aea5bc40-eae7-4a27-89d2-7cf2b9aba26d)
  - 맵 절차적 생성 /  몬스터 랜덤 생성

    ![녹화_2025_08_12_12_18_48_137-min+(2) (2)](https://github.com/user-attachments/assets/4d592e8a-0773-4b35-91c8-b71d77cb1e41)
  - 부숴지는 오브젝트
    ![녹화_2025_08_12_12_20_58_75-min (2)](https://github.com/user-attachments/assets/22811b5e-ea3e-4958-9544-6460b90e9e50)
  - 상점 기능
    ![녹화_2025_08_12_12_19_46_74-min](https://github.com/user-attachments/assets/2b5cda90-1584-4289-958a-a5c3384ae6c6)
  - 층 이동/저주 기능
    ![녹화_2025_08_12_12_23_17_682](https://github.com/user-attachments/assets/2ac6966b-d8e1-451e-a40d-47e1a7af10f6)
  - 미니맵/이동 기능
    ![녹화_2025_08_12_12_22_37_39](https://github.com/user-attachments/assets/17610fde-555c-4076-bb34-2378c28ccf63)
  - 축복/리롤 기능
    ![녹화_2025_08_12_12_16_59_397-min (2)](https://github.com/user-attachments/assets/481572c5-7c66-46d6-9085-5d55cd0b7f33)
  - 인트로씬
    ![녹화_2025_08_12_12_25_34_903-min](https://github.com/user-attachments/assets/fcca6d86-04ec-4518-a09e-acd6f81f0058)
  - 사망씬
    ![녹화_2025_08_12_12_49_18_41-min](https://github.com/user-attachments/assets/4e3b1629-7fe3-4107-b768-79af5a2adc14)
  - 보스 전투
  ![boss](https://github.com/user-attachments/assets/cc49ed30-e81f-4a27-a7bc-40dd2ec8f22c)

---

## 🗂️ 폴더 구조

```
📦Assets            
┗ 📂02.Scripts
  ┣ 📂Audio
  ┣ 📂Camera
  ┣ 📂Character
  ┃  ┣ 📂Enemy
  ┃  ┣ 📂NPC
  ┃  ┗ 📂Player
  ┣ 📂Common
  ┃  ┣ 📂Data
  ┃  ┣ 📂Interface
  ┃  ┗ 📂UI
  ┣ 📂Dialogue
  ┣ 📂Items
  ┃  ┣ 📂Data
  ┃  ┗ 📂Weapon
  ┣ 📂Light
  ┣ 📂Quest
  ┣ 📂Stat
  ┣ 📂Status
  ┃  ┗ 📂Effects
  ┣ 📂Tower
  ┃  ┣ 📂Floor
  ┃  ┣ 📂Object
  ┃  ┣ 📂Room
  ┃  ┗ 📂UI
  ┣ 📂Town
  ┃  ┗ 📂UI
  ┣ 📂Utils
  ┗ 📂VFX

```
---

## ✍️ 개발 규칙 및 컨벤션

### 1. 코드/네이밍 규칙

| 항목            | 컨벤션                      | 예시                        |
| --------------- | -------------------------- | --------------------------- |
| private 필드    | _ + camelCase              | _moveSpeed                  |
| protected 필드  | camelCase / PascalCase     | jumpForce                   |
| public 필드     | PascalCase(프로퍼티)       | JumpForce                   |
| 지역 변수       | camelCase                  | jumpForce                   |
| 상수            | PascalCase                 | MaxHealth                   |
| enum            | PascalCase                 | WeaponType                  |
| 인스펙터 노출   | [SerializeField] private   | [SerializeField] private float jumpPower; |
| 클래스          | PascalCase                 | PlayerController            |
| 메서드          | PascalCase                 | GetHp                       |
| bool            | is/can/has                 | isDead, canJump, hasKey     |
| interface       | I + PascalCase             | ICharacterState             |
| SO              | "Scriptable Objects/…"     | Scriptable Objects/Datas/MonsterData |

- **public 멤버**는 무조건 상단  
  (public static > public abstract > public 프로퍼티 > public ... > protected ... > private ...)
- private → 필요시 get 프로퍼티 추가
- 약어·축약 지양 (Unity 스타일)
- 이름 중복 지양 (itemData > itemPrice, itemIcon처럼 구체적으로)

---

### 2. 작업/관리 규칙

- **씬 저장 시** 변경사항 꼼꼼히 확인 (특히 scene 파일)
- **에셋 패키지**는 필요한 것만 정리해 사용
- **임시 폴더/씬** 분리(본 작업물 영향 최소화)
- **작업중 프리팹** 건드리지 않기(소유자 체크)
- **디버그/테스트 코드**는 꼭 제거
- **SO, 프리팹 등** 직접 참조 구조로 재사용성 높임

---

## 🏗️ 시스템 구조 및 구현 방식

### 1. 데이터(SO) 기반 구조
- **SO(ScriptableObject)**로 몬스터, 아이템, 스테이지 등 모든 데이터 관리  
- 데이터 변경/확장, 파라미터 테스트 시 빠른 대응  
- ScriptableObject/… 폴더 체계화

### 2. UI 시스템
- BaseUI 추상클래스 기반, UIManager에서 상태·활성 관리
- Canvas 최소화 & 계층 분리 (HUD, Popup, Overlay 등)
- UI는 "뼈-근육-피부" 순서처럼 작은 단위부터 만들고 묶음
- 예시: `OpenUI<T>()` 메서드로 재사용/활성화 & Stack 관리

### 3. 플레이어 FSM
- 상태 전이/동작 분리, 확장 쉬운 구조
- Move, Attack, Dodge 등 상태별로 개별 State Script 분리

### 4. 맵 및 미니맵
- 절차적 맵 생성 + Tilemap 활용
- 미니맵: 플레이어/적/방 상태 실시간 동기화
- 미니맵 전용 레이어/카메라, 전체맵 UI, 자동 스케일링/범위 조절

### 5. 적/AI/몬스터
- SO 기반 데이터, AI 패턴 분리 설계
- FSM 또는 간단 AI State Machine 구조

### 6. NPC/상호작용
- SO 데이터 기반, 이벤트/대사 분리
- NPC 대화, 상점, 퀘스트 등 확장성 고려

---

## 👥 팀 역할 분배

- **기획**: 시스템 설계, 스테이지/몹/아이템 데이터 정의
- **프로그래밍**: Player, Monster, NPC, Map, UI, SO, FSM 등 구현
- **UI/UX**: UI 설계, BaseUI·UIManager 구조화, HUD/Popup/미니맵 등 전체 흐름
- **아트**: 도트 그래픽, 타일셋, UI Sprite 등
- **레벨디자인**: 던전/방 구조, 맵 밸런싱, 플레이테스트

---

## 🔑 기타 체크리스트/팁

- **버전관리**: Git Hub 적극 활용, 브랜치/커밋 관리 철저
- **Git hub Proejects**: 일정, 역할, 작업 분배/트래킹
- **트러블슈팅**: 임포트/세팅 충돌 시 백업 브랜치로 복구
- **작업 진행**: 프로토타입 → 테스트 → 확장 → 리팩토링 순서
- **에셋/임포트**: 불필요한 씬/파일 혼입 주의

---

## 📢 컨트리뷰션 가이드

- Pull Request 시 본인 파트 및 관련 SO/Prefab/Script만 수정/추가
- 리뷰어 지정, 변경내역 주석 필수
- 컨벤션 위반/불명확 구조 발견 시 팀에 즉시 공유

---

> **문의/참여**
>  
> 프로젝트와 관련된 문의·참여 제안은 팀 노션을 이용해 주세요!
