/*
 * Created by SharpDevelop.
 * User: sergey
 * Date: 15.11.2015
 * Time: 16:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Engine.Controls;

namespace Engine
{
	/// <summary>
	/// RenderSystem - manages render device and form.
	/// Бесполезный класс
	/// </summary>
	public static class RenderSystem
	{
		#region Previous class structure
		
		/*private RenderDevice device;
		
		public RenderSystem(RenderDeviceType deviceType, IRenderControl control, DeviceOptions parametrs)
		{
			//
		}
		
		public RenderDevice CurrentDevice
		{
			get
			{
				return device;
			}
		}*/
		
		#endregion
		
		public static RenderDevice CreateDevice(/*this*/ IRenderControl control, RenderDeviceType deviceType, DeviceOptions parameters)
		{
			RenderDevice device = null;
			
			//check for exist renderer
			if (control.Renderer != null)
			{
				throw new InvalidOperationException("Can not replace existing renderer!");
			}
			
			//convert to internal interface and check if successed
			IInternalRenderControl internalControl = control as IInternalRenderControl;
			if (internalControl == null)
			{
				throw new NotSupportedException("Invalid control");
			}
			
			//create our device
			switch (deviceType)
			{
				case RenderDeviceType.DirectX9:
					#if DX9
						device = new DX9RenderDevice(internalControl, parameters);
						break;
					#else
						throw new NotImplementedException("DX9 not included in this version!");
					#endif
					
				case RenderDeviceType.OpenGL:
					#if OGL
						throw new NotImplementedException();
						break;
					#else
						throw new NotImplementedException("OpenGL not included in this version!");
					#endif
				
				default:
					throw new NotSupportedException(string.Format("Specified device type {0} is unavailable!", deviceType.ToString()));
			}
			
			//sve current renderer
			internalControl.Renderer = device;
			
			//overhead of forms
			internalControl.ControlSize = new System.Drawing.Size(parameters.windowedX, parameters.windowedY);
			
			if ((control as IInternalRenderForm) != null)
			{
				((IInternalRenderForm)control).ShowForm();
			}
			
			//and exit
			return device;
		}
		
		public static DeviceInfo[] EnumerateDevices()
		{
		    // TODO: rewrite in generic way
		    #if DX9
		    using (var d3d9 = new SlimDX.Direct3D9.Direct3D())
		    {
		        var res = new DeviceInfo[d3d9.AdapterCount];
		        for (var i = 0; i < res.Length; i++)
		        {
		            res[i] = new DeviceInfo(d3d9.Adapters[i]);
		        }
		        return res;
		    }
		    #else
		    throw new NotImplementedException("DX9 not included in this version!");
		    #endif
		}
	}
	
	public enum RenderDeviceType
	{
		DirectX9,
		OpenGL
	}
	
	public struct DeviceInfo
	{
	    public readonly string Name;
	    public readonly ScreenMode[] ScreenModes;
	    public readonly VertexProcessingMode[] VertexProcessingModes;
	    
	    #if DX9
	    public DeviceInfo(SlimDX.Direct3D9.AdapterInformation adapter)
	    {
	        Name = adapter.Details.Description;
	        
	        var modes = adapter.GetDisplayModes(adapter.CurrentDisplayMode.Format);
	        var listModes = new System.Collections.Generic.List<ScreenMode>(modes.Count);
	        var prevScreenMode = new ScreenMode();
	        for (var i = 0; i < modes.Count; i++)
	        {
	            var screenMode = new ScreenMode(modes[i].Width, modes[i].Height);
	            if (!screenMode.Equals(prevScreenMode))
	            {
	                listModes.Add(screenMode);
	                prevScreenMode = screenMode;
	            }
	        }
	        ScreenModes = listModes.ToArray();
	        
	        VertexProcessingModes = new VertexProcessingMode[]{
	            VertexProcessingMode.Hardware,
	            VertexProcessingMode.Software,
	            VertexProcessingMode.Mixed
	        };
	    }
	    #endif
	    
	    public override string ToString()
	    {
	        return Name;
	    }
	}
	
	public struct ScreenMode : IEquatable<ScreenMode>
	{
	    public readonly int Width;
	    public readonly int Height;
	    
	    public ScreenMode(int width, int height)
	    {
	        Width = width;
	        Height = height;
	    }
	    
	    public override string ToString()
	    {
	        return string.Format("{0}x{1}", Width, Height);
	    }

	    #region IEquatable implementation

	    public bool Equals(ScreenMode other)
	    {
	        return Width == other.Width && Height == other.Height;
	    }

	    #endregion
	}
	
	public enum VertexProcessingMode
	{
	    Software = 0,
	    Hardware = 1,
	    Mixed = 2
	}
}
