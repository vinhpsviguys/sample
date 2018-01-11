

using System.Collections;

namespace CoreLib
{
    public class BeginTurnResult
    {
        public int attackerID;
        public bool continued;
        public ArrayList states = new ArrayList();


        public BeginTurnResult(int id, bool con)
        {
            attackerID = id;
            continued = con;
        }
    }
}
