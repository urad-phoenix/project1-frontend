using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public class TweenFade : UIFadeBase
    {
        public DOTweenAnimation[] Anims;

        [SerializeField]
        private string _FadeId = "Fade";

        private List<DOTweenAnimation> _Dotweens;

        private void Start()
        {
            CheckDotween();
        }

        private void CheckDotween()
        {
            if(_Dotweens == null)
            {
                _Dotweens = new List<DOTweenAnimation>();

                if(Anims != null && Anims.Length != 0)
                {
                    foreach(var anim in Anims)
                    {
                        _Dotweens.AddRange(anim.GetComponents<DOTweenAnimation>().ToList());
                    }
                }
            }
        }

        public override IObservable<Unit> FadeIn()
        {            
            gameObject.SetActive(true);

            CheckDotween();

            return Observable.Create<Unit>(observer =>
            {
                Observable.WhenAll(_Dotweens.Select(x => x.PlayForwardByIdAsObservable(_FadeId)))
                .Subscribe(x =>
                {
                   observer.OnNext(Unit.Default);
                   observer.OnCompleted();                     
                })
                .AddTo(this);

                return Disposable.Empty;
            });
        }

        public override IObservable<Unit> FadeOut()
        {
            CheckDotween();

            return Observable.Create<Unit>(observer =>
            {
                Observable.WhenAll(_Dotweens.Select(x => x.PlayBackwardsByIdAsObservable(_FadeId)))                                                               
                .Subscribe(x =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();

                    gameObject.SetActive(!_IsFinishedDisable);
                })
                .AddTo(this);

                return Disposable.Empty;
            });
        }
    }

    public static class DotweenExtensions
    {
        public static IObservable<DOTweenAnimation> PlayAllByIdAsObservable(this DOTweenAnimation animtion, string id, bool rewind = false)
        {
            return Observable.Create<DOTweenAnimation>(observer =>
            {
                if(animtion.id == id)
                {
                    if(rewind)
                        animtion.DORewind();

                    animtion.DOPlayAllById(id);

                    animtion.tween.OnComplete(() =>
                    {
                        observer.OnNext(animtion);
                        observer.OnCompleted();
                    });
                }
                else
                {
                    observer.OnNext(animtion);
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            });
        }

        public static IObservable<DOTweenAnimation> RestartAsObservable(this DOTweenAnimation animtion, bool fromHere = false)
        {
            return Observable.Create<DOTweenAnimation>(observer =>
            {
                animtion.DORestart();

                animtion.tween.OnComplete(() =>
                {
                    observer.OnNext(animtion);
                    observer.OnCompleted();
                });             

                return Disposable.Empty;
            });          
        }

        public static IObservable<DOTweenAnimation> RestartByIdAsObservable(this DOTweenAnimation animtion, string id)
        {
            return Observable.Create<DOTweenAnimation>(observer =>
            {
                if(animtion.id == id)
                {
                    animtion.DORestartById(id);

                    animtion.tween.OnComplete(() =>
                    {
                        observer.OnNext(animtion);
                        observer.OnCompleted();
                    });
                }
                else
                {
                    observer.OnNext(animtion);
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            });           
        }

        public static IObservable<DOTweenAnimation> RestartAllByIdAsObservable(this DOTweenAnimation animtion, string id)
        {
            return Observable.Create<DOTweenAnimation>(observer =>
            {
                if(animtion.id == id)
                {
                    animtion.DORestartAllById(id);

                    animtion.tween.OnComplete(() =>
                    {
                        observer.OnNext(animtion);
                        observer.OnCompleted();
                    });
                }
                else
                {
                    observer.OnNext(animtion);
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            });          
        }

        public static IObservable<DOTweenAnimation> PlayForwardByIdAsObservable(this DOTweenAnimation animtion, string id)
        {
            return Observable.Create<DOTweenAnimation>(observer =>
            {
                if(animtion.id == id)
                {

                    animtion.DOPlayForwardById(id);

                    animtion.tween.OnComplete(() =>
                    {
                        observer.OnNext(animtion);
                        observer.OnCompleted();
                    });
                }
                else
                {
                    observer.OnNext(animtion);
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            });
        }

        public static IObservable<DOTweenAnimation> PlayBackwardsByIdAsObservable(this DOTweenAnimation animtion, string id)
        {
            return Observable.Create<DOTweenAnimation>(observer =>
            {
                if(animtion.id == id)
                {

                    animtion.DOPlayBackwardsById(id);

                    animtion.tween.OnComplete(() =>
                    {
                        observer.OnNext(animtion);
                        observer.OnCompleted();
                    });
                }
                else
                {
                    observer.OnNext(animtion);
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            });          
        }
    }
}
