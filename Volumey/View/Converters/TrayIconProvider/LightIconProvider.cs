﻿using System.Windows.Media.Imaging;
using log4net;

namespace Volumey.View.Converters
{
	public partial class TrayIconConverter
	{
		internal class LightIconProvider : TrayIconProvider
		{
			private BitmapImage trayHigh;
			protected override BitmapImage High => trayHigh ??= App.Current.FindResource("TrayHigh") as BitmapImage;

			private BitmapImage trayMid;
			protected override BitmapImage Mid => trayMid ??= App.Current.FindResource("TrayMid") as BitmapImage;

			private BitmapImage trayLow;
			protected override BitmapImage Low => trayLow ??= App.Current.FindResource("TrayLow") as BitmapImage;

			private BitmapImage trayMute;
			protected override BitmapImage Mute => trayMute ??= App.Current.FindResource("TrayMute") as BitmapImage;

			private BitmapImage trayNoDevice;
			protected override BitmapImage NoDevice => trayNoDevice ??= App.Current.FindResource("TrayNoDevice") as BitmapImage;
			
			private ILog _logger;
			protected override ILog Logger => _logger ??= LogManager.GetLogger(typeof(LightIconProvider));
		}
	}

}