using Phoenix.Project1.Common.Battles;
using Phoenix.Project1.Game;
using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    internal class BattleReferee : IStatus
    {        
        private readonly Stage _Stage;
        
        public event System.Action<Common.Battles.Winner> VictoryEvent;
        public event System.Action<Actor> PerformEvent;


        public BattleReferee(Battles.Stage stage)
        {           
            this._Stage = stage;            
        }

        void IStatus.Enter()
        {
            if (_Stage.CheckFinish())
                return;
            if (_CheckVictory())
                return;
            
            if (_CheckPerformr())
                return;
        }

        private bool _CheckPerformr()
        {


            

            PerformEvent(_Stage.NextPerformer());
            return true;
        }

        private bool _CheckVictory()
        {
            if(_Stage.Attacker.Actors.All(a => !a.IsDamageable() ))
            {
                VictoryEvent(Common.Battles.Winner.Defenecer);
                return true;
            }
            if (_Stage.Defender.Actors.All(a => !a.IsDamageable()))
            {
                VictoryEvent(Common.Battles.Winner.Attacker);

                return true;
            }
            return false;
        }

        void IStatus.Leave()
        {            
        }

        void IStatus.Update()
        { 
        }
    }
}