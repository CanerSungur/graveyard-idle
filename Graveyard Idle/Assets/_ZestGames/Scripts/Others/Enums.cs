namespace ZestGames
{
    public class Enums
    {
        public enum GameState { WaitingToStart, Started, PlatrofmEnded, GameEnded }
        public enum GameEnd { None, Success, Fail }
        public enum PoolStamp { Something, Coffin, InteractableGroundCanvas, PopupText, CollectMoney, SpendMoney, ThrowSoilPS, EmptyCoffin }
        public enum AudioType { Testing_PlayerMove, Button_Click, MoneySpawn, GraveBuilt, Action }
        public enum SoilThrowTarget { Grave, Pile }
        public enum GraveState { NotActivated, Activated, Dug, WaitingToBeFilled, Completed }
    }
}
