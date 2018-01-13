using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
//using UnityEngine;

//using BestHTTP;
//using BestHTTP.SocketIO;

namespace CoreLib // loi sendSkillINTurn truyen 6 byte ma nhan duoc la 8 byte
{

    class Pac
    {
        public string type = "";
        public MyDictionary<string, object> data = new MyDictionary<string, object>();
        public Pac(string type, MyDictionary<string, object> data)
        {
            this.type = type;
            this.data = data;
        }
    }

    abstract class Connector
    {
        public abstract void OnLeavePlayer(bool isMe);
        public abstract void OnSetKeyRoom(bool isMe);
        public abstract bool isDisconnected();
        public abstract void OnCmdVariable(string type, MyDictionary<string, object> data);
        public abstract void Log(string tag, string status);
        public abstract void LeaveGameRoom();
        public abstract void showWaitingDialog();
        public abstract void hideWaitingDialog();
    }

    public delegate void runnable();


    enum Status
    {
        INIT_GAME,
        INIT_GAME_WAITING,
        ROLL_DICE,
        ROLL_DICE_WAITING,
        ROLL_DICE_RENDER,
        BEGIN_TURN,
        BEGIN_TURN_WAITING,
        BEGIN_TURN_RENDER,
        IN_TURN,
        IN_TURN_WAITING,
        IN_TURN_RENDER,
        END_TURN,
        END_TURN_WAITING,
        END_TURN_RENDER,
        PAUSED,
        FINISHED,
        FAILED_TO_CONNECT
    };

    class Adapter : Connector
    {
        static public MyDictionary<string, NewSkill> skills = new MyDictionary<string, NewSkill>();
        static public MyDictionary<string, AbnormalStatus> abs = new MyDictionary<string, AbnormalStatus>();

        public const string BEGIN_TURN_CONFIRM = "begin_turn_confirm";
        public const string IN_TURN_CONFIRM = "in_turn_confirm";
        public const string END_TURN_CONFIRM = "end_turn_confirm";
        //public const string CONNECTING_REQUEST = "connecting_request";
        public const string IN_TURN_SKILLS = "in_turn_skills";
        public const string RESTORING_REQUEST = "restoring_request";
        public const string BEGIN_TURN_REQUEST = "begin_turn_request";
        public const string IN_TURN_REQUEST = "in_turn_request";
        public const string END_TURN_REQUEST = "end_turn_request";

        public const string ASK_STATUS_REQUEST = "ask_status_request";
        public const string ANSWER_STATUS_REQUEST = "answer_status_request";
        public const string STATUS_CONFIRM = "status_confirm"; // dung de khoi tao ket noi dau game hoac de reconnect

        public const string ROLL_DICE_REQUEST = "roll_dice_request";
        public const string ROLL_DICE_CONFIRM_MATSER = "roll_dice_confirm_master";
        public const string ROLL_DICE_CONFIRM_SLAVE = "roll_dice_confirm_slave";

        static public long timeoutInMS = 10 * 1000;
        static public int timeoutLimits = 3;
        static public long prorogationMS = (long)(500);
        static public long eventsMS = 30 * 1000;

        public bool isOffline = false;
        public NewLogic logic = null;
        public bool isMaster = true;

        public CharacterPlayer me; // luu tru cau hinh Chracter hien tai
        public CharacterPlayer you;
        //public Character me; // luu tru cau hinh Chracter hien tai
        //public Character you;

        public NewCharacterStatus attStatus = null;// dung de doi lan muon biet lan nao thi vao day
        public NewCharacterStatus defStatus = null;

        public Status status;
        public int turns;
        public RollDiceResult diceResult;
        public BeginTurnResult beginResult;
        public InTurnResult inResult;
        public EndTurnResult endResult;

        public int firstID = 0;
        public int messageID = 0;
        public int msgAnchorID = 0;
        public int msgSheet = 1;
        public Timeouter resultTimeouter;
        public Timeouter eventsTimeouter;
        public ArrayList sendingQueue = new ArrayList();
        public MyDictionary<string, ArrayList> delayedPacQueue = new MyDictionary<string, ArrayList>();
        // for slave
        public ArrayList slavePacQueue = new ArrayList();
        public bool isReadyToNewPac = true;
        //

        public runnable rollDiceRenderUpdate;
        public runnable beginTurnRenderUpdate;
        public runnable inTurnRenderUpdate;
        public runnable endTurnRenderUpdate;
        public ArrayList selectedSkills;


        //public Adapter(Character me, Character you, int firstID = 1, bool isMaster = true, bool isOffline = false) // ai di truoc , adapter nay cua ai, co phai master hay khong, co offline hay khong
        //{
        //    //this.socket = socket;

        //    this.me = me;
        //    this.you = you;
        //    this.isMaster = isMaster;
        //    this.isOffline = isOffline;
        //    this.firstID = firstID;
        //    if (firstID == me.playerId)
        //    {
        //        logic = new NewLogic(me, you);
        //        status = Status.BEGIN_TURN;
        //    }
        //    else
        //    {
        //        logic = new NewLogic(you, me);
        //        status = Status.BEGIN_TURN_WAITING;
        //    }

        //    status = Status.INIT_GAME;

        //    if (isMaster)
        //    {
        //        // tao timeouter cho ask_status_request
        //        resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME, Status.FAILED_TO_CONNECT, () => { }, timeoutInMS, 1, false);
        //    }
        //    else { 
        //        // gui ask_status_request

        //        // tao timeouter cho answer_status_request
        //        resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME, Status.FAILED_TO_CONNECT, () => {
        //            SendAskingStatus();
        //        }, timeoutInMS, 1, true);
        //    }

        //    showWaitingDialog();

        //    attStatus = logic.getStatusByPlayerID(firstID);
        //    defStatus = logic.getStatusByPlayerID(3 - firstID);

        //    this.turns = NewLogic.LIMIT_TURNS;



        //}

        public void addPassiveSkills(ArrayList skills, NewCharacterStatus target)
        {
            logic.addPassiveSkills(skills, target);
            //NewCharacterStatus one = logic.getStatusByPlayerID(me.playerId);
            //NewCharacterStatus two = logic.getStatusByPlayerID(you.playerId);
            //foreach (int idSkill in skills) {
            //    NewSkill skill = one.character.newSkillDic.ContainsKey("Skill" + idSkill) ? attStatus.character.newSkillDic["Skill" + idSkill] : null;
            //    skill.affect(one, two);
            //}
        }


        public Adapter(CharacterPlayer me, CharacterPlayer you, int firstID = 1, bool isMaster = true, bool isOffline = false) // ai di truoc , adapter nay cua ai, co phai master hay khong, co offline hay khong
        {
            //this.socket = socket;

            this.me = me;
            this.you = you;
            this.isMaster = isMaster;
            this.isOffline = isOffline;
            this.firstID = firstID;
            if (firstID == me.playerId)
            {
                logic = new NewLogic(me, you);
                status = Status.BEGIN_TURN;
            }
            else
            {
                logic = new NewLogic(you, me);
                status = Status.BEGIN_TURN_WAITING;
            }

            status = Status.INIT_GAME;

            if (isMaster)
            {
                // tao timeouter cho ask_status_request
                resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME, Status.FAILED_TO_CONNECT, () => { }, timeoutInMS, 1, false);
            }
            else
            {
                // gui ask_status_request

                // tao timeouter cho answer_status_request
                resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME, Status.FAILED_TO_CONNECT, () =>
                {
                    SendAskingStatus();
                }, timeoutInMS / 2, 3, true);
            }

            showWaitingDialog();

            attStatus = logic.getStatusByPlayerID(firstID);
            attStatus.setConditionManager(new ConditionManager(attStatus, this.logic));
            defStatus = logic.getStatusByPlayerID(3 - firstID);
            defStatus.setConditionManager(new ConditionManager(defStatus, this.logic));
            this.turns = NewLogic.LIMIT_TURNS;

           

        }



        public int getCoolDownBySkillID(int skill)
        {
            NewCharacterStatus status = logic.getStatusByPlayerID(me.playerId);
            return status.character.newSkillDic.ContainsKey("Skill" + skill) ? status.character.newSkillDic["Skill" + skill].getCoolDown() : 0;
        }

        public RollDiceResult getDiceResult()
        {
            return diceResult;
        }

        public BeginTurnResult getBeginResult()
        {
            return beginResult;
        }

        public InTurnResult getInResult()
        {
            return inResult;
        }

        public EndTurnResult getEndResult()
        {
            return endResult;
        }

        public static void LoadAssets()
        {
            //skills = SplitDataFromServe.skillInit;
            //abs = SplitDataFromServe.absInit;
            int i = 0;
            while (true)
            {
                i++;
                try
                {
                    string path = "Skills/Skill" + i + ".txt";
                    StreamReader reader = new StreamReader(path);
                    string text = reader.ReadToEnd();//System.IO.File.ReadAllText("Assets/Skills/Skill" + i + ".txt");
                    //text = text.Replace(" ", "");
                    reader.Close();
                    text = text.Replace("\r\n", "");
                    //Log(text);
                    NewSkill skill = new NewSkill(JSONNode.Parse(text));
                    skill.addField("nick", "Skill" + i);
                    skills.Add("Skill" + i, skill);

                    Console.WriteLine("load " + i + " " + skill.data.ToString());

                    if (i == 80) break;

                    //Log(node.SaveToCompressedBase64());
                }
                catch (Exception e)
                {
                    i--;
                    Console.WriteLine(e.Message);
                    break;
                }


            }



            i = 0;
            while (true)
            {
                i++;
                try
                {
                    string path = "AbnormalStatuses/AS" + i + ".txt";
                    StreamReader reader = new StreamReader(path);
                    string text = reader.ReadToEnd();//System.IO.File.ReadAllText("Assets/AbnormalStatuses/AS" + i + ".txt");
                    reader.Close();
                    text = text.Replace("\r\n", "");
                    //Log(text);
                    AbnormalStatus skill = new AbnormalStatus(JSONNode.Parse(text));
                    skill.addField("nick", "AS" + i);
                    abs.Add("AS" + i, skill);
                    //Log(skill.data.ToString());
                    //Log(node.SaveToCompressedBase64());
                }
                catch (Exception e)
                {
                    //
                    i--;
                    Console.WriteLine(e.Message);
                    break;
                }

            }


        }


        public void receivePacket(string type, MyDictionary<string, object> data)
        {

            if (isMaster)
            {
                processPacInMaster(type, data);
            }
            else
            {
                processPacInSlave(type, data);
            }
        }

        private void processPacInMaster(string type, MyDictionary<string, object> data)
        {

            switch (status)
            {
                case Status.INIT_GAME:
                    if (type == ASK_STATUS_REQUEST)
                    {
                        // hoi thu xy ly luon
                        SendAnswerStatus();// co the la ben slave la reconnect hoac connect luc dau game
                        status = Status.INIT_GAME_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.INIT_GAME_WAITING, Status.FAILED_TO_CONNECT, () => { }, timeoutInMS, 1, false); // cho ket qua tiep
                        Log("Master", "receive ask and answer status when game is initital");
                    }
                    break;
                case Status.INIT_GAME_WAITING:
                    if (type == STATUS_CONFIRM)
                    {
                        // bat dau game thoi
                        Log("Master", "receive confirm status and begin game");
                        resultTimeouter = null;// huy cho confirm tu slave
                        hideWaitingDialog();
                        status = Status.BEGIN_TURN;
                    }
                    break;
                case Status.FAILED_TO_CONNECT:
                    break;
                default:
                    if (type == ASK_STATUS_REQUEST) // co the bi hoi trong luc dang chay game
                    {
                        // hoi thu xy ly luon
                        SendAnswerStatus();// co the la ben slave la reconnect hoac connect luc dau game
                        Log("Master", "receive ask and answer status when game is running");
                    }
                    else
                        if (isMyTurn(attStatus))
                    {
                        processPacketOfMeInMaster(type, data);
                    }
                    else
                    {
                        processPacketOfYouInMaster(type, data);

                    }

                    break;
            }
        }

        private void processPacInSlave(string type, MyDictionary<string, object> data)
        {
            Console.WriteLine("process pack in slave " + type + " " + status.ToString());
            switch (status)
            {
                case Status.INIT_GAME:
                    //case Status.INIT_GAME_WAITING:

                    if (type == ANSWER_STATUS_REQUEST)
                    {
                        Console.WriteLine("process pack in slave check  " + type + " " + status.ToString());
                        resultTimeouter = null; // huy cho doi cau tra loi tu master
                        status = Status.INIT_GAME_WAITING;
                        //sendCon

                        string masterStatus = data["masterStatus"] as string;
                        Status mStatus = Utilities.ParseEnum<Status>(masterStatus);
                        switch (mStatus)
                        {
                            case Status.INIT_GAME:
                            case Status.INIT_GAME_WAITING:
                                Console.WriteLine("Slave" + "master answer when init game");
                                break;
                            default:
                                Console.WriteLine("Slave" + "master answer when running game");
                                break;
                        }
                        SendStatusConfirm(); // phan hoi lai tra loi rang slave da dong y

                    }
                    break;
                //case Status.INIT_GAME_WAITING:
                //    break;
                case Status.FAILED_TO_CONNECT:
                    break;
                default:

                    Console.WriteLine("" + "receive Pack in slave " + type + " " + isReadyToNewPac);
                    slavePacQueue.Add(new Pac(type, data));
                    processNewPacInSlave();
                    break;
            }
        }

        public void processNewPacInSlave()
        {
            if (isReadyToNewPac)
            {
                Pac pac = slavePacQueue.Count > 0 ? (Pac)slavePacQueue[0] : null;
                if (pac == null)
                {
                    return;
                }
                slavePacQueue.RemoveAt(0);
                isReadyToNewPac = false;
                if (isMyTurn(attStatus))
                {
                    processPacketOfMeInSlave(pac.type, pac.data);
                }
                else
                {
                    processPacketOfYouInSlave(pac.type, pac.data);

                }
            }
        }

        public bool canRollDice()
        {
            return isMyTurn(attStatus) && status == Status.ROLL_DICE && eventsTimeouter != null && beginResult != null && beginResult.continued;
        }

        public bool rollDice()
        {
            if (canRollDice())
            {
                System.Random random = new System.Random();
                diceResult = new RollDiceResult(random.Next(6) + 1, random.Next(6) + 1, random.Next(6) + 1);
                if (isMaster)
                {
                    eventsTimeouter = null;
                    if (!isOffline)
                    {
                        status = Status.ROLL_DICE_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.ROLL_DICE_WAITING, Status.ROLL_DICE_RENDER, () => { //gui dice qua ben kia
                            SendRollDice();
                        }, timeoutInMS, timeoutLimits, true);
                    }
                }
                else
                {
                    eventsTimeouter = null;
                    status = Status.ROLL_DICE_WAITING;
                    SendRollDice();
                    // gui dice qua ben kia
                }
                return true;
            }
            return false;
        }


        public void attackWithSkills(ArrayList skills)
        {
            Log("attackSkills", "" + canSelectSkill());
            if (canSelectSkill())
            {

                selectedSkills = skills;

                foreach (int skill in skills)
                {// su dung
                    attStatus.character.newSkillDic["Skill" + skill].use();
                }
                if (isMaster)
                {
                    ArrayList actions = new ArrayList();
                    foreach (int skill in skills)
                    {

                        ActionHandle a = new ActionHandle(0, skill, 0, 0);
                        actions.Add(a);
                    }

                    inResult = new InTurnResult(logic.inTurn(actions)); // truyen vao arrayListActionHandles
                    Log("inTurn+++++++++++++++++++++ UpdateMeInMaster", "" + inResult.states.Count + " isOffline = " + isOffline);
                    foreach (State state in inResult.states)
                    {
                        Log("state in inTurn UpdateMeInMaster", state.toJSON().ToString());
                    }
                    if (!isOffline)
                    {
                        status = Status.IN_TURN_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.IN_TURN_WAITING, Status.IN_TURN_RENDER, () => { SendGameDataInTurn(); }, timeoutInMS, timeoutLimits, true);
                    }
                    else
                    {

                    }
                    eventsTimeouter = null;
                }
                else
                {
                    SendSkillListInTurn(skills);
                    status = Status.IN_TURN_WAITING;
                    eventsTimeouter = null;
                }
            }
        }

        public void Update()
        {
            if (isMaster)
            {
                UpdateMaster();
            }
            else
            {
                UpdateSlave();
            }
            turns = logic.getRemainingTurns();
            //Log("Update","Base");
        }

        public int getEventTime()
        {
            return eventsTimeouter == null ? 0 : (int)Math.Round(eventsTimeouter.getRemainingTime() * 1.0f / 1000);
        }

        public bool canSelectSkill()
        {
            return isMyTurn(attStatus) && status == Status.IN_TURN && eventsTimeouter != null && beginResult != null && beginResult.continued;
        }

        public Status getStatus()
        {
            return status;
        }

        public int getTurn()
        {
            return attStatus.playerID;
        }

        public int getMyID()
        {
            return me.playerId;
        }

        public int getFirstID()
        {
            return firstID;
        }

        public bool isOnLine()
        {
            return !isOffline;
        }

        public void setMaster(bool master)
        {
            this.isMaster = master;
        }

        public CharacterPlayer getMe()
        {
            return me;
        }

        public NewCharacterStatus getStatusOfMe() {
            return logic.getStatusByPlayerID(me.playerId);
        }

        //public Character getMe()
        //{
        //    return me;
        //}

        

        public int getTurnTimes()
        {
            return turns;
        }

        public void processPacketOfMeInMaster(string type, MyDictionary<string, object> data)
        {
            Console.WriteLine("type" + type);
            int messageID = Convert.ToInt32(data["messageID"]);
            Log("processPacketOfMeInMaster", type + " messageID" + messageID + " in (" + " " + msgAnchorID + "," + (msgAnchorID + msgSheet) + " ), isOffline = " + isOffline);
            if (isOffline)
            {// ghi nhan co goi tin den trong luc offline cho xu ly o cac luc BEGIN_TURN,END_TURN,IN_TURN
                ArrayList list = !delayedPacQueue.ContainsKey(type) ? new ArrayList() : delayedPacQueue[type];
                list.Add(new Pac(type, data));
                delayedPacQueue.Add(type, list);
                Log("DelayedQueue", "recevice a packet");
            }
            else
            if (!isValid(messageID))
            {
                /// khong hop le ma online thi khong xet vi den tre
                /// vi du action cua doi phuong

                return;
            }
            switch (status)
            {
                case Status.BEGIN_TURN_WAITING:
                    if (type == BEGIN_TURN_CONFIRM)
                    {
                        resultTimeouter = null;
                        Log("processPacketOfMeInMaster", status.ToString());
                        status = Status.BEGIN_TURN_RENDER;

                    }
                    break;

                case Status.ROLL_DICE_WAITING:
                    if (type == ROLL_DICE_CONFIRM_SLAVE)
                    {
                        resultTimeouter = null;
                        Log("processPacketOfMeInMaster", status.ToString());
                        status = Status.ROLL_DICE_RENDER;

                    }
                    break;
                case Status.END_TURN_WAITING:
                    if (type == END_TURN_CONFIRM)
                    {
                        resultTimeouter = null;
                        Log("processPacketOfMeInMaster", status.ToString());
                        status = Status.END_TURN_RENDER;

                    }
                    break;
                case Status.IN_TURN_WAITING:
                    if (type == IN_TURN_CONFIRM)
                    {
                        resultTimeouter = null;
                        Log("processPacketOfMeInMaster", status.ToString());
                        status = Status.IN_TURN_RENDER;

                    }
                    break;
                default:
                    break;
            }

        }

        public void processPacketOfYouInMaster(string type, MyDictionary<string, object> data)
        {
            int messageID = Convert.ToInt32(data["messageID"]);
            Log("processPacketOfYouInMaster", type + " messageID" + messageID + " in (" + " " + msgAnchorID + "," + (msgAnchorID + msgSheet) + " ), isOffline = " + isOffline);
            if (isOffline)
            {// ghi nhan co goi tin den trong luc offline cho xu ly o cac luc BEGIN_TURN,END_TURN,IN_TURN
                ArrayList list = !delayedPacQueue.ContainsKey(type) ? new ArrayList() : delayedPacQueue[type];
                list.Add(new Pac(type, data));
                delayedPacQueue.Add(type, list);
                Log("processPacketOfYouInMaster", "recevice a packet");
            }
            else
            if (!isValid(messageID))
            {
                /// khong hop le ma online thi khong xet vi den tre
                /// vi du action cua doi phuong
                Log("processPacketOfYouInMaster", "vao khong hop le");
                return;
            }
            switch (status)
            {
                case Status.BEGIN_TURN_WAITING:
                    if (type == BEGIN_TURN_CONFIRM)
                    {
                        // xoa bo timeout 
                        // chuyen sang render ngay
                        resultTimeouter = null;
                        status = Status.BEGIN_TURN_RENDER;
                        Log("processPacketOfYouInMaster", "BEGIN_TURN_CONFIRM packet");
                    }
                    break;
                case Status.END_TURN_WAITING:
                    if (type == END_TURN_CONFIRM)
                    {
                        resultTimeouter = null;
                        status = Status.END_TURN_RENDER;
                        Log("processPacketOfYouInMaster", "END_TURN_CONFIRM packet");
                    }
                    break;
                case Status.IN_TURN:// dang o trong IN_TURN cho thi nhan duoc su kien nay truyen cua doi phuong
                    if (type == IN_TURN_SKILLS)
                    {
                        // loai bo doi action tu ben kia
                        // lap timout doi ben kia confirm
                        //try {
                        eventsTimeouter = null;
                        ArrayList actionHandles = new ArrayList();
                        //Log("processPacketOfYouInMaster", "in_turn_skills 1");
                        try
                        {
                            string bys = data["skills"] as string;
                            //Log("processPacketOfYouInMaster", "in_turn_skills 2 |" );
                            byte[] bytes = Utilities.convertStringToByteArr(bys);
                            //Log("processPacketOfYouInMaster", "in_turn_skills 3 bytes length = "+ bytes.Length); // truyen 6 nhan duoc la 8 tai sao
                            //byte[] bytes = data["skills"] as byte[];

                            for (int i = 0; i < bytes.Length / 2; i++)
                            {
                                byte[] part = Utilities.SubArray<byte>(bytes, 2 * i, 2);
                                ushort skillID = BitConverter.ToUInt16(part, 0);
                                Log("processPacketOfYouInMaster", "skills " + skillID);
                                actionHandles.Add(new ActionHandle(-1, skillID, 0, 0));
                                attStatus.character.newSkillDic["Skill" + skillID].use();
                            }
                        }
                        catch (Exception e)
                        {
                            actionHandles = new ArrayList();
                        }

                        //Log("processPacketOfYouInMaster", "in_turn_skills 4");
                        inResult = new InTurnResult(logic.inTurn(actionHandles));
                        //Log("processPacketOfYouInMaster", "in_turn_skills 5");

                        status = Status.IN_TURN_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.IN_TURN_WAITING, Status.IN_TURN_RENDER, () => { SendGameDataInTurn(); }, timeoutInMS, timeoutLimits, true);

                        Log("processPacketOfYouInMaster", "IN_TURN has IN_TURN_SKILLS ");
                        foreach (State state in inResult.states)
                        {
                            Log("processPacketOfYouInMaster", "state = " + state.toJSON().ToString());
                        }
                        //} catch (Exception e) {
                        //    Log("processPacketOfYouInMaster", "in_turn_skills "+e.Message);
                        //}

                    }
                    break;
                case Status.IN_TURN_WAITING:
                    if (type == IN_TURN_CONFIRM)
                    {
                        resultTimeouter = null;
                        status = Status.IN_TURN_RENDER;
                        Log("processPacketOfYouInMaster", "IN_TURN_CONFIRM packet");
                    }
                    break;
                case Status.ROLL_DICE:
                    if (type == ROLL_DICE_REQUEST)
                    {
                        int dice1 = Convert.ToByte(data["dice1"]);
                        int dice2 = Convert.ToByte(data["dice2"]);
                        int dice3 = Convert.ToByte(data["dice3"]);
                        diceResult = new RollDiceResult(dice1, dice2, dice3);
                        eventsTimeouter = null;// khong can cho doi phuong nua
                        resultTimeouter = createTimeouterForSendingData(Status.ROLL_DICE_WAITING, Status.ROLL_DICE_RENDER, () => { ConfirmRollDiceOfMaster(); }, timeoutInMS, timeoutLimits, true);


                    }
                    break;
                case Status.ROLL_DICE_WAITING:
                    if (type == ROLL_DICE_CONFIRM_SLAVE)
                    {
                        resultTimeouter = null;
                        status = Status.ROLL_DICE_RENDER;
                    }
                    break;
                default:
                    break;
            }

        }



        public void processPacketOfMeInSlave(string type, MyDictionary<string, object> data)
        {


            // truye sang ben kia
            Log("processPacketOfMeInSlave", type + " messageID" + messageID + " in (" + " " + msgAnchorID + "," + (msgAnchorID + msgSheet) + " ), isOffline = " + isOffline);
            switch (type)
            {
                case RESTORING_REQUEST:
                    ParseRestoringData(data);
                    break;
                case BEGIN_TURN_REQUEST:
                    ParseBeginTurnData(data);
                    break;
                case END_TURN_REQUEST:
                    ParseEndTurnData(data);
                    break;

                case IN_TURN_REQUEST:

                    ParseInTurnData(data);
                    break;
                case ROLL_DICE_CONFIRM_MATSER:
                    if (status != Status.ROLL_DICE && status != Status.ROLL_DICE_WAITING && status != Status.INIT_GAME_WAITING)
                    {
                        Log("processPacketOfMeInSlave", "ROLL_DICE_CONFIRM_MATSER is'nt on status " + status.ToString());
                        return;
                    }
                    status = Status.ROLL_DICE_RENDER;
                    ConfirmRollDiceOfSlave();
                    //isReadyToNewPac = true;
                    break;


            }
        }

        public void processPacketOfYouInSlave(string type, MyDictionary<string, object> data)
        {
            // xet xem co phai goi 
            Log("processPacketOfYouInSlave", type + " messageID" + messageID + " in (" + " " + msgAnchorID + "," + (msgAnchorID + msgSheet) + " ), isOffline = " + isOffline);


            switch (type)
            {
                case ROLL_DICE_REQUEST:
                    NewCharacterStatus one = null;
                    NewCharacterStatus two = null;
                    if (status != Status.ROLL_DICE && status != Status.ROLL_DICE_WAITING && status != Status.INIT_GAME_WAITING)
                    {
                        Log("processPacketOfYouInSlave", "ROLL_DICE_REQUEST is'nt on time " + status.ToString());
                        return;
                    }
                    messageID = Convert.ToInt32(data["messageID"]) - 1;
                    ConfirmRollDiceOfSlave();
                    eventsTimeouter = null;

                    int dice1 = Convert.ToByte(data["dice1"]);
                    int dice2 = Convert.ToByte(data["dice2"]);
                    int dice3 = Convert.ToByte(data["dice3"]);
                    diceResult = new RollDiceResult(dice1, dice2, dice3);
                    status = Status.ROLL_DICE_RENDER;

                    break;
                case RESTORING_REQUEST:
                    ParseRestoringData(data);
                    break;
                case BEGIN_TURN_REQUEST:
                    ParseBeginTurnData(data);
                    break;
                case END_TURN_REQUEST:
                    ParseEndTurnData(data);
                    break;
                case IN_TURN_REQUEST:
                    ParseInTurnData(data);
                    break;

            }
        }



        public bool isMyTurn(NewCharacterStatus turnStatus)
        {
            return me.playerId == turnStatus.playerID;
        }

        public void UpdateMaster()
        {
            switch (status)
            {
                case Status.INIT_GAME:
                case Status.INIT_GAME_WAITING:
                    if (resultTimeouter != null) resultTimeouter.update();
                    break;
                case Status.FAILED_TO_CONNECT:
                    Log("master", "failed to init game in master");
                    LeaveGameRoom();
                    status = Status.FINISHED;
                    break;
                default:
                    if (isMyTurn(attStatus))
                    {

                        UpdateMeInMaster();
                    }
                    else
                    {

                        UpdateYouInMaster();
                    }
                    break;
            }

        }



        public void fixMe(int materID)
        { // cai dat lai playerID cua me kem theo set lai playerId cua myAttatus
            if (!isMaster)
                if (me.playerId == materID)
                {
                    NewCharacterStatus statusMe = logic.getStatusByPlayerID(me.playerId);
                    NewCharacterStatus statusYou = logic.getStatusByPlayerID(you.playerId);
                    me.playerId = 3 - materID;
                    statusMe.playerID = me.playerId;
                    you.playerId = materID;
                    statusYou.playerID = you.playerId;
                }
        }

        public void fixTurn(int turn)
        {
            logic.fixTurn(turn);
        }

        public void fixInfo(int firstID, int masterID, int turnID)
        {
            this.firstID = firstID;
            logic.setFirstID(firstID);
            fixMe(masterID);
            fixTurn(turnID);
            attStatus = logic.getStatusByPlayerID(turnID);
            defStatus = logic.getStatusByPlayerID(3 - turnID);
        }

        public Timeouter createTimeouterForSendingData(Status beforeTimeout, Status afterTimeout, runnable sendData, long timeoutInMS, int maxTimes, bool network)
        {
            Log("create Timeout", beforeTimeout.ToString() + " " + afterTimeout.ToString() + " " + timeoutInMS + " " + maxTimes);
            status = beforeTimeout;
            Timeouter timeouter = new Timeouter(timeoutInMS, maxTimes, (long times, int timesLimit) => { // gui du lieu 3 lan voi goi tin msgAnchorID- > msgAnchorID + 2

                // xet gioi han timeout
                if (times == timesLimit)
                {
                    if (network)
                    {
                        if (isDisconnected())
                        {
                            isMaster = false;
                            status = Status.PAUSED;
                        }
                        else
                        {

                            isOffline = true;
                            status = afterTimeout;
                        }
                    }
                    else
                    {
                        status = afterTimeout;
                    }

                }
                else if (times < timesLimit)
                {

                    // gui thong tin o day
                    // gui goi
                    if (network)
                    {
                        sendData();
                        if (times == 0)
                        {
                            msgAnchorID = messageID;
                            msgSheet = timesLimit;
                        }
                    }

                }
            });
            return timeouter;
        }

        public void UpdateSlave()
        {

            switch (status)
            {
                case Status.INIT_GAME:
                    if (resultTimeouter != null) resultTimeouter.update();
                    break;
                case Status.FAILED_TO_CONNECT:
                    // cho ket qua dau tien se chuyen sang status khac neu nhu ben slave dut ket noi do khong nhan duoc confirm cua thi se dut ket noi
                    Log("slave", "failed to connect/reconnect to master");
                    LeaveGameRoom();
                    status = Status.FINISHED;
                    break;
                default:
                    if (isMyTurn(attStatus))
                    {
                        UpdateMeInSlave();
                    }
                    else
                    {
                        UpdateYouInSlave();
                    }
                    break;
            }


        }

        public void UpdateMeInMaster()
        {
            System.Random random = new System.Random();

            switch (status)
            {
                case Status.ROLL_DICE:
                    if (eventsTimeouter != null) eventsTimeouter.update(); // offline hay online deu cho su kien dice
                    if (isOffline)
                    {
                        if (eventsTimeouter != null) return; // chua thuc hien khi chua cho het eventsTimouter
                        if (delayedPacQueue.Count > 0)
                        {
                            isOffline = false;
                            delayedPacQueue.Clear();


                            resultTimeouter = createTimeouterForSendingData(Status.ROLL_DICE_WAITING, Status.ROLL_DICE_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);

                        }
                        else
                        {
                            status = Status.ROLL_DICE_RENDER;
                            Log("UpdateMeMaster", "status " + status.ToString() + " " + isMaster);
                        }
                    }
                    else
                    {
                        // doi nhan nut roi gui
                    }
                    break;
                case Status.ROLL_DICE_RENDER:
                    if (rollDiceRenderUpdate != null) rollDiceRenderUpdate();
                    break;
                case Status.BEGIN_TURN:
                    beginResult = logic.beginTurn(attStatus, defStatus);
                    bool continued = beginResult.continued;
                    ArrayList states = beginResult.states;
                    foreach (State state in states)
                    {
                        Log("state in beginTurn UpdateMeInMaster", state.toJSON().ToString());
                    }

                    // gui
                    if (isOffline)
                    {
                        if (delayedPacQueue.Count > 0)
                        {
                            isOffline = false;
                            delayedPacQueue.Clear();
                            resultTimeouter = createTimeouterForSendingData(Status.BEGIN_TURN_WAITING, Status.BEGIN_TURN_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);
                        }
                        else
                            status = Status.BEGIN_TURN_RENDER;
                    }
                    else
                    {
                        status = Status.BEGIN_TURN_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.BEGIN_TURN_WAITING, Status.BEGIN_TURN_RENDER, () => { SendGameDataBeginTurn(); }, timeoutInMS, timeoutLimits, true);

                    }
                    break;
                case Status.BEGIN_TURN_RENDER:
                    //status = Status.IN_TURN;
                    if (beginTurnRenderUpdate != null) beginTurnRenderUpdate();

                    // nen tao 1 timer outter moi de dem lui su kien nhan skill

                    break;
                case Status.IN_TURN:
                    continued = beginResult.continued;
                    if (continued)
                    { // nhan su kien va xu ly su kien o day la tu chon o master luon
                      //ActionHandle a = new ActionHandle(0, random.Next(8) + 13, 0, 0);
                      //                  inResult = new InTurnResult(logic.inTurn(new ArrayList() { a })); // truyen vao arrayListActionHandles
                      //Log("inTurn+++++++++++++++++++++ UpdateMeInMaster", ""+inResult.states.Count);
                      //foreach (State state in inResult.states)
                      //{
                      //	Log("state in inTurn UpdateMeInMaster", state.toJSON().ToString());
                      //}

                        // du co offline online hay khong van cho su kien
                        if (eventsTimeouter != null) eventsTimeouter.update();

                        if (isOffline)
                        {
                            if (eventsTimeouter != null) return; // chua thuc hien khi chua cho het eventsTimouter
                            Log("UpdateMeMaster", "Update when offfline " + (delayedPacQueue.Count > 0));
                            if (delayedPacQueue.Count > 0)
                            {
                                isOffline = false;
                                delayedPacQueue.Clear();
                                status = Status.IN_TURN_WAITING;
                                resultTimeouter = createTimeouterForSendingData(Status.IN_TURN_WAITING, Status.IN_TURN_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);

                            }
                            else
                            {
                                status = Status.IN_TURN_RENDER;
                                Log("UpdateMeMaster", "status " + status.ToString() + " " + isMaster);
                            }
                        }
                        else
                        {

                            // cho lenh nhan su kien

                        }

                    }
                    else
                    {
                        status = Status.END_TURN;
                    }





                    break;
                case Status.IN_TURN_RENDER:
                    if (inTurnRenderUpdate != null)
                    {
                        inTurnRenderUpdate();
                    }
                    //status = Status.END_TURN;
                    break;

                case Status.END_TURN:
                    endResult = logic.endTurn(attStatus, defStatus);
                    int result = endResult.combatResult;
                    Log("mau player att UpdateMeInMaster", "" + attStatus.getCurrentIndex(Indexes.hp_na));
                    Log("mau player def UpdateMeInMaster", "" + defStatus.getCurrentIndex(Indexes.hp_na));
                    ArrayList effectResleased = endResult.releasedState;
                    Log("deo hieu", "tai sao1");
                    if (effectResleased != null)
                    {
                        foreach (NewEffect effect in effectResleased)
                        {
                            Log("state in Released UpdateMeInMaster", effect.toJSON().ToString());
                        }
                    }

                    if (isOffline)
                    {
                        Log("deo hieu", "tai sao2");
                        if (delayedPacQueue.Count > 0)
                        {
                            isOffline = false;
                            delayedPacQueue.Clear();
                            resultTimeouter = createTimeouterForSendingData(Status.END_TURN_WAITING, Status.END_TURN_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);
                        }
                        else
                            status = Status.END_TURN_RENDER;
                    }
                    else
                    {
                        Log("deo hieu", "tai sao3");
                        status = Status.END_TURN_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.END_TURN_WAITING, Status.END_TURN_RENDER, () => { SendGameDataEndTurn(); }, timeoutInMS, timeoutLimits, true);

                    }
                    Log("deo hieu", "tai sao4");
                    break;
                case Status.END_TURN_RENDER:
                    // gui
                    if (endTurnRenderUpdate != null)
                    {
                        endTurnRenderUpdate();
                    }
                    break;
                case Status.ROLL_DICE_WAITING:
                case Status.END_TURN_WAITING:
                case Status.IN_TURN_WAITING:
                case Status.BEGIN_TURN_WAITING:
                    if (resultTimeouter != null) resultTimeouter.update();
                    break;
                case Status.PAUSED:
                    break;

            }
        }

        public void UpdateYouInMaster()
        {
            System.Random random = new System.Random();

            switch (status)
            {
                case Status.ROLL_DICE:
                    if (isOffline)
                    {// da offline thi khong can cho doi phuong
                     //tu chay dice luon


                        diceResult = new RollDiceResult(random.Next(6) + 1, random.Next(6) + 1, random.Next(6) + 1);
                        //
                        if (delayedPacQueue.Count > 0)
                        {
                            isOffline = false;
                            delayedPacQueue.Clear();


                            resultTimeouter = createTimeouterForSendingData(Status.ROLL_DICE_WAITING, Status.ROLL_DICE_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);
                        }
                        else
                            status = Status.ROLL_DICE_RENDER;
                    }
                    else
                    {// cho doi phuong xem
                        if (eventsTimeouter != null) eventsTimeouter.update();// 2 timeouter khong he ton tai dong thoi trong 1 lan
                                                                              //if (resultTimeouter != null) resultTimeouter.update();// timeouter nay la 2 lan cho 1 lan cho ket qua chon skill , lan ke la doi ben kia cap nhat ve
                    }
                    break;
                case Status.ROLL_DICE_RENDER:
                    if (rollDiceRenderUpdate != null) rollDiceRenderUpdate();
                    break;
                case Status.BEGIN_TURN:
                    beginResult = logic.beginTurn(attStatus, defStatus);
                    bool continued = beginResult.continued;
                    ArrayList states = beginResult.states;
                    foreach (State state in states)
                    {
                        Log("state in beginTurn UpdateYouInMaster", state.toJSON().ToString());
                    }

                    // gui
                    if (isOffline)
                    {
                        if (delayedPacQueue.Count > 0)
                        {
                            isOffline = false;
                            delayedPacQueue.Clear();
                            resultTimeouter = createTimeouterForSendingData(Status.BEGIN_TURN_WAITING, Status.BEGIN_TURN_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);
                        }
                        else
                            status = Status.BEGIN_TURN_RENDER;
                    }
                    else
                    {
                        // truyen goi tin cho doi thu va doi

                        status = Status.BEGIN_TURN_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.BEGIN_TURN_WAITING, Status.BEGIN_TURN_RENDER, () => { SendGameDataBeginTurn(); }, timeoutInMS, timeoutLimits, true);

                    }
                    break;
                case Status.BEGIN_TURN_RENDER:
                    // render va chuyen ngay sang INturn
                    //status = Status.IN_TURN;
                    if (beginTurnRenderUpdate != null) beginTurnRenderUpdate();
                    break;
                case Status.IN_TURN:
                    // 2 lan timeouter
                    continued = beginResult.continued;
                    if (continued)
                    {

                        if (isOffline)
                        {// da offline thi khong can cho doi phuong phuong// can Fixxxxxxxxxxxxxxxxxxxxxxxxxx
                         //AI excutes here
                         // 

                            ArrayList choices = new ArrayList();
                            MyDictionary<string, NewSkill> dicts = attStatus.character.newSkillDicOfCharacter;
                            if (dicts.Keys.Count > 0)
                            {
                                foreach (string skill in dicts.Keys)
                                {
                                    NewSkill act = dicts[skill];
                                    if (act.canCastSkill(logic))
                                    choices.Add(dicts[skill].getID());
                                }

                                ActionHandle a = new ActionHandle(0, (int)choices[random.Next(choices.Count)], 0, 0);
                                // truyen vao arrayListActionHandles
                                choices.Clear();
                                choices.Add(a);
                            }
                            states = logic.inTurn(choices);
                            inResult = new InTurnResult(states);
                            //
                            if (delayedPacQueue.Count > 0)
                            {
                                isOffline = false;
                                delayedPacQueue.Clear();
                                status = Status.IN_TURN_WAITING;
                                resultTimeouter = createTimeouterForSendingData(Status.IN_TURN_WAITING, Status.IN_TURN_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);
                            }
                            else
                                status = Status.IN_TURN_RENDER;
                        }
                        else
                        {// cho doi phuong xem
                            if (eventsTimeouter != null) eventsTimeouter.update();// 2 timeouter khong he ton tai dong thoi trong 1 lan
                            //if (resultTimeouter != null) resultTimeouter.update();// timeouter nay la 2 lan cho 1 lan cho ket qua chon skill , lan ke la doi ben kia cap nhat ve
                        }


                    }
                    else
                    {
                        status = Status.END_TURN;
                    }

                    break;
                case Status.IN_TURN_RENDER:
                    if (inTurnRenderUpdate != null)
                    {
                        inTurnRenderUpdate();
                    }
                    break;

                case Status.END_TURN:
                    endResult = logic.endTurn(attStatus, defStatus);
                    int result = endResult.combatResult;
                    Log("mau player att UpdateYouInMaster", "" + attStatus.getCurrentIndex(Indexes.hp_na));
                    Log("mau player def UpdateYouInMaster", "" + defStatus.getCurrentIndex(Indexes.hp_na));
                    ArrayList effectResleased = endResult.releasedState;
                    if (effectResleased != null)
                    {
                        foreach (NewEffect effect in effectResleased)
                        {
                            Log("state in Released UpdateYouInMaster", effect.toJSON().ToString());
                        }
                    }
                    // gui


                    if (isOffline)
                    {
                        if (delayedPacQueue.Count > 0)
                        {
                            isOffline = false;
                            delayedPacQueue.Clear();
                            status = Status.END_TURN_WAITING;
                            resultTimeouter = createTimeouterForSendingData(Status.END_TURN_WAITING, Status.END_TURN_RENDER, () => { SendRestoringData(); }, timeoutInMS, 1, true);
                        }
                        else
                            status = Status.END_TURN_RENDER;
                    }
                    else
                    {
                        status = Status.END_TURN_WAITING;
                        resultTimeouter = createTimeouterForSendingData(Status.END_TURN_WAITING, Status.END_TURN_RENDER, () => { SendGameDataEndTurn(); }, timeoutInMS, timeoutLimits, true);

                    }
                    break;
                case Status.END_TURN_RENDER:
                    if (endTurnRenderUpdate != null)
                    {
                        endTurnRenderUpdate();
                    }
                    break;
                case Status.ROLL_DICE_WAITING:
                case Status.END_TURN_WAITING:
                case Status.IN_TURN_WAITING:
                case Status.BEGIN_TURN_WAITING:
                    if (resultTimeouter != null) resultTimeouter.update();
                    break;
                case Status.PAUSED:
                    break;

            }
        }

        public void UpdateMeInSlave()
        { // lan cua slave trong slave
            switch (status)
            {
                case Status.ROLL_DICE:
                    if (eventsTimeouter != null) eventsTimeouter.update();
                    break;
                case Status.ROLL_DICE_WAITING:
                    break;
                case Status.ROLL_DICE_RENDER:
                    if (rollDiceRenderUpdate != null) rollDiceRenderUpdate();
                    break;
                case Status.BEGIN_TURN:
                    break;
                case Status.BEGIN_TURN_WAITING:
                    break;
                case Status.BEGIN_TURN_RENDER:
                    //status = Status.IN_TURN;
                    if (beginTurnRenderUpdate != null) beginTurnRenderUpdate();

                    break;
                case Status.IN_TURN:
                    if (eventsTimeouter != null) eventsTimeouter.update();
                    break;
                case Status.IN_TURN_WAITING:
                    break;
                case Status.IN_TURN_RENDER:
                    if (inTurnRenderUpdate != null)
                    {
                        inTurnRenderUpdate();
                    }
                    //status = Status.END_TURN_WAITING; 
                    break;
                case Status.END_TURN:
                    break;
                case Status.END_TURN_WAITING:
                    //status = Status.END_TURN_RENDER; doi ket qua roi moi render
                    break;
                case Status.END_TURN_RENDER:
                    if (endTurnRenderUpdate != null)
                    {
                        endTurnRenderUpdate();
                    }
                    break;
            }
        }

        public void UpdateYouInSlave()
        {// lan cua master trong slave
            switch (status)
            {
                case Status.ROLL_DICE:
                    if (eventsTimeouter != null) eventsTimeouter.update();
                    break;
                case Status.ROLL_DICE_WAITING:
                    break;
                case Status.ROLL_DICE_RENDER:
                    if (rollDiceRenderUpdate != null) rollDiceRenderUpdate();
                    break;
                case Status.BEGIN_TURN:
                    break;
                case Status.BEGIN_TURN_WAITING:

                    //status = Status.BEGIN_TURN_RENDER; doi ket qua tra ve
                    break;
                case Status.BEGIN_TURN_RENDER:
                    //status = Status.IN_TURN_WAITING;
                    if (beginTurnRenderUpdate != null) beginTurnRenderUpdate();
                    break;
                case Status.IN_TURN:
                    if (eventsTimeouter != null) eventsTimeouter.update();
                    break;
                case Status.IN_TURN_WAITING:
                    //status = Status.IN_TURN_RENDER; doi ket qua tra ve
                    break;
                case Status.IN_TURN_RENDER:
                    //status = Status.END_TURN_WAITING;
                    if (inTurnRenderUpdate != null)
                    {
                        inTurnRenderUpdate();
                    }
                    break;
                case Status.END_TURN:
                    break;
                case Status.END_TURN_WAITING:
                    //status = Status.END_TURN_RENDER;
                    break;
                case Status.END_TURN_RENDER:
                    if (endTurnRenderUpdate != null)
                    {
                        endTurnRenderUpdate();
                    }

                    break;
            }
        }





        public bool isValid(int comingMessageID)
        {
            int maxRange = msgAnchorID + msgSheet;
            if (msgAnchorID - 1 < comingMessageID && comingMessageID < maxRange) return true;
            return false;
        }

        // request from Slave to Master

        public void SendAskingStatus()
        {
            messageID++;
            string type = ASK_STATUS_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            sendingQueue.Add(new Pac(type, data));
            Log("send Ask Status", "create msg " + messageID + " " + type);
        }

        public void ConfirmRollDiceOfSlave()
        {
            messageID++;
            string type = ROLL_DICE_CONFIRM_SLAVE;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            sendingQueue.Add(new Pac(type, data));
            Log("confirm RollDice Of Slave", "create msg " + messageID + " " + type);
        }

        private void ParseInTurnData(MyDictionary<string, object> data)
        {
            if (status != Status.IN_TURN && status != Status.IN_TURN_WAITING && status != Status.INIT_GAME_WAITING)
            {
                Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave", "IN_TURN_REQUEST is'nt on status " + status.ToString());
                return;
            }
            if (status == Status.INIT_GAME_WAITING)
            {
                hideWaitingDialog();
            }
            messageID = Convert.ToInt32(data["messageID"]) - 1;
            ConfirmGameDataInTurn();

            int i = -1;
            int turn = Convert.ToByte(data["turn"]);
            ArrayList states = new ArrayList();
            byte[] bytes = null;
            while (true)
            {

                i++;
                if (data.ContainsKey("state_" + i))
                {
                    //List<byte> bys = data["state_" + i] as List<byte>;
                    bytes = Utilities.convertStringToByteArr(data["state_" + i] as string);

                    //bytes = data["state_" + i] as byte[];
                    State state = State.parseBytes(bytes);
                    states.Add(state);
                    // state o day khong mang effect gi moi ngoai healthChanges
                    // cap nhat healthAtt
                    logic.getStatusByPlayerID(turn).curIndexes.Add(Indexes.hp_na, turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                    logic.getStatusByPlayerID(turn).midIndexes.Add(Indexes.hp_na, turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                    logic.getStatusByPlayerID(3 - turn).curIndexes.Add(Indexes.hp_na, 3 - turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                    logic.getStatusByPlayerID(3 - turn).midIndexes.Add(Indexes.hp_na, 3 - turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                }
                else
                {
                    i--;
                    Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave IN_TURN_REQUEST", "Number of States " + (i + 1));
                    break;
                }
            }
            foreach (State state in states)
            {
                foreach (NewEffect effect in state.effects)
                {
                    if (effect.ConvertTypeToNumber() < 2000)
                    {
                        // la newEffect
                        // la abnormal
                        NewCharacterStatus att = logic.getStatusByPlayerID(turn);
                        NewCharacterStatus def = logic.getStatusByPlayerID(3 - turn);
                        foreach (AtomicEffect atomic in effect.atomicEffects)
                        {
                            // them vao
                            ArrayList atomics = att.atomics.ContainsKey(atomic.index) ? att.atomics[atomic.index] : new ArrayList();
                            atomics.Add(atomic);
                            NewCharacterStatus targetStatus = atomic.enemy ? def : att;

                            targetStatus.atomics.Add(atomic.index, atomics);
                            if (atomic.condition != "")
                            {
                                ArrayList enableAtomics = targetStatus.enableAtomics.ContainsKey(atomic.condition) ? targetStatus.enableAtomics[atomic.condition] : new ArrayList();
                                enableAtomics.Add(atomic);
                                targetStatus.enableAtomics.Add(atomic.condition, enableAtomics);
                            }
                        }

                        if (effect.playerID == turn) att.replaceEffect(effect.name, effect);
                        else if (effect.playerID == 3 - turn) def.replaceEffect(effect.name, effect);
                        else
                        {
                            att.replaceEffect(effect.name, effect);
                            def.replaceEffect(effect.name, effect);
                        }

                        if (effect.condition != "")
                        {
                            if (effect.originID == def.playerID)
                            {
                                ArrayList enableSkills = def.enableSkills.ContainsKey(effect.condition) ? def.enableSkills[effect.condition] : new ArrayList();
                                enableSkills.Add(effect);
                                def.enableSkills.Add(effect.condition, enableSkills);
                            }
                            else if (effect.originID == att.playerID)
                            {
                                ArrayList enableSkills = att.enableSkills.ContainsKey(effect.condition) ? att.enableSkills[effect.condition] : new ArrayList();
                                enableSkills.Add(effect);
                                att.enableSkills.Add(effect.condition, enableSkills);
                            }
                        }
                        effect.parseAtomics(att, def);

                    }
                    else if (effect.name == "RemoveAllAbnormal")
                    {
                        // remove All abnormal

                        NewCharacterStatus att = logic.getStatusByPlayerID(turn);

                        foreach (string status in att.character.abDic.Keys)
                        {// luu y danh gia
                            AbnormalStatus ab = att.character.abDic[status];
                            if (att.op_effects.ContainsKey(ab.getName()))
                            {
                                att.removeEffect(ab.getName(), 3 - attStatus.playerID);
                            }
                        }
                    }

                }
            }

            inResult = new InTurnResult(states);

            status = Status.IN_TURN_RENDER;
            Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave", "inTurnResult = ");
            foreach (State state in inResult.states)
            {
                Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave", "state = " + state.toJSON().ToString());
            }

            NewCharacterStatus one = logic.getStatusByPlayerID(1);
            NewCharacterStatus two = logic.getStatusByPlayerID(2);
            one.conManager.parseToDict(data["p1_conditions"] as string);
            two.conManager.parseToDict(data["p2_conditions"] as string);
        }

        public void ConfirmGameDataInTurn()
        {
            messageID++;
            string type = IN_TURN_CONFIRM;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            sendingQueue.Add(new Pac(type, data));
            Log("confirm", "create msg " + messageID + " " + type);
        }

        public void ConfirmGameDataBeginTurn()
        {
            messageID++;
            string type = BEGIN_TURN_CONFIRM;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            sendingQueue.Add(new Pac(type, data));
            Log("confirm", "create msg " + messageID + " " + type);
        }

        public void ConfirmGameDataEndTurn()
        {
            messageID++;
            string type = END_TURN_CONFIRM;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);

            sendingQueue.Add(new Pac(type, data));

            Log("confirm", "create msg " + messageID + " " + type);
        }



        public void SendSkillListInTurn(ArrayList skills)
        {
            messageID++;
            string type = IN_TURN_SKILLS;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);

            byte[] bytes = null;
            foreach (int skill in skills)
            {
                bytes = bytes == null ? Utilities.convertToByteArr((ushort)skill) : Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((ushort)skill));
            }
            if (bytes != null)
            {
                Log("sendSkills byte", "" + bytes.Length);
                data.Add("skills", Utilities.convertByteArrToString(bytes));
            }

            sendingQueue.Add(new Pac(type, data));
            Log("sendSkill", "create msg " + messageID + " " + type);
        }

        // request from Master to Slave

        public void ConfirmRollDiceOfMaster()
        {
            messageID++;
            string type = ROLL_DICE_CONFIRM_MATSER;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            sendingQueue.Add(new Pac(type, data));
            Log("confirm RollDice Of Master", "create msg " + messageID + " " + type);
        }


        public void SendGameDataInTurn()
        {
            messageID++;
            string type = IN_TURN_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            byte[] bytes = null;
            for (int i = 0; i < inResult.states.Count; i++)
            {
                bytes = ((State)inResult.states[i]).convertToByteArr();
                Log("state_Byte_length", "" + bytes.Length);
                data.Add("state_" + i, Utilities.convertByteArrToString(bytes));
            }

            data.Add("turn", attStatus.playerID);


            bytes = null;
            if (selectedSkills != null)
            foreach (int skill in selectedSkills)
            {
                bytes = bytes == null ? Utilities.convertToByteArr((ushort)skill) : Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((ushort)skill));
            }
            if (bytes != null)
            {
                Log("sendSkills byte", "" + bytes.Length);
                data.Add("skills", Utilities.convertByteArrToString(bytes));
            }

            NewCharacterStatus one = logic.getStatusByPlayerID(1);
            NewCharacterStatus two = logic.getStatusByPlayerID(2);
            data.Add("p1_conditions", one.conManager.convertToString());
            data.Add("p2_conditions", two.conManager.convertToString());

            sendingQueue.Add(new Pac(type, data));
            Log("send In Turn", "create msg " + messageID + " " + type + " " + inResult.states.Count + " " + data.Keys.Count);
        }

        public void SendGameDataBeginTurn()
        {
            messageID++;
            string type = BEGIN_TURN_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            for (int i = 0; i < beginResult.states.Count; i++)
                data.Add("state_" + i, Utilities.convertByteArrToString(((State)beginResult.states[i]).convertToByteArr()));


            data.Add("turn", attStatus.playerID);
            data.Add("continued", beginResult.continued);
            sendingQueue.Add(new Pac(type, data));
            Log("send Begin Turn", "create msg " + messageID + " " + type);
        }

        private void ParseBeginTurnData(MyDictionary<string, object> data)
        {
            if (status != Status.BEGIN_TURN_WAITING && status != Status.INIT_GAME_WAITING)
            {
                Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave", "BEGIN_TURN_REQUEST is'nt on time " + status.ToString());
                return;
            }
            if (status == Status.INIT_GAME_WAITING)
            {
                hideWaitingDialog();
            }
            messageID = Convert.ToInt32(data["messageID"]) - 1;
            ConfirmGameDataBeginTurn();


            int i = -1;
            int turn = Convert.ToByte(data["turn"]);
            bool continued = Convert.ToBoolean(data["continued"]);
            ArrayList states = new ArrayList();
            byte[] bytes = null;
            while (true)
            {
                i++;
                if (data.ContainsKey("state_" + i))
                {
                    bytes = Utilities.convertStringToByteArr(data["state_" + i] as string);
                    //bytes = data["state_" + i] as byte[];
                    State state = State.parseBytes(bytes);
                    states.Add(state);
                    // state o day khong mang effect gi moi ngoai healthChanges
                    // cap nhat healthAtt
                    logic.getStatusByPlayerID(turn).curIndexes.Add(Indexes.hp_na, turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                    logic.getStatusByPlayerID(turn).midIndexes.Add(Indexes.hp_na, turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                    logic.getStatusByPlayerID(3 - turn).curIndexes.Add(Indexes.hp_na, 3 - turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                    logic.getStatusByPlayerID(3 - turn).midIndexes.Add(Indexes.hp_na, 3 - turn == 1 ? state.healthPlayer1 : state.healthPlayer2);
                }
                else
                {
                    i--;
                    Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave BEGIN_TURN_REQUEST", "Number of States " + (i + 1));
                    break;
                }
            }

            beginResult = new BeginTurnResult(turn, continued);
            beginResult.states = states;
            status = Status.BEGIN_TURN_RENDER;
            Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave", "beginTurn " + beginResult.continued);
            foreach (State sate in beginResult.states)
            {
                Log("", "state =" + sate.toJSON().ToString());
            }
            //begin
            logic.beginTurnWithoutLogic(attStatus, defStatus);
        }

        private void ParseEndTurnData(MyDictionary<string, object> data)
        {
            if (status == Status.INIT_GAME_WAITING)
            {
                hideWaitingDialog();
            }
            messageID = Convert.ToInt32(data["messageID"]) - 1;
            ConfirmGameDataEndTurn();

            int result = Convert.ToSByte(data["result"]);
            int i = -1;
            ArrayList endReleased = new ArrayList();
            byte[] bytes = null;
            while (true)
            {

                i++;
                if (data.ContainsKey("effect_" + i))
                {
                    bytes = Utilities.convertStringToByteArr(data["effect_" + i] as string);
                    //bytes = data["effect_" + i] as byte[];
                    NewEffect effect = NewEffect.parseBytes(bytes);
                    endReleased.Add(effect);
                }
                else
                {
                    i--;
                    Log("Number of effects", "" + (i + 1));
                    break;
                }


            }

            int turn = Convert.ToByte(data["turn"]);

            NewCharacterStatus att = logic.getStatusByPlayerID(turn);
            NewCharacterStatus def = logic.getStatusByPlayerID(3 - turn);
            Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "You") + "InSlave", "removeEffect = ");
            foreach (NewEffect effect in endReleased)
            {
                if (effect.playerID == att.playerID) attStatus.removeEffect(effect.name, effect.originID);
                else if (effect.playerID == defStatus.playerID) defStatus.removeEffect(effect.name, effect.originID);
                else
                {
                    attStatus.removeEffect(effect.name, effect.originID);
                    defStatus.removeEffect(effect.name, effect.originID);
                }
                Log("", "effect = " + effect.toJSON().ToString());
            }
            endResult = new EndTurnResult(result, endReleased);
            status = Status.END_TURN_RENDER;
            logic.endTurnWithoutLogic(attStatus, defStatus);
            NewCharacterStatus one = logic.getStatusByPlayerID(1);
            NewCharacterStatus two = logic.getStatusByPlayerID(2);
            one.conManager.parseToDict(data["p1_conditions"] as string);
            two.conManager.parseToDict(data["p2_conditions"] as string);
        }

        public void SendGameDataEndTurn()
        {
            messageID++;
            string type = END_TURN_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);

            for (int i = 0; i < endResult.releasedState.Count; i++)
                data.Add("effect_" + i, Utilities.convertByteArrToString(((NewEffect)endResult.releasedState[i]).convertToByteArr()));

            //data.Add("states", endResult.releasedState);
            data.Add("turn", attStatus.playerID);
            data.Add("result", endResult.combatResult);

            NewCharacterStatus one = logic.getStatusByPlayerID(1);
            NewCharacterStatus two = logic.getStatusByPlayerID(2);
            data.Add("p1_conditions", one.conManager.convertToString());
            data.Add("p2_conditions", two.conManager.convertToString());
            sendingQueue.Add(new Pac(type, data));
            Log("send End Turn", "create msg " + messageID + " " + type);
        }

        private void ParseRestoringData(MyDictionary<string, object> data)
        {
            if (status == Status.INIT_GAME_WAITING)
            {
                hideWaitingDialog();
            }

            // messageID
            // turnID
            // masterID
            messageID = Convert.ToInt32(data["messageID"]) - 1;


            int firstID = Convert.ToByte(data["firstID"]);
            int turn = Convert.ToByte(data["turnID"]);
            int masterID = Convert.ToByte(data["masterID"]);
            fixInfo(firstID, masterID, turn);
            Log("Restoring PacketOf" + (isMyTurn(attStatus) ? "Me" : "Your") + "InSlave+++++++++++++++++++++++++++whenFix" + " myID = " + getMyID() + " nowturn = " + attStatus.playerID, "tai sao nhi 3");
            // sau do khoi phuc lai effect
            // clear Attatus
            // clear DefStatus
            NewCharacterStatus master = logic.getStatusByPlayerID(masterID);
            NewCharacterStatus slave = logic.getStatusByPlayerID(3 - masterID);

            logic.getStatusByPlayerID(1).clear();
            logic.getStatusByPlayerID(2).clear();


            int i = -1;
            ArrayList list = new ArrayList();
            byte[] bytes = null;
            while (true)
            {
                i++;
                if (data.ContainsKey("effect_" + i))
                {
                    //List<byte> bys = data["effect_" + i] as List<byte>;
                    bytes = Utilities.convertStringToByteArr(data["effect_" + i] as string);

                    //bytes = data["effect_" + i] as byte[];
                    list.Add(NewEffect.parseBytes(bytes));
                }
                else
                {
                    i--;
                    Log("Number of effects", "" + (i + 1));
                    break;

                }

            }

            foreach (NewEffect effect in list)
            {
                int originID = effect.originID;// gay ra boi cai gi de doi sanh enemy cua atomic
                int playerID = effect.playerID;// xem tac dong va nhet vao dau
                NewCharacterStatus one = logic.getStatusByPlayerID(1);
                NewCharacterStatus two = logic.getStatusByPlayerID(2);


                // them cac atmic truoc
                foreach (AtomicEffect atomic in effect.atomicEffects)
                {
                    bool enemy = atomic.enemy;
                    NewCharacterStatus targetStatus = null;
                    if (enemy)
                    {
                        targetStatus = one.playerID == originID ? two : one;
                    }
                    else
                    {
                        targetStatus = one.playerID == originID ? one : two;
                    }
                    ArrayList atomics = targetStatus.atomics.ContainsKey(atomic.index) ? targetStatus.atomics[atomic.index] : new ArrayList();
                    atomics.Add(atomic);
                    targetStatus.atomics.Add(atomic.index, atomics);
                    if (atomic.condition != "")
                    {
                        ArrayList enableAtomics = targetStatus.enableAtomics.ContainsKey(atomic.condition) ? targetStatus.enableAtomics[atomic.condition] : new ArrayList();
                        enableAtomics.Add(atomic);
                        targetStatus.enableAtomics.Add(atomic.condition, enableAtomics);
                    }

                }
                // them cac danh sach effect co condition
                if (effect.condition != "")
                {
                    if (effect.originID == one.playerID)
                    {
                        ArrayList enableSkills = one.enableSkills.ContainsKey(effect.condition) ? one.enableSkills[effect.condition] : new ArrayList();
                        enableSkills.Add(effect);
                        one.enableSkills.Add(effect.condition, enableSkills);
                    }
                    else if (effect.originID == two.playerID)
                    {
                        ArrayList enableSkills = two.enableSkills.ContainsKey(effect.condition) ? two.enableSkills[effect.condition] : new ArrayList();
                        enableSkills.Add(effect);
                        two.enableSkills.Add(effect.condition, enableSkills);
                    }


                }

                if (effect.playerID == 1) one.replaceEffect(effect.name, effect);
                else if (effect.playerID == 2) two.replaceEffect(effect.name, effect);
                else
                {
                    one.replaceEffect(effect.name, effect);
                    two.replaceEffect(effect.name, effect);
                }

                // xet RemoevAllNormal Status
                // parse Special Atomic
                effect.parseAtomics(logic.getStatusByPlayerID(originID), logic.getStatusByPlayerID(3 - originID));
                // removeAllNormalStatus
                if (effect.name == "RemoveAllAbnormal")
                {// remove het tren phien ban nay

                    NewCharacterStatus attStatus = logic.getStatusByPlayerID(originID);
                    foreach (string status in attStatus.character.abDic.Keys)
                    {// luu y danh gia
                        AbnormalStatus ab = attStatus.character.abDic[status];
                        if (attStatus.op_effects.ContainsKey(ab.getName()))
                        {
                            attStatus.removeEffect(ab.getName(), 3 - attStatus.playerID);
                        }
                    }
                }

            }

            // masterCondition
            master.conManager.parseToDict(data["masterCondition"] as string);
            // slaveCondition
            slave.conManager.parseToDict(data["slaveCondition"] as string);

            Log("Restoring PacketOf" + (isMyTurn(attStatus) ? "Me" : "Your") + "InSlave+++++++++++++++++++++++++++parseEffect", "2");
            Status masterStatus = Utilities.ParseEnum<Status>(data["masterStatus"] as string);
            Log("Restoring PacketOf" + (isMyTurn(attStatus) ? "Me" : "Your") + "InSlave+++++++++++++++++++++++++++getMasterStatus " + masterStatus.ToString(), "tai sao nhi 4");

            // load Skill lai skill me and you trong do co the khac nhau trong qua trinh reconnect lai
            you.newSkillDic.Clear();
            me.newSkillDic.Clear();

            // cooldown master = you
            if (data.ContainsKey("masterCooldown"))
            {
                string bys = data["masterCooldown"] as string;
                bytes = Utilities.convertStringToByteArr(bys);
                for (i = 0; i < bytes.Length / 3; i++)
                {
                    byte[] part = Utilities.SubArray<byte>(bytes, 3 * i, 2);
                    ushort skillID = BitConverter.ToUInt16(part, 0);

                    part = Utilities.SubArray<byte>(bytes, 3 * i + 2, 1);
                    byte cooldown = part[0];
                    you.newSkillDic.Add("Skill" + skillID, skills["Skill" + skillID].clone());
                    NewSkill skill = you.newSkillDic["Skill" + skillID];
                    skill.setCoolDown(cooldown);

                }
            }

            // cooldown slave = me
            if (data.ContainsKey("slaveCooldown"))
            {
                string bys = data["slaveCooldown"] as string;
                bytes = Utilities.convertStringToByteArr(bys);
                for (i = 0; i < bytes.Length / 3; i++)
                {
                    byte[] part = Utilities.SubArray<byte>(bytes, 3 * i, 2);
                    ushort skillID = BitConverter.ToUInt16(part, 0);

                    part = Utilities.SubArray<byte>(bytes, 3 * i + 2, 1);
                    byte cooldown = part[0];
                    me.newSkillDic.Add("Skill" + skillID, skills["Skill" + skillID].clone());
                    NewSkill skill = me.newSkillDic["Skill" + skillID];
                    skill.setCoolDown(cooldown);

                }
            }



            switch (masterStatus)
            {
                case Status.ROLL_DICE_WAITING://
                    int dice1 = Convert.ToByte(data["dice1"]);
                    int dice2 = Convert.ToByte(data["dice2"]);
                    int dice3 = Convert.ToByte(data["dice3"]);
                    diceResult = new RollDiceResult(dice1, dice2, dice3);
                    beginResult = new BeginTurnResult(turn, true);
                    logic.beginTurnWithoutLogic(attStatus, defStatus);
                    status = Status.ROLL_DICE_RENDER;
                    ConfirmRollDiceOfSlave();
                    break;
                case Status.BEGIN_TURN_WAITING:
                    status = Status.BEGIN_TURN_RENDER;
                    bool continued = Convert.ToBoolean(data["continued"]);
                    beginResult = new BeginTurnResult(turn, continued);
                    logic.beginTurnWithoutLogic(attStatus, defStatus);
                    ConfirmGameDataBeginTurn();
                    break;
                case Status.IN_TURN_WAITING:
                    inResult = null;
                    logic.beginTurnWithoutLogic(attStatus, defStatus);
                    status = Status.IN_TURN_RENDER;
                    ConfirmGameDataInTurn();
                    break;
                case Status.END_TURN_WAITING:
                    int result = Convert.ToInt32(data["result"]);
                    endResult = new EndTurnResult(result, new ArrayList());
                    status = Status.END_TURN_RENDER;
                    logic.beginTurnWithoutLogic(attStatus, defStatus);
                    logic.endTurnWithoutLogic(attStatus, defStatus);

                    ConfirmGameDataEndTurn();
                    break;
            }
            isReadyToNewPac = true;
            Log("processPacketOf" + (isMyTurn(attStatus) ? "Me" : "Your") + "InSlave", "restoring data " + status.ToString());
        }

        public void SendRestoringData()
        {
            messageID++;
            string type = RESTORING_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            // turnID
            // masterID
            data.Add("firstID", (byte)firstID);
            data.Add("turnID", (byte)attStatus.playerID);
            data.Add("masterID", (byte)me.playerId);

            data.Add("masterStatus", status.ToString());

            if (status == Status.ROLL_DICE_WAITING)
            {
                data.Add("dice1", (byte)diceResult.dice1);
                data.Add("dice2", (byte)diceResult.dice2);
                data.Add("dice3", (byte)diceResult.dice3);
            }

            if (status == Status.BEGIN_TURN_WAITING)
            {
                data.Add("continued", beginResult.continued);
            }

            if (status == Status.END_TURN_WAITING)
            {
                data.Add("result", endResult.combatResult);
            }

            // conditions
            NewCharacterStatus master = logic.getStatusByPlayerID(me.playerId);
            NewCharacterStatus slave = logic.getStatusByPlayerID(you.playerId);
            // masterCondition
            data.Add("masterCondition", master.conManager.convertToString());
            // slaveCondition
            data.Add("slaveCondition", slave.conManager.convertToString());
            // effects + op_effects
            ArrayList list = attStatus.getEffectsOfPlayerID(attStatus.playerID);
            list.AddRange(attStatus.getOp_EffectsOfPlayerID(attStatus.playerID));
            list.AddRange(defStatus.getEffectsOfPlayerID(defStatus.playerID));
            list.AddRange(defStatus.getOp_EffectsOfPlayerID(defStatus.playerID));
            list.AddRange(attStatus.getEffectsOfPlayerID(3));
            list.AddRange(attStatus.getOp_EffectsOfPlayerID(3));
            for (int i = 0; i < list.Count; i++)
            {
                NewEffect effect = (NewEffect)list[i];
                data.Add("effect_" + i, Utilities.convertByteArrToString(effect.convertToByteArr()));
            }

            byte[] bytes = null;
            // cooldown Skill of Master
            foreach (string key in me.newSkillDic.Keys)
            {
                NewSkill skill = me.newSkillDic[key];
                byte[] part = Utilities.Concat<byte>(Utilities.convertToByteArr((ushort)skill.getID()), Utilities.convertToByteArr((byte)skill.getCoolDownByTurn()));
                bytes = bytes == null ? part : Utilities.Concat<byte>(bytes, part);
            }
            if (bytes != null)
                data.Add("masterCooldown", Utilities.convertByteArrToString(bytes));
            bytes = null;

            // coolDown Skill of slave
            foreach (string key in you.newSkillDic.Keys)
            {
                NewSkill skill = you.newSkillDic[key];
                byte[] part = Utilities.Concat<byte>(Utilities.convertToByteArr((ushort)skill.getID()), Utilities.convertToByteArr((byte)skill.getCoolDownByTurn()));
                bytes = bytes == null ? part : Utilities.Concat<byte>(bytes, part);
            }
            if (bytes != null)
                data.Add("slaveCooldown", Utilities.convertByteArrToString(bytes));

            sendingQueue.Add(new Pac(type, data));
            Log("send Restore Data", "create msg " + messageID + " " + type);

        }



        public void SendAnswerStatus()
        {
            messageID++;
            string type = ANSWER_STATUS_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            data.Add("masterStatus", status.ToString());
            sendingQueue.Add(new Pac(type, data));
            Log("send Answer Status", "create msg " + messageID + " " + type);
        }


        public void SendStatusConfirm()
        {
            messageID++;
            string type = STATUS_CONFIRM;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            sendingQueue.Add(new Pac(type, data));
            Log("send Status Confirm", "create msg " + messageID + " " + type + " isReadyNewPac" + isReadyToNewPac);
        }

        public void SendRollDice()
        {
            messageID++;
            string type = ROLL_DICE_REQUEST;
            MyDictionary<string, object> data = new MyDictionary<string, object>();
            data.Add("messageID", messageID);
            data.Add("dice1", (byte)diceResult.dice1);
            data.Add("dice2", (byte)diceResult.dice2);
            data.Add("dice3", (byte)diceResult.dice3);
            sendingQueue.Add(new Pac(type, data));
            Log("send RollDice", "create msg " + messageID + " " + type);
        }

        private void parseRollDiceData()
        {

        }


        public override void OnLeavePlayer(bool isMe)
        {
            if (!isMe)
            {
                if (!isMaster)
                {
                    switch (status)
                    {
                        case Status.INIT_GAME:
                        case Status.INIT_GAME_WAITING:
                            status = Status.FAILED_TO_CONNECT;
                            Log("Slave", "Failed to connect master when init");
                            break;

                    }
                }
                else
                {
                    isOffline = true;
                }
            }
        }

        public override void OnSetKeyRoom(bool isMe)
        {
            Log("keyRoom", "isMe " + isMe + " isMaster " + isMaster);
            if (isMe != isMaster)
            {
                if (isMe && !isMaster)
                {// truoc day khong la master gio duoc chuyen thanh master do phia master truoc do bi dut ket noi
                    isMaster = isMe;
                    isOffline = true;
                    if (isMyTurn(attStatus))
                    {
                        switch (status)
                        {
                            case Status.BEGIN_TURN:
                            case Status.BEGIN_TURN_WAITING:
                                status = Status.BEGIN_TURN;
                                break;
                            case Status.ROLL_DICE_WAITING:
                                status = Status.ROLL_DICE_RENDER; // roll dice roi thi chuyen thoi sang render
                                break;

                            case Status.IN_TURN_WAITING:

                                ArrayList actionsHandles = new ArrayList();
                                foreach (int idSkill in selectedSkills)
                                {
                                    ActionHandle action = new ActionHandle(0, idSkill, 0, 0);
                                    actionsHandles.Add(action);
                                }
                                inResult = new InTurnResult(logic.inTurn(actionsHandles));
                                status = Status.IN_TURN;
                                break;
                            case Status.END_TURN_WAITING:
                                status = Status.END_TURN;
                                break;
                            case Status.IN_TURN:
                                // dang dut luc cooldownskill
                                status = Status.IN_TURN;
                                break;

                            case Status.BEGIN_TURN_RENDER:
                                break;
                            case Status.IN_TURN_RENDER:
                                break;
                            case Status.END_TURN_RENDER:
                                break;
                            case Status.ROLL_DICE_RENDER:
                                break;
                        }
                    }
                    else
                    {
                        switch (status)
                        {
                            case Status.BEGIN_TURN_WAITING:
                                status = Status.BEGIN_TURN;
                                break;
                            case Status.ROLL_DICE_WAITING:
                                status = Status.ROLL_DICE; // tu roll_Dice trong vong UpdateYouInMaster
                                break;
                            case Status.IN_TURN_WAITING:
                                status = Status.IN_TURN; // tu chon skill va tinh logic trong UpdateYouInMaster
                                break;
                            case Status.END_TURN_WAITING:
                                status = Status.END_TURN;
                                break;
                            case Status.BEGIN_TURN_RENDER:
                                break;
                            case Status.IN_TURN_RENDER:
                                break;
                            case Status.END_TURN_RENDER:
                                break;
                            case Status.ROLL_DICE_RENDER:
                                break;
                        }
                    }
                }
                else
                {// dang la master bi xet thanh slave
                    //isMaster = false;
                    //isOffline = true;
                    // bi dut sex phai gui lai cau lenh reconnect
                }
            }
        }

        public override bool isDisconnected()
        {
            return false;
        }

        public override void OnCmdVariable(string type, MyDictionary<string, object> data)
        {
            receivePacket(type, data);
        }

        public void Log(string log)
        {
            Console.WriteLine(log);
        }

        public override void Log(string tag, string status)
        {
            Console.WriteLine(tag + " : " + status);
        }


        public override void LeaveGameRoom()
        {
            Console.WriteLine("levelRoom : master =" + isMaster + " ");
        }

        public override void showWaitingDialog()
        {
            Log("WaitingDialog", "Show");
        }

        public override void hideWaitingDialog()
        {
            Log("WaitingDialog", "Hide");
        }
    }


}
