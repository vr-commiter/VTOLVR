using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using TrueGearSDK;
using System.Linq;
using UnityEngine;


namespace MyTrueGear
{
    public class TrueGearMod
    {
        private static TrueGearPlayer _player = null;

        private static ManualResetEvent lowheartbeatMRE = new ManualResetEvent(false);
        private static ManualResetEvent midheartbeatMRE = new ManualResetEvent(false);
        private static ManualResetEvent fastheartbeatMRE = new ManualResetEvent(false);
        private static ManualResetEvent engineshockMRE = new ManualResetEvent(false);
        private static ManualResetEvent surfaceshockMRE = new ManualResetEvent(false);


        

        public void LowHeartBeat()
        {
            while(true)
            {
                lowheartbeatMRE.WaitOne();
                _player.SendPlay("HeartBeat");
                Thread.Sleep(1000);
            }            
        }

        public void MidHeartBeat()
        {
            while (true)
            {
                midheartbeatMRE.WaitOne();
                _player.SendPlay("HeartBeat");
                Thread.Sleep(600);
            }
        }

        public void FastHeartBeat()
        {
            while (true)
            {
                fastheartbeatMRE.WaitOne();
                _player.SendPlay("HeartBeat");
                Thread.Sleep(400);
            }
        }

        public void EngineShock()
        {
            while (true)
            {
                engineshockMRE.WaitOne();
                Debug.Log("---------------------------------------");
                Debug.Log("EngineShock");
                _player.SendPlay("EngineShock");
                Thread.Sleep(2000);                
            }
        }

        public void SurfaceShock()
        {
            while (true)
            {
                surfaceshockMRE.WaitOne();
                Debug.Log("---------------------------------------");
                Debug.Log("SurfaceShock");
                _player.SendPlay("SurfaceShock");
                Thread.Sleep(1500);
                
            }
        }

        public TrueGearMod() 
        {
            _player = new TrueGearPlayer("667970","VoltVR");
            _player.PreSeekEffect("DefaultDamage");
            _player.PreSeekEffect("PlayerBulletDamage");
            _player.Start();
            new Thread(new ThreadStart(this.LowHeartBeat)).Start();
            new Thread(new ThreadStart(this.MidHeartBeat)).Start();
            new Thread(new ThreadStart(this.FastHeartBeat)).Start();
            new Thread(new ThreadStart(this.EngineShock)).Start();
            new Thread(new ThreadStart(this.SurfaceShock)).Start();
        }  

        public void Play(string Event)
        { 
            _player.SendPlay(Event);
        }

        public void PlayAngle(string tmpEvent, float tmpAngle, float tmpVertical)
        {
            try
            {
                float angle = (tmpAngle - 22.5f) > 0f ? tmpAngle - 22.5f : 360f - tmpAngle;
                int horCount = (int)(angle / 45) + 1;

                int verCount = tmpVertical > 0.1f ? -4 : tmpVertical < 0f ? 8 : 0;

                EffectObject oriObject = _player.FindEffectByUuid(tmpEvent);
                EffectObject rootObject = EffectObject.Copy(oriObject);

                foreach (TrackObject track in rootObject.trackList)
                {
                    if (track.action_type == ActionType.Shake)
                    {
                        for (int i = 0; i < track.index.Length; i++)
                        {
                            if (verCount != 0)
                            {
                                track.index[i] += verCount;
                            }
                            if (horCount < 8)
                            {
                                if (track.index[i] < 50)
                                {
                                    int remainder = track.index[i] % 4;
                                    if (horCount <= remainder)
                                    {
                                        track.index[i] = track.index[i] - horCount;
                                    }
                                    else if (horCount <= (remainder + 4))
                                    {
                                        var num1 = horCount - remainder;
                                        track.index[i] = track.index[i] - remainder + 99 + num1;
                                    }
                                    else
                                    {
                                        track.index[i] = track.index[i] + 2;
                                    }
                                }
                                else
                                {
                                    int remainder = 3 - (track.index[i] % 4);
                                    if (horCount <= remainder)
                                    {
                                        track.index[i] = track.index[i] + horCount;
                                    }
                                    else if (horCount <= (remainder + 4))
                                    {
                                        var num1 = horCount - remainder;
                                        track.index[i] = track.index[i] + remainder - 99 - num1;
                                    }
                                    else
                                    {
                                        track.index[i] = track.index[i] - 2;
                                    }
                                }
                            }
                        }
                        if (track.index != null)
                        {
                            track.index = track.index.Where(i => !(i < 0 || (i > 19 && i < 100) || i > 119)).ToArray();
                        }
                    }
                    else if (track.action_type == ActionType.Electrical)
                    {
                        for (int i = 0; i < track.index.Length; i++)
                        {
                            if (horCount <= 4)
                            {
                                track.index[i] = 0;
                            }
                            else
                            {
                                track.index[i] = 100;
                            }
                            if (horCount == 1 || horCount == 8 || horCount == 4 || horCount == 5)
                            {
                                track.index = new int[2] { 0, 100 };
                            }
                        }
                    }
                }
                _player.SendPlayEffectByContent(rootObject);
            }
            catch(Exception ex)
            { 
                Debug.Log("TrueGear Mod PlayAngle Error :" + ex.Message);
                _player.SendPlay(tmpEvent);
            }          
        }


        public void StartLowHeartBeat()
        {
            lowheartbeatMRE.Set();
        }

        public void StopLowHeartBeat()
        {
            lowheartbeatMRE.Reset();
        }

        public void StartMidHeartBeat()
        {
            midheartbeatMRE.Set();
        }

        public void StopMidHeartBeat()
        {
            midheartbeatMRE.Reset();
        }

        public void StartFastHeartBeat()
        {
            fastheartbeatMRE.Set();
        }

        public void StopFastHeartBeat()
        {
            fastheartbeatMRE.Reset();
        }

        public void StartEngineShock()
        {
            engineshockMRE.Set();
        }

        public void StopEngineShock()
        {
            engineshockMRE.Reset();
        }

        public void StartSurfaceShock()
        {
            surfaceshockMRE.Set();
        }

        public void StopSurfaceShock()
        {
            surfaceshockMRE.Reset();
        }

    }
}
