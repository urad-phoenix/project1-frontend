using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using System;

namespace Phoenix.Project1.Battles
{
    internal class BattleTime : IBattleTime
    {
        readonly TimePusher _Pusher;
        readonly TimeCounter _Counter;

        readonly Property<int> _Frames;
        public BattleTime()
        {
            _Counter = new TimeCounter();
            _Pusher = new TimePusher();
            _Frames = new Property<int>();
        }
        int IBattleTime.Frame => _Frames;

        public Property<int> Frames => _Frames;

        int IBattleTime.Advance()
        {
            var frames = _Pusher.Advance(_Counter.Second);
            _Counter.Reset();
            _Frames.Value += frames;
            
            return frames;
        }

        internal void Reset()
        {
            _Counter.Reset();
            _Frames.Value = 0;
        }
    }
}
