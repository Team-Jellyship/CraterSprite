using Godot;

namespace CraterSprite.DebugHelpers;

public class Drawing
{
    private const float ArrowheadAngleOffsetRads = 2.5f;
    private const float ArrowheadLength = 5.0f;

    public static void DrawArrow(CanvasItem item, Vector2 position, Vector2 direction, float length, Color color)
    {
        var arrowEnd = position;
        arrowEnd.X += direction.X * length;
        arrowEnd.Y += direction.Y * length;
            

        var arrowDirection = Mathf.Atan2(direction.Y, direction.X);
        var arrowRight = arrowEnd;
        var arrowLeft = arrowEnd;
        arrowRight.X += Mathf.Cos(arrowDirection + ArrowheadAngleOffsetRads) * ArrowheadLength;
        arrowRight.Y += Mathf.Sin(arrowDirection + ArrowheadAngleOffsetRads) * ArrowheadLength;
        arrowLeft.X  += Mathf.Cos(arrowDirection - ArrowheadAngleOffsetRads) * ArrowheadLength;
        arrowLeft.Y  += Mathf.Sin(arrowDirection - ArrowheadAngleOffsetRads) * ArrowheadLength;
            
        item.DrawLine(position, arrowEnd, color);
        item.DrawLine(arrowEnd, arrowRight, color);
        item.DrawLine(arrowEnd, arrowLeft, color);
    }

    public static void DrawArrow(CanvasItem item, Vector2 start, Vector2 end, Color color)
    {
        var arrowDirection = Mathf.Atan2(end.Y - start.Y, end.X - start.X);
        var arrowRight = end;
        var arrowLeft = end;
        arrowRight.X += Mathf.Cos(arrowDirection + ArrowheadAngleOffsetRads) * ArrowheadLength;
        arrowRight.Y += Mathf.Sin(arrowDirection + ArrowheadAngleOffsetRads) * ArrowheadLength;
        arrowLeft.X  += Mathf.Cos(arrowDirection - ArrowheadAngleOffsetRads) * ArrowheadLength;
        arrowLeft.Y  += Mathf.Sin(arrowDirection - ArrowheadAngleOffsetRads) * ArrowheadLength;
            
        item.DrawLine(start, end, color);
        item.DrawLine(end, arrowRight, color);
        item.DrawLine(end, arrowLeft, color);
    }
}