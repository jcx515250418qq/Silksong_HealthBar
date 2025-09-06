using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace InControl
{
	public class PlayerAction : OneAxisInputControl
	{
		public BindingListenOptions ListenOptions;

		public BindingSourceType LastInputType;

		public ulong LastInputTypeChangedTick;

		public InputDeviceClass LastDeviceClass;

		public InputDeviceStyle LastDeviceStyle;

		private readonly List<BindingSource> defaultBindings = new List<BindingSource>();

		private readonly List<BindingSource> regularBindings = new List<BindingSource>();

		private readonly List<BindingSource> visibleBindings = new List<BindingSource>();

		private readonly ReadOnlyCollection<BindingSource> bindings;

		private readonly ReadOnlyCollection<BindingSource> unfilteredBindings;

		private readonly BindingSourceListener[] bindingSourceListeners = new BindingSourceListener[4]
		{
			new DeviceBindingSourceListener(),
			new UnknownDeviceBindingSourceListener(),
			new KeyBindingSourceListener(),
			new MouseBindingSourceListener()
		};

		private bool triggerBindingEnded;

		private bool triggerBindingChanged;

		private InputDevice device;

		private InputDevice activeDevice;

		public string Name { get; private set; }

		public PlayerActionSet Owner { get; private set; }

		public object UserData { get; set; }

		public bool IsListeningForBinding => Owner.listenWithAction == this;

		public ReadOnlyCollection<BindingSource> Bindings => bindings;

		public ReadOnlyCollection<BindingSource> UnfilteredBindings => unfilteredBindings;

		internal InputDevice Device
		{
			get
			{
				if (device == null)
				{
					device = Owner.Device;
					UpdateVisibleBindings();
				}
				return device;
			}
			set
			{
				if (device != value)
				{
					device = value;
					UpdateVisibleBindings();
				}
			}
		}

		public InputDevice ActiveDevice => activeDevice ?? InputDevice.Null;

		private bool LastInputTypeIsDevice
		{
			get
			{
				if (LastInputType != BindingSourceType.DeviceBindingSource)
				{
					return LastInputType == BindingSourceType.UnknownDeviceBindingSource;
				}
				return true;
			}
		}

		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float LowerDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float UpperDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public event Action<BindingSourceType> OnLastInputTypeChanged;

		public event Action OnBindingsChanged;

		public PlayerAction(string name, PlayerActionSet owner)
		{
			Raw = true;
			Name = name;
			Owner = owner;
			bindings = new ReadOnlyCollection<BindingSource>(visibleBindings);
			unfilteredBindings = new ReadOnlyCollection<BindingSource>(regularBindings);
			owner.AddPlayerAction(this);
		}

		public void AddDefaultBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return;
			}
			if (binding.BoundTo != null)
			{
				throw new InControlException("Binding source is already bound to action " + binding.BoundTo.Name);
			}
			if (!defaultBindings.Contains(binding))
			{
				defaultBindings.Add(binding);
				binding.BoundTo = this;
			}
			if (!regularBindings.Contains(binding))
			{
				regularBindings.Add(binding);
				binding.BoundTo = this;
				if (binding.IsValid)
				{
					visibleBindings.Add(binding);
				}
			}
		}

		public void AddDefaultBinding(params Key[] keys)
		{
			AddDefaultBinding(new KeyBindingSource(keys));
		}

		public void AddDefaultBinding(KeyCombo keyCombo)
		{
			AddDefaultBinding(new KeyBindingSource(keyCombo));
		}

		public void AddDefaultBinding(Mouse control)
		{
			AddDefaultBinding(new MouseBindingSource(control));
		}

		public void AddDefaultBinding(InputControlType control)
		{
			AddDefaultBinding(new DeviceBindingSource(control));
		}

		public bool AddBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return false;
			}
			if (binding.BoundTo != null)
			{
				return false;
			}
			if (regularBindings.Contains(binding))
			{
				return false;
			}
			regularBindings.Add(binding);
			binding.BoundTo = this;
			if (binding.IsValid)
			{
				visibleBindings.Add(binding);
			}
			triggerBindingChanged = true;
			return true;
		}

		public bool InsertBindingAt(int index, BindingSource binding)
		{
			if (index < 0 || index > visibleBindings.Count)
			{
				throw new InControlException("Index is out of range for bindings on this action.");
			}
			if (index == visibleBindings.Count)
			{
				return AddBinding(binding);
			}
			if (binding == null)
			{
				return false;
			}
			if (binding.BoundTo != null)
			{
				Logger.LogWarning("Binding source is already bound to action " + binding.BoundTo.Name);
				return false;
			}
			if (regularBindings.Contains(binding))
			{
				return false;
			}
			int index2 = ((index != 0) ? regularBindings.IndexOf(visibleBindings[index]) : 0);
			regularBindings.Insert(index2, binding);
			binding.BoundTo = this;
			if (binding.IsValid)
			{
				visibleBindings.Insert(index, binding);
			}
			triggerBindingChanged = true;
			return true;
		}

		public bool ReplaceBinding(BindingSource findBinding, BindingSource withBinding)
		{
			if (findBinding == null || withBinding == null)
			{
				return false;
			}
			if (withBinding.BoundTo != null)
			{
				Logger.LogWarning("Binding source is already bound to action " + withBinding.BoundTo.Name);
				return false;
			}
			int num = regularBindings.IndexOf(findBinding);
			if (num < 0)
			{
				Logger.LogWarning("Binding source to replace is not present in this action.");
				return false;
			}
			findBinding.BoundTo = null;
			regularBindings[num] = withBinding;
			withBinding.BoundTo = this;
			num = visibleBindings.IndexOf(findBinding);
			if (num >= 0)
			{
				visibleBindings[num] = withBinding;
			}
			triggerBindingChanged = true;
			return true;
		}

		public bool HasBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return false;
			}
			BindingSource bindingSource = FindBinding(binding);
			if (bindingSource == null)
			{
				return false;
			}
			return bindingSource.BoundTo == this;
		}

		public BindingSource FindBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return null;
			}
			int num = regularBindings.IndexOf(binding);
			if (num >= 0)
			{
				return regularBindings[num];
			}
			return null;
		}

		private void HardRemoveBinding(BindingSource binding)
		{
			if (binding == null)
			{
				return;
			}
			int num = regularBindings.IndexOf(binding);
			if (num >= 0)
			{
				BindingSource bindingSource = regularBindings[num];
				if (bindingSource.BoundTo == this)
				{
					bindingSource.BoundTo = null;
					regularBindings.RemoveAt(num);
					UpdateVisibleBindings();
					triggerBindingChanged = true;
				}
			}
		}

		public void RemoveBinding(BindingSource binding)
		{
			BindingSource bindingSource = FindBinding(binding);
			if (bindingSource != null && bindingSource.BoundTo == this)
			{
				bindingSource.BoundTo = null;
				triggerBindingChanged = true;
			}
		}

		public void RemoveBindingAt(int index)
		{
			if (index < 0 || index >= regularBindings.Count)
			{
				throw new InControlException("Index is out of range for bindings on this action.");
			}
			regularBindings[index].BoundTo = null;
			triggerBindingChanged = true;
		}

		private int CountBindingsOfType(BindingSourceType bindingSourceType)
		{
			int num = 0;
			int count = regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = regularBindings[i];
				if (bindingSource.BoundTo == this && bindingSource.BindingSourceType == bindingSourceType)
				{
					num++;
				}
			}
			return num;
		}

		private void RemoveFirstBindingOfType(BindingSourceType bindingSourceType)
		{
			int count = regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = regularBindings[i];
				if (bindingSource.BoundTo == this && bindingSource.BindingSourceType == bindingSourceType)
				{
					bindingSource.BoundTo = null;
					regularBindings.RemoveAt(i);
					triggerBindingChanged = true;
					break;
				}
			}
		}

		private int IndexOfFirstInvalidBinding()
		{
			int count = regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				if (!regularBindings[i].IsValid)
				{
					return i;
				}
			}
			return -1;
		}

		public void ClearBindings()
		{
			int count = regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				regularBindings[i].BoundTo = null;
			}
			regularBindings.Clear();
			visibleBindings.Clear();
			triggerBindingChanged = true;
		}

		public void ResetBindings()
		{
			ClearBindings();
			regularBindings.AddRange(defaultBindings);
			int count = regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = regularBindings[i];
				bindingSource.BoundTo = this;
				if (bindingSource.IsValid)
				{
					visibleBindings.Add(bindingSource);
				}
			}
			triggerBindingChanged = true;
		}

		public void ListenForBinding()
		{
			ListenForBindingReplacing(null);
		}

		public void ListenForBindingReplacing(BindingSource binding)
		{
			(ListenOptions ?? Owner.ListenOptions).ReplaceBinding = binding;
			Owner.listenWithAction = this;
			int num = bindingSourceListeners.Length;
			for (int i = 0; i < num; i++)
			{
				bindingSourceListeners[i].Reset();
			}
		}

		public void StopListeningForBinding()
		{
			if (IsListeningForBinding)
			{
				Owner.listenWithAction = null;
				triggerBindingEnded = true;
			}
		}

		private void RemoveOrphanedBindings()
		{
			for (int num = regularBindings.Count - 1; num >= 0; num--)
			{
				if (regularBindings[num].BoundTo != this)
				{
					regularBindings.RemoveAt(num);
				}
			}
		}

		internal void Update(ulong updateTick, float deltaTime, InputDevice device)
		{
			Device = device;
			UpdateBindings(updateTick, deltaTime);
			if (triggerBindingChanged)
			{
				if (this.OnBindingsChanged != null)
				{
					this.OnBindingsChanged();
				}
				triggerBindingChanged = false;
			}
			if (triggerBindingEnded)
			{
				(ListenOptions ?? Owner.ListenOptions).CallOnBindingEnded(this);
				triggerBindingEnded = false;
			}
			DetectBindings();
		}

		private void UpdateBindings(ulong updateTick, float deltaTime)
		{
			bool flag = IsListeningForBinding || (Owner.IsListeningForBinding && Owner.PreventInputWhileListeningForBinding);
			BindingSourceType bindingSourceType = LastInputType;
			ulong num = LastInputTypeChangedTick;
			ulong updateTick2 = base.UpdateTick;
			InputDeviceClass lastDeviceClass = LastDeviceClass;
			InputDeviceStyle lastDeviceStyle = LastDeviceStyle;
			int count = regularBindings.Count;
			for (int num2 = count - 1; num2 >= 0; num2--)
			{
				BindingSource bindingSource = regularBindings[num2];
				if (bindingSource.BoundTo != this)
				{
					regularBindings.RemoveAt(num2);
					visibleBindings.Remove(bindingSource);
					triggerBindingChanged = true;
				}
				else if (!flag)
				{
					float value = bindingSource.GetValue(Device);
					if (UpdateWithValue(value, updateTick, deltaTime))
					{
						bindingSourceType = bindingSource.BindingSourceType;
						num = updateTick;
						lastDeviceClass = bindingSource.DeviceClass;
						lastDeviceStyle = bindingSource.DeviceStyle;
					}
				}
			}
			if (flag || count == 0)
			{
				UpdateWithValue(0f, updateTick, deltaTime);
			}
			Commit();
			ownerEnabled = Owner.Enabled;
			if (num > LastInputTypeChangedTick && (bindingSourceType != BindingSourceType.MouseBindingSource || Utility.Abs(base.LastValue - base.Value) >= MouseBindingSource.JitterThreshold))
			{
				bool flag2 = bindingSourceType != LastInputType;
				LastInputType = bindingSourceType;
				LastInputTypeChangedTick = num;
				LastDeviceClass = lastDeviceClass;
				LastDeviceStyle = lastDeviceStyle;
				if (this.OnLastInputTypeChanged != null && flag2)
				{
					this.OnLastInputTypeChanged(bindingSourceType);
				}
			}
			if (base.UpdateTick > updateTick2)
			{
				activeDevice = (LastInputTypeIsDevice ? Device : null);
			}
		}

		private void DetectBindings()
		{
			if (!IsListeningForBinding)
			{
				return;
			}
			BindingSource bindingSource = null;
			BindingListenOptions bindingListenOptions = ListenOptions ?? Owner.ListenOptions;
			int num = bindingSourceListeners.Length;
			for (int i = 0; i < num; i++)
			{
				bindingSource = bindingSourceListeners[i].Listen(bindingListenOptions, device);
				if (bindingSource != null)
				{
					break;
				}
			}
			if (bindingSource == null || !bindingListenOptions.CallOnBindingFound(this, bindingSource))
			{
				return;
			}
			if (HasBinding(bindingSource))
			{
				if (bindingListenOptions.RejectRedundantBindings)
				{
					bindingListenOptions.CallOnBindingRejected(this, bindingSource, BindingSourceRejectionType.DuplicateBindingOnAction);
					return;
				}
				StopListeningForBinding();
				bindingListenOptions.CallOnBindingAdded(this, bindingSource);
				return;
			}
			if (bindingListenOptions.UnsetDuplicateBindingsOnSet)
			{
				int count = Owner.Actions.Count;
				for (int j = 0; j < count; j++)
				{
					Owner.Actions[j].HardRemoveBinding(bindingSource);
				}
			}
			if (!bindingListenOptions.AllowDuplicateBindingsPerSet && Owner.HasBinding(bindingSource))
			{
				bindingListenOptions.CallOnBindingRejected(this, bindingSource, BindingSourceRejectionType.DuplicateBindingOnActionSet);
				return;
			}
			StopListeningForBinding();
			if (bindingListenOptions.ReplaceBinding == null)
			{
				if (bindingListenOptions.MaxAllowedBindingsPerType != 0)
				{
					while (CountBindingsOfType(bindingSource.BindingSourceType) >= bindingListenOptions.MaxAllowedBindingsPerType)
					{
						RemoveFirstBindingOfType(bindingSource.BindingSourceType);
					}
				}
				else if (bindingListenOptions.MaxAllowedBindings != 0)
				{
					while (regularBindings.Count >= bindingListenOptions.MaxAllowedBindings)
					{
						int index = Mathf.Max(0, IndexOfFirstInvalidBinding());
						regularBindings.RemoveAt(index);
						triggerBindingChanged = true;
					}
				}
				AddBinding(bindingSource);
			}
			else
			{
				ReplaceBinding(bindingListenOptions.ReplaceBinding, bindingSource);
			}
			UpdateVisibleBindings();
			bindingListenOptions.CallOnBindingAdded(this, bindingSource);
		}

		private void UpdateVisibleBindings()
		{
			visibleBindings.Clear();
			int count = regularBindings.Count;
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = regularBindings[i];
				if (bindingSource.IsValid)
				{
					visibleBindings.Add(bindingSource);
				}
			}
		}

		internal void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			ClearBindings();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				BindingSourceType bindingSourceType = (BindingSourceType)reader.ReadInt32();
				BindingSource bindingSource;
				switch (bindingSourceType)
				{
				case BindingSourceType.DeviceBindingSource:
					bindingSource = new DeviceBindingSource();
					break;
				case BindingSourceType.KeyBindingSource:
					bindingSource = new KeyBindingSource();
					break;
				case BindingSourceType.MouseBindingSource:
					bindingSource = new MouseBindingSource();
					break;
				case BindingSourceType.UnknownDeviceBindingSource:
					bindingSource = new UnknownDeviceBindingSource();
					break;
				default:
					throw new InControlException("Don't know how to load BindingSourceType: " + bindingSourceType);
				case BindingSourceType.None:
					continue;
				}
				bindingSource.Load(reader, dataFormatVersion);
				AddBinding(bindingSource);
			}
		}

		internal void Save(BinaryWriter writer)
		{
			RemoveOrphanedBindings();
			writer.Write(Name);
			int count = regularBindings.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				BindingSource bindingSource = regularBindings[i];
				writer.Write((int)bindingSource.BindingSourceType);
				bindingSource.Save(writer);
			}
		}
	}
}
