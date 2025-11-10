using System.Drawing;
using System.Numerics;

namespace SharpFAI.Framework;

/// <summary>
/// Interface for text rendering
/// 文本渲染接口
/// </summary>
public interface IText
{
	/// <summary>
	/// Text content / 文本内容
	/// </summary>
	string Content { get; set; }
	
	/// <summary>
	/// Font family name / 字体名称
	/// </summary>
	string FontFamily { get; set; }
	
	/// <summary>
	/// Font size in points / 字体大小（pt）
	/// </summary>
	float FontSize { get; set; }
	
	/// <summary>
	/// Text color / 文本颜色
	/// </summary>
	Color Color { get; set; }
	
	/// <summary>
	/// World position / 世界坐标位置
	/// </summary>
	Vector2 Position { get; set; }
	
	/// <summary>
	/// Rotation in degrees / 旋转角度（度）
	/// </summary>
	float Rotation { get; set; }
	
	/// <summary>
	/// Scale / 缩放
	/// </summary>
	Vector2 Scale { get; set; }
	
	/// <summary>
	/// Alignment / 对齐方式
	/// </summary>
	TextAlign Alignment { get; set; }
	
	/// <summary>
	/// Measure text size in pixels / 测量文本像素尺寸
	/// </summary>
	Vector2 Measure();
	
	/// <summary>
	/// Ensure underlying texture/geometry updated after property changes
	/// 在属性变更后更新底层纹理/几何
	/// </summary>
	void Update();
	
	/// <summary>
	/// Render the text using a shader / 使用着色器渲染文本
	/// </summary>
	/// <param name="shader">Shader / 着色器</param>
	void Render(IShader shader);
	
	/// <summary>
	/// Get the backing texture (read-only) / 获取底层纹理（只读）
	/// </summary>
	ITexture Texture { get; }
	
	/// <summary>
	/// Dispose resources / 释放资源
	/// </summary>
	void Dispose();
}

/// <summary>
/// Text alignment / 文本对齐
/// </summary>
public enum TextAlign
{
	Left,
	Center,
	Right
}



