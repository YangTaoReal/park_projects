using QTFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : QTComponent
{
    public UIEntity m_kParentEntity
    {
        get
        {
            return ParentEntity as UIEntity;
        }
    }

    public virtual void TranslateUI()
    {

    }
}
