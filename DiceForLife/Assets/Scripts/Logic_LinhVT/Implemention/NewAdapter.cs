﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreLib;
using BestHTTP.SocketIO;
using UnityEngine.UI;
using System;

delegate IEnumerator ThisWillBeExecutedOnTheMainThread();

class NewAdapter : Adapter
{

    private Socket socket;
    private GameObject content;
    private GameObject prefab;
    public static bool isLog = true;
    


    // public NewAdapter(Socket socket, Character me, Character you, int firstID = 1, bool isMaster = true, bool isOffline = false) : base(me, you, firstID, isMaster, isOffline)
    // {
    //     this.socket = socket;
    //     beginTurnRenderUpdate = () => {

    //         if (this.isMaster) {
    //             Log("BeginRender Master", "Finish");
    //             if (isMyTurn(attStatus))
    //             {
    //                 status = Status.ROLL_DICE;
    //                 if (beginResult.continued)
    //                     eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.END_TURN, () => { }, 30 * 1000, 1, false);
    //                 else status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon

    //             }
    //             else
    //             {
    //                 status = Status.ROLL_DICE;
    //                 if (!isOffline && beginResult.continued)
    //                 {
    //                     eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.END_TURN, () => { }, 30 * 1000, 1, false);
    //                 } else if (!beginResult.continued) {
    //                     status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon
    //                 }

    //             }
    //         } else {
    //             Log("BeginRender Slave", "Finish");
    //             if (isMyTurn(attStatus))
    //             {
    //                 status = Status.ROLL_DICE;
    //                 if (beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
    //                     eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.ROLL_DICE_WAITING, () => { }, 30 * 1000, 1,false);
    //                 else status = Status.END_TURN_WAITING; // khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay

    //             }
    //             else {
    //                 status = Status.ROLL_DICE;
    //                 if (beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
    //                     eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.ROLL_DICE_WAITING, () => { }, 30 * 1000, 1, false);
    //                 else status = Status.END_TURN_WAITING; // khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay
    //             }

    //             UnityMainThreadDispatcher.Instance().Enqueue(() => {
    //                 this.isReadyToNewPac = true;
    //                 processNewPacInSlave();

    //             });
    //         }
    //         diceResult = null;//reset ket qua truoc
    //         inResult = null;// reset ket qua truoc
    //     };

    //     rollDiceRenderUpdate = () =>
    //     {
    //         if (this.isMaster)
    //         {
    //             Log("RollDice Master", "Finish");
    //             if (isMyTurn(attStatus))
    //             {
    //                 status = Status.IN_TURN;
    //                 if (beginResult.continued)
    //                     eventsTimeouter = createTimeouterForSendingData(Status.IN_TURN, Status.END_TURN, () => { }, 30 * 1000, 1, false);
    //                 else status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon

    //             }
    //             else
    //             {
    //                 status = Status.IN_TURN;
    //                 if (!isOffline && beginResult.continued)
    //                 {
    //                     eventsTimeouter = createTimeouterForSendingData(Status.IN_TURN, Status.END_TURN, () => { }, 30 * 1000, 1, false);
    //                 } else if (!beginResult.continued) {
    //                     status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon
    //                 }

    //             }
    //         }
    //         else
    //         {
    //             Log("RollDice Slave", "Finish");
    //             if (isMyTurn(attStatus))
    //             {
    //                 status = Status.IN_TURN;
    //                 if (beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
    //                     eventsTimeouter = createTimeouterForSendingData(Status.IN_TURN, Status.IN_TURN_WAITING, () => { }, 30 * 1000, 1, false);
    //                 else status = Status.END_TURN_WAITING;// khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay

    //             }
    //             else {
    //                 status = Status.IN_TURN;
    //                 if (beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
    //                     eventsTimeouter = createTimeouterForSendingData(Status.IN_TURN, Status.IN_TURN_WAITING, () => { }, 30 * 1000, 1, false);
    //                 else status = Status.END_TURN_WAITING;// khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay
    //             }
    //             UnityMainThreadDispatcher.Instance().Enqueue(() => {
    //                 this.isReadyToNewPac = true;
    //                 processNewPacInSlave();

    //             });
    //         }
    //         inResult = null;
    //     };

    //     inTurnRenderUpdate = () => {

    //if (this.isMaster)
    //{
    //             Log("InturnRender Master","Finish");
    //	if (isMyTurn(attStatus))
    //	{
    //                 status = Status.END_TURN;
    //	}
    //             else status = Status.END_TURN;
    //}
    //else
    //{
    //             Log("InturnRender Slave", "Finish");
    //             if (isMyTurn(attStatus)) status = Status.END_TURN_WAITING;
    //	else status = Status.END_TURN_WAITING;
    //             UnityMainThreadDispatcher.Instance().Enqueue(() => {
    //                 this.isReadyToNewPac = true;
    //                 processNewPacInSlave();

    //             });
    //}
    //         endResult = null;


    //     };
    //     endTurnRenderUpdate = () => {
    //NewCharacterStatus temp = attStatus;
    //attStatus = defStatus;
    //defStatus = temp;

    //         NewCharacterStatus one = logic.getStatusByPlayerID(me.playerId);
    //         NewCharacterStatus two = logic.getStatusByPlayerID(you.playerId);
    //         foreach (string key in one.character.newSkillDic.Keys) {
    //             one.character.newSkillDic[key].decreaseCoolDown();
    //         }
    //         foreach (string key in two.character.newSkillDic.Keys)
    //         {
    //             two.character.newSkillDic[key].decreaseCoolDown();
    //         }
    //         // phai cap nhat coolDown khi doi phuong va minh chon skill

    //         if (this.isMaster) {
    //             Log("EndturnRender Mater", "Finish");
    //	status = Status.BEGIN_TURN;
    //         } else {
    //             Log("EndturnRender Slave", "Finish");
    //             status = Status.BEGIN_TURN_WAITING;

    //             UnityMainThreadDispatcher.Instance().Enqueue(() => {
    //                 this.isReadyToNewPac = true;
    //                 processNewPacInSlave();

    //             });
    //         }


    //if (endResult.combatResult > 0)
    //{
    //	Log("Ket qua", " Player " + endResult.combatResult + " win ");
    //	status = Status.FINISHED;
    //             //RunScriptGameKimCuong._instance.exportResult(endResult.combatResult == me.playerId);
    //}

    //        beginResult = null;
    //    };
    //    prefab = Resources.Load("Text") as GameObject;
    //}


    public NewAdapter(Socket socket, CharacterPlayer me, CharacterPlayer you, int firstID = 1, bool isMaster = true, bool isOffline = false) : base(me, you, firstID, isMaster, isOffline)
    {
        this.socket = socket;
        beginTurnRenderUpdate = () => {

            // get beginTurn result  and display
            BattleSceneController.Instance.BeginTurnRender();

            if (this.isMaster)
            {
                Log("BeginRender Master", "Finish");
                if (isMyTurn(attStatus))
                {
                    status = Status.ROLL_DICE;
                    if (beginResult.continued)
                        eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.END_TURN, () => { }, 30 * 1000, 1, false);
                    else status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon

                }
                else
                {
                    status = Status.ROLL_DICE;
                    if (!isOffline && beginResult.continued)
                    {
                        eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.END_TURN, () => { }, 30 * 1000, 1, false);
                    }
                    else if (!beginResult.continued)
                    {
                        status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon
                    }

                }
            }
            else
            {
                Log("BeginRender Slave", "Finish");
                if (isMyTurn(attStatus))
                {
                    status = Status.ROLL_DICE;
                    if (beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
                        eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.ROLL_DICE_WAITING, () => { }, 30 * 1000, 1, false);
                    else status = Status.END_TURN_WAITING; // khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay

                }
                else
                {
                    status = Status.ROLL_DICE;
                    if (beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
                        eventsTimeouter = createTimeouterForSendingData(Status.ROLL_DICE, Status.ROLL_DICE_WAITING, () => { }, 30 * 1000, 1, false);
                    else status = Status.END_TURN_WAITING; // khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay
                }

                processNewPacInSlave();
            }
            diceResult = null;//reset ket qua truoc
            inResult = null;// reset ket qua truoc
        };

        rollDiceRenderUpdate = () =>
        {
            BattleSceneController.Instance.DisplayDicePoint(BattleSceneController.Instance.adapter.getDiceResult().dice1, BattleSceneController.Instance.adapter.getDiceResult().dice2, BattleSceneController.Instance.adapter.getDiceResult().dice3);


        };

        inTurnRenderUpdate = () => {
            BattleSceneController.Instance.RenderDataBattleState();
            //if (this.isMaster)
            //{
            //    Log("InturnRender Master", "Finish");
            //    if (isMyTurn(attStatus))
            //    {
            //        status = Status.END_TURN;
            //    }
            //    else status = Status.END_TURN;
            //}
            //else
            //{
            //    Log("InturnRender Slave", "Finish");
            //    if (isMyTurn(attStatus)) status = Status.END_TURN_WAITING;
            //    else status = Status.END_TURN_WAITING;
            //}
            //endResult = null;


        };
        endTurnRenderUpdate = () => {
            NewCharacterStatus temp = attStatus;
            attStatus = defStatus;
            defStatus = temp;

            NewCharacterStatus one = logic.getStatusByPlayerID(me.playerId);
            NewCharacterStatus two = logic.getStatusByPlayerID(you.playerId);
            foreach (string key in one.character.newSkillDic.Keys)
            {
                one.character.newSkillDic[key].decreaseCoolDown();
            }
            foreach (string key in two.character.newSkillDic.Keys)
            {
                two.character.newSkillDic[key].decreaseCoolDown();
            }
            // phai cap nhat coolDown khi doi phuong va minh chon skill
            BattleSceneController.Instance.UpdateSkillCooldownOnCharacter();
            if (endResult != null)
                BattleSceneController.Instance.ReleaseEffectExpire(endResult.releasedState);
            if (this.isMaster)
            {
                Log("EndturnRender Mater", "Finish");
                status = Status.BEGIN_TURN;
            }
            else
            {
                Log("EndturnRender Slave", "Finish");
                status = Status.BEGIN_TURN_WAITING;
                processNewPacInSlave();
            }


            if (endResult.combatResult > 0)
            {
                Log("Ket qua", " Player " + endResult.combatResult + " win ");
                status = Status.FINISHED;
                // not finish when someone leave battle
                if (sendingQueue.Count == 0)
                    BattleSceneController.Instance.ShowGameOverPanel(endResult.combatResult);
                else
                    UnityMainThreadDispatcher.Instance().Enqueue(() => {
                        BattleSceneController.Instance.ShowGameOverPanel(endResult.combatResult);
                    });
                //RunScriptGameKimCuong._instance.exportResult(endResult.combatResult == me.playerId);
            }

            beginResult = null;
        };
        prefab = Resources.Load("Text") as GameObject;
    }


    public override bool isDisconnected()
    {
        return !socket.IsOpen;
    }

    public void setParentLog(GameObject contentParent)
    {

    }

    public void setLog(GameObject content)
    {
        this.content = content;
    }

    public void Update()
    {
        //Log("Begin","++++++++++++++");;
        base.Update();
        if (sendingQueue.Count > 0)
        {
            for (int i = 0; i < sendingQueue.Count; i++)
            {
                Pac pack = (Pac)sendingQueue[i];
                pack.data.Add("from", CharacterInfo._instance.idUserSocketIO);

                socket.Emit("cmd", pack.type, pack.data);
                Log("send pack ", pack.type + " " + pack.data["messageID"] + " " + pack.data.Keys.Count);

                if (pack.type == IN_TURN_REQUEST)
                {
                    foreach (string key in pack.data.Keys)
                    {
                        Log("before sending pack in Turn Request in Master ", key);
                    }
                }

                sendingQueue.RemoveAt(i);

                i--;
            }
        }
        //Log("End", "++++++++++++++"); ;
    }

    public void receivePacket(string type, MyDictionary<string, object> data)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            Debug.Log("new adapter receive package " + type);

            base.receivePacket(type, data);


            //base.receivePacket(type, data);
            //Debug.Log("What the fuck");
        });
    }

    public void OnSetKeyRoom(bool isMe)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            base.OnSetKeyRoom(isMe);
        });
    }

    public override void OnLeavePlayer(bool isMe)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            base.OnLeavePlayer(isMe);



        });
    }

    public override void Log(string tag, string status)
    {
        if (!isLog) return;
        Debug.Log(tag + " : " + status);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (content != null)
            {
                //GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                //obj.transform.SetParent(content.transform, false);
                //obj.GetComponent<Text>().text = tag + " : " + status;
            }

        });

    }

    public void processNewPacInSlave()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            this.isReadyToNewPac = true;
            base.processNewPacInSlave();

        });
    }


    public override void LeaveGameRoom()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {

            Log("leaveRoom", "isMaster " + isMaster + " isOnline " + isOnLine());
            SocketIOController.Instance.LeaveRoom();
            WaitingRoomUI.Instance.FailedToConnect();
            //BattleSceneUI.Instance.BackToMainMenu();
            //RunScriptGameKimCuong._instance.OnLeaveGameButtonClick();
        });
    }

    public override void showWaitingDialog()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            base.showWaitingDialog();
            BattleSceneUI.Instance._panelLoading.SetActive(true);
            //RunScriptGameKimCuong._instance.showWaitingDialog();
        });
    }

    public override void hideWaitingDialog()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            base.hideWaitingDialog();
            BattleSceneUI.Instance._panelLoading.SetActive(false);
            //RunScriptGameKimCuong._instance.hideWaitingDialog();
        });

    }
}
