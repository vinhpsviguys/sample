

using System.Collections;


namespace CoreLib
{
    public class EndTurnResult
    {
        public int combatResult;
        public ArrayList releasedState;

        public EndTurnResult(int result, ArrayList state)
        {
            this.combatResult = result;
            this.releasedState = state;
        }
    }
}
