using System;
using System.Collections.Generic;

public class HololensInputConverter : InputConverter
{
    protected override IDictionary<Type, Type> GetInputConversions()
    {
        return new Dictionary<Type, Type>
        {
            { typeof(HitboxPlaceholder), typeof(Hitbox) },
            { typeof(HandDraggablePlaceholder), typeof(MyHandDraggable) },
            { typeof(BillboardPlaceholder), typeof(MyBillboard) },
            { typeof(HandZoomablePlaceholder), typeof(HandZoomable) },
        };
    }
}