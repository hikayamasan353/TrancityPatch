/*
 * Created by SharpDevelop.
 * User: sergey
 * Date: 29.10.2015
 * Time: 23:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
#if DX9
using System;
using System.Threading;
using Engine.Controls;
using SlimDX;
using SlimDX.Direct3D9;

namespace Engine
{
	/// <summary>
	/// DX9RenderDevice - obviously enough
	/// </summary>
	public class DX9RenderDevice : RenderDevice, IRawDevice<Device>
	{
		private Direct3D d3d9device;
		private Device device;
		private PresentParameters presentParams;
		
		internal DX9RenderDevice(IInternalRenderControl control, DeviceOptions parameters) : base(control, parameters)
		{
			d3d9device = new Direct3D();
			Logger.Log("DirectX9", "Direct3D created. Availabale devices (adapters):");
			AdapterDetails details;
			int deviceCount = d3d9device.AdapterCount;
			for (int i = 0; i < deviceCount; i++)
			{
				details = d3d9device.Adapters[i].Details;
				Logger.Log(details.Adapter.ToString(),
				           string.Format("{0} with driver version {1}",
				                         details.Description, details.DriverVersion));
			}
			CreateFlags createFlags;
			switch (parameters.vertexProcessingMode)
			{
				case 0:
					createFlags = CreateFlags.SoftwareVertexProcessing;
					break;
				
				case 2:
					createFlags = CreateFlags.MixedVertexProcessing;
					break;
				
				case 1:
				default:
					createFlags = CreateFlags.HardwareVertexProcessing;
					break;
			}
			Logger.Log("DirectX9", "CreateFlags = " + createFlags.ToString());
			createFlags |= CreateFlags.Multithreaded;
			//...
#if DEBUG
			SlimDX.Configuration.EnableObjectTracking = true;
			SlimDX.Configuration.DetectDoubleDispose = true;
			Logger.DebugLog("SlimDX", "Debug tracking enabled!");
#endif
			//...
			if (parameters.adapterID >= deviceCount)
			{
				Logger.Log("DirectX9", "Invalid adapter ID, used 0 instead");
				parameters.adapterID = 0;
			}
			else
			{
				Logger.Log("DirectX9", "Uses adapter with ID " + parameters.adapterID);
			}
			Logger.Log("DirectX9", parameters.windowed ? "Uses windowed mode" : "Uses fullscreen mode");
			//...
			presentParams = new PresentParameters
			{
				AutoDepthStencilFormat = Format.D16,
				BackBufferCount = 1,
				BackBufferFormat = Format.A8R8G8B8,
				BackBufferHeight = parameters.windowed ? parameters.windowedY : parameters.fullscreenY,
				BackBufferWidth = parameters.windowed ? parameters.windowedX : parameters.fullscreenX,
				DeviceWindowHandle = control.ControlHandle,
				EnableAutoDepthStencil = true,
				PresentationInterval = parameters.vSync ? PresentInterval.One : PresentInterval.Immediate,
				SwapEffect = SwapEffect.Discard,
				Windowed = parameters.windowed
			};
			device = new Device(d3d9device, parameters.adapterID, DeviceType.Hardware, control.ControlHandle, createFlags, new [] { presentParams } );
			Logger.Log("DirectX9", "Device created successfully");
		}
		
		public override bool IsDeviceLost
		{
			get
			{
				Result result = device.TestCooperativeLevel();
				// TODO: replace strings by result codes
				if (result.Name == "D3DERR_DEVICELOST")
				{
					Thread.Sleep(50);
					return true;
				}
				if (result.Name == "D3DERR_DEVICENOTRESET")
				{
					device.Reset(new [] { presentParams } );
					return false;
				}
				else if (result.IsFailure)
				{
					//internal error
					// TODO: replace by slimdx-independed exception
					throw new Direct3D9Exception(result);
				}
				return false;
			}
		}
		
		public override void BeginScene()
		{
			device.BeginScene();
			base.BeginScene();
		}
		
		public override void Clear(DeviceClearFlags flags, System.Drawing.Color color)
		{
			ClearFlags dxflags = ClearFlags.None;
			if ((flags & DeviceClearFlags.Target) == DeviceClearFlags.Target)
			{
				dxflags |= ClearFlags.Target;
			}
			if ((flags & DeviceClearFlags.Stencil) == DeviceClearFlags.Stencil)
			{
				dxflags |= ClearFlags.Stencil;
			}
			if ((flags & DeviceClearFlags.ZBuffer) == DeviceClearFlags.ZBuffer)
			{
				dxflags |= ClearFlags.ZBuffer;
			}
			device.Clear(dxflags, color.ToArgb(), 1f, 0);
		}
		
		public override void EndScene()
		{
			device.EndScene();
			try
			{
				device.Present();
			}
			catch (Direct3D9Exception exc)
			{
				//probably, device is lost
				// TODO: replace strings by result codes
				if (exc.ResultCode.Name != "D3DERR_DEVICELOST")
				{
					// exception rethrows when there is no args
					throw;
				}
			}
			base.EndScene();
		}
		
		public Device RawDevice
		{
			get
			{
				return device;
			}
		}
	}
}
#endif