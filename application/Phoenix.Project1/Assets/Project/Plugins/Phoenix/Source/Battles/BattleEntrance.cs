using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Phoenix.Project1.Battles
{
    internal class BattleEntrance : IStatus
    {                
        int _WaitFrame;
        private readonly IBattleTime _Time;
        private readonly Stage _Stage;

        public BattleEntrance(IBattleTime time,  Stage stage)
        {
                    
            this._Time = time;
            this._Stage = stage;
        }

        public event System.Action<int> DoneEvent;
        public event System.Action<ActorEntranceTimestamp> EntranceEvent;


        void IStatus.Enter()
        {
            int maxFrames = 0;
            var actors = new List<ActorEntrance>();
            foreach (var actor in _Stage.Attacker.Actors.Union(_Stage.Defender.Actors))
            {
                actors.Add(new ActorEntrance { Id = actor.Id });
                Common.Battles.Motion motion = actor.GetMotion(MotionType.Entrance);
                if (maxFrames < motion.Frames)
                {
                    maxFrames = motion.Frames;
                }
            }
            _WaitFrame = maxFrames;
            EntranceEvent(new ActorEntranceTimestamp { Frames = _Time.Frame, ActorEntrances = actors.ToArray() });            
        }

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            _Advance();
        }
        void _Advance()
        {
            var frames = _Time.Advance();
            _WaitFrame -= frames;
            if (_WaitFrame <= 0)
                DoneEvent(Math.Abs(_WaitFrame));
        }

    }
}