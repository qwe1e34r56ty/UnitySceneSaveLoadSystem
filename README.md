# Unity 자동 저장 시스템

## 개요

이 프로젝트는 **Unity 기반 게임에서 자동 저장 및 상태 복원 기능**을 제공하는 시스템입니다.  
특히 **씬 전환 없이도 층 구조를 오갈 수 있는 던전 로딩**, **런타임에 동적으로 생성된 오브젝트의 저장**,  
**Prefab 기반 복원**, **Achievement 처리**를 포함한 **문서 기반 자동 저장 프레임워크**를 구현합니다.

## 주요 기능

### ✅ 자동 저장 시스템
- `GameContext`가 모든 게임 데이터를 관리하며, JSON 파일을 기반으로 저장/불러오기 수행
- Player, NPC, Tile 오브젝트는 추상 클래스를 상속받아 `Save()` 메서드를 통해 위치 및 상태 자동 저장

### ✅ Scene 전환 없이도 갱신되는 구조
- `FloorLoader`는 씬 전환 없이도 하나의 씬 안에서 층 이동(상/하강)을 처리
- 각 층(`FloorData`) 별 상태를 `SaveData`에 저장하여 층 전환 후에도 NPC, 타일, 플레이어 위치 유지

### ✅ 런타임 프리팹 저장/복원
- 런타임에 `Instantiate()`된 프리팹이 자동으로 `GameContext`에 등록됨
- `prefabPath` 정보를 기준으로 다시 로딩 가능
- 개발 중 씬에 직접 프리팹 배치 후 실행해도 해당 객체가 저장/복원 대상이 됨
- 오브젝트는 데이터만 참조하는 방식으로 씬 전환 과정에서 오브젝트가 파괴되어도 데이터의 영속성을 보장하며 참조를 이용한 자동 동기화

### ✅ Scene + Floor 데이터 분리
- `SceneData`는 일반적인 씬 진입 시 저장되는 데이터 (맵 구조, 캐릭터 위치 등)
- `FloorData`는 던전의 각 층별 저장용도로 분리 관리
- `DontSaveCurSceneBundle()`을 통해 현재 씬 저장 생략 가능 (던전 또는 일반 scene 내 런타임 작업 용도)

### ✅ Achievement 시스템
- 업적(`AchievementID`) 달성 시 `AchievementSO` ScriptableObject를 기반으로 UnlockUI 출력

---

## 구조 요약

```text
📁 Scripts
├── GameContext.cs          ← 모든 저장/불러오기 로직과 상태 보관
├── SceneLoader.cs          ← 씬 로드 시 자동으로 SceneData 적용
├── FloorLoader.cs          ← 씬 내에서 층을 오르내릴 수 있는 구조
├── ANPC.cs / ATile.cs      ← NPC 및 타일 기본 추상 클래스 (자동 저장/복원 기능 내장)
├── APlayer.cs              ← 플레이어 기본 추상 클래스 (자동 저장/복원 기능 내장)
├── SaveData.cs             ← SaveData, SceneData, FloorData 구조 정의

이후 육각형 그리드 타일을 이용해 타일간의 연결성을 정의하는 시스템을 새 리포지토리에서 개발할 예정
