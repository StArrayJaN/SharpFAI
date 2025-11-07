using System.Numerics;

namespace SharpFAI.Framework;

public interface IPlanets
{
    public void Render();
    public void Move(Vector2 target);
}