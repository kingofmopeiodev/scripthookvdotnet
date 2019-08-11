using GTA.Native;
using System;
using System.Runtime.InteropServices;

namespace GTA
{
	[StructLayout(LayoutKind.Explicit, Size = 0x138)]
	internal unsafe struct DlcWeaponData
	{
		[FieldOffset(0x00)] private int validCheck;

		[FieldOffset(0x08)] private int weaponHash;

		[FieldOffset(0x18)] private int weaponCost;

		[FieldOffset(0x20)] private int ammoCost;

		[FieldOffset(0x28)] private int ammoType;

		[FieldOffset(0x30)] private int defaultClipSize;

		[FieldOffset(0x38)] private fixed byte name[0x40];

		[FieldOffset(0x78)] private fixed byte desc[0x40];

		[FieldOffset(0xB8)] private fixed byte simpleDesc[0x40]; //usually refers to "the " + name

		[FieldOffset(0xF8)] private fixed byte upperCaseName[0x40];

		public WeaponHash Hash => (WeaponHash)weaponHash;

		public string DisplayName
		{
			get
			{
				fixed (byte* ptr = name)
				{
					return SHVDN.NativeMemory.PtrToStringUTF8(new IntPtr(ptr));
				}
			}
		}
	}
}
