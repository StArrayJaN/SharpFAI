using System.Drawing;
using System.Numerics;

namespace SharpFAI.Framework;

/// <summary>
/// Interface for mesh data management
/// 网格数据管理接口
/// </summary>
public interface IMesh
{
    /// <summary>
    /// Vertex positions / 顶点位置
    /// </summary>
    Vector3[] Vertices { get; set; }
    
    /// <summary>
    /// Triangle indices / 三角形索引
    /// </summary>
    int[] Indices { get; set; }
    
    /// <summary>
    /// Vertex colors / 顶点颜色
    /// </summary>
    Color[] Colors { get; set; }
    
    /// <summary>
    /// Texture coordinates / 纹理坐标
    /// </summary>
    Vector2[] TexCoords { get; set; }
    
    /// <summary>
    /// Vertex normals / 顶点法线
    /// </summary>
    Vector3[] Normals { get; set; }
    
    /// <summary>
    /// Upload mesh data to GPU / 上传网格数据到GPU
    /// </summary>
    void Upload();
    
    /// <summary>
    /// Update mesh data / 更新网格数据
    /// </summary>
    void UpdateData();
    
    /// <summary>
    /// Bind mesh for rendering / 绑定网格用于渲染
    /// </summary>
    void Bind();
    
    /// <summary>
    /// Unbind mesh / 解绑网格
    /// </summary>
    void Unbind();
    
    /// <summary>
    /// Dispose mesh resources / 释放网格资源
    /// </summary>
    void Dispose();
    
    /// <summary>
    /// Get vertex count / 获取顶点数量
    /// </summary>
    int VertexCount { get; }
    
    /// <summary>
    /// Get triangle count / 获取三角形数量
    /// </summary>
    int TriangleCount { get; }
    
    /// <summary>
    /// Mesh position in world space / 网格在世界空间的位置
    /// </summary>
    Vector3 Position { get; set; }
    
    /// <summary>
    /// Mesh rotation in degrees / 网格旋转角度（度）
    /// </summary>
    Vector3 Rotation { get; set; }
    
    /// <summary>
    /// Mesh scale / 网格缩放
    /// </summary>
    Vector3 Scale { get; set; }
    
    /// <summary>
    /// Get the model matrix for this mesh / 获取此网格的模型矩阵
    /// </summary>
    Matrix4x4 GetModelMatrix();
    
    /// <summary>
    /// Get the axis-aligned bounding box in local space / 获取局部空间的轴对齐包围盒
    /// </summary>
    void GetLocalBounds(out Vector3 min, out Vector3 max);
    
    /// <summary>
    /// Get the axis-aligned bounding box in world space / 获取世界空间的轴对齐包围盒
    /// </summary>
    void GetWorldBounds(out Vector3 min, out Vector3 max);
}


