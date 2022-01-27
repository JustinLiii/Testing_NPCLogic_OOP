using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum Decision
    {
        ToEat = 0,
        ToLearn = 1,
        ToPlay = 2,
        ToSocial = 3,
        ToSleep = 4,
        ToExercise = 5,
        Stop = 6
    }

    public enum Status
    {
        Eating = 0,
        Learning = 1,
        Playing = 2,
        Socializing = 3,
        Sleeping = 4,
        Exercising = 5,
        Stopping = 6
    }

    //饥饿值
    //学习压力
    //娱乐需求
    //社交需求
    //休息需求
    //运动需求
    private int _hungry;
    private int _stress;
    private int _entertainNeed;
    private int _socialNeed;
    private int _tired;
    private int _sportNeed;
    private Status status;
    private Decision decision;
    public bool UseJobs;//负优化，不要开

    void MakeDecision()
    {
        //先后顺序：吃、睡、上课、运动、摸鱼、外出社交
        //默认为摸鱼
        if(status == Status.Stopping)
        {
            if (_hungry - (_stress + _entertainNeed + _socialNeed + _tired + _sportNeed) * 0.04 > 30)
            {
                decision = Decision.ToEat;
            }
            else if (_tired - (_hungry + _stress + _entertainNeed + _socialNeed + _sportNeed) * 0.06 > 30)
            {
                decision = Decision.ToSleep;
            }
            else if (_stress - (_hungry + _entertainNeed + _socialNeed + _tired + _sportNeed) * 0.1 > 50)
            {
                decision = Decision.ToLearn;
            }
            else if (_sportNeed - (_hungry + _stress + _entertainNeed + _socialNeed + _tired) * 0.2 > 50)
            {
                decision = Decision.ToExercise;
            }
            else if (_entertainNeed - (_hungry + _stress + _socialNeed + _tired + _sportNeed) * 0.2 > 50)
            {
                decision = Decision.ToPlay;
            }
            else if (_entertainNeed * 0.3 + _socialNeed + 0.7 - (_hungry + _stress + _tired + _sportNeed) * 0.2 > 50)
            {
                decision = Decision.ToSocial;
            }
            else
            {
                decision = Decision.ToPlay;
            }
        }
        else
        {
            switch (status)
            {
                case Status.Eating:
                    if (_hungry - (_stress + _tired + _entertainNeed + _socialNeed + _sportNeed) * 0.06 < 20)
                    {
                        decision = Decision.Stop;
                    }
                    break;
                case Status.Sleeping:
                    if (_tired - (_hungry + _stress + _entertainNeed + _socialNeed + _sportNeed) * 0.06 < 10)
                    {
                        decision = Decision.Stop;
                    }
                    break;
                case Status.Learning:
                    if (_stress - (_hungry + _tired + _entertainNeed + _socialNeed + _sportNeed) * 0.1 < 20)
                    {
                        decision = Decision.Stop;
                    }
                    break;
                case Status.Exercising:
                    if (_sportNeed - (_hungry + _stress + _tired + _entertainNeed + _socialNeed) * 0.1 < 40)
                    {
                        decision = Decision.Stop;
                    }
                    break;
                case Status.Playing:
                    if (_entertainNeed - (_hungry + _stress + _tired + _socialNeed + _sportNeed) * 0.2 < 40)
                    {
                        decision = Decision.Stop;
                    }
                    break;
                case Status.Socializing:
                    if (_socialNeed * 0.7 + _entertainNeed * 0.3 - (_hungry + _stress + _tired + _sportNeed) * 0.2 < 40)
                    {
                        decision = Decision.Stop;
                    }
                    break;
            }
        }
    }

    void PerformDecision()
    {
        status = decision switch
        {
            Decision.Stop => Status.Stopping,
            Decision.ToEat => Status.Eating,
            Decision.ToSleep => Status.Sleeping,
            Decision.ToLearn => Status.Learning,
            Decision.ToExercise => Status.Exercising,
            Decision.ToPlay => Status.Playing,
            Decision.ToSocial => Status.Socializing,
            _ => status
        };
    }

    void UpdateProperties()
    {
        //基础消耗
        if (_hungry < 99)
        {
            _hungry += 2;
        }
        if (_tired < 99)
        {
            _tired += 2;
        }
        if (_stress < 99)
        {
            _stress += 2;
        }
        if (_sportNeed < 99)
        {
            _sportNeed += 2;
        }
        if (_entertainNeed < 99)
        {
            _entertainNeed += 2;
        }
        if (_socialNeed < 99)
        {
            _socialNeed += 2;
        }

        switch (status)
        {
            case Status.Eating:
                _hungry -= 11;
                if (_hungry < 0)
                {
                    _hungry = 0;
                }
                break;
            case Status.Sleeping:
                _tired -= 6;
                if (_tired < 0)
                {
                    _tired = 0;
                }
                break;
            case Status.Learning:
                _stress -= 9;
                if (_stress < 0)
                {
                    _stress = 0;
                }
                break;
            case Status.Exercising:
                _sportNeed -= 7;
                if (_sportNeed < 0)
                {
                    _sportNeed = 0;
                }
                break;
            case Status.Playing:
                _entertainNeed -= 11;
                if (_entertainNeed < 0)
                {
                    _entertainNeed = 0;
                }
                break;
            case Status.Socializing:
                _entertainNeed -= 3;
                _socialNeed -= 11;
                if (_entertainNeed < 0)
                {
                    _entertainNeed = 0;
                }
                if (_socialNeed < 0)
                {
                    _socialNeed = 0;
                }
                break;
        }
    }

    // void LogicThreadJobs()
    // {
    //     while (true)
    //     {
    //         UpdateProperties();
    //         MakeDecision();
    //         PerformDecision();
    //         Thread.Sleep(1000);
    //     }
    // }
        
    // Start is called before the first frame update
    void Start()
    {
        _hungry = Random.Range(0, 100);
        _stress = Random.Range(0, 100);
        _entertainNeed = Random.Range(0, 100);
        _socialNeed = Random.Range(0, 100);
        _tired = Random.Range(0, 100);
        _sportNeed = Random.Range(0, 100);
        status = Status.Stopping;
        MakeDecision();
        //Thread updateThread = new Thread(new ThreadStart(LogicThreadJobs));
        //updateThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        NativeArray<int> result = new NativeArray<int>(6,Allocator.TempJob);
        NativeArray<Status> resultStatus = new NativeArray<Status>(1, Allocator.TempJob);
        NativeArray<Decision> resultDecision = new NativeArray<Decision>(1, Allocator.TempJob);
        if (!UseJobs)
        {
            UpdateProperties();
            MakeDecision();
            PerformDecision();
        }
        else
        {
            //Debug.LogError("Use Jobs");
            NPCUpdateJob npcUpdateJob = new NPCUpdateJob();
            npcUpdateJob.SetPropreties(_hungry,_stress,_tired,_entertainNeed,_socialNeed,_sportNeed,status,decision);
            npcUpdateJob.result = result;
            npcUpdateJob.resultDecision = resultDecision;
            npcUpdateJob.resultStatus = resultStatus;
            npcUpdateJob.Schedule().Complete();
            _hungry = npcUpdateJob.result[0];
            _stress = npcUpdateJob.result[1];
            _tired = npcUpdateJob.result[2];
            _entertainNeed = npcUpdateJob.result[3];
            _socialNeed = npcUpdateJob.result[4];
            _sportNeed = npcUpdateJob.result[5];
            decision = resultDecision[0];
            status = resultStatus[0];
        }
        result.Dispose();
        resultDecision.Dispose();
        resultStatus.Dispose();
    }

    public struct NPCUpdateJob : IJob
    {
        private int _hungry;
        private int _stress;
        private int _tired;
        private int _entertainNeed;
        private int _socialNeed;
        private int _sportNeed;
        public Status status;
        public Decision decision;
        public NativeArray<Decision> resultDecision;
        public NativeArray<Status> resultStatus;
        public NativeArray<int> result;

        public void SetPropreties(int hungry, int stress, int tired, int entertainNeed, int socialNeed, int sportNeed, Status status, Decision decision)
        {
            _hungry = hungry;
            _stress = stress;
            _tired = tired;
            _entertainNeed = entertainNeed;
            _socialNeed = socialNeed;
            _sportNeed = sportNeed;
            this.status = status;
            this.decision = decision;
        }
        void UpdateProperties()
        {
            //基础消耗
            if (_hungry < 99)
            {
                result[0] = _hungry + 2;
            }
            if (_tired < 99)
            {
                result[2] = _tired + 2;
            }
            if (_stress < 99)
            {
                result[1] = _stress + 2;
            }
            if (_sportNeed < 99)
            {
                result[5] = _sportNeed + 2;
            }
            if (_entertainNeed < 99)
            {
                result[3] = _entertainNeed + 2;
            }
            if (_socialNeed < 99)
            {
                result[4] = _socialNeed + 2;
            }

            switch (status)
            {
                case Status.Eating:
                    result[0] = _hungry - 11;
                    if (_hungry < 0)
                    {
                        result[0] = 0;
                    }
                    break;
                case Status.Sleeping:
                    result[2] = _tired - 6;
                    if (_tired < 0)
                    {
                        result[2] = 0;
                    }
                    break;
                case Status.Learning:
                    result[1] = _stress - 9;
                    if (_stress < 0)
                    {
                        result[1] = 0;
                    }
                    break;
                case Status.Exercising:
                    result[5]  = _sportNeed - 7;
                    if (_sportNeed < 0)
                    {
                        result[5] = 0;
                    }
                    break;
                case Status.Playing:
                    result[3] = _entertainNeed - 11;
                    if (_entertainNeed < 0)
                    {
                        result[3] = 0;
                    }
                    break;
                case Status.Socializing:
                    result[3] = _entertainNeed - 3;
                    result[4] = _socialNeed - 11;
                    if (_entertainNeed < 0)
                    {
                        result[3] = 0;
                    }
                    if (_socialNeed < 0)
                    {
                        result[4] = 0;
                    }
                    break;
            }
        }
        void MakeDecision()
        {
            //先后顺序：吃、睡、上课、运动、摸鱼、外出社交
            //默认为摸鱼
            if(status == Status.Stopping)
            {
                if (_hungry - (_stress + _entertainNeed + _socialNeed + _tired + _sportNeed) * 0.04 > 30)
                {
                    resultDecision[0] = Decision.ToEat;
                }
                else if (_tired - (_hungry + _stress + _entertainNeed + _socialNeed + _sportNeed) * 0.06 > 30)
                {
                    resultDecision[0] = Decision.ToSleep;
                }
                else if (_stress - (_hungry + _entertainNeed + _socialNeed + _tired + _sportNeed) * 0.1 > 50)
                {
                    resultDecision[0] = Decision.ToLearn;
                }
                else if (_sportNeed - (_hungry + _stress + _entertainNeed + _socialNeed + _tired) * 0.2 > 50)
                {
                    resultDecision[0] = Decision.ToExercise;
                }
                else if (_entertainNeed - (_hungry + _stress + _socialNeed + _tired + _sportNeed) * 0.2 > 50)
                {
                    resultDecision[0] = Decision.ToPlay;
                }
                else if (_entertainNeed * 0.3 + _socialNeed + 0.7 - (_hungry + _stress + _tired + _sportNeed) * 0.2 > 50)
                {
                    resultDecision[0] = Decision.ToSocial;
                }
                else
                {
                    resultDecision[0] = Decision.ToPlay;
                }
            }
            else
            {
                switch (status)
                {
                    case Status.Eating:
                        if (_hungry - (_stress + _tired + _entertainNeed + _socialNeed + _sportNeed) * 0.06 < 20)
                        {
                            resultDecision[0] = Decision.Stop;
                        }
                        break;
                    case Status.Sleeping:
                        if (_tired - (_hungry + _stress + _entertainNeed + _socialNeed + _sportNeed) * 0.06 < 10)
                        {
                            decision = Decision.Stop;
                        }
                        break;
                    case Status.Learning:
                        if (_stress - (_hungry + _tired + _entertainNeed + _socialNeed + _sportNeed) * 0.1 < 20)
                        {
                            decision = Decision.Stop;
                        }
                        break;
                    case Status.Exercising:
                        if (_sportNeed - (_hungry + _stress + _tired + _entertainNeed + _socialNeed) * 0.1 < 40)
                        {
                            decision = Decision.Stop;
                        }
                        break;
                    case Status.Playing:
                        if (_entertainNeed - (_hungry + _stress + _tired + _socialNeed + _sportNeed) * 0.2 < 40)
                        {
                            decision = Decision.Stop;
                        }
                        break;
                    case Status.Socializing:
                        if (_socialNeed * 0.7 + _entertainNeed * 0.3 - (_hungry + _stress + _tired + _sportNeed) * 0.2 < 40)
                        {
                            decision = Decision.Stop;
                        }
                        break;
                }
            }
        }
        void PerformDecision()
        {
            resultStatus[0] = decision switch
            {
                Decision.Stop => Status.Stopping,
                Decision.ToEat => Status.Eating,
                Decision.ToSleep => Status.Sleeping,
                Decision.ToLearn => Status.Learning,
                Decision.ToExercise => Status.Exercising,
                Decision.ToPlay => Status.Playing,
                Decision.ToSocial => Status.Socializing,
                _ => status
            };
        }

        public void Execute()
        {
            UpdateProperties();
            MakeDecision();
            PerformDecision();
        }
    }
}
