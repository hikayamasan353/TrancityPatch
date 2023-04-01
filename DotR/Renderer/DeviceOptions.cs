/*
 * Created by SharpDevelop.
 * User: Sergey
 * Date: 01.05.2015
 * Time: 18:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;

namespace Engine
{
	/// <summary>
	/// DeviceOptions - so u now wat is it
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct DeviceOptions
	{
		public int adapterID;
		//only for dx?
    	public int vertexProcessingMode;
    	//device type
    	public int deviceType;
        public int fullscreenX;
        public int fullscreenY;
        public bool windowed;
        public int windowedX;
        public int windowedY;
        public bool vSync;
	}
}
