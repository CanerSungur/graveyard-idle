namespace ZestGames
{
    public class Enums
    {
        public enum GameState { WaitingToStart, Started, PlatrofmEnded, GameEnded }
        public enum GameEnd { None, Success, Fail }
        public enum PoolStamp { Something, Coffin, InteractableGroundCanvas, PopupText, CollectMoney, SpendMoney, ThrowSoilPS }
        public enum AudioType { Testing_PlayerMove, Button_Click, MoneySpawn, GraveBuilt }
        public enum SoilThrowTarget { Grave, Pile }
    }
}
