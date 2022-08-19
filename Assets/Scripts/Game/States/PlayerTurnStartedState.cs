using UnityEngine;

namespace Assets.Scripts.Game.States
{
    public class PlayerTurnStartedState : GameStateBase
    {

            public int? SelectedDominoId;
            public bool IsTrackDominoSelected;

            public override void EnterState(GameStateContext gameStateContext)
            {
                // add player dominoes
                Debug.Log("STATE: PlayerTurnStartedState");

                SelectedDominoId = null;
                IsTrackDominoSelected = false;

                //gameStateContext.DominoSelected.OnEventRaised.AddListener(SelectDomino);
            }

            public override void UpdateState(GameStateContext gameStateContext)
            {
                if (!SelectedDominoId.HasValue)
                {
                    return;
                }

                //var playerSelectedDominoID = gameStateContext.GameplayManager.GetSelectedDominoID();
                //if (!playerSelectedDominoID.HasValue)
                //{
                //    gameStateContext.GameplayManager.SelectDomino(SelectedDominoId.Value);
                //    gameStateContext.SwitchState(gameStateContext.PlayerSelectedPlayerDominoState);
                //    return;
                //}

            }

            public override void LeaveState(GameStateContext gameStateContext)
            {
                gameStateContext.DominoSelected.OnEventRaised.RemoveListener(SelectDomino);
            }

            public void SelectDomino(int dominoId)
            {
                SelectedDominoId = dominoId;
            }
    
    }
}