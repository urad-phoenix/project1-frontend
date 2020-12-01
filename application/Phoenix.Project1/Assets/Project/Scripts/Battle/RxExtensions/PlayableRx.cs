using System;
using Phoenix.Project1.Client.Utilities.RxExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
     public class PlayableObservable :  UniRx.Operators.OperatorObservableBase<PlayableDirector>
    {
        private PlayableDirector _PlayableDirector;

        private int _StartFrame;

        private int _EndFrame;

        private int _CurrentFrame;
        
        public PlayableObservable(PlayableDirector value, int startFrame, int endFrame, int currentFrame) : base(false)
        {
            _PlayableDirector = value;

            _StartFrame = startFrame;

            _CurrentFrame = currentFrame;

            _EndFrame = endFrame;
        }

        protected override IDisposable SubscribeCore(IObserver<PlayableDirector> observer, IDisposable cancel)
        {
            return new PlayableObserver(_PlayableDirector, _StartFrame, _EndFrame, _CurrentFrame, observer, cancel).Run();
        }
        
        class PlayableObserver : UniRx.Operators.OperatorObserverBase<PlayableDirector, PlayableDirector>
        {
            readonly CompositeDisposable _CancellationToken = new CompositeDisposable();
            
            bool _IsCompleted;                   
            
            private PlayableDirector _PlayableDirector;
            
            private int _CurrentFrame;
            
            private int _StartFrame;

            private int _EndFrame;

            private int _PlayableTick;

            private bool _IsPlay;
            
            public PlayableObserver(PlayableDirector playableDirector, int startFrame, int endFrame, int currentFrame, IObserver<PlayableDirector> observer, IDisposable cancel) : base(observer, cancel)
            {                
                _PlayableDirector = playableDirector;

                _CurrentFrame = currentFrame;

                _StartFrame = startFrame;

                _EndFrame = endFrame;

                _PlayableTick = 0;
                
                _IsCompleted = false;

                _IsPlay = false;
            }

            private void Update(int frame)
            {              
                if(_IsCompleted)
                    return;

                var currFrame = _CurrentFrame + frame;
                if (!_IsPlay)
                {
                    if (currFrame >= _StartFrame)
                    {
                        _IsPlay = true;
                        _PlayableDirector.initialTime = 0;
                        _PlayableDirector.Play();
                    
                        Debug.Log($"Play : {_StartFrame}  {_EndFrame} {currFrame}");
                    }
                }
                
                if(_IsPlay)
                {
                    _PlayableTick++;
//                    _PlayableDirector.time = _PlayableTick / 30;
                    _PlayableDirector.Evaluate();                  
                    
                    if (_PlayableDirector.state == PlayState.Paused)
                    {
                        observer.OnNext(_PlayableDirector);
                        OnCompleted();
                        Debug.Log($"End Play : {_StartFrame} {_EndFrame} duration {_PlayableDirector.duration}");
                    }                                                                                               
                }

//                    if (frame >= _FrameData.StartFrame)
//                    {                  
//                        //Debug.Log($"ActorObserver {_FrameData.Motion.MotionId} currentFrame {frame}, target frame {_FrameData.StartFrame}");
//                        observer.OnNext(_FrameData.Motion);                        
//                        OnCompleted();
//                    }
            }
           
            public IDisposable Run()
            {                                   
                var scheduling = UniRx.Observable.EveryUpdate()
                    .ObserveOnMainThread().Subscribe(frame => Update((int)frame)).AddTo(_CancellationToken);
                
                return StableCompositeDisposable.Create(_CancellationToken, scheduling);
            }         

            public override void OnNext(PlayableDirector value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try
                {
                    observer.OnError(error);
                    _CancellationToken.Dispose();
                }
                finally
                {
                    Dispose(); 
                    _CancellationToken.Dispose();
                }
            }

            public override void OnCompleted()
            {
                try
                {
                    _IsCompleted = true;

                    observer.OnCompleted();
                    
                    _CancellationToken.Dispose();
                }
                finally
                {
                    Dispose();
                    _CancellationToken.Dispose();
                }
            }
        }          
    }
    
    public static class PlayableRx
    {
        public static IObservable<PlayableDirector> PlayAsObservable(this PlayableDirector director, IDisposable disposable)
        {
            return Observable.Create<PlayableDirector>(obs =>
            {                
                director.Play();
                
                director.stopped += playableDirector =>
                {
                    obs.OnNext(playableDirector);
                    obs.OnCompleted();
                    disposable.Dispose();
                };

                return disposable;
            });
        }  
        
        public static IObservable<PlayableDirector> PlayAsObservable(this PlayableDirector director, int startFrame,
            int endFrame, int currentFrame, IDisposable disposable)
        {
            return new PlayableObservable(director, startFrame, endFrame, currentFrame);
        }
    }
}