using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_FillAmount")]
public struct PlayerHpBarComponent : IComponentData
{
    public float FillAmount;
}
