using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_FillAmount")]
public struct EnemyHpBarComponent : IComponentData
{
    public float FillAmount;
}
