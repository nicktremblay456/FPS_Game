using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [Serializable]
    public class SoundBank
    {
        public string name;
        public AudioClip[] clips;
    }

    [Serializable]
    public class MaterialAudioOverride
    {
        public Material[] materials;
        public SoundBank[] banks;
    }

    public bool randomizePitch = false;
    public float pitchRandomRange = 0.2f;
    public float playDelay = 0f;
    public SoundBank defaultBank = new SoundBank();
    public MaterialAudioOverride[] overrides;

    [HideInInspector] public bool playing;
    [HideInInspector] public bool canPlay;

    protected AudioSource m_AudioSource;
    protected Dictionary<Material, SoundBank[]> m_Lookup = new Dictionary<Material, SoundBank[]>();

    public AudioSource audioSource
    {
        get => m_AudioSource;
    }

    public AudioClip clip { get; private set; }

    private void Awake() 
    {
        m_AudioSource = GetComponent<AudioSource>();
        for (int i = 0; i < overrides.Length; i++)
        {
            foreach(Material material in overrides[i].materials)
            {
                m_Lookup[material] = overrides[i].banks;
            }
        }
    }

    public AudioClip PlayRandomClip(Material overrideMaterial, int bankId = 0)
    {
        if (overrideMaterial == null)
        {
            return null;
        }
        return InternalPlayRandomClip(overrideMaterial, bankId);
    }

    public void PlayRandomClip()
    {
        clip = InternalPlayRandomClip(null, bankId: 0);
    }

    private AudioClip InternalPlayRandomClip(Material overrideMaterial, int bankId)
    {
        SoundBank[] banks = null;
        SoundBank bank = defaultBank;
        if (overrideMaterial != null)
        {
            if (m_Lookup.TryGetValue(overrideMaterial, out banks))
            {
                if (bankId < banks.Length)
                {
                    bank = banks[bankId];
                }
            }
        }
        if (bank.clips == null)
        {
            return null;
        }

        AudioClip clip = bank.clips[Random.Range(0, bank.clips.Length)];

        if (clip == null)
        {
            return null;
        }

        m_AudioSource.pitch = randomizePitch ? Random.Range(1.0f - pitchRandomRange, 1.0f + pitchRandomRange) : 1.0f;
        m_AudioSource.clip = clip;
        m_AudioSource.PlayDelayed(playDelay);

        return clip;
    }
}