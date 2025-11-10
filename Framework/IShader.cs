using System.Drawing;
using System.Numerics;

namespace SharpFAI.Framework;

/// <summary>
/// Interface for shader program management
/// 着色器程序管理接口
/// </summary>
public interface IShader
{
    /// <summary>
    /// Compile and link shader program / 编译并链接着色器程序
    /// </summary>
    void Compile();
    
    /// <summary>
    /// Use this shader for rendering / 使用此着色器进行渲染
    /// </summary>
    void Use();
    
    /// <summary>
    /// Set uniform integer value / 设置uniform整数值
    /// </summary>
    void SetInt(string name, int value);
    
    /// <summary>
    /// Set uniform float value / 设置uniform浮点值
    /// </summary>
    void SetFloat(string name, float value);
    
    /// <summary>
    /// Set uniform Vector2 value / 设置uniform Vector2值
    /// </summary>
    void SetVector2(string name, Vector2 value);
    
    /// <summary>
    /// Set uniform Vector3 value / 设置uniform Vector3值
    /// </summary>
    void SetVector3(string name, Vector3 value);
    
    /// <summary>
    /// Set uniform Vector4 value / 设置uniform Vector4值
    /// </summary>
    void SetVector4(string name, Vector4 value);
    
    /// <summary>
    /// Set uniform Color value / 设置uniform颜色值
    /// </summary>
    void SetColor(string name, Color value);
    
    /// <summary>
    /// Set uniform Matrix4x4 value / 设置uniform Matrix4x4值
    /// </summary>
    void SetMatrix4x4(string name, Matrix4x4 value);
    
    /// <summary>
    /// Get uniform location / 获取uniform位置
    /// </summary>
    int GetUniformLocation(string name);
    
    /// <summary>
    /// Get attribute location / 获取attribute位置
    /// </summary>
    int GetAttributeLocation(string name);
    
    /// <summary>
    /// Check if shader is compiled / 检查着色器是否已编译
    /// </summary>
    bool IsCompiled { get; }
    
    /// <summary>
    /// Get compilation log / 获取编译日志
    /// </summary>
    string CompileLog { get; }
    
    /// <summary>
    /// Check if there were compilation errors / 检查是否有编译错误
    /// </summary>
    bool HasCompileErrors { get; }
    
    /// <summary>
    /// Dispose shader resources / 释放着色器资源
    /// </summary>
    void Dispose();
}

