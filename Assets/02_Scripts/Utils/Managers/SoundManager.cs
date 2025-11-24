using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Default
{
    public class SoundManager
    {
        public float[] volume = new float[(int)Define.Sound.MaxCOUNT];
        public bool[] isMute = new bool[(int)Define.Sound.MaxCOUNT];
        public readonly AudioSource[] _baseAudioSource = new AudioSource[(int)Define.Sound.MaxCOUNT];
        readonly List<AudioSource> SceneBGMSources = new List<AudioSource>();
        Dictionary<AudioSource,Sequence> AudioSequences = new Dictionary<AudioSource,Sequence>();
        Dictionary<AudioSource,Tween> fadeTweens = new Dictionary<AudioSource,Tween>();
        private Dictionary<AudioSource, bool> pauseCheckDict = new();
        AudioMixer _audioMixer = null;
        private AudioSourceUtil ArenaBGMSource;
        Dictionary<string,AudioClip> loadedClips = new ();
        
        private TA_SceneMusic.SceneBGMInfo _sceneBGMInfo;
        private float SceneBGMVolume;
        private float SFXVolumeForFading;
        private GameObject root;
        
        public void Init()
        {
            if (_audioMixer == null)
            {
                _audioMixer = ResourceUtil.Load<AudioMixer>("SoundMixer");
            }
           
            if (root != null) return;

            root = new GameObject("@Sound");
            Object.DontDestroyOnLoad(root);
            string[] soundNames = Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject(soundNames[i]);
                _baseAudioSource[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }
            _baseAudioSource[(int)Define.Sound.BGM].loop = true;
            for (int i = 0; i < _baseAudioSource.Length; i++)
            {
                _baseAudioSource[i].dopplerLevel = 0;
                _baseAudioSource[i].reverbZoneMix = 0;
            }
           
            _baseAudioSource[(int)Define.Sound.BGM].outputAudioMixerGroup = ResourceUtil.Load<AudioMixerGroup>("SoundMixer[BGM]");
            _baseAudioSource[(int)Define.Sound.SFX].outputAudioMixerGroup = ResourceUtil.Load<AudioMixerGroup>("SoundMixer[SFX]");
            _baseAudioSource[(int)Define.Sound.UI].outputAudioMixerGroup = ResourceUtil.Load<AudioMixerGroup>("SoundMixer[UI]");
            _baseAudioSource[(int)Define.Sound.Ambience].outputAudioMixerGroup = ResourceUtil.Load<AudioMixerGroup>("SoundMixer[Ambience]");
            _baseAudioSource[(int)Define.Sound.Master].outputAudioMixerGroup = ResourceUtil.Load<AudioMixerGroup>("SoundMixer[Master]");

            ChangeVolume(0.5f);
            ChangeVolume(0.5f, Define.Sound.BGM);
            ChangeVolume(0.5f, Define.Sound.UI);
            ChangeVolume(0.5f, Define.Sound.Ambience);
            ChangeVolume(0.5f, Define.Sound.Master);
            loadedClips ??= new();
            SceneBGMVolume = 1;
            _sceneBGMInfo.channel = -1;
            LoadAll("SFX"); // sfx 전부 로드
            LoadAll("BGM"); // bgm 전부 로드
            GameManager.instance.WhenReturnedToTitle.RemoveListener(Reset);
            GameManager.instance.WhenReturnedToTitle.AddListener(Reset);
        }

        private void Reset()
        {
            _sceneBGMInfo.channel = -1;
        }
        Tween DoFadeIn(AudioSource source, float endValue, float fadeTime)
        {
            if (fadeTweens.TryGetValue(source, out var tween))
            {
                tween?.Kill();
            }

            tween = source.DOFade(endValue, fadeTime).SetUpdate(true);;
            fadeTweens.TryAdd(source,tween);
            fadeTweens[source] = tween;
            return tween;
        }

        Tween DoFadeOut(AudioSource source, float fadeTime)
        {
            if (fadeTweens.TryGetValue(source, out var tween))
            {
                tween?.Kill();
            }

            tween = source.DOFade(0, fadeTime).SetUpdate(true);
            fadeTweens.TryAdd(source,tween);
            fadeTweens[source] = tween;
            return tween;
        }

        Sequence GetSequence(AudioSource source)
        {
            Sequence tween = DOTween.Sequence();

            AudioSequences.TryAdd(source,tween);
            AudioSequences[source] = tween;
            return tween;
        }

        void KillSequence(AudioSource source)
        {
            if (AudioSequences.TryGetValue(source, out var tween))
            {
                tween?.Kill();
                AudioSequences.Remove(source);
            }
        }
        AudioSource CreateAudioSource(string sourceName, Define.Sound soundType)
        {
            GameObject go = new GameObject(sourceName);
            var source = go.AddComponent<AudioSource>();
            go.transform.parent = root.transform;
            source.outputAudioMixerGroup = _baseAudioSource[(int)soundType].outputAudioMixerGroup;
            return source;
        }

        void Pause(AudioSource source)
        {
            // AudioSource에 IsPaused 기능이 없어서 추가함
            pauseCheckDict.TryAdd(source, true);
            if (!source.isPlaying)
            {
                pauseCheckDict[source] = false;
            }
            else
            {
                source.Pause();
            }
        }
        
        void UnPause(AudioSource source)
        {
            // Audio Source의 기존 UnPause는 플레이중에 Pause된 경우가 아니면 Clip을 플레이 하지 않음.
            // 그래서 이 경우가 아닐 시, Clip을 플레이 하도록 따로 IsPaused를 추가해서 넣었음
            if (IsPaused(source))
            {
                source.UnPause();
                pauseCheckDict[source] = false;
            }
            // else // 음악이 플레이 되지 않고 있었을 때 UnPause하면 음악이 다시 나오는 경우가 생겨버려서 지움
            // {
            //     source.Play();
            // }
        }
        bool IsPaused(AudioSource source)
        {
            if (pauseCheckDict.TryGetValue(source, out var isPause))
            {
                return isPause;
            }

            return false;
        }

        void LoadAll(string label) // 테스트용
        {
            var list = ResourceUtil.LoadAll<AudioClip>(label);
            list.ForEach(y =>
            {
                loadedClips.TryAdd(y.name, y);
            });
        }
        private string lastPath = "";
        readonly Dictionary<string,CustomQueue<AudioSource>> playingSFXSources = new();
        
        public void Play(string path, Define.Sound type = Define.Sound.SFX,bool change = true)
        {
            if (string.IsNullOrEmpty(path)) return;
            
            if (type == Define.Sound.BGM)
            {
                if (lastPath == path)
                {
                    return;
                }
                lastPath = path;
            }
            if (path.Contains("Sounds/"+type+"/") == false)
            {
                path = $"Sounds/{type}/{path}";
            }

            var audioClip = ResourceUtil.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.LogError($"AudioClip Load Fail : {path}");
                return;
            }
            var audioSourceIndex = (int)type;

            var audioSource = _baseAudioSource[audioSourceIndex];
            if (isMute[audioSourceIndex])
            {
                _audioMixer.SetFloat(type.ToString(), Mathf.Log10(0.001f) * 20);
            }
            else
            {
                _audioMixer.SetFloat(type.ToString(), Mathf.Log10(volume[audioSourceIndex]) * 20);
            }
            if (type == Define.Sound.BGM)
            {
                StopSceneBGM();
                if (audioSource.isPlaying && change)
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.SetUpdate(true);
                    sequence.Append(audioSource.DOFade(0, 1)).AppendCallback(() =>
                    {
                        audioSource.Stop();
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }).Append(audioSource.DOFade(volume[audioSourceIndex], 0.3f));
                }
                else
                {
                    audioSource.volume = 0;
                    audioSource.clip = audioClip;
                    audioSource.Play();
                    audioSource.DOFade(volume[audioSourceIndex], 0.3f).SetUpdate(true);
                }
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

        public void StopSceneBGM()
        {
            _sceneBGMInfo.channel = -1;
            SceneBGMSources?.ForEach(x =>
            {
                if (x.isPlaying)
                {
                    DoFadeOut(x, 1).onComplete += x.Stop;
                }
            });
        }

       
        private int currentIndex;
        
        /// <summary>
        /// 입력한 채널로 변경하고 등록된 sceneBGM 목록 교체 
        /// </summary>
        /// <param name="info"></param>
        public void PlaySceneBGM(TA_SceneMusic.SceneBGMInfo info)
        {
            if (_sceneBGMInfo.channel == info.channel)
            {
                Debug.Log($"{info.channel} 이미 해당 채널입니다.");
                return;
            }

            _sceneBGMInfo = info;
            var audioSource = _baseAudioSource[(int)Define.Sound.BGM];
            audioSource.Stop();

            int count = SceneBGMSources.Count;
            for (int i = count; i < info.clipAddresses.Count; i++)
            {
                var source = CreateAudioSource($"BGMSource[{i + SceneBGMSources.Count + 1}]", Define.Sound.BGM);
                SceneBGMSources.Add(source);
                source.loop = true;
            }

            for (int i = info.clipAddresses.Count; i < count; i++)
            {
                SceneBGMSources[i].clip = null;
                SceneBGMSources[i].Stop();
            }
            for (int i = 0; i < info.clipAddresses.Count; i++)
            {
                string path = info.clipAddresses[i];
                if (path.Contains("Sounds/BGM/") == false)
                {
                    path = $"Sounds/BGM/{path}";
                }
                int temp = i;
                Sequence seq = GetSequence(SceneBGMSources[i]).SetUpdate(true);
                SceneBGMSources[temp].clip = ResourceUtil.Load<AudioClip>(path);

                if (SceneBGMSources[i].isPlaying)
                {
                    DoFadeOut(SceneBGMSources[i], info.fadeOutTime);
                    seq.AppendInterval(info.delay);
                    seq.AppendCallback(() =>
                    {
                        SceneBGMSources[temp].Play();
                    });
                }
                else
                {
                    SceneBGMSources[i].volume = 0;
                    seq.SetDelay(info.delay);
                    seq.AppendCallback(() =>
                    {
                        SceneBGMSources[temp].Play();
                    });
                }
                
                if (i == info.initialNumber)
                {
                    SceneBGMSources[info.initialNumber].volume = 0;
                    seq.Append(DoFadeIn(SceneBGMSources[info.initialNumber], volume[(int)Define.Sound.BGM],
                        info.fadeInTime));
                }

                seq.onKill += () =>
                {
                    SceneBGMSources[info.initialNumber].volume = volume[(int)Define.Sound.BGM];
                };
            }
            currentIndex = info.initialNumber;
        }

        /// <summary>
        /// 등록된 sceneBGM 목록중에 변경
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fadeTime"></param>
        public void SwapSceneBGMWithIndex(int index,float fadeTime)
        {
            if (index == currentIndex)
            {
                Debug.Log("이미 해당 bgm을 실행중입니다");
                return;
            }
            if (index >= SceneBGMSources.Count || index < 0 || SceneBGMSources[index].clip == null)
            {
                Debug.LogError("해당 인덱스만큼 bgm이 추가되지 않았습니다");
                return;
            }

            var previous = SceneBGMSources[currentIndex];
            var next = SceneBGMSources[index];
            
            KillSequence(previous);
            DoFadeOut(previous, fadeTime);
            KillSequence(next);
            DoFadeIn(next, volume[(int)Define.Sound.BGM], fadeTime);
            currentIndex = index;
        }

        public void SetSceneBGMVolume(float _volume,float fadeTime)
        {
            if (SceneBGMSources.Count <= currentIndex) return;
            _volume = Mathf.Clamp(_volume, 0, 1);
            SceneBGMVolume = _volume;
            DoFadeIn(SceneBGMSources[currentIndex], volume[(int)Define.Sound.BGM]* _volume, fadeTime);
        }

        public void PauseSceneBGM(float fadeTime)
        {
            SceneBGMSources.ForEach(x =>
            {
                KillSequence(x);
                DoFadeOut(x, fadeTime).onComplete += () =>
                {
                    Pause(x);
                };
            });
        }

        public void ResumeSceneBGM(float fadeTime)
        {
            if (SceneBGMSources == null || SceneBGMSources.Count == 0) return;
            KillSequence(SceneBGMSources[currentIndex]);

            SceneBGMSources.ForEach(UnPause);
            SceneBGMSources[currentIndex].volume = 0;
            DoFadeIn(SceneBGMSources[currentIndex], volume[(int)Define.Sound.BGM], fadeTime);
        }

        public void PlayArenaBGM(List<string> address, float fadeTime)
        {
            if (address == null || address.Count == 0) return;
            if (ArenaBGMSource == null)
            {
                var source = CreateAudioSource("ArenaBGMSource", Define.Sound.BGM);
                source.loop = false;
                ArenaBGMSource = Utils.GetOrAddComponent<AudioSourceUtil>(source.gameObject);
            }
            PauseSceneBGM(fadeTime);

            CustomQueue<AudioClip> clips = new();
            for (int i = 0; i < address.Count; i++)
            {
                string x = address[i];
                if (x.Contains("Sounds/BGM/") == false)
                {
                    x = $"Sounds/BGM/{x}";
                }
            
                var clip = ResourceUtil.Load<AudioClip>(x);
                clips.Enqueue(clip);
            }
            ArenaBGMSource.AudioSource.volume = 0;
            KillSequence(ArenaBGMSource.AudioSource);
            if (clips.Count == 1)
            {
                ArenaBGMSource.AudioSource.loop = true;
                ArenaBGMSource.AudioSource.clip = clips.Dequeue();
                ArenaBGMSource.Play();
            }
            else
            {
                ArenaBGMSource.Play(AudioSourceUtil.PlayingType.IntroThenLoop,clips,false);
            }
            
            DoFadeIn(ArenaBGMSource.AudioSource, volume[(int)Define.Sound.BGM], fadeTime);
        }

        public void StopArenaBGM(float fadeTime)
        {
            if (ArenaBGMSource == null || !ArenaBGMSource.AudioSource.isPlaying) return;
            KillSequence(ArenaBGMSource.AudioSource);
            DoFadeOut(ArenaBGMSource.AudioSource, fadeTime).onComplete += () =>
            {
                ArenaBGMSource.Stop();
            };
           
            ResumeSceneBGM(fadeTime);
        }

        public void ChangeArenaBGM(List<string> address, float fadeTime)
        {
            if (ArenaBGMSource == null || !ArenaBGMSource.AudioSource.isPlaying) return;
            KillSequence(ArenaBGMSource.AudioSource);
            DoFadeOut(ArenaBGMSource.AudioSource, fadeTime).onComplete += () =>
            {
                ArenaBGMSource.Stop();
            };
            ChangeArenaClip(address);
            
            DoFadeIn(ArenaBGMSource.AudioSource, volume[(int)Define.Sound.BGM], fadeTime);
        }

        public void ChangeArenaClip(List<string> address)
        {
            CustomQueue<AudioClip> clips = new();
            for (int i = 0; i < address.Count; i++)
            {
                string x = address[i];
                if (x.Contains("Sounds/BGM/") == false)
                {
                    x = $"Sounds/BGM/{x}";
                }
            
                var clip = ResourceUtil.Load<AudioClip>(x);
                clips.Enqueue(clip);
            }
            if (clips.Count == 1)
            {
                ArenaBGMSource.AudioSource.loop = true;
                ArenaBGMSource.AudioSource.clip = clips.Dequeue();
                ArenaBGMSource.Play();
            }
            else
            {
                ArenaBGMSource.Play(AudioSourceUtil.PlayingType.IntroThenLoop,clips,false);
            }
        }
        public void PauseArenaBGM(float fadeTime)
        {
            KillSequence(ArenaBGMSource.AudioSource);
            DoFadeOut(ArenaBGMSource.AudioSource, fadeTime).onComplete += () => { Pause(ArenaBGMSource.AudioSource); };
        }

        public void ResumeArenaBGM(float fadeTime)
        {
            if (ArenaBGMSource == null) return;
            KillSequence(ArenaBGMSource.AudioSource);

            ArenaBGMSource.AudioSource.UnPause();
            ArenaBGMSource.AudioSource.volume = 0;
            DoFadeIn(ArenaBGMSource.AudioSource, volume[(int)Define.Sound.BGM], fadeTime);
        }
        public AudioSourceUtil PlayInPosition(string clipAddress,string audioSettingAddress,Vector2 playPos,Define.Sound sound)
        {
            if (string.IsNullOrEmpty(clipAddress)) return null;
            
            playingSFXSources.TryAdd(clipAddress, new ());
            
            var queue = playingSFXSources[clipAddress];
            AudioSource source =
                GameManager.Factory.Get<AudioSource>(FactoryManager.FactoryType.Normal, audioSettingAddress, playPos);
            source.outputAudioMixerGroup = _baseAudioSource[(int)sound].outputAudioMixerGroup;
            queue.Enqueue(source);
            while (queue.Count > 2)
            {
                var temp = queue.Dequeue();
                temp.Stop();
                GameManager.Factory.Return(temp.gameObject);
            }

            var clip = loadedClips[clipAddress];
            source.clip = clip;
            AudioSourceUtil util = Utils.GetOrAddComponent<AudioSourceUtil>(source.gameObject);
            util.OnEnd.AddListener(ReturnSource);
            util.Play();

            void ReturnSource()
            {
                util.Destroy();
                util.OnEnd.RemoveListener(ReturnSource);
            }

            return util;
        }

        public AudioSourceUtil PlayInPosition(string[] clipAddresses, string audioSettingAddress, AudioSourceUtil.PlayingType playingType,Vector2 playPos, bool isLoop, Define.Sound soundType)
        {
            if (clipAddresses == null || clipAddresses.Length == 0) return null;
            
            AudioSourceUtil util = GameManager.Factory.Get<AudioSourceUtil>(FactoryManager.FactoryType.Normal, audioSettingAddress, playPos);
            AudioSource source = util.AudioSource;
            source.outputAudioMixerGroup = _baseAudioSource[(int)soundType].outputAudioMixerGroup;
            
            CustomQueue<AudioClip> clips = new();
            
            clipAddresses.ForEach(x =>
            {
                if (loadedClips.TryGetValue(x, out var clip))
                {
                    clips.Enqueue(clip);
                }
                else
                {
                    Debug.LogError($"{x}가 어드레서블에 SFX로 등록되어있지않음");
                }
            });
            
            util.OnEnd.AddListener(ReturnSource);
            util.Play(playingType, clips,isLoop);
            
            void ReturnSource()
            {
                util.Destroy();
                util.OnEnd.RemoveListener(ReturnSource);
            }
            return util;
        }
        
        readonly Queue<string> reservations = new();

        bool isReserved = false;
        public void ReserveBGM(string path)
        {
            if (path.Contains("Sounds/BGM/") == false)
            {
                path = $"Sounds/BGM/{path}";
            }

            var audioSource = _baseAudioSource[(int)Define.Sound.BGM];
            audioSource.loop = false;

            if (!isReserved)
            {
                GameManager.instance.StartCoroutine(PlayNextBGM(audioSource, path));
                isReserved = true;
            }
            else
            {
                reservations.Enqueue(path);
            }
        }
        IEnumerator PlayNextBGM(AudioSource audio,string path)
        {
            yield return new WaitUntil(() => !audio.isPlaying);
            var audioClip = ResourceUtil.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.LogError($"AudioClip Load Fail : {path}");
                yield break;
            }
            audio.clip = audioClip;
            audio.Play();
            audio.loop = true;

            if (reservations.Count > 0) 
            {
                string str = reservations.Dequeue();
                GameManager.instance.StartCoroutine(PlayNextBGM(audio,str));
            }
            else
            {
                isReserved = false;
            }
        }
        public void PlaySceneChangeSound(string path)
        {
            if (path.Contains("Sounds/") == false)
            {
                path = $"Sounds/{path}";
            }

            var audioClip = ResourceUtil.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.Log($"AudioClip Load Fail : {path}");
                return;
            }
            var audioSource = _baseAudioSource[(int)Define.Sound.SFX];
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private Tween volumeFade;
        public void FadeVolume(float amount, float fadeTime,Define.Sound soundType)
        {
            // log를 이용해서 볼륨 계산을 하기 때문에, 수치가 0이 되면 안됨.

            amount = Mathf.Clamp(amount, 0.01f, 1);
            volumeFade?.Kill();
            volumeFade = _audioMixer.DOSetFloat(soundType.ToString(), Mathf.Log10(amount) * 20, fadeTime).SetAutoKill(false);
            volumeFade.onKill += () =>
            {
                _audioMixer.SetFloat(soundType.ToString(), Mathf.Log10(amount) * 20);
                volumeFade = null;
            };
        }
        public void ChangeVolume(float volume, Define.Sound type = Define.Sound.SFX)
        {
            // log를 이용해서 볼륨 계산을 하기 때문에, 수치가 0이 되면 안됨.
            
            volume = Mathf.Clamp(volume, 0.01f, 1);
            if (!isMute[(int)type])
            {
                _audioMixer.SetFloat(type.ToString(), Mathf.Log10(volume) * 20);
            }
            this.volume[(int)type] = volume;
        }

        public void Mute(bool ismute, Define.Sound type = Define.Sound.SFX)
        {
            isMute[(int)type] = ismute;
            if (ismute)
                _audioMixer.SetFloat(type.ToString(), Mathf.Log10(0.001f) * 20);
            else
                _audioMixer.SetFloat(type.ToString(), Mathf.Log10(volume[(int)type]) * 20);
        }

        public void Stop(Define.Sound type)
        {
            _baseAudioSource[(int)type].Stop();
        }

        public void Stop(Define.Sound type, float fadeTime)
        {
            AudioSource audioSource = _baseAudioSource[(int)type];
            lastPath = "";
            audioSource.DOFade(0, fadeTime).OnComplete(() =>
            {
                audioSource.Stop();
            });
        }

        public void StopAllSounds()
        {
            Stop(Define.Sound.SFX);
            Stop(Define.Sound.BGM,1);
            StopSceneBGM();
            StopArenaBGM(1);
        }
    }
}
