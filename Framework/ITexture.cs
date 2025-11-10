using System.Drawing;
using System.Numerics;

namespace SharpFAI.Framework;

/// <summary>
/// Interface for texture management
/// 纹理管理接口
/// </summary>
public interface ITexture
{
	/// <summary>
	/// Texture width in pixels / 纹理宽度（像素）
	/// </summary>
	int Width { get; }
	
	/// <summary>
	/// Texture height in pixels / 纹理高度（像素）
	/// </summary>
	int Height { get; }
	
	/// <summary>
	/// Whether the texture is loaded / 纹理是否已加载
	/// </summary>
	bool IsLoaded { get; }
	
	/// <summary>
	/// Load texture from file / 从文件加载纹理
	/// </summary>
	/// <param name="path">Image file path / 图像文件路径</param>
	void Load(string path);
	
	/// <summary>
	/// Update a sub-region of the texture with raw RGBA data
	/// 使用原始RGBA数据更新纹理的子区域
	/// </summary>
	/// <param name="offset">Top-left offset in pixels / 左上角偏移（像素）</param>
	/// <param name="size">Region size in pixels / 区域尺寸（像素）</param>
	/// <param name="rgba">Row-major RGBA byte array / 按行存储的RGBA字节数组</param>
	void Update(Vector2 offset, Vector2 size, byte[] rgba);
	
	/// <summary>
	/// Bind the texture to a texture unit / 绑定纹理到指定纹理单元
	/// </summary>
	/// <param name="unit">Texture unit index / 纹理单元索引</param>
	void Bind(int unit = 0);
	
	/// <summary>
	/// Unbind the texture / 解绑纹理
	/// </summary>
	void Unbind();
	
	/// <summary>
	/// Set texture filtering mode / 设置纹理过滤模式
	/// </summary>
	void SetFilter(TextureFilter min, TextureFilter mag);
	
	/// <summary>
	/// Set texture wrapping mode / 设置纹理环绕模式
	/// </summary>
	void SetWrap(TextureWrap s, TextureWrap t);
	
	/// <summary>
	/// Dispose texture resources / 释放纹理资源
	/// </summary>
	void Dispose();
}

/// <summary>
/// Texture filtering modes / 纹理过滤模式
/// </summary>
public enum TextureFilter
{
	Nearest,
	Linear
}

/// <summary>
/// Texture wrapping modes / 纹理环绕模式
/// </summary>
public enum TextureWrap
{
	Repeat,
	ClampToEdge
}



