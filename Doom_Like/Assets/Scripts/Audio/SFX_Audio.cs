using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Audio : PoolableObject
{
    private AudioSource m_AudioSource;
    private float m_Timer;

    protected override void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    protected override void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_LifeTime)
        {
            ClearObject();
        }
    }

    public void SetUp(AudioClip clip)
    {
        m_AudioSource.clip = clip;
        m_LifeTime = clip.length;
    }

    public void Play()
    {
        if (m_AudioSource.clip == null)
        {
            Debug.LogError("You need to set up the audio before playing it");
            return;
        }

        m_AudioSource.Play();
    }

    public override void OnDespawn()
    {
        m_AudioSource.clip = null;
        m_Timer = 0;
        m_LifeTime = 0;
    }
}