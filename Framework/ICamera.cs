using System.Numerics;

namespace SharpFAI.Framework;

/// <summary>
/// Interface for camera control
/// 相机控制接口
/// </summary>
public interface ICamera
{
    /// <summary>
    /// Camera position in world space / 相机在世界空间的位置
    /// </summary>
    Vector2 Position { get; set; }
    
    /// <summary>
    /// Camera rotation in degrees / 相机旋转角度
    /// </summary>
    float Rotation { get; set; }
    
    /// <summary>
    /// Camera zoom level / 相机缩放级别
    /// </summary>
    float Zoom { get; set; }
    
    /// <summary>
    /// Get the view matrix / 获取视图矩阵
    /// </summary>
    Matrix4x4 GetViewMatrix();
    
    /// <summary>
    /// Get the projection matrix / 获取投影矩阵
    /// </summary>
    Matrix4x4 GetProjectionMatrix();
    
    /// <summary>
    /// Update camera state / 更新相机状态
    /// </summary>
    void Update(float deltaTime);

    /// <summary>
    /// Set camera to follow a target / 设置相机跟随目标
    /// </summary>
    void SetTarget(Vector2 target);
    
    /// <summary>
    /// Reset camera to default state / 重置相机到默认状态
    /// </summary>
    void Reset();
    
    /// <summary>
    /// Apply screen shake effect / 应用屏幕震动效果
    /// </summary>
    void Shake(float intensity, float duration);
    
    /// <summary>
    /// Convert screen coordinates to world coordinates / 将屏幕坐标转换为世界坐标
    /// </summary>
    Vector2 ScreenToWorld(Vector2 screenPosition);
    
    /// <summary>
    /// Convert world coordinates to screen coordinates / 将世界坐标转换为屏幕坐标
    /// </summary>
    Vector2 WorldToScreen(Vector2 worldPosition);
    
    /// <summary>
    /// Apply camera matrices to shader / 将相机矩阵应用到着色器
    /// </summary>
    /// <param name="shader">Shader to apply matrices to / 要应用矩阵的着色器</param>
    void Render(IShader shader);
    
    /// <summary>
    /// Check if a point is visible in the camera frustum / 检查点是否在相机视锥体内
    /// </summary>
    bool IsPointVisible(Vector2 point);
    
    /// <summary>
    /// Check if a circle is visible in the camera frustum / 检查圆形是否在相机视锥体内
    /// </summary>
    bool IsCircleVisible(Vector2 center, float radius);
    
    /// <summary>
    /// Check if an axis-aligned bounding box is visible in the camera frustum / 检查轴对齐包围盒是否在相机视锥体内
    /// </summary>
    bool IsAABBVisible(Vector2 min, Vector2 max);
    
    /// <summary>
    /// Get the frustum bounds in world space / 获取视锥体在世界空间的边界
    /// </summary>
    void GetFrustumBounds(out Vector2 min, out Vector2 max);
}
