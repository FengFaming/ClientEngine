/*
 * Creator:ffm
 * Desc:框架消息头
 * Time:2020/4/11 14:48:01
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
	public class EngineMessageHead
	{
		/// <summary>
		/// 场景加载
		///		true:false
		/// </summary>
		public static readonly string CHANGE_SCENE_MESSAGE = "CHANGE_SCENE_MESSAGE";
		public static readonly string CHANGE_SCENE_PRESS_VALUE = "CHANGE_SCENE_PRESS_VALUE";

		/// <summary>
		/// 发送属性修改
		///		修改前的值:修改后的值
		/// </summary>
		public static readonly string CHANGE_CHARACTER_ATTRIBUTE_VALUE = "CHANGE_CHARACTER_ATTRIBUTE_VALUE{0}:{1}";

		/// <summary>
		/// 鼠标监听
		/// </summary>
		public static readonly string LISTEN_MOUSE_EVENT_FOR_INPUT_MANAGER = "LISTEN_MOUSE_EVENT_FOR_INPUT_MANAGER";
		public static readonly string LISTEN_KEY_EVENT_FOR_INPUT_MANAGER = "LISTEN_KEY_EVENT_FOR_INPUT_MANAGER";

		/// <summary>
		/// 场景相关内容
		/// </summary>
		public static readonly string SCENE_LIGHTMAP_COMBINE_NAME = "{0}_Lightmap";

		/// <summary>
		/// 协议头重组
		/// </summary>
		public static readonly string NET_CLIENT_MESSAGE_HEAD = "NET_CLIENT_MESSAGE_HEAD:{0}";

		/// <summary>
		/// 服务器时间下发
		/// </summary>
		public static readonly int NET_CLIENT_TIME_RESPONSE = 100001;

		/// <summary>
		/// 客户端时间上传
		/// </summary>
		public static readonly int NET_CLIENT_TIME_REQUEST = 100002;

		/// <summary>
		/// 申请对比版本号
		/// </summary>
		public static readonly int NET_CLIENT_VERSION_REQUEST = 10;

		/// <summary>
		/// 服务器返回差异文件
		/// </summary>
		public static readonly int NET_CLIENT_VERSION_RESPONSE = 11;

		/// <summary>
		/// 申请下载文件
		/// </summary>
		public static readonly int NET_CLIENT_DOWNLOAD_FILE_REQUEST = 12;

		/// <summary>
		/// 返回文件
		/// </summary>
		public static readonly int NET_CLIENT_DOWNLOAD_FILE_RESPONSE = 13;

		/// <summary>
		/// 存储版本号
		/// </summary>
		public static readonly string CLIENT_VERSION_BIG_VALUE = "CLIENT_VERSION_BIG_VALUE";
		public static readonly string CLIENT_VERSION_SMALL_VALUE = "CLIENT_VERSION_SMALL_VALUE";
	}
}
