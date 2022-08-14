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
        public static Action OnEnteredDigZone, OnExitedDigZone, OnEnteredFillZone, OnExitedFillZone, OnPutDownShovel, OnPullOutShovel, OnStartedMaintenance, OnStoppedMaintenance, OnMaintenanceSuccessfull;
        public static Action OnSetCurrentDigSpeed, OnSetCurrentMaintenanceSpeed;
        public static Action OnStopSpendingMoney;
    }

    public static class PlayerUpgradeEvents
    {
        public static Action OnUpdateUpgradeTexts, OnUpgradeDigSpeed, OnUpgradeMaintenanceSpeed;
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateLevelText, OnUpdateCollectableText;
        public static Action<string, FeedBackUi.Colors> OnGiveFeedBack;
        public static Action<UnityEngine.Vector3, string> OnPopupText;
    }

    public static class CollectableEvents
    {
        public static Action<int> OnCollect, OnConsume;
    }

    public static class InputEvents
    {
        public static Action OnTapHappened, OnTouchStarted, OnTouchStopped;
    }

    public static class CoffinAreaEvents
    {
        public static Action OnStackedCoffin, OnUnStackedCoffin, OnThrowACoffinToCarriers;
        public static Action<Truck> OnAssignTruck;
    }

    public static class ShovelEvents
    {
        public static Action OnCanDig, OnCantDig, OnDigHappened, OnFillHappened, OnEnableMesh, OnDisableMesh;
        public static Action OnThrowSoilToGrave, OnThrowSoilToPile;
        public static Action<Enums.SoilThrowTarget> OnPlaySoilFX;
    }
    
    public static class WateringCanEvents
    {
        public static Action OnStartedWatering, OnStoppedWatering;
        public static Action<Grave> OnMaintenanceSuccessfull;
    }

    public static class GraveEvents
    {
        public static Action<GraveDiggable> OnAGraveIsDug;
    }

    public static class TruckEvents
    {
        public static Action<CoffinArea> OnEnableTruck;
        public static Action<CoffinMachine> OnEnableSellTruck;
        public static Action OnDisableTruck, OnDisableSellTruck;
    }

    public static class GraveManagerEvents
    {
        public static Action OnGraveActivated, OnGraveDigged, OnCoffinThrownToGrave, OnCoffinPickedUp, OnEmptyCoffinSold, OnCoffinSpawned, OnCheckForCarrierActivation;
    }

    public static class AudioEvents
    {
        public static Action OnPlayCollectMoney, OnPlaySpendMoney;
    }

    public static class CoffinMachineEvents
    {
        public static Action OnStartMakingACoffin, OnMadeACoffin, OnACoffinTaken, OnStackedEmptyCoffin, OnUnStackedEmptyCoffin;
        public static Action<SellTruck> OnAssignSellTruck;
    }
    
    public static class CoffinMakerEvents
    {
        public static Action OnStartWaiting, OnGoToMachine, OnGoToCounter, OnPushTheButton;
    }

    public static class CoffinCarrierEvents
    {
        public static Action<Coffin> OnSendCarriersToHandles;
        public static Action OnLeaveCoffin, OnReturnToWaitingPosition, OnReadyForDuty;
    }
}
