

//public enum TYPEACTION
//{
//    NONE=0,
//    NORMALATTACK=1,
//    SKILL=2,
//}

public class ActionHandle {

    //public int idAction = -1;
    public int _typeAction= 0;
    public int idSkill = -1;
    public float timeAction = 2f;
    public int index = -1;
    //public CharacterPlayer target;

    public ActionHandle(int _type, int _idSkill, float _timeAction, int _idx)
    {
        this._typeAction = _type;
        this.idSkill = _idSkill;
        this.timeAction = _timeAction;
        //target = _target;
        this.index = _idx;
    }


}
