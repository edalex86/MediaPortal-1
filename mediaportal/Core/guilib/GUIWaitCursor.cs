/* 
 *	Copyright (C) 2005 Media Portal
 *	http://mediaportal.sourceforge.net
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

using System;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Threading;

namespace MediaPortal.GUI.Library
{
	public sealed class GUIWaitCursor : GUIControl
	{
		#region Constructors

		private GUIWaitCursor()
		{
		}

		#endregion Constructors

		#region Methods

		public void Dispose()
		{
			_animation.Dispose();
		}

		public static void Init()
		{
			_animation = new GUIAnimation();

			foreach(string filename in Directory.GetFiles(GUIGraphicsContext.Skin + @"\media\", "common.waiting.*.png"))
				_animation.Filenames.Add(filename);
		}

		public override void Render(float timePassed)
		{
		}
			
		public static void Render()
		{
			if(_showCount <= 0)
				return;

			_animation.Render(GUIGraphicsContext.TimePassed);
		}

		public static void Show()
		{
			if(Interlocked.Increment(ref _showCount) == 0)
				Interlocked.Exchange(ref _tickCount, Environment.TickCount);
		}

		public static void Hide()
		{
			Interlocked.Decrement(ref _showCount);
		}

		#endregion Methods

		#region Fields

		static GUIAnimation				_animation;
		static int						_showCount;
		static float					_tickCount = 0;

		#endregion Fields
	}
}
