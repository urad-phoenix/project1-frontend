﻿using Phoenix.Project1.Common.Battles;
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

        readonly CircularQueue<Actor> _ActorCircular;

        public BattleReferee(Battles.Stage stage)
        {           
            this._Stage = stage;
            var actors = stage.Attacker.Actors.Union(stage.Defender.Actors);
            _ActorCircular = new CircularQueue<Actor>(actors);
        }

        void IStatus.Enter()
        {
            if (_CheckVictory())
                return;
            
            if (_CheckPerformr())
                return;
        }

        private bool _CheckPerformr()
        {


            Actor actor;
            do
            {
                actor = _ActorCircular.GetCurrentAndNext();
            }
            while (actor.Hp == 0);

            PerformEvent(actor);
            return true;
        }

        private bool _CheckVictory()
        {
            if(_Stage.Attacker.Actors.All(a => a.Hp == 0))
            {
                VictoryEvent(Common.Battles.Winner.Defenecer);
                return true;
            }
            if (_Stage.Defender.Actors.All(a => a.Hp == 0))
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