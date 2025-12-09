using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CAA_BaseStateFSM
{
    public abstract Type StateUpdate();
    public abstract Type StateEnter();
    public abstract Type StateExit();
}

