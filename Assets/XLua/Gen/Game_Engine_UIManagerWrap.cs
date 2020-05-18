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
    public class GameEngineUIManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Game.Engine.UIManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 5, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OpenUI", _m_OpenUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShowUI", _m_GetShowUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RecoveryUIModel", _m_RecoveryUIModel);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddUpdate", _m_AddUpdate);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearAllUI", _m_ClearAllUI);
			
			
			
			
			
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
					
					Game.Engine.UIManager gen_ret = new Game.Engine.UIManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.UIManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OpenUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.UIManager gen_to_be_invoked = (Game.Engine.UIManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    Game.Engine.UILayer _layer;translator.Get(L, 3, out _layer);
                    object[] _arms = translator.GetParams<object>(L, 4);
                    
                    gen_to_be_invoked.OpenUI( _name, _layer, _arms );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShowUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.UIManager gen_to_be_invoked = (Game.Engine.UIManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<Game.Engine.UILayer>(L, 3)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    Game.Engine.UILayer _layer;translator.Get(L, 3, out _layer);
                    
                        Game.Engine.IUIModelControl gen_ret = gen_to_be_invoked.GetShowUI( _name, _layer );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                        Game.Engine.IUIModelControl gen_ret = gen_to_be_invoked.GetShowUI( _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.UIManager.GetShowUI!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RecoveryUIModel(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.UIManager gen_to_be_invoked = (Game.Engine.UIManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<Game.Engine.IUIModelControl>(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    Game.Engine.IUIModelControl _ui = (Game.Engine.IUIModelControl)translator.GetObject(L, 2, typeof(Game.Engine.IUIModelControl));
                    bool _manager = LuaAPI.lua_toboolean(L, 3);
                    
                    gen_to_be_invoked.RecoveryUIModel( _ui, _manager );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<Game.Engine.UILayer>(L, 3)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    Game.Engine.UILayer _layer;translator.Get(L, 3, out _layer);
                    
                    gen_to_be_invoked.RecoveryUIModel( _name, _layer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.UIManager.RecoveryUIModel!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddUpdate(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.UIManager gen_to_be_invoked = (Game.Engine.UIManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Game.Engine.IUIModelControl _control = (Game.Engine.IUIModelControl)translator.GetObject(L, 2, typeof(Game.Engine.IUIModelControl));
                    
                    gen_to_be_invoked.AddUpdate( _control );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearAllUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.UIManager gen_to_be_invoked = (Game.Engine.UIManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ClearAllUI(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
