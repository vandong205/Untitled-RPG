
public interface IEnemy
{
    float m_health { get; set; }
    float m_attackDmg { get; set; }
    void TakeDamage(float dmg);
}
