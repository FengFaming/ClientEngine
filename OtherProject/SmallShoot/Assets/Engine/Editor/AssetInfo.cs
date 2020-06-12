/*
 * Creator:ffm
 * Desc:AB包详细内容
 * Time:2019/11/8 14:59:04
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetInfo
{
	/// <summary>
	/// 是不是打包文件夹下得文件
	/// </summary>
	private bool m_IsRootAsset;

	/// <summary>
	/// 路径
	/// </summary>
	private string m_AssetPath;
	public string AssetPath
	{
		get { return m_AssetPath; }
		set { m_AssetPath = value; }
	}

	/// <summary>
	/// 父子关系内容
	/// </summary>
	private HashSet<AssetInfo> m_ChildSet;
	private HashSet<AssetInfo> m_ParentSet;

	public AssetInfo(string path, bool isRootAsset)
	{
		m_AssetPath = path;
		m_IsRootAsset = isRootAsset;

		m_ChildSet = new HashSet<AssetInfo>();
		m_ParentSet = new HashSet<AssetInfo>();

		m_ChildSet.Clear();
		m_ParentSet.Clear();
	}

	/// <summary>
	/// 获取路径下得对象
	/// </summary>
	/// <returns></returns>
	public UnityEngine.Object GetAsset()
	{
		UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(m_AssetPath);
		return asset;
	}

	/// <summary>
	/// 添加父亲节点
	/// </summary>
	/// <param name="parent"></param>
	public void AddParent(AssetInfo parent)
	{
		if (parent == this ||
			parent == null ||
			IsParentEarlyDep(parent))
		{
			return;
		}

		m_ParentSet.Add(parent);
		parent.AddChild(this);

		parent.RemoveRepeatChildDep(this);
		RemoveRepeatParentDep(parent);
	}

	/// <summary>
	/// 添加儿子节点
	/// </summary>
	/// <param name="child"></param>
	public void AddChild(AssetInfo child)
	{
		m_ChildSet.Add(child);
	}

	/// <summary>
	/// 设置AB包名字
	///		首先我们看看这个资源是不是使用在UI上面
	///			如果是UI的，那么我们使用UI的大图作为一个ab名字
	///		如果是图片资源，但是又不是UI部分，那么我们使用文件夹名字作为AB包名字
	///		如果说是预制体，那么直接使用文件名字作为AB包名字
	///		如果是大于打包粒度的，那么使用文件夹名字作为AB名字
	///		其他情况都使用文件名字作为AB名字
	/// </summary>
	/// <param name="pieceThreshold">打包的粒度</param>
	public void SetAssetBundleName(int pieceThreshold)
	{
		AssetImporter ai = AssetImporter.GetAtPath(m_AssetPath);
		string fileName = m_AssetPath.Substring(7);
		fileName = fileName.Replace("/", ".");
		fileName = fileName.Replace("UseAB.", "");

		string folder = fileName.Substring(0, fileName.LastIndexOf("."));
		string[] folders = folder.Split('.');
		folder = folders[folders.Length - 1 - 1];
		//folder = folder.Substring(0, folder.LastIndexOf("."));
		//folder = folder.Substring(folder.LastIndexOf("."));
		//folder = folder.Substring(folder.LastIndexOf(".") + 1);
		if (ai is TextureImporter)
		{
			TextureImporter tai = ai as TextureImporter;
			if (!string.IsNullOrEmpty(tai.spritePackingTag))
			{
				tai.SetAssetBundleNameAndVariant(tai.spritePackingTag, null);
			}
			else
			{
				if (this.m_ParentSet.Count > pieceThreshold)
				{
					tai.SetAssetBundleNameAndVariant(folder, null);
				}
				else
				{
					tai.SetAssetBundleNameAndVariant(fileName, null);
				}
			}
		}
		else if (ai.assetPath.IndexOf(".prefab") != -1)
		{
			ai.SetAssetBundleNameAndVariant(fileName, null);
		}
		else
		{
			if (this.m_ParentSet.Count > pieceThreshold)
			{
				ai.SetAssetBundleNameAndVariant(folder, null);
			}
			else if (m_ParentSet.Count == 0)
			{
				ai.SetAssetBundleNameAndVariant(fileName, null);
			}
			else if (m_IsRootAsset)
			{
				ai.SetAssetBundleNameAndVariant(fileName, null);
			}
			else
			{
				ai.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
			}
		}
	}

	/// <summary>
	/// 判断一下有没有这个父节点
	/// </summary>
	/// <param name="p"></param>
	/// <returns></returns>
	public bool HasParent(AssetInfo p)
	{
		if (m_ParentSet.Contains(p))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 清除我子节点当中被我父节点的重复引用，保证树状结构
	/// </summary>
	/// <param name="parent"></param>
	private void RemoveRepeatParentDep(AssetInfo parent)
	{
		List<AssetInfo> infolist = new List<AssetInfo>(m_ChildSet);
		for (int i = 0; i < infolist.Count; i++)
		{
			AssetInfo info = infolist[i];
			info.RemoveParent(parent);
			info.RemoveRepeatParentDep(parent);
		}
	}

	/// <summary>
	/// 清除我父节点对我子节点的重复引用，保证树状结构
	/// </summary>
	/// <param name="child"></param>
	private void RemoveRepeatChildDep(AssetInfo child)
	{
		List<AssetInfo> infolist = new List<AssetInfo>(m_ParentSet);
		for (int i = 0; i < infolist.Count; i++)
		{
			AssetInfo info = infolist[i];
			info.RemoveChild(child);
			info.RemoveRepeatChildDep(child);
		}
	}

	/// <summary>
	/// 移除孩子节点
	/// </summary>
	/// <param name="child"></param>
	private void RemoveChild(AssetInfo child)
	{
		m_ChildSet.Remove(child);
		child.m_ParentSet.Remove(this);
	}

	/// <summary>
	/// 移除父亲节点
	/// </summary>
	/// <param name="parent"></param>
	private void RemoveParent(AssetInfo parent)
	{
		parent.m_ChildSet.Remove(this);
		m_ParentSet.Remove(parent);
	}

	/// <summary>
	/// 判断一下这个父节点在不在列表当中了
	/// </summary>
	/// <param name="parent"></param>
	/// <returns></returns>
	private bool IsParentEarlyDep(AssetInfo parent)
	{
		if (m_ParentSet.Contains(parent))
		{
			return true;
		}

		var e = m_ParentSet.GetEnumerator();
		while (e.MoveNext())
		{
			if (e.Current.IsParentEarlyDep(parent))
			{
				return true;
			}
		}

		return false;
	}
}
