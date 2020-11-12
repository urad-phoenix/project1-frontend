using System.Collections.Generic;

namespace Phoenix.Project1.Editors.Tools
{
    public class TimelineOutputData
    {
        public string Key;
        
        public int TotalFrame;

        public List<TimelineHitData> HitDatas;
    }

    public class TimelineHitData
    {
        public string Key;

        public int Frame;
    }
}

