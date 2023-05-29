using Godot;

namespace FirstGodotGame.Util;

public static class Utility
{
    public static Vector2 GetPointAlongPerimeter(this Rect2 rect, float t)
    {
        float section = t * 4;
        int index = Mathf.FloorToInt(section);
        section %= 1;

        switch (index)
        {
            case 0:
                return new Vector2(Mathf.Lerp(rect.Position.X, rect.End.X, section), rect.Position.Y);
            case 1:
                return new Vector2(rect.End.X, Mathf.Lerp(rect.Position.Y, rect.End.Y, section));
            case 2:
                return new Vector2(Mathf.Lerp(rect.End.X, rect.Position.X, section), rect.End.Y);
            case 3:
                return new Vector2(rect.Position.X, Mathf.Lerp(rect.End.Y, rect.Position.Y, section));
        }

        return rect.Position;
    }

    public static void Normalize(this ref Vector2 vec)
    {
        vec = vec.Normalized();
    }
}
