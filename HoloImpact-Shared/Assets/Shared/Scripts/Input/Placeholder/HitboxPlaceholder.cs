using UnityEngine;
using System;

[Serializable]
public class HitboxSettings : IInputSettings
{
    public RegisteredInputs GetInputType()
    {
        return RegisteredInputs.Hitbox;
    }
}

[RequireComponent(typeof(MeshRenderer))]
public class HitboxPlaceholder : InputPlaceholder
{
    public HitboxSettings settings;

    public override IInputSettings GetInputSettings()
    {
        return settings;
    }
}