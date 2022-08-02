using GraveyardIdle;
using System;

namespace ZestGames
{
    public static class EventManager { }

    public static class GameEvents
    {
        public static Action OnGameStart, OnLevelSuccess, OnLevelFail, OnChangePhase;
        public static Action<Enums.GameEnd> OnGameEnd, OnChangeScene;
    }

    public static class PlayerEvents
    {
        public static Action OnMove, OnIdle, OnTakeACoffin, OnStartDigging, OnStopDigging, OnStartFilling, OnStopFilling;
        public static Action<Coffin, InteractableGround> OnDropCoffin;
        public static Action<Coffin> OnSetCarryingCoffin;
        public static Action OnEnteredDigZone, OnExitedDigZone, OnEnteredFillZone, OnExitedFillZone;
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateLevelText, OnUpdateCollectableText;
        public static Action<string, FeedBackUi.Colors> OnGiveFeedBack;
        public static Action<UnityEngine.Vector3, string> OnPopupText;
    }

    public static class CollectableEvents
    {
        public static Action<int> OnCollect;
    }

    public static class InputEvents
    {
        public static Action OnTapHappened, OnTouchStarted, OnTouchStopped;
    }

    public static class CoffinAreaEvents
    {
        public static Action OnStackedCoffin, OnUnStackedCoffin;
        public static Action<Truck> OnAssignTruck;
    }

    public static class ShovelEvents
    {
        public static Action OnCanDig, OnCantDig, OnDigHappened, OnFillHappened;
        public static Action OnThrowSoilToGrave, OnThrowSoilToPile;
        public static Action<Enums.SoilThrowTarget> OnPlaySoilFX;
    }

    public static class GraveEvents
    {
        public static Action<GraveDiggable> OnAGraveIsDug;
    }

    public static class TruckEvents
    {
        public static Action<CoffinArea> OnEnableTruck;
        public static Action OnDisableTruck;
    }

    public static class GraveManagerEvents
    {
        public static Action OnGraveActivated, OnGraveDigged, OnCoffinThrownToGrave, OnCoffinPickedUp;
    }
}
