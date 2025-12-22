using System;
using Defines;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SceneMusicAttribute : Attribute
{
    public AudioEnum Music { get; }

    public SceneMusicAttribute(AudioEnum music)
    {
        Music = music;
    }
}