namespace SharpFAI.Framework;

public interface IPlayer
{
    public void CreatePlayer();
    public void UpdatePlayer(double delta);
    public void RenderPlayer(double delta);
    public void MoveToNextFloor(Floor next);
    public void StartPlay();
    public void StopPlay();
    public void PausePlay();
    public void ResumePlay();
    public void ResetPlayer();
    public void DestroyPlayer();
}