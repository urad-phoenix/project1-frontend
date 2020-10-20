namespace Phoenix.Playables
{    
    public enum ActionType
    {
        Skill,
    }

    public enum PlayableType
    {
        None,
        Building,
        Sorting,
        Play,
        Playing,
        OnFinished,
        Finished        
    }
    
    public enum BindingTrackType
    {
        Actor,
        Scene,
        Target,
        None
    }
    
    
    public enum BindingCategory
    {
        Animator,
        Camera,
        Transform,
        GameObject,
        Event,
        Material,
        VFX,
        Director,
        UIVFX,
        BuffVFX,
        Audio,
        Max
    }
    
    public enum AnimationCategory
    {
        Character,
        Camera    
    }
    
    public enum EventType
    {
        Hit,
        Health,
        Buff,
        TimelineEnd
    }

    public enum SkillEventType
    {
        Skill,
        End
    }
    
    public enum DiedPlayType
    {
        Wait,
        Play,
        Playing,
        Finished
    }

    public enum RevivalPlayType
    {
        Wait,
        Play,
        Playing,
        Finished
    }
}
