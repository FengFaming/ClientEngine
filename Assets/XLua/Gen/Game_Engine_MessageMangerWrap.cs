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
    public class GameEngineMessageMangerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Game.Engine.MessageManger);
			Utils.BeginObjectRegister(type, L, translator, 0, 5, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddMessageListener", _m_AddMessageListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveMessageListener", _m_RemoveMessageListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendMessage", _m_SendMessage);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendMessageThread", _m_SendMessageThread);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RearrangeMessage", _m_RearrangeMessage);
			
			
			
			
			
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
					
					Game.Engine.MessageManger gen_ret = new Game.Engine.MessageManger();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.MessageManger constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddMessageListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.MessageManger gen_to_be_invoked = (Game.Engine.MessageManger)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<Game.Engine.IMessageEventListener>(L, 3)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    Game.Engine.IMessageEventListener _listener = (Game.Engine.IMessageEventListener)translator.GetObject(L, 3, typeof(Game.Engine.IMessageEventListener));
                    
                    gen_to_be_invoked.AddMessageListener( _head, _listener );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object[]>>(L, 3)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    System.Action<object[]> _fun = translator.GetDelegate<System.Action<object[]>>(L, 3);
                    
                    gen_to_be_invoked.AddMessageListener( _head, _fun );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Action<object[]>>(L, 4)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    bool _clear = LuaAPI.lua_toboolean(L, 3);
                    System.Action<object[]> _fun = translator.GetDelegate<System.Action<object[]>>(L, 4);
                    
                    gen_to_be_invoked.AddMessageListener( _head, _clear, _fun );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 3)&& translator.Assignable<System.Action<object[]>>(L, 4)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.GameObject _owner = (UnityEngine.GameObject)translator.GetObject(L, 3, typeof(UnityEngine.GameObject));
                    System.Action<object[]> _fun = translator.GetDelegate<System.Action<object[]>>(L, 4);
                    
                    gen_to_be_invoked.AddMessageListener( _head, _owner, _fun );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 3)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 4)&& translator.Assignable<System.Action<object[]>>(L, 5)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.GameObject _owner = (UnityEngine.GameObject)translator.GetObject(L, 3, typeof(UnityEngine.GameObject));
                    bool _clear = LuaAPI.lua_toboolean(L, 4);
                    System.Action<object[]> _fun = translator.GetDelegate<System.Action<object[]>>(L, 5);
                    
                    gen_to_be_invoked.AddMessageListener( _head, _owner, _clear, _fun );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.MessageManger.AddMessageListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveMessageListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.MessageManger gen_to_be_invoked = (Game.Engine.MessageManger)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.GameObject>(L, 2)) 
                {
                    UnityEngine.GameObject _owner = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                    gen_to_be_invoked.RemoveMessageListener( _owner );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<Game.Engine.IMessageEventListener>(L, 3)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    Game.Engine.IMessageEventListener _listen = (Game.Engine.IMessageEventListener)translator.GetObject(L, 3, typeof(Game.Engine.IMessageEventListener));
                    
                    gen_to_be_invoked.RemoveMessageListener( _head, _listen );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action<object[]>>(L, 3)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    System.Action<object[]> _fun = translator.GetDelegate<System.Action<object[]>>(L, 3);
                    
                    gen_to_be_invoked.RemoveMessageListener( _head, _fun );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 3)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.GameObject _owner = (UnityEngine.GameObject)translator.GetObject(L, 3, typeof(UnityEngine.GameObject));
                    
                    gen_to_be_invoked.RemoveMessageListener( _head, _owner );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.GameObject>(L, 3)&& translator.Assignable<System.Action<object[]>>(L, 4)) 
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.GameObject _owner = (UnityEngine.GameObject)translator.GetObject(L, 3, typeof(UnityEngine.GameObject));
                    System.Action<object[]> _fun = translator.GetDelegate<System.Action<object[]>>(L, 4);
                    
                    gen_to_be_invoked.RemoveMessageListener( _head, _owner, _fun );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Game.Engine.MessageManger.RemoveMessageListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendMessage(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.MessageManger gen_to_be_invoked = (Game.Engine.MessageManger)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    object[] _arms = translator.GetParams<object>(L, 3);
                    
                    gen_to_be_invoked.SendMessage( _head, _arms );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendMessageThread(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.MessageManger gen_to_be_invoked = (Game.Engine.MessageManger)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _head = LuaAPI.lua_tostring(L, 2);
                    object[] _arms = translator.GetParams<object>(L, 3);
                    
                    gen_to_be_invoked.SendMessageThread( _head, _arms );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RearrangeMessage(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Game.Engine.MessageManger gen_to_be_invoked = (Game.Engine.MessageManger)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.RearrangeMessage(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
