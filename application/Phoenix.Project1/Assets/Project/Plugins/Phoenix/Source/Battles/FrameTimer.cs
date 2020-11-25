using System;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    internal class FrameTimer
    {
        readonly System.Collections.Generic.List<Tuple<int, Action>> _Actions;
        public FrameTimer()
        {
            _Actions = new System.Collections.Generic.List<Tuple<int, Action>>();
        }
        internal void Register(int frame, Action action)
        {
            _Actions.Add(new Tuple<int, Action>(frame, action));
        }

        internal void Run(int current_frames)
        {
            var frameActions = from a in _Actions where a.Item1 == current_frames select a;
            foreach (var frameAction in frameActions.ToArray())
            {
                frameAction.Item2();
                _Actions.Remove(frameAction);
            }

            
        }
    }
}