using UnityEngine;

public class DashSkill : Skill
{
    public override void Use()
    {
        base.Use();

        Debug.Log("Created clone behind");
    }
}
