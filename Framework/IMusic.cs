namespace SharpFAI.Framework;

/// <summary>
/// Interface for music/audio playback management
/// 音乐/音频播放管理接口
/// </summary>
public interface IMusic
{
    /// <summary>
    /// Load audio file / 加载音频文件
    /// </summary>
    /// <param name="path">Path to audio file / 音频文件路径</param>
    void Load(string path);
    
    /// <summary>
    /// Play the music / 播放音乐
    /// </summary>
    void Play();
    
    /// <summary>
    /// Pause the music / 暂停音乐
    /// </summary>
    void Pause();
    
    /// <summary>
    /// Stop the music / 停止音乐
    /// </summary>
    void Stop();
    
    /// <summary>
    /// Resume the music / 恢复音乐
    /// </summary>
    void Resume();
    
    /// <summary>
    /// Seek to a specific position / 跳转到指定位置
    /// </summary>
    /// <param name="position">Position in seconds / 位置（秒）</param>
    void Seek(double position);
    
    /// <summary>
    /// Get current playback position in seconds / 获取当前播放位置（秒）
    /// </summary>
    double Position { get; }
    
    /// <summary>
    /// Get total duration in seconds / 获取总时长（秒）
    /// </summary>
    double Duration { get; }
    
    /// <summary>
    /// Music volume (0.0 - 1.0) / 音乐音量（0.0 - 1.0）
    /// </summary>
    float Volume { get; set; }
    
    /// <summary>
    /// Music pitch multiplier / 音乐音调倍数
    /// </summary>
    float Pitch { get; set; }
    
    /// <summary>
    /// Whether the music is playing / 音乐是否正在播放
    /// </summary>
    bool IsPlaying { get; }
    
    /// <summary>
    /// Whether the music is paused / 音乐是否暂停
    /// </summary>
    bool IsPaused { get; }
    
    /// <summary>
    /// Whether the music is looping / 音乐是否循环
    /// </summary>
    bool IsLooping { get; set; }
    
    /// <summary>
    /// Update music state / 更新音乐状态
    /// </summary>
    void Update();
    
    /// <summary>
    /// Dispose music resources / 释放音乐资源
    /// </summary>
    void Dispose();
}