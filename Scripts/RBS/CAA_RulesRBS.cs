using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAA_RulesRBS
{
    public void AddRule(CAA_RuleRBS rule)
    {
        GetRules.Add(rule);
    }

    public List<CAA_RuleRBS> GetRules { get; } = new List<CAA_RuleRBS>();
}
