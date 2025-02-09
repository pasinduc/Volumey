﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Input;
using log4net;
using Volumey.Controls;

namespace Volumey.DataProvider
{
	[Serializable]
	public enum AppTheme
	{
		Dark,
		Light,
		System
	};
	
	[Serializable]
	public class AppSettings
	{
		private readonly HotkeysAppSettings hotkeysSettings = new HotkeysAppSettings();

		public HotkeysAppSettings HotkeysSettings => hotkeysSettings;
		
		private AppTheme currentAppTheme = AppTheme.Light;
		public AppTheme CurrentAppTheme
		{
			get => currentAppTheme;
			set => currentAppTheme = value;
		}

		private int volumeStep = 1;
		public int VolumeStep
		{
			get => volumeStep;
			set => volumeStep = value;
		}

		private string appLanguage;
		public string AppLanguage
		{
			get => appLanguage;
			set => appLanguage = value;
		}

		private string systemLanguage;
		public string SystemLanguage
		{
			get => systemLanguage;
			set => systemLanguage = value;
		}

		private string systemSoundsName;
		public string SystemSoundsName
		{
			get => systemSoundsName;
			set => systemSoundsName = value;
		}

		[OptionalField]
		private bool volumeLimitIsOn;
		public bool VolumeLimitIsOn
		{
			get => volumeLimitIsOn;
			set => volumeLimitIsOn = value;
		}

		[OptionalField]
		private int volumeLimit = 50;
		public int VolumeLimit
		{
			get => volumeLimit;
			set => volumeLimit = value;
		}

		[OptionalField]
		private bool userHasRated;
		public bool UserHasRated
		{
			get => userHasRated;
			set => userHasRated = value;
		}
		
		[OptionalField]
		private DateTime firstLaunchDate;
		public DateTime FirstLaunchDate
		{
			get => firstLaunchDate;
			set => firstLaunchDate = value;
		}

		[OptionalField]
		private int launchCount = 1;
		public int LaunchCount
		{
			get => launchCount;
			set => launchCount = value;
		}

		[OptionalField]
		private bool popupEnabled;
		public bool PopupEnabled
		{
			get => popupEnabled;
			set => popupEnabled = value;
		}

		[OptionalField]
		private bool alwaysOnTop;
		public bool AlwaysOnTop
		{
			get => alwaysOnTop;
			set => alwaysOnTop = value;
		}

		// [OptionalField]
		// private bool blockHotkeys = true;
		// public bool BlockHotkeys
		// {
		// 	get => blockHotkeys;
		// 	set => blockHotkeys = value;
		// }
		//
		// [OptionalField]
		// private bool allowDuplicates = false;
		// public bool AllowDuplicates
		// {
		// 	get => allowDuplicates;
		// 	set => allowDuplicates = value;
		// }

		internal bool HotkeyExists(HotKey key) => this.hotkeysSettings.HotkeyExists(key);
		internal bool HotkeysExist(HotKey key, HotKey key2) => this.hotkeysSettings.HotkeysExist(key, key2);
		
		[Serializable]
		public class HotkeysAppSettings
		{
			private string MusicAppName { get; set; }

			private bool VolumeUpIsEmpty => VolumeUpKey == Key.None && VolumeUpModifiers == ModifierKeys.None;
			private bool VolumeDownIsEmpty => VolumeDownKey == Key.None && VolumeDownModifiers == ModifierKeys.None;

			private Key VolumeUpKey;
			private ModifierKeys VolumeUpModifiers;

			private Key VolumeDownKey;
			private ModifierKeys VolumeDownModifiers;

			private Key OpenMixerKey;
			private ModifierKeys OpenMixerModifiers;
			
			[OptionalField]
			private Key DeviceMuteKey;
			[OptionalField]
			private ModifierKeys DeviceMuteModifiers;
			
			/// <summary>
			/// Default output device increase volume hotkey. Returns null if hotkey is not set. 
			/// </summary>
			internal HotKey DeviceVolumeUp
			{
				get
				{
					if(VolumeUpKey != Key.None || VolumeUpModifiers != ModifierKeys.None)
						return new HotKey(VolumeUpKey, VolumeUpModifiers);
					return null;
				}
				set
				{
					if(value == null)
					{
						this.VolumeUpKey = Key.None;
						this.VolumeUpModifiers = ModifierKeys.None;
					}
					else
					{
						this.VolumeUpKey = value.Key;
						this.VolumeUpModifiers = value.ModifierKeys;
					}
				}
			}

			/// <summary>
			/// Default output device decrease volume hotkey. Returns null if hotkey is not set. 
			/// </summary>
			internal HotKey DeviceVolumeDown
			{
				get
				{
					if(VolumeDownKey != Key.None || VolumeDownModifiers != ModifierKeys.None)
						return new HotKey(VolumeDownKey, VolumeDownModifiers);
					return null;
				}
				set
				{
					if(value == null)
					{
						this.VolumeDownKey = Key.None;
						this.VolumeDownModifiers = ModifierKeys.None;
					}
					else
					{
						this.VolumeDownKey = value.Key;
						this.VolumeDownModifiers = value.ModifierKeys;
					}
				}
			}

			/// <summary>
			/// Default output device mute/unmute hotkey. Returns null if hotkey is not set. 
			/// </summary>
			internal HotKey DeviceMute
			{
				get
				{
					if(DeviceMuteKey != Key.None || DeviceMuteModifiers != ModifierKeys.None)
						return new HotKey(DeviceMuteKey, DeviceMuteModifiers);
					return null;
				}
				set
				{
					if(value == null)
					{
						DeviceMuteKey = Key.None;
						DeviceMuteModifiers = ModifierKeys.None;
					}
					else
					{
						DeviceMuteKey = value.Key;
						DeviceMuteModifiers = value.ModifierKeys;
					}
				}
			}

			internal HotKey OpenMixer
			{
				get
				{
					if(OpenMixerKey != Key.None || OpenMixerModifiers != ModifierKeys.None)
						return new HotKey(OpenMixerKey, OpenMixerModifiers);
					return null;
				}
				set
				{
					if(value == null)
					{
						this.OpenMixerKey = Key.None;
						this.OpenMixerModifiers = ModifierKeys.None;
					}
					else
					{
						this.OpenMixerKey = value.Key;
						this.OpenMixerModifiers = value.ModifierKeys;
					}
				}
			}

			[OptionalField]
			private Dictionary<string, (SerializableHotkey up, SerializableHotkey down)> serializableRegisteredSessions;

			internal ObservableConcurrentDictionary<string, Tuple<HotKey, HotKey>> GetRegisteredSessions()
			{
				var dictionary = new ObservableConcurrentDictionary<string, Tuple<HotKey, HotKey>>();
				if(this.serializableRegisteredSessions == null)
				{
					this.serializableRegisteredSessions = new Dictionary<string, (SerializableHotkey, SerializableHotkey)>();
					return dictionary;
				}
				if(this.serializableRegisteredSessions.Count == 0)
					return dictionary;
				foreach(var (key, (up, down)) in this.serializableRegisteredSessions)
				{
					dictionary.Add(key, new Tuple<HotKey, HotKey>(up.ToHotKey(), down.ToHotKey()));
				}
				return dictionary;
			}
			
			internal bool HotkeyExists(HotKey key)
			{
				if(DeviceVolumeUp is HotKey up && key.Equals(up))
					return true;
				if(DeviceVolumeDown is HotKey down && key.Equals(down))
					return true;
				if(OpenMixer is HotKey open && key.Equals(open))
					return true;
				foreach(var session in this.serializableRegisteredSessions)
				{
					if(session.Value.up.ToHotKey().Equals(key) || session.Value.down.ToHotKey().Equals(key))
						return true;
				}
				foreach(var value in this.serializableRegisteredDevices)
				{
					var hotkey = value.Key.ToHotKey();
					if(hotkey.Equals(key))
						return true;
				}
				return false;
			}
			
			internal bool HotkeysExist(HotKey key, HotKey key2)
			{
				if(DeviceVolumeUp is HotKey volUp && (key.Equals(volUp) || key2.Equals(volUp)))
					return true;
				if(DeviceVolumeDown is HotKey volDown && (key.Equals(volDown) || key2.Equals(volDown)))
					return true;
				if(OpenMixer is HotKey open && (key.Equals(open) || key2.Equals(open)))
					return true;
				foreach(var (_, hotkeys) in this.serializableRegisteredSessions)
				{
					var up = hotkeys.up.ToHotKey();
					var down = hotkeys.down.ToHotKey();
					if(up.Equals(key) || up.Equals(key2) || down.Equals(key) || down.Equals(key2))
						return true;
				}

				foreach(var value in this.serializableRegisteredDevices)
				{
					var hotkey = value.Key.ToHotKey();
					if(hotkey.Equals(key) || hotkey.Equals(key2))
						return true;
				}
				return false;
			}

			internal void AddRegisteredSession(string sessionName, Tuple<HotKey, HotKey> hotkeys)
			{
				if(sessionName == null || hotkeys.Item1 == null || hotkeys.Item2 == null)
					return;
				this.serializableRegisteredSessions.Add(sessionName, hotkeys.ToTuple());
			}

			internal void RemoveRegisteredSession(string sessionName)
			{
				if(string.IsNullOrEmpty(sessionName))
					return;
				this.serializableRegisteredSessions.Remove(sessionName);
			}

			[OptionalField]
			private Dictionary<SerializableHotkey, (string id, string name)> serializableRegisteredDevices;

			internal ObservableConcurrentDictionary<HotKey, Tuple<string, string>> GetRegisteredDevices()
			{
				var dict = new ObservableConcurrentDictionary<HotKey, Tuple<string, string>>();
				if(this.serializableRegisteredDevices == null)
				{
					this.serializableRegisteredDevices = new Dictionary<SerializableHotkey, (string id, string name)>();
					return dict;
				}

				if(this.serializableRegisteredDevices.Count == 0)
					return dict;

				foreach(var tuple in this.serializableRegisteredDevices)
					dict.Add(tuple.Key.ToHotKey(), new Tuple<string, string>(tuple.Value.id, tuple.Value.name));

				return dict;
			}

			internal void AddRegisteredDevice(HotKey hotkey, string deviceId, string name)
			{
				this.serializableRegisteredDevices.Add(hotkey.ToSerializableHotkey(), (deviceId, name));
			}

			internal void RemoveRegisteredDevice(HotKey hotkey)
			{
				this.serializableRegisteredDevices.Remove(hotkey.ToSerializableHotkey());
			}

			[OnDeserialized]
			private void OnDeserialized(StreamingContext context)
			{
				//convert values from the old version of settings class to the new one
				if(this.MusicAppName != null && !VolumeUpIsEmpty && !VolumeDownIsEmpty)
				{
					this.serializableRegisteredSessions ??= new Dictionary<string, (SerializableHotkey, SerializableHotkey)>();
					this.serializableRegisteredSessions.Add(MusicAppName, (new SerializableHotkey(this.VolumeUpKey, this.VolumeUpModifiers),
						                                         new SerializableHotkey(this.VolumeDownKey, this.VolumeDownModifiers)));
					LogManager.GetLogger(typeof(AppSettings)).Info($"Converted old hotkeys to a new version. App: [{this.MusicAppName}] +vol: [{this.VolumeUpKey}] -vol: [{this.VolumeDownKey}]");
					this.MusicAppName = null;
					this.VolumeUpKey = VolumeDownKey = Key.None;
					this.VolumeUpModifiers = VolumeDownModifiers = ModifierKeys.None;
				}
			}
		}

		[Serializable]
		internal readonly struct SerializableHotkey
		{
			private readonly Key _key;
			private readonly ModifierKeys _modifierKeys;
			internal HotKey ToHotKey() => new HotKey(this._key, this._modifierKeys);

			internal SerializableHotkey(Key key, ModifierKeys modifierKeys)
			{
				_key = key;
				_modifierKeys = modifierKeys;
			}
		}
	}
}