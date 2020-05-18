#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class GameEngineLuaManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Game.Engine.LuaManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 7, 1, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CreateTable", _m_CreateTable);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetGameObject", _m_GetGameObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetComp", _m_GetComp);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OpenUI", _m_OpenUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetUI", _m_GetUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CloseUI", _m_CloseUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ChangeScene", _m_ChangeScene);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "MyLuaEnv", _g_get_MyLuaEnv);
            
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					Game.Engine.LuaManager gen_ret = new Game.Engine.LuaManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.LuaManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateTable(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 2);
                    
                        XLua.LuaTable gen_ret = gen_to_be_invoked.CreateTable( _fileName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<object>(L, 3)) 
                {
                    string _fileName = LuaAPI.lua_tostring(L, 2);
                    object _self = translator.GetObject(L, 3, typeof(object));
                    
                        XLua.LuaTable gen_ret = gen_to_be_invoked.CreateTable( _fileName, _self );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.LuaManager.CreateTable!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetGameObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.GameObject _parent = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    string _name = LuaAPI.lua_tostring(L, 3);
                    
                        UnityEngine.GameObject gen_ret = gen_to_be_invoked.GetGameObject( _parent, _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetComp(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.GameObject _target = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    string _name = LuaAPI.lua_tostring(L, 3);
                    
                        UnityEngine.Component gen_ret = gen_to_be_invoked.GetComp( _target, _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OpenUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _ui = LuaAPI.lua_tostring(L, 2);
                    string _layer = LuaAPI.lua_tostring(L, 3);
                    object[] _arms = translator.GetParams<object>(L, 4);
                    
                    gen_to_be_invoked.OpenUI( _ui, _layer, _arms );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _ui = LuaAPI.lua_tostring(L, 2);
                    string _layer = LuaAPI.lua_tostring(L, 3);
                    
                        Game.Engine.IUIModelControl gen_ret = gen_to_be_invoked.GetUI( _ui, _layer );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CloseUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _ui = LuaAPI.lua_tostring(L, 2);
                    string _layer = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.CloseUI( _ui, _layer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ChangeScene(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.ChangeScene( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MyLuaEnv(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Game.Engine.LuaManager gen_to_be_invoked = (Game.Engine.LuaManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.MyLuaEnv);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
