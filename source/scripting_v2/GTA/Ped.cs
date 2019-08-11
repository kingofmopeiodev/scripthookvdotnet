using System;
using GTA.Math;
using GTA.Native;

namespace GTA
{
	public sealed class Ped : Entity
	{
		Tasks _tasks;
		NaturalMotion.Euphoria _euphoria;
		WeaponCollection _weapons;

		internal static readonly string[] _speechModifierNames = {
			"SPEECH_PARAMS_STANDARD",
			"SPEECH_PARAMS_ALLOW_REPEAT",
			"SPEECH_PARAMS_BEAT",
			"SPEECH_PARAMS_FORCE",
			"SPEECH_PARAMS_FORCE_FRONTEND",
			"SPEECH_PARAMS_FORCE_NO_REPEAT_FRONTEND",
			"SPEECH_PARAMS_FORCE_NORMAL",
			"SPEECH_PARAMS_FORCE_NORMAL_CLEAR",
			"SPEECH_PARAMS_FORCE_NORMAL_CRITICAL",
			"SPEECH_PARAMS_FORCE_SHOUTED",
			"SPEECH_PARAMS_FORCE_SHOUTED_CLEAR",
			"SPEECH_PARAMS_FORCE_SHOUTED_CRITICAL",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY",
			"SPEECH_PARAMS_MEGAPHONE",
			"SPEECH_PARAMS_HELI",
			"SPEECH_PARAMS_FORCE_MEGAPHONE",
			"SPEECH_PARAMS_FORCE_HELI",
			"SPEECH_PARAMS_INTERRUPT",
			"SPEECH_PARAMS_INTERRUPT_SHOUTED",
			"SPEECH_PARAMS_INTERRUPT_SHOUTED_CLEAR",
			"SPEECH_PARAMS_INTERRUPT_SHOUTED_CRITICAL",
			"SPEECH_PARAMS_INTERRUPT_NO_FORCE",
			"SPEECH_PARAMS_INTERRUPT_FRONTEND",
			"SPEECH_PARAMS_INTERRUPT_NO_FORCE_FRONTEND",
			"SPEECH_PARAMS_ADD_BLIP",
			"SPEECH_PARAMS_ADD_BLIP_ALLOW_REPEAT",
			"SPEECH_PARAMS_ADD_BLIP_FORCE",
			"SPEECH_PARAMS_ADD_BLIP_SHOUTED",
			"SPEECH_PARAMS_ADD_BLIP_SHOUTED_FORCE",
			"SPEECH_PARAMS_ADD_BLIP_INTERRUPT",
			"SPEECH_PARAMS_ADD_BLIP_INTERRUPT_FORCE",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CLEAR",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CRITICAL",
			"SPEECH_PARAMS_SHOUTED",
			"SPEECH_PARAMS_SHOUTED_CLEAR",
			"SPEECH_PARAMS_SHOUTED_CRITICAL",
		};

		public Ped(int handle) : base(handle)
		{
		}

		public int Accuracy
		{
			get => Function.Call<int>(Hash.GET_PED_ACCURACY, Handle);
			set => Function.Call(Hash.SET_PED_ACCURACY, Handle, value);
		}

		public bool AlwaysDiesOnLowHealth
		{
			set => Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, Handle, value);
		}

		public bool AlwaysKeepTask
		{
			set => Function.Call(Hash.SET_PED_KEEP_TASK, Handle, value);
		}

		public int Armor
		{
			get => Function.Call<int>(Hash.GET_PED_ARMOUR, Handle);
			set => Function.Call(Hash.SET_PED_ARMOUR, Handle, value);
		}

		public bool BlockPermanentEvents
		{
			set => Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, Handle, value);
		}

		public bool CanRagdoll
		{
			get => Function.Call<bool>(Hash.CAN_PED_RAGDOLL, Handle);
			set => Function.Call(Hash.SET_PED_CAN_RAGDOLL, Handle, value);
		}

		public bool CanSwitchWeapons
		{
			set => Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, Handle, value);
		}

		public bool CanSufferCriticalHits
		{
			get
			{
				var address = SHVDN.NativeMemory.GetEntityAddress(Handle);
				if (address == IntPtr.Zero)
				{
					return false;
				}

				int offset = Game.Version >= GameVersion.VER_1_0_372_2_STEAM ? 0x13BC : 0x13AC;
				offset = (Game.Version >= GameVersion.VER_1_0_877_1_STEAM ? 0x13E4 : offset);
				offset = Game.Version >= GameVersion.VER_1_0_944_2_STEAM ? 0x13F4 : offset;

				return (SHVDN.NativeMemory.ReadByte(address + offset) & (1 << 2)) == 0;
			}
			set => Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, Handle, value);
		}

		public bool CanFlyThroughWindscreen
		{
			get => Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, 32, true);
			set => Function.Call(Hash.SET_PED_CONFIG_FLAG, Handle, 32, value);
		}

		public bool CanBeKnockedOffBike
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_KNOCKED_OFF_VEHICLE, Handle, value);
		}

		public bool CanBeDraggedOutOfVehicle
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_DRAGGED_OUT, Handle, value);
		}

		public bool CanBeTargetted
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_TARGETTED, Handle, value);
		}

		public bool CanBeShotInVehicle
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_SHOT_IN_VEHICLE, Handle, value);
		}

		public bool CanPlayGestures
		{
			set => Function.Call(Hash.SET_PED_CAN_PLAY_GESTURE_ANIMS, Handle, value);
		}

		public bool CanWearHelmet
		{
			set => Function.Call(Hash.SET_PED_HELMET, Handle, value);
		}

		public bool CanWrithe
		{
			get => !GetConfigFlag(281);
			set => SetConfigFlag(281, !value);
		}

		public PedGroup CurrentPedGroup
		{
			get
			{
				return IsInGroup ? Function.Call<PedGroup>(Hash.GET_PED_GROUP_INDEX, Handle, false) : null;
			}
		}

		public Vehicle CurrentVehicle => Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_IN, Handle, false);

		public bool DiesInstantlyInWater
		{
			set => Function.Call(Hash.SET_PED_DIES_INSTANTLY_IN_WATER, Handle, value);
		}

		public float DrivingSpeed
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_CRUISE_SPEED, Handle, value);
		}

		public DrivingStyle DrivingStyle
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, Handle, (int)value);
		}

		public bool DrownsInWater
		{
			set => Function.Call(Hash.SET_PED_DIES_IN_WATER, Handle, value);
		}

		public bool DropsWeaponsOnDeath
		{
			get
			{
				IntPtr address = SHVDN.NativeMemory.GetEntityAddress(Handle);
				if (address == IntPtr.Zero)
					return false;

				int offset = (Game.Version >= GameVersion.VER_1_0_877_1_STEAM ? 0x13E5 : 0x13BD);
				offset = (Game.Version >= GameVersion.VER_1_0_944_2_STEAM ? 0x13F5 : offset);

				return SHVDN.NativeMemory.IsBitSet(address + offset, 6);
			}
			set => Function.Call(Hash.SET_PED_DROPS_WEAPONS_WHEN_DEAD, Handle, value);
		}

		public bool DrownsInSinkingVehicle
		{
			set => Function.Call(Hash.SET_PED_DIES_IN_SINKING_VEHICLE, Handle, value);
		}

		public NaturalMotion.Euphoria Euphoria
		{
			get
			{
				if (_euphoria is null)
				{
					_euphoria = new NaturalMotion.Euphoria(this);
				}
				return _euphoria;
			}
		}

		public FiringPattern FiringPattern
		{
			set => Function.Call(Hash.SET_PED_FIRING_PATTERN, Handle, (int)value);
		}

		public Gender Gender => Function.Call<bool>(Hash.IS_PED_MALE, Handle) ? Gender.Male : Gender.Female;

		public string MovementAnimationSet
		{
			set
			{
				Function.Call(Hash.REQUEST_ANIM_SET, value);
				var endtime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, 1000);

				while (!Function.Call<bool>(Hash.HAS_ANIM_SET_LOADED, value))
				{
					Script.Yield();

					if (DateTime.UtcNow >= endtime)
					{
						return;
					}
				}

				Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, value, 1.0f);
			}
		}

		public bool IsAimingFromCover
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_AIMING_FROM_COVER, Handle);
			}
		}

		public bool IsCuffed
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_CUFFED, Handle);
			}
		}

		public bool IsBeingJacked
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_BEING_JACKED, Handle);
			}
		}

		public bool IsBeingStealthKilled
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_BEING_STEALTH_KILLED, Handle);
			}
		}
		public bool IsBeingStunned
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_BEING_STUNNED, Handle);
			}
		}

		public bool IsDoingDriveBy
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_DOING_DRIVEBY, Handle);
			}
		}

		public bool IsDucking
		{
			get => Function.Call<bool>(Hash.IS_PED_DUCKING, Handle);
			set => Function.Call(Hash.SET_PED_DUCKING, Handle, value);
		}

		public bool IsEnemy
		{
			set => Function.Call(Hash.SET_PED_AS_ENEMY, Handle, value);
		}

		public bool IsHuman
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_HUMAN, Handle);
			}
		}

		public bool IsIdle
		{
			get
			{
				return !IsInjured && !IsRagdoll && !IsInAir && !IsOnFire && !IsDucking && !IsGettingIntoAVehicle && !IsInCombat && !IsInMeleeCombat && !(IsInVehicle() && !IsSittingInVehicle());
			}
		}

		public bool IsProne
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_PRONE, Handle);
			}
		}

		public bool IsGettingUp
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_GETTING_UP, Handle);
			}
		}

		public bool IsGettingIntoAVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, Handle);
			}
		}

		public bool IsFalling
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_FALLING, Handle);
			}
		}

		public bool IsJumping
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_JUMPING, Handle);
			}
		}

		public bool IsClimbing
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_CLIMBING, Handle);
			}
		}

		public bool IsVaulting
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_VAULTING, Handle);
			}
		}

		public bool IsDiving
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_DIVING, Handle);
			}
		}

		public bool IsGoingIntoCover
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_GOING_INTO_COVER, Handle);
			}
		}

		public bool IsFleeing
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_FLEEING, Handle);
			}
		}

		public bool IsInjured
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_INJURED, Handle);
			}
		}

		public bool IsInBoat
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_BOAT, Handle);
			}
		}

		public bool IsInCombat
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_COMBAT, Handle);
			}
		}

		public bool IsInCoverFacingLeft
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_COVER_FACING_LEFT, Handle);
			}
		}

		public bool IsInGroup
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_GROUP, Handle);
			}
		}

		public bool IsInFlyingVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_FLYING_VEHICLE, Handle);
			}
		}

		public bool IsInHeli
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_HELI, Handle);
			}
		}

		public bool IsInMeleeCombat
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_MELEE_COMBAT, Handle);
			}
		}

		public bool IsInPlane
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_PLANE, Handle);
			}
		}

		public bool IsInPoliceVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_POLICE_VEHICLE, Handle);
			}
		}

		public bool IsInParachuteFreeFall
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_PARACHUTE_FREE_FALL, Handle);
			}
		}

		public bool IsInSub
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_SUB, Handle);
			}
		}

		public bool IsInTrain
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_TRAIN, Handle);
			}
		}

		public bool IsInTaxi
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_TAXI, Handle);
			}
		}

		public bool IsJacking
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_JACKING, Handle);
			}
		}

		public bool IsOnFoot
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_ON_FOOT, Handle);
			}
		}

		public bool IsOnBike
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_ON_ANY_BIKE, Handle);
			}
		}

		public bool IsPerformingStealthKill
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_PERFORMING_STEALTH_KILL, Handle);
			}
		}

		public bool IsPlayer
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_A_PLAYER, Handle);
			}
		}

		public bool IsPriorityTargetForEnemies
		{
			set => Function.Call(Hash.SET_ENTITY_IS_TARGET_PRIORITY, Handle, value, 0);
		}

		public bool IsRagdoll
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_RAGDOLL, Handle);
			}
		}

		public bool IsReloading
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_RELOADING, Handle);
			}
		}

		public bool IsRunning
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_RUNNING, Handle);
			}
		}

		public bool IsShooting
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SHOOTING, Handle);
			}
		}

		public bool IsSprinting
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SPRINTING, Handle);
			}
		}

		public bool IsStopped
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_STOPPED, Handle);
			}
		}

		public bool IsSwimming
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SWIMMING, Handle);
			}
		}

		public bool IsSwimmingUnderWater
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SWIMMING_UNDER_WATER, Handle);
			}
		}

		public bool IsTryingToEnterALockedVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, Handle);
			}
		}

		public bool IsWalking
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_WALKING, Handle);
			}
		}

		public bool IsWearingHelmet
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_WEARING_HELMET, Handle);
			}
		}

		public bool IsJumpingOutOfVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_JUMPING_OUT_OF_VEHICLE, Handle);
			}
		}

		public Vehicle LastVehicle
		{
			get
			{
				return Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_IN, Handle, true);
			}
		}

		public float MaxDrivingSpeed
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_MAX_CRUISE_SPEED, Handle, value);
		}

		public override int MaxHealth
		{
			get => Function.Call<int>(Hash.GET_PED_MAX_HEALTH, Handle);
			set => Function.Call(Hash.SET_PED_MAX_HEALTH, Handle, value);
		}

		public int Money
		{
			get => Function.Call<int>(Hash.GET_PED_MONEY, Handle);
			set => Function.Call(Hash.SET_PED_MONEY, Handle, value);
		}

		public bool NeverLeavesGroup
		{
			set => Function.Call(Hash.SET_PED_NEVER_LEAVES_GROUP, Handle, value);
		}

		public bool StaysInVehicleWhenJacked
		{
			set => Function.Call(Hash.SET_PED_STAY_IN_VEHICLE_WHEN_JACKED, Handle, value);
		}

		public int RelationshipGroup
		{
			get => Function.Call<int>(Hash.GET_PED_RELATIONSHIP_GROUP_HASH, Handle);
			set => Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, Handle, value);
		}

		public VehicleSeat SeatIndex
		{
			get
			{
				var address = SHVDN.NativeMemory.GetEntityAddress(Handle);
				if (address == IntPtr.Zero)
				{
					return VehicleSeat.None;
				}

				int offset = (Game.Version >= GameVersion.VER_1_0_877_1_STEAM ? 0x158A : 0x1542);
				offset = Game.Version >= GameVersion.VER_1_0_944_2_STEAM ? 0x159A : offset;
				offset = Game.Version >= GameVersion.VER_1_0_1290_1_STEAM ? 0x15A2 : offset;

				int seatIndex = SHVDN.NativeMemory.ReadByte(address + offset);

				if (seatIndex == -1 || !IsInVehicle())
				{
					return VehicleSeat.None;
				}

				return (VehicleSeat)(seatIndex - 1);
			}
		}

		public int ShootRate
		{
			set => Function.Call(Hash.SET_PED_SHOOT_RATE, Handle, value);
		}

		public float Sweat
		{
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				if (value > 100)
				{
					value = 100;
				}

				Function.Call(Hash.SET_PED_SWEAT, Handle, value);
			}
		}

		public Tasks Task
		{
			get
			{
				if (_tasks is null)
				{
					_tasks = new Tasks(this);
				}
				return _tasks;
			}
		}

		public int TaskSequenceProgress
		{
			get
			{
				return Function.Call<int>(Hash.GET_SEQUENCE_PROGRESS, Handle);
			}
		}

		public string Voice
		{
			set => Function.Call(Hash.SET_AMBIENT_VOICE_NAME, Handle, value);
		}

		public bool WasKilledByStealth
		{
			get
			{
				return Function.Call<bool>(Hash.WAS_PED_KILLED_BY_STEALTH, Handle);
			}
		}

		public bool WasKilledByTakedown
		{
			get
			{
				return Function.Call<bool>(Hash.WAS_PED_KILLED_BY_TAKEDOWN, Handle);
			}
		}

		public WeaponCollection Weapons
		{
			get
			{
				if (_weapons is null)
				{
					_weapons = new WeaponCollection(this);
				}
				return _weapons;
			}
		}

		public float WetnessHeight
		{
			set => Function.Call<float>(Hash.SET_PED_WETNESS_HEIGHT, Handle, value);
		}

		public bool IsInVehicle()
		{
			return Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Handle, 0);
		}
		public bool IsInVehicle(Vehicle vehicle)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Handle, vehicle.Handle, 0);
		}
		public bool IsSittingInVehicle()
		{
			return Function.Call<bool>(Hash.IS_PED_SITTING_IN_ANY_VEHICLE, Handle);
		}
		public bool IsSittingInVehicle(Vehicle vehicle)
		{
			return Function.Call<bool>(Hash.IS_PED_SITTING_IN_VEHICLE, Handle, vehicle.Handle);
		}
		public Relationship GetRelationshipWithPed(Ped ped)
		{
			return (Relationship)Function.Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_PEDS, Handle, ped.Handle);
		}
		public void SetIntoVehicle(Vehicle vehicle, VehicleSeat seat)
		{
			Function.Call(Hash.SET_PED_INTO_VEHICLE, Handle, vehicle.Handle, (int)seat);
		}
		public bool IsInCover()
		{
			return IsInCover(false);
		}
		public bool IsInCover(bool expectUseWeapon)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_COVER, Handle, expectUseWeapon);
		}
		public bool IsInCombatAgainst(Ped target)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_COMBAT, Handle, target);
		}
		public bool IsHeadtracking(Entity entity)
		{
			return Function.Call<bool>(Hash.IS_PED_HEADTRACKING_ENTITY, Handle, entity);
		}

		public Ped GetJacker()
		{
			return Function.Call<Ped>(Hash.GET_PEDS_JACKER, Handle);
		}
		public Ped GetJackTarget()
		{
			return Function.Call<Ped>(Hash.GET_JACK_TARGET, Handle);
		}
		public Ped GetMeleeTarget()
		{
			return Function.Call<Ped>(Hash.GET_MELEE_TARGET_FOR_PED, Handle);
		}
		public Entity GetKiller()
		{
			return Function.Call<Entity>(Hash._GET_PED_KILLER, Handle);
		}
		public Vehicle GetVehicleIsTryingToEnter()
		{
			return Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_TRYING_TO_ENTER, Handle);
		}

		public void Kill()
		{
			Health = -1;
		}
		public void ResetVisibleDamage()
		{
			Function.Call(Hash.RESET_PED_VISIBLE_DAMAGE, Handle);
		}
		public void ClearBloodDamage()
		{
			Function.Call(Hash.CLEAR_PED_BLOOD_DAMAGE, Handle);
		}
		public void RandomizeOutfit()
		{
			Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, Handle, false);
		}
		public void SetDefaultClothes()
		{
			Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Handle);
		}
		public void LeaveGroup()
		{
			Function.Call(Hash.REMOVE_PED_FROM_GROUP, Handle);
		}
		public void Clone()
		{
			Clone(0.0F);
		}
		public void Clone(float heading)
		{
			Function.Call(Hash.CLONE_PED, Handle, heading, false, false);
		}
		public void ApplyDamage(int damageAmount)
		{
			Function.Call(Hash.APPLY_DAMAGE_TO_PED, Handle, damageAmount, true);
		}
		public Vector3 GetBoneCoord(Bone BoneID)
		{
			return GetBoneCoord(BoneID, Vector3.Zero);
		}
		public Vector3 GetBoneCoord(Bone BoneID, Vector3 Offset)
		{
			return Function.Call<Vector3>(Hash.GET_PED_BONE_COORDS, Handle, (int)BoneID, Offset.X, Offset.Y, Offset.Z);
		}
		public int GetBoneIndex(Bone BoneID)
		{
			return Function.Call<int>(Hash.GET_PED_BONE_INDEX, Handle, (int)BoneID);
		}

		public Vector3 GetLastWeaponImpactCoords()
		{
			var outCoords = new OutputArgument();
			if (Function.Call<bool>(Hash.GET_PED_LAST_WEAPON_IMPACT_COORD, Handle, outCoords))
				return outCoords.GetResult<Vector3>();
			return Vector3.Zero;
		}

		public bool GetConfigFlag(int flagID)
		{
			return Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, flagID, true);
		}
		public void SetConfigFlag(int flagID, bool value)
		{
			Function.Call(Hash.SET_PED_CONFIG_FLAG, Handle, flagID, value);
		}
		public void ResetConfigFlag(int flagID)
		{
			Function.Call(Hash.SET_PED_RESET_FLAG, Handle, flagID, true);
		}

		public void GiveHelmet(bool canBeRemovedByPed, HelmetType helmetType, int textureIndex)
		{
			Function.Call(Hash.GIVE_PED_HELMET, Handle, !canBeRemovedByPed, (int)helmetType, textureIndex);
		}
		public void RemoveHelmet(bool instantly)
		{
			Function.Call(Hash.REMOVE_PED_HELMET, Handle, instantly);
		}
	}
}
