using System.Numerics;

namespace SharpFAI.Framework;

public interface IPlanets
{
    public void Render(IShader shader,ICamera camera);
    public void MoveTo(Vector2 target);
    public interface ITail
    {
        public void Render();
    }
}