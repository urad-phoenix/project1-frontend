using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1.Game
{
    public class Recorder
    {
        private int _Order;
        private int _Sequence;
        private List<Round> _Rounds;
        private Round _Current;

        public Recorder()
        {
            _Rounds = new List<Round>();
            _Order = 1;
            _Sequence = 1;
            _SetNewRound();
        }

        public void Next()
        {
            _Sequence++;
            _SetNewRound();
        }

        private void _SetNewRound()
        {
            _Current = new Round(_Sequence);
            _Rounds.Add(_Current);
        }

        public Action GenerateAction()
        {
            var action = new Action(_Sequence ,_Order);
            _Current.Actions.Add(action);
            _Order++;
            return action;
        }

       /* public BattleResult GenerateResult()
        {
            var result = new BattleResult();
            var actions = from round in _Rounds
                        from action in round.Actions
                        select action;
            result.Actions = actions.ToArray();
            return result;
        }*/
        
    }
}