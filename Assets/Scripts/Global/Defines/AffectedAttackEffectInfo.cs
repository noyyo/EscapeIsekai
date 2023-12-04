public enum AttackEffectTypes
{
    None,
    KnockBack,
    Airborne,
    Stun,
}

public class AffectedAttackEffectInfo
{
    private int canAffectedFlag;
    public static readonly int NoneFlag = 1 << 0;
    public static readonly int KnockBackFlag = 1 << 1;
    public static readonly int AirborneFlag = 1 << 2;
    public static readonly int StunFlag = 1 << 3;
    public static readonly int AllFlag = -1;
    public static readonly int ZeroFlag = 0;

    public void SetFlag(AttackEffectTypes attackEffectType, bool flag)
    {
        if (flag)
        {
            canAffectedFlag += 1 << (int)attackEffectType;
        }
        else
        {
            canAffectedFlag -= 1 << (int)attackEffectType;
        }
    }
    /// <summary>
    /// AffectedEffectInfo�� Flag�� �̿��ؼ� ��Ʈ�������� �� ���� Setting�� �����մϴ�.
    /// ������ Flag������ ������ϴ�.
    /// </summary>
    public void SetFlag(int flags)
    {
        canAffectedFlag = flags;
    }
    public bool CanBeAffected(AttackEffectTypes attackEffectType)
    {
        return (canAffectedFlag & 1 << (int)attackEffectType) == 1 << (int)attackEffectType;
    }
    /// <summary>
    /// �������� �÷��׸� ��� �����ϴ��� ���θ� �����մϴ�.
    /// </summary>
    public bool CanBeAffected(int flags)
    {
        return (canAffectedFlag & flags) == flags;
    }

}
