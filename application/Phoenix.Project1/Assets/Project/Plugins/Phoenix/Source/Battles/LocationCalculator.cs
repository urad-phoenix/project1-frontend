using System;

namespace Phoenix.Project1.Battles
{
    internal class LocationCalculator
    {
        readonly int _YCount;
        public LocationCalculator(int y_count)
        {
            _YCount = y_count;
        }
        internal int FrontSignle(int location)
        {
            int targetFrontY = 1;
            if (_IsLeft(location))
                targetFrontY = 2;

            int y = (location-1) / _YCount;
            int dis = targetFrontY - y;
            return (location + dis * _YCount) ;
        }

        private bool _IsLeft(int location)
        {
            return location >= 1 && location <= 6;
                
        }

        internal int BackSignle(int location)
        {
            int targetFrontY = 0;
            if (_IsLeft(location))
                targetFrontY = 3;

            int y = (location - 1) / _YCount;
            int dis = targetFrontY - y;
            return (location + dis * _YCount);
        }

        internal System.Collections.Generic.IEnumerable<int> BackMultiple(int location)
        {            
            if (_IsLeft(location))
                return new []{10,11,12 };
            return new[] { 1, 2, 3};
        }

        
    }
}