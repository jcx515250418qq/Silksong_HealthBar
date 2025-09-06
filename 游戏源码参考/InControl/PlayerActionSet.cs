using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace InControl
{
	public abstract class PlayerActionSet
	{
		public BindingSourceType LastInputType;

		public ulong LastInputTypeChangedTick;

		public InputDeviceClass LastDeviceClass;

		public InputDeviceStyle LastDeviceStyle;

		private List<PlayerAction> actions = new List<PlayerAction>();

		private List<PlayerOneAxisAction> oneAxisActions = new List<PlayerOneAxisAction>();

		private List<PlayerTwoAxisAction> twoAxisActions = new List<PlayerTwoAxisAction>();

		private Dictionary<string, PlayerAction> actionsByName = new Dictionary<string, PlayerAction>();

		private BindingListenOptions listenOptions = new BindingListenOptions();

		internal PlayerAction listenWithAction;

		private InputDevice activeDevice;

		private const ushort currentDataFormatVersion = 2;

		public InputDevice Device { get; set; }

		public List<InputDevice> IncludeDevices { get; private set; }

		public List<InputDevice> ExcludeDevices { get; private set; }

		public ReadOnlyCollection<PlayerAction> Actions { get; private set; }

		public ulong UpdateTick { get; protected set; }

		public bool Enabled { get; set; }

		public bool PreventInputWhileListeningForBinding { get; set; }

		public object UserData { get; set; }

		public PlayerAction this[string actionName]
		{
			get
			{
				if (actionsByName.TryGetValue(actionName, out var value))
				{
					return value;
				}
				throw new KeyNotFoundException("Action '" + actionName + "' does not exist in this action set.");
			}
		}

		public bool IsListeningForBinding => listenWithAction != null;

		public BindingListenOptions ListenOptions
		{
			get
			{
				return listenOptions;
			}
			set
			{
				listenOptions = value ?? new BindingListenOptions();
			}
		}

		public InputDevice ActiveDevice => activeDevice ?? InputDevice.Null;

		public event Action<BindingSourceType> OnLastInputTypeChanged;

		protected PlayerActionSet()
		{
			Enabled = true;
			PreventInputWhileListeningForBinding = true;
			Device = null;
			IncludeDevices = new List<InputDevice>();
			ExcludeDevices = new List<InputDevice>();
			Actions = new ReadOnlyCollection<PlayerAction>(actions);
			InputManager.AttachPlayerActionSet(this);
		}

		public void Destroy()
		{
			this.OnLastInputTypeChanged = null;
			InputManager.DetachPlayerActionSet(this);
		}

		protected PlayerAction CreatePlayerAction(string name)
		{
			return new PlayerAction(name, this);
		}

		internal void AddPlayerAction(PlayerAction action)
		{
			action.Device = FindActiveDevice();
			if (actionsByName.ContainsKey(action.Name))
			{
				throw new InControlException("Action '" + action.Name + "' already exists in this set.");
			}
			actions.Add(action);
			actionsByName.Add(action.Name, action);
		}

		protected PlayerOneAxisAction CreateOneAxisPlayerAction(PlayerAction negativeAction, PlayerAction positiveAction)
		{
			PlayerOneAxisAction playerOneAxisAction = new PlayerOneAxisAction(negativeAction, positiveAction);
			oneAxisActions.Add(playerOneAxisAction);
			return playerOneAxisAction;
		}

		protected PlayerTwoAxisAction CreateTwoAxisPlayerAction(PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction)
		{
			PlayerTwoAxisAction playerTwoAxisAction = new PlayerTwoAxisAction(negativeXAction, positiveXAction, negativeYAction, positiveYAction);
			twoAxisActions.Add(playerTwoAxisAction);
			return playerTwoAxisAction;
		}

		public PlayerAction GetPlayerActionByName(string actionName)
		{
			if (actionsByName.TryGetValue(actionName, out var value))
			{
				return value;
			}
			return null;
		}

		internal void Update(ulong updateTick, float deltaTime)
		{
			InputDevice device = Device ?? FindActiveDevice();
			BindingSourceType lastInputType = LastInputType;
			ulong lastInputTypeChangedTick = LastInputTypeChangedTick;
			InputDeviceClass lastDeviceClass = LastDeviceClass;
			InputDeviceStyle lastDeviceStyle = LastDeviceStyle;
			int count = actions.Count;
			for (int i = 0; i < count; i++)
			{
				PlayerAction playerAction = actions[i];
				playerAction.Update(updateTick, deltaTime, device);
				if (playerAction.UpdateTick > UpdateTick)
				{
					UpdateTick = playerAction.UpdateTick;
					activeDevice = playerAction.ActiveDevice;
				}
				if (playerAction.LastInputTypeChangedTick > lastInputTypeChangedTick)
				{
					lastInputType = playerAction.LastInputType;
					lastInputTypeChangedTick = playerAction.LastInputTypeChangedTick;
					lastDeviceClass = playerAction.LastDeviceClass;
					lastDeviceStyle = playerAction.LastDeviceStyle;
				}
			}
			int count2 = oneAxisActions.Count;
			for (int j = 0; j < count2; j++)
			{
				oneAxisActions[j].Update(updateTick, deltaTime);
			}
			int count3 = twoAxisActions.Count;
			for (int k = 0; k < count3; k++)
			{
				twoAxisActions[k].Update(updateTick, deltaTime);
			}
			if (lastInputTypeChangedTick > LastInputTypeChangedTick)
			{
				bool flag = lastInputType != LastInputType;
				LastInputType = lastInputType;
				LastInputTypeChangedTick = lastInputTypeChangedTick;
				LastDeviceClass = lastDeviceClass;
				LastDeviceStyle = lastDeviceStyle;
				if (this.OnLastInputTypeChanged != null && flag)
				{
					this.OnLastInputTypeChanged(lastInputType);
				}
			}
		}

		public void Reset()
		{
			int count = actions.Count;
			for (int i = 0; i < count; i++)
			{
				actions[i].ResetBindings();
			}
		}

		private InputDevice FindActiveDevice()
		{
			bool flag = IncludeDevices.Count > 0;
			bool flag2 = ExcludeDevices.Count > 0;
			if (flag || flag2)
			{
				InputDevice inputDevice = InputDevice.Null;
				int count = InputManager.Devices.Count;
				for (int i = 0; i < count; i++)
				{
					InputDevice inputDevice2 = InputManager.Devices[i];
					if (inputDevice2 != inputDevice && inputDevice2.LastInputAfter(inputDevice) && !inputDevice2.Passive && (!flag2 || !ExcludeDevices.Contains(inputDevice2)) && (!flag || IncludeDevices.Contains(inputDevice2)))
					{
						inputDevice = inputDevice2;
					}
				}
				return inputDevice;
			}
			return InputManager.ActiveDevice;
		}

		public void ClearInputState()
		{
			int count = actions.Count;
			for (int i = 0; i < count; i++)
			{
				actions[i].ClearInputState();
			}
			int count2 = oneAxisActions.Count;
			for (int j = 0; j < count2; j++)
			{
				oneAxisActions[j].ClearInputState();
			}
			int count3 = twoAxisActions.Count;
			for (int k = 0; k < count3; k++)
			{
				twoAxisActions[k].ClearInputState();
			}
		}

		public bool HasBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return false;
			}
			int count = actions.Count;
			for (int i = 0; i < count; i++)
			{
				if (actions[i].HasBinding(binding))
				{
					return true;
				}
			}
			return false;
		}

		public void RemoveBinding(BindingSource binding)
		{
			if (!(binding == null))
			{
				int count = actions.Count;
				for (int i = 0; i < count; i++)
				{
					actions[i].RemoveBinding(binding);
				}
			}
		}

		public byte[] SaveData()
		{
			using MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8))
			{
				binaryWriter.Write((byte)66);
				binaryWriter.Write((byte)73);
				binaryWriter.Write((byte)78);
				binaryWriter.Write((byte)68);
				binaryWriter.Write((ushort)2);
				int count = actions.Count;
				binaryWriter.Write(count);
				for (int i = 0; i < count; i++)
				{
					actions[i].Save(binaryWriter);
				}
			}
			return memoryStream.ToArray();
		}

		public void LoadData(byte[] data)
		{
			if (data == null)
			{
				return;
			}
			try
			{
				using MemoryStream input = new MemoryStream(data);
				using BinaryReader binaryReader = new BinaryReader(input);
				if (binaryReader.ReadUInt32() != 1145981250)
				{
					throw new Exception("Unknown data format.");
				}
				ushort num = binaryReader.ReadUInt16();
				if (num < 1 || num > 2)
				{
					throw new Exception("Unknown data format version: " + num);
				}
				int num2 = binaryReader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					if (actionsByName.TryGetValue(binaryReader.ReadString(), out var value))
					{
						value.Load(binaryReader, num);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError("Provided state could not be loaded:\n" + ex.Message);
				Reset();
			}
		}

		public string Save()
		{
			return Convert.ToBase64String(SaveData());
		}

		public void Load(string data)
		{
			if (data == null)
			{
				return;
			}
			try
			{
				LoadData(Convert.FromBase64String(data));
			}
			catch (Exception ex)
			{
				Logger.LogError("Provided state could not be loaded:\n" + ex.Message);
				Reset();
			}
		}
	}
}
