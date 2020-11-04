using Phoenix.Project1.Common.Battles;
using Regulus.Utility;
using System;

namespace Phoenix.Project1.Battles
{
    internal class BattleTime : IBattleTime
    {
        readonly TimePusher _Pusher;
        readonly TimeCounter _Counter;
        int _Frames;
        public BattleTime()
        {
            _Counter = new TimeCounter();
            _Pusher = new TimePusher();
        }
        int IBattleTime.Frame => _Frames;
        

        int IBattleTime.Advance()
        {
            var frames = _Pusher.Advance(_Counter.Second);
            _Frames += frames;
            _Counter.Reset();
            return frames;
        }
    }
}
