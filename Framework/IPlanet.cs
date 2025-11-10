using System.Drawing;
using System.Numerics;

namespace SharpFAI.Framework;

/// <summary>
/// Interface for a single planet
/// 单个行星的接口
/// </summary>
public interface IPlanet
{
    /// <summary>
    /// Planet position / 行星位置
    /// </summary>
    Vector2 Position { get; set; }
    
    /// <summary>
    /// Planet radius / 行星半径
    /// </summary>
    float Radius { get; set; }
    
    /// <summary>
    /// Planet color / 行星颜色
    /// </summary>
    Color Color { get; set; }
    
    /// <summary>
    /// Planet rotation angle / 行星旋转角度
    /// </summary>
    float Rotation { get; set; }
    
    /// <summary>
    /// Tail attached to this planet / 附加到此行星的尾迹
    /// </summary>
    ITail Tail { get; }
    
    /// <summary>
    /// Update planet state / 更新行星状态
    /// </summary>
    void Update(float deltaTime);
    
    /// <summary>
    /// Render planet / 渲染行星
    /// </summary>
    void Render(IShader shader);
    
    /// <summary>
    /// Move planet to target position / 移动行星到目标位置
    /// </summary>
    void MoveTo(Vector2 target);
}

/// <summary>
/// Interface for planet tail
/// 行星尾迹的接口
/// </summary>
public interface ITail
{
    /// <summary>
    /// Maximum tail length / 最大尾迹长度
    /// </summary>
    int MaxLength { get; set; }
    
    /// <summary>
    /// Tail width / 尾迹宽度
    /// </summary>
    float Width { get; set; }
    
    /// <summary>
    /// Tail color / 尾迹颜色
    /// </summary>
    Color Color { get; set; }
    
    /// <summary>
    /// Add a position to the tail / 向尾迹添加位置
    /// </summary>
    void AddPosition(Vector2 position);
    
    /// <summary>
    /// Clear tail / 清除尾迹
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Update tail state / 更新尾迹状态
    /// </summary>
    void Update(float deltaTime);
    
    /// <summary>
    /// Render tail / 渲染尾迹
    /// </summary>
    void Render(IShader shader);
}





