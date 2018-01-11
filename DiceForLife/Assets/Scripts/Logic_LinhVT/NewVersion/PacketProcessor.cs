
﻿using System;
using System.Collections;

namespace CoreLib
{
    //class PacketProcessor
    //{

    //    public Dispatcher dispatcher;
    //    public Receiver receiver;
    //    public Adapter adapter;


    //    public PacketProcessor(Adapter adapter)
    //    {
    //        this.adapter = adapter;
    //        this.receiver = new Receiver(this);
    //    }

    //    public void receivePacket(string type, MyDictionary<string, object> data) {
    //        receiver.receivePacket(type, data);
    //    }
    //}

    //class Dispatcher {
    //    private const string BEGIN_TURN_CONFIRM = "begin_turn_confirm";
    //    private const string IN_TURN_CONFIRM = "in_turn_confirm";
    //    private const string END_TURN_CONFIRM = "end_turn_confirm";
    //    //public const string CONNECTING_REQUEST = "connecting_request";
    //    private const string IN_TURN_SKILLS = "in_turn_skills";
    //    private const string RESTORING_REQUEST = "restoring_request";
    //    private const string BEGIN_TURN_REQUEST = "begin_turn_request";
    //    private const string IN_TURN_REQUEST = "in_turn_request";
    //    private const string END_TURN_REQUEST = "end_turn_request";

    //    private const string ASK_STATUS_REQUEST = "ask_status_request";
    //    private const string ANSWER_STATUS_REQUEST = "answer_status_request";
    //    private const string STATUS_CONFIRM = "status_confirm"; // dung de khoi tao ket noi dau game hoac de reconnect

    //    private const string ROLL_DICE_REQUEST = "roll_dice_request";
    //    private const string ROLL_DICE_CONFIRM_MATSER = "roll_dice_confirm_master";
    //    private const string ROLL_DICE_CONFIRM_SLAVE = "roll_dice_confirm_slave";

    //    PacketProcessor parent;
    //    Adapter adapter;
    //    ArrayList sendingQueue;
    //    int messageID = 0;

    //    public Dispatcher(PacketProcessor parent) {
    //        this.parent = parent;
    //        this.adapter = parent.adapter;
    //    }

    //    // request from Slave to Master

    //    public void SendAskingStatus()
    //    {
    //        messageID++;
    //        string type = ASK_STATUS_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send Ask Status", "create msg " + messageID + " " + type);
    //    }

    //    public void ConfirmRollDiceOfSlave()
    //    {
    //        messageID++;
    //        string type = ROLL_DICE_CONFIRM_SLAVE;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("confirm RollDice Of Slave", "create msg " + messageID + " " + type);
    //    }

    //    public void ConfirmGameDataInTurn()
    //    {
    //        messageID++;
    //        string type = IN_TURN_CONFIRM;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("confirm", "create msg " + messageID + " " + type);
    //    }

    //    public void ConfirmGameDataBeginTurn()
    //    {
    //        messageID++;
    //        string type = BEGIN_TURN_CONFIRM;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("confirm", "create msg " + messageID + " " + type);
    //    }

    //    public void ConfirmGameDataEndTurn()
    //    {
    //        messageID++;
    //        string type = END_TURN_CONFIRM;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);

    //        sendingQueue.Add(new Pac(type, data));

    //        adapter.Log("confirm", "create msg " + messageID + " " + type);
    //    }



    //    public void SendSkillListInTurn(ArrayList skills)
    //    {
    //        messageID++;
    //        string type = IN_TURN_SKILLS;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);

    //        byte[] bytes = null;
    //        foreach (int skill in skills)
    //        {
    //            bytes = bytes == null ? Utilities.convertToByteArr((ushort)skill) : Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((ushort)skill));
    //        }
    //        if (bytes != null)
    //        {
    //            adapter.Log("sendSkills byte", "" + bytes.Length);
    //            data.Add("skills", Utilities.convertByteArrToString(bytes));
    //        }

    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("sendSkill", "create msg " + messageID + " " + type);
    //    }

    //    // request from Master to Slave

    //    public void ConfirmRollDiceOfMaster()
    //    {
    //        messageID++;
    //        string type = ROLL_DICE_CONFIRM_MATSER;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("confirm RollDice Of Master", "create msg " + messageID + " " + type);
    //    }


    //    public void SendGameDataInTurn()
    //    {
    //        messageID++;
    //        string type = IN_TURN_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        byte[] bytes = null;
    //        for (int i = 0; i < inResult.states.Count; i++)
    //        {
    //            bytes = ((State)inResult.states[i]).convertToByteArr();
    //            adapter.Log("state_Byte_length", "" + bytes.Length);
    //            data.Add("state_" + i, Utilities.convertByteArrToString(bytes));
    //        }

    //        data.Add("turn", attStatus.playerID);


    //        bytes = null;
    //        foreach (int skill in selectedSkills)
    //        {
    //            bytes = bytes == null ? Utilities.convertToByteArr((ushort)skill) : Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((ushort)skill));
    //        }
    //        if (bytes != null)
    //        {
    //            adapter.Log("sendSkills byte", "" + bytes.Length);
    //            data.Add("skills", Utilities.convertByteArrToString(bytes));
    //        }


    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send In Turn", "create msg " + messageID + " " + type + " " + inResult.states.Count + " " + data.Keys.Count);
    //    }

    //    public void SendGameDataBeginTurn()
    //    {
    //        messageID++;
    //        string type = BEGIN_TURN_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        for (int i = 0; i < beginResult.states.Count; i++)
    //            data.Add("state_" + i, Utilities.convertByteArrToString(((State)beginResult.states[i]).convertToByteArr()));


    //        data.Add("turn", attStatus.playerID);
    //        data.Add("continued", beginResult.continued);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send Begin Turn", "create msg " + messageID + " " + type);
    //    }

    //    private void SendGameDataEndTurn()
    //    {
    //        messageID++;
    //        string type = END_TURN_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);

    //        for (int i = 0; i < endResult.releasedState.Count; i++)
    //            data.Add("effect_" + i, Utilities.convertByteArrToString(((NewEffect)endResult.releasedState[i]).convertToByteArr()));

    //        //data.Add("states", endResult.releasedState);
    //        data.Add("turn", attStatus.playerID);
    //        data.Add("result", endResult.combatResult);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send End Turn", "create msg " + messageID + " " + type);
    //    }

    //    private void SendRestoringData()
    //    {
    //        messageID++;
    //        string type = RESTORING_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        // turnID
    //        // masterID
    //        data.Add("firstID", (byte)firstID);
    //        data.Add("turnID", (byte)attStatus.playerID);
    //        data.Add("masterID", (byte)me.playerId);
    //        ArrayList list = attStatus.getEffectsOfPlayerID(attStatus.playerID);
    //        list.AddRange(defStatus.getEffectsOfPlayerID(defStatus.playerID));
    //        list.AddRange(attStatus.getEffectsOfPlayerID(3));
    //        for (int i = 0; i < list.Count; i++)
    //        {
    //            NewEffect effect = (NewEffect)list[i];
    //            data.Add("effect_" + i, Utilities.convertByteArrToString(effect.convertToByteArr()));
    //        }
    //        data.Add("masterStatus", status.ToString());

    //        if (status == Status.ROLL_DICE_WAITING)
    //        {
    //            data.Add("dice1", (byte)diceResult.dice1);
    //            data.Add("dice2", (byte)diceResult.dice2);
    //            data.Add("dice3", (byte)diceResult.dice3);
    //        }

    //        byte[] bytes = null;

    //        foreach (string key in me.newSkillDic.Keys)
    //        {
    //            NewSkill skill = me.newSkillDic[key];
    //            byte[] part = Utilities.Concat<byte>(Utilities.convertToByteArr((ushort)skill.getID()), Utilities.convertToByteArr((byte)skill.getCoolDownByTurn()));
    //            bytes = bytes == null ? part : Utilities.Concat<byte>(bytes, part);
    //        }

    //        data.Add("masterCooldown", Utilities.convertByteArrToString(bytes));
    //        bytes = null;

    //        foreach (string key in you.newSkillDic.Keys)
    //        {
    //            NewSkill skill = you.newSkillDic[key];
    //            byte[] part = Utilities.Concat<byte>(Utilities.convertToByteArr((ushort)skill.getID()), Utilities.convertToByteArr((byte)skill.getCoolDownByTurn()));
    //            bytes = bytes == null ? part : Utilities.Concat<byte>(bytes, part);
    //        }
    //        data.Add("slaveCooldown", Utilities.convertByteArrToString(bytes));

    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send Restore Data", "create msg " + messageID + " " + type);
    //    }



    //    public void SendAnswerStatus()
    //    {
    //        messageID++;
    //        string type = ANSWER_STATUS_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        data.Add("masterStatus", status.ToString());
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send Answer Status", "create msg " + messageID + " " + type);
    //    }


    //    public void SendStatusConfirm()
    //    {
    //        messageID++;
    //        string type = STATUS_CONFIRM;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send Status Confirm", "create msg " + messageID + " " + type);
    //    }

    //    public void SendRollDice()
    //    {
    //        messageID++;
    //        string type = ROLL_DICE_REQUEST;
    //        MyDictionary<string, object> data = new MyDictionary<string, object>();
    //        data.Add("messageID", messageID);
    //        data.Add("dice1", (byte)diceResult.dice1);
    //        data.Add("dice2", (byte)diceResult.dice2);
    //        data.Add("dice3", (byte)diceResult.dice3);
    //        sendingQueue.Add(new Pac(type, data));
    //        adapter.Log("send RollDice", "create msg " + messageID + " " + type);
    //    }

    //}

    //class Receiver
    //{

    //    private const string BEGIN_TURN_CONFIRM = "begin_turn_confirm";
    //    private const string IN_TURN_CONFIRM = "in_turn_confirm";
    //    private const string END_TURN_CONFIRM = "end_turn_confirm";
    //    //public const string CONNECTING_REQUEST = "connecting_request";
    //    private const string IN_TURN_SKILLS = "in_turn_skills";
    //    private const string RESTORING_REQUEST = "restoring_request";
    //    private const string BEGIN_TURN_REQUEST = "begin_turn_request";
    //    private const string IN_TURN_REQUEST = "in_turn_request";
    //    private const string END_TURN_REQUEST = "end_turn_request";

    //    private const string ASK_STATUS_REQUEST = "ask_status_request";
    //    private const string ANSWER_STATUS_REQUEST = "answer_status_request";
    //    private const string STATUS_CONFIRM = "status_confirm"; // dung de khoi tao ket noi dau game hoac de reconnect

    //    private const string ROLL_DICE_REQUEST = "roll_dice_request";
    //    private const string ROLL_DICE_CONFIRM_MATSER = "roll_dice_confirm_master";
    //    private const string ROLL_DICE_CONFIRM_SLAVE = "roll_dice_confirm_slave";

    //    PacketProcessor parent;
    //    Adapter adapter;

    //    public Receiver(PacketProcessor parent)
    //    {
    //        this.parent = parent;
    //        this.adapter = parent.adapter;
    //    }

    //    private void processPacketInMaster(string type, MyDictionary<string, object> data) {
    //        switch (status)
    //        {
    //            case Status.INIT_GAME:
    //                if (type == ASK_STATUS_REQUEST)
    //                {
    //                    // hoi thu xy ly luon
    //                    SendAnswerStatus();// co the la ben slave la reconnect hoac connect luc dau game
    //                    status = Status.INIT_GAME_WAITING;
    //                    resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME_WAITING, Status.FAILED_TO_CONNECT, () => { }, timeoutInMS, 1, false); // cho ket qua tiep
    //                    adapter.Log("Master", "receive ask and answer status when game is initital");
    //                }
    //                break;
    //            case Status.INIT_GAME_WAITING:
    //                if (type == STATUS_CONFIRM)
    //                {
    //                    // bat dau game thoi
    //                    adapter.Log("Master", "receive confirm status and begin game");
    //                    resultTimeouter = null;// huy cho confirm tu slave
    //                    hideWaitingDialog();
    //                    status = Status.BEGIN_TURN;
    //                }
    //                break;
    //            case Status.FAILED_TO_CONNECT:
    //                break;
    //            default:
    //                if (type == ASK_STATUS_REQUEST) // co the bi hoi trong luc dang chay game
    //                {
    //                    // hoi thu xy ly luon
    //                    SendAnswerStatus();// co the la ben slave la reconnect hoac connect luc dau game
    //                    adapter.Log("Master", "receive ask and answer status when game is running");
    //                }
    //                else
    //                    if (isMyTurn(attStatus))
    //                {
    //                    processPacketOfMeInMaster(type, data);
    //                }
    //                else
    //                {
    //                    processPacketOfYouInMaster(type, data);

    //                }

    //                break;
    //        }
    //    }

    //    private void processPacketInSlave(string type, MyDictionary<string, object> data) {
    //        switch (status)
    //        {
    //            case Status.INIT_GAME:
    //                if (type == ANSWER_STATUS_REQUEST)
    //                {
    //                    // hoi thu xy ly luon
    //                    //Sen();// co the la ben slave la reconnect hoac connect luc dau game
    //                    //status = Status.INIT_GAME_WAITING;
    //                    //resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME_WAITING, Status.FAILED_TO_CONNECT, () => { }, timeoutInMS, 1, false); // cho ket qua tiep
    //                    //adapter.Log("Master", "receive ask and answer status when game is initital");
    //                    resultTimeouter = null; // huy cho doi cau tra loi tu master
    //                    status = Status.INIT_GAME_WAITING;
    //                    //sendCon

    //                    string masterStatus = data["masterStatus"] as string;
    //                    Status mStatus = Utilities.ParseEnum<Status>(masterStatus);
    //                    switch (mStatus)
    //                    {
    //                        case Status.INIT_GAME:
    //                        case Status.INIT_GAME_WAITING:
    //                            adapter.Log("Slave", "master answer when init game");
    //                            break;
    //                        default:
    //                            adapter.Log("Slave", "master answer when running game");
    //                            break;
    //                    }
    //                    SendStatusConfirm(); // phan hoi lai tra loi rang slave da dong y

    //                }
    //                break;
    //            //case Status.INIT_GAME_WAITING:
    //            //    break;
    //            case Status.FAILED_TO_CONNECT:
    //                break;
    //            default:
    //                if (isMyTurn(attStatus))
    //                {
    //                    processPacketOfMeInSlave(type, data);

    //                }
    //                else
    //                {
    //                    processPacketOfYouInSlave(type, data);

    //                }

    //                break;
    //        }
    //    }


    //    public void receivePacket(string type, MyDictionary<string, object> data)
    //    {
    //        if (adapter.isMaster)
    //            processPacketInMaster(type, data);
    //        else
    //            processPacketInSlave(type, data);
    //    }
    //}
}

