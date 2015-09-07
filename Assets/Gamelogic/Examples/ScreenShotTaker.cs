//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class ScreenShotTaker : GLMonoBehaviour
	{
		public KeyCode screenShotKey = KeyCode.Q;
		public int scale;

		private static ScreenShotTaker instance;

		private static ScreenShotTaker Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<ScreenShotTaker>();
				}

				return instance;
			}
		}

		public void Update()
		{
			if (Input.GetKeyDown(screenShotKey))
			{
				Take();
			}
		}

		public static void Take()
		{
			Instance.Take__();
		}

		private void Take__()
		{
			string path = "screen_" + DateTime.Now.Ticks + ".png";
			Application.CaptureScreenshot(path, scale);
		}
	}
}