namespace SharpFAI.Framework;

public interface IPlayer
{
    public void CreatePlayer();
    public void UpdatePlayer();
    public void RenderPlayer();
    public void MoveToNextFloor(Floor next);
    public void StartPlay();
    public void StopPlay();
    public void PausePlay();
    public void ResumePlay();
    public void ResetPlayer();
}