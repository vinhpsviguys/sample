public enum EventID
{
    None = 0,
    LoadItemInBag,
    AddItemToBag,
    RemoveItemToBag,
    EquipItemToPlayer,

    ItemHeroSelected,
    
    OnPropertiesChange,

    OnReadInitDataSuccessfully,

    OnLoginSocketIO,
    OnDisconnectSocketIO,

    InitListRoom,
    LeaveRoom,
    CreateRoom,
    JoinRoom,
    ReconnectBattleScene,
    CreateTimeoutConfirmLoadData,

    OnSendMessage,
    CreateBattle,
    StartBattle,

    OnInitMECharacter,
    OnInitEnemyCharacter,

    OnBattleBeginHostSend,
    OnCountDownBattleBegin,

    OnTurnBeginHostSendIDAttack,
    OnTurnBeginSubCheckIDAttack,

    OnRollingDiceBegin,
    OnRollingDice,
    OnEmitDataRollingDice,
    OnEnemyRollingDice,
    OnGetEnemyRollingDiceData,
    OnRollingDiceEnd,

    OnCharacterOrderAction,
    OnEmitActionData,
    OnGetEnemyActionData,
    OnEnemyReceivedDataBattleSucessfully,
    OnCharacterAttackNormal,
    OnCharacterUseSkill,
    OnCharacterUpdateUIState,
    OnCharacterDie,

    OnSendDataEndTurn,
    OnConfirmEndTurn,
    OnSendReleasedEffect,
    OnGetRealeaseEffect,
    OnBattleEnd,
    RefreshRoom

}