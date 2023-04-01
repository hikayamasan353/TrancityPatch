using System;
using System.Threading;
using System.Windows.Forms;
using Common;
using Engine;

namespace Trancity
{
	public class ThreadPoolTest
	{
		private static Game _game;

		private static Mutex mutex;

		private static bool mutex2 = false;

		private static object locker = new object();

		private static bool sound_flag = false;

		public static void RunGameProcess(Game game, bool sound)
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
			_game = game;
			sound_flag = sound;
			mutex = new Mutex(initiallyOwned: false);
			ThreadPool.QueueUserWorkItem(MainThread, manualResetEvent);
			ThreadPool.QueueUserWorkItem(RenderThread, manualResetEvent);
			manualResetEvent.WaitOne();
			MyDirectInput.Free();
		}

		private static void MainThread(object arg)
		{
			try
			{
				while (true)
				{
					lock (locker)
					{
						if (!MyDirectInput.Process() && MyDirectInput.alt_f4)
						{
							break;
						}
						_game.Process_Input();
						if (_game.активна)
						{
							_game.мир.Обновить(_game.игроки);
						}
						else
						{
							_game.мир.Обновить_время();
						}
						if (sound_flag)
						{
							_game.мир.UpdateSound(_game.игроки, _game.активна);
						}
						mutex.WaitOne();
						goto IL_009d;
					}
					IL_009d:
					_game.RenderMain();
					mutex2 = true;
					mutex.ReleaseMutex();
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				Logger.LogException(ex, "MainThread");
				MessageBox.Show(string.Concat("Unhandled exception of type ", ex.GetType(), "\n", ex, "\n\nПожалуйста, сообщите создателю игры об этой ошибке."), "Trancity", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			((ManualResetEvent)arg).Set();
		}

		private static void RenderThread(object arg)
		{
			try
			{
				while (true)
				{
					lock (locker)
					{
						mutex.WaitOne();
					}
					if (mutex2)
					{
						_game.RenderThread();
					}
					mutex2 = false;
					mutex.ReleaseMutex();
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				Logger.LogException(ex, "RenderThread");
				MessageBox.Show(string.Concat("Unhandled exception of type ", ex.GetType(), "\n", ex, "\n\nПожалуйста, сообщите создателю игры об этой ошибке."), "Trancity", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			((ManualResetEvent)arg).Set();
		}
	}
}
