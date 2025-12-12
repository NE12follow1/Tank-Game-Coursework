using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAA_RuleRBS
{
    public string antecedentA;
    public string antecedentB;
    public Type consequentState;
    public Predicate compare;
    public enum Predicate { And, Or, nAnd }

    public CAA_RuleRBS(string antecedentA, string antecedentB, Type consequentState, Predicate compare)
    {
        this.antecedentA = antecedentA;
        this.antecedentB = antecedentB;
        this.consequentState = consequentState;
        this.compare = compare;
    }

    public Type CheckRule(Dictionary<string, bool> stats)
    {
        bool antecedentABool = stats[antecedentA];
        bool antecedentBBool = stats[antecedentB];

        switch (compare)
        {
            case Predicate.And:
                if (antecedentABool && antecedentBBool)
                {
                    return consequentState;
                }
                else
                {
                    return null;
                }
            case Predicate.Or:
                if (antecedentABool || antecedentBBool)
                {
                    return consequentState;
                }
                else
                {
                    return null;
                }
            case Predicate.nAnd:
                if (!antecedentABool && !antecedentBBool)
                {
                    return consequentState;
                }
                else
                {
                    return null;
                }
            default:
                return null;
        }
    }
}
