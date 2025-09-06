using System;

public static class WorldNavigation
{
	[Serializable]
	public class SceneItem
	{
		public string Path;

		public string Name;

		public TransitionItem[] Transitions;
	}

	[Serializable]
	public class TransitionItem
	{
		public string Name;
	}

	public static readonly SceneItem[] Scenes = new SceneItem[376]
	{
		new SceneItem
		{
			Path = "Assets/Scenes/Tutorial_01.unity",
			Name = "Tutorial_01",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Town.unity",
			Name = "Town",
			Transitions = new TransitionItem[12]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_bretta"
				},
				new TransitionItem
				{
					Name = "door_jiji"
				},
				new TransitionItem
				{
					Name = "door_sly"
				},
				new TransitionItem
				{
					Name = "door_station"
				},
				new TransitionItem
				{
					Name = "door_mapper"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "room_divine"
				},
				new TransitionItem
				{
					Name = "room_grimm"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Town_Stag_Station.unity",
			Name = "Room_Town_Stag_Station",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Charm_Shop.unity",
			Name = "Room_Charm_Shop",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Mender_House.unity",
			Name = "Room_Mender_House",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_mapper.unity",
			Name = "Room_mapper",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_nailmaster.unity",
			Name = "Room_nailmaster",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_nailmaster_02.unity",
			Name = "Room_nailmaster_02",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_nailmaster_03.unity",
			Name = "Room_nailmaster_03",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_nailsmith.unity",
			Name = "Room_nailsmith",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_shop.unity",
			Name = "Room_shop",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Sly_Storeroom.unity",
			Name = "Room_Sly_Storeroom",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_temple.unity",
			Name = "Room_temple",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_ruinhouse.unity",
			Name = "Room_ruinhouse",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Mask_Maker.unity",
			Name = "Room_Mask_Maker",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Mansion.unity",
			Name = "Room_Mansion",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Tram.unity",
			Name = "Room_Tram",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Tram_RG.unity",
			Name = "Room_Tram_RG",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Bretta.unity",
			Name = "Room_Bretta",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Bretta_Basement.unity",
			Name = "Room_Bretta_Basement",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Fungus_Shaman.unity",
			Name = "Room_Fungus_Shaman",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Ouiji.unity",
			Name = "Room_Ouiji",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Jinn.unity",
			Name = "Room_Jinn",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Colosseum_01.unity",
			Name = "Room_Colosseum_01",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Colosseum_02.unity",
			Name = "Room_Colosseum_02",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "top2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Colosseum_Bronze.unity",
			Name = "Room_Colosseum_Bronze",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1 (1)"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Colosseum_Silver.unity",
			Name = "Room_Colosseum_Silver",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1 (1)"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Colosseum_Gold.unity",
			Name = "Room_Colosseum_Gold",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1 (1)"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Colosseum_Spectate.unity",
			Name = "Room_Colosseum_Spectate",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Slug_Shrine.unity",
			Name = "Room_Slug_Shrine",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_01.unity",
			Name = "Crossroads_01",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_02.unity",
			Name = "Crossroads_02",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_03.unity",
			Name = "Crossroads_03",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_04.unity",
			Name = "Crossroads_04",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_charmshop"
				},
				new TransitionItem
				{
					Name = "door_Mender_House"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_05.unity",
			Name = "Crossroads_05",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_06.unity",
			Name = "Crossroads_06",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_07.unity",
			Name = "Crossroads_07",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_08.unity",
			Name = "Crossroads_08",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_09.unity",
			Name = "Crossroads_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_10.unity",
			Name = "Crossroads_10",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_11_alt.unity",
			Name = "Crossroads_11_alt",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_12.unity",
			Name = "Crossroads_12",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_13.unity",
			Name = "Crossroads_13",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_14.unity",
			Name = "Crossroads_14",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_15.unity",
			Name = "Crossroads_15",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_16.unity",
			Name = "Crossroads_16",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_18.unity",
			Name = "Crossroads_18",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_19.unity",
			Name = "Crossroads_19",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_21.unity",
			Name = "Crossroads_21",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_22.unity",
			Name = "Crossroads_22",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_25.unity",
			Name = "Crossroads_25",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_27.unity",
			Name = "Crossroads_27",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_30.unity",
			Name = "Crossroads_30",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_31.unity",
			Name = "Crossroads_31",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_33.unity",
			Name = "Crossroads_33",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_35.unity",
			Name = "Crossroads_35",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_36.unity",
			Name = "Crossroads_36",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_37.unity",
			Name = "Crossroads_37",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_38.unity",
			Name = "Crossroads_38",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_39.unity",
			Name = "Crossroads_39",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_40.unity",
			Name = "Crossroads_40",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_42.unity",
			Name = "Crossroads_42",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_43.unity",
			Name = "Crossroads_43",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_45.unity",
			Name = "Crossroads_45",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_46.unity",
			Name = "Crossroads_46",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_tram"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_46b.unity",
			Name = "Crossroads_46b",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_tram"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_ShamanTemple.unity",
			Name = "Crossroads_ShamanTemple",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_47.unity",
			Name = "Crossroads_47",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_48.unity",
			Name = "Crossroads_48",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_49.unity",
			Name = "Crossroads_49",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "elev_entrance"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_49b.unity",
			Name = "Crossroads_49b",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "elev_entrance"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_50.unity",
			Name = "Crossroads_50",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Crossroads/Crossroads_52.unity",
			Name = "Crossroads_52",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins_House_01.unity",
			Name = "Ruins_House_01",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins_House_02.unity",
			Name = "Ruins_House_02",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins_House_03.unity",
			Name = "Ruins_House_03",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins_Elevator.unity",
			Name = "Ruins_Elevator",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins_Bathhouse.unity",
			Name = "Ruins_Bathhouse",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_01.unity",
			Name = "Ruins1_01",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_02.unity",
			Name = "Ruins1_02",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_03.unity",
			Name = "Ruins1_03",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_04.unity",
			Name = "Ruins1_04",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_05.unity",
			Name = "Ruins1_05",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_05b.unity",
			Name = "Ruins1_05b",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_06.unity",
			Name = "Ruins1_06",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_09.unity",
			Name = "Ruins1_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_17.unity",
			Name = "Ruins1_17",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_18.unity",
			Name = "Ruins1_18",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_23.unity",
			Name = "Ruins1_23",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_24.unity",
			Name = "Ruins1_24",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_25.unity",
			Name = "Ruins1_25",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_27.unity",
			Name = "Ruins1_27",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_28.unity",
			Name = "Ruins1_28",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_29.unity",
			Name = "Ruins1_29",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_30.unity",
			Name = "Ruins1_30",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_31.unity",
			Name = "Ruins1_31",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins1_32.unity",
			Name = "Ruins1_32",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_01.unity",
			Name = "Ruins2_01",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_03.unity",
			Name = "Ruins2_03",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_04.unity",
			Name = "Ruins2_04",
			Transitions = new TransitionItem[8]
			{
				new TransitionItem
				{
					Name = "door_Ruin_House_02"
				},
				new TransitionItem
				{
					Name = "door_Ruin_House_01"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_Ruin_Elevator"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "door_Ruin_House_03"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_05.unity",
			Name = "Ruins2_05",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_06.unity",
			Name = "Ruins2_06",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_07.unity",
			Name = "Ruins2_07",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_08.unity",
			Name = "Ruins2_08",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_09.unity",
			Name = "Ruins2_09",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_10.unity",
			Name = "Ruins2_10",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "elev_entrance"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_10b.unity",
			Name = "Ruins2_10b",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "elev_entrance"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_11.unity",
			Name = "Ruins2_11",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/City/Ruins2_Watcher_Room.unity",
			Name = "Ruins2_Watcher_Room",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_01.unity",
			Name = "Fungus1_01",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_01b.unity",
			Name = "Fungus1_01b",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_02.unity",
			Name = "Fungus1_02",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_03.unity",
			Name = "Fungus1_03",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_04.unity",
			Name = "Fungus1_04",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_05.unity",
			Name = "Fungus1_05",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_06.unity",
			Name = "Fungus1_06",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_07.unity",
			Name = "Fungus1_07",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_08.unity",
			Name = "Fungus1_08",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_09.unity",
			Name = "Fungus1_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_10.unity",
			Name = "Fungus1_10",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_11.unity",
			Name = "Fungus1_11",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_12.unity",
			Name = "Fungus1_12",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_13.unity",
			Name = "Fungus1_13",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_14.unity",
			Name = "Fungus1_14",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_15.unity",
			Name = "Fungus1_15",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_16_alt.unity",
			Name = "Fungus1_16_alt",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_17.unity",
			Name = "Fungus1_17",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_19.unity",
			Name = "Fungus1_19",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_20_v02.unity",
			Name = "Fungus1_20_v02",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_21.unity",
			Name = "Fungus1_21",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_22.unity",
			Name = "Fungus1_22",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_23.unity",
			Name = "Fungus1_23",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_24.unity",
			Name = "Fungus1_24",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_25.unity",
			Name = "Fungus1_25",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_26.unity",
			Name = "Fungus1_26",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_SlugShrine"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_28.unity",
			Name = "Fungus1_28",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_29.unity",
			Name = "Fungus1_29",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_30.unity",
			Name = "Fungus1_30",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top3"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_31.unity",
			Name = "Fungus1_31",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_32.unity",
			Name = "Fungus1_32",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_34.unity",
			Name = "Fungus1_34",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_35.unity",
			Name = "Fungus1_35",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_36.unity",
			Name = "Fungus1_36",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_37.unity",
			Name = "Fungus1_37",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus1_Slug.unity",
			Name = "Fungus1_Slug",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_01.unity",
			Name = "Fungus2_01",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_02.unity",
			Name = "Fungus2_02",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_03.unity",
			Name = "Fungus2_03",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_04.unity",
			Name = "Fungus2_04",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_05.unity",
			Name = "Fungus2_05",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_06.unity",
			Name = "Fungus2_06",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_07.unity",
			Name = "Fungus2_07",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_08.unity",
			Name = "Fungus2_08",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_09.unity",
			Name = "Fungus2_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_10.unity",
			Name = "Fungus2_10",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_11.unity",
			Name = "Fungus2_11",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_12.unity",
			Name = "Fungus2_12",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_13.unity",
			Name = "Fungus2_13",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_14.unity",
			Name = "Fungus2_14",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_15.unity",
			Name = "Fungus2_15",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "top3"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_17.unity",
			Name = "Fungus2_17",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_18.unity",
			Name = "Fungus2_18",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_19.unity",
			Name = "Fungus2_19",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_20.unity",
			Name = "Fungus2_20",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_21.unity",
			Name = "Fungus2_21",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_23.unity",
			Name = "Fungus2_23",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_25.unity",
			Name = "Fungus2_25",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right1 (1)"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_26.unity",
			Name = "Fungus2_26",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_28.unity",
			Name = "Fungus2_28",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_29.unity",
			Name = "Fungus2_29",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_30.unity",
			Name = "Fungus2_30",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_31.unity",
			Name = "Fungus2_31",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_32.unity",
			Name = "Fungus2_32",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_33.unity",
			Name = "Fungus2_33",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus2_34.unity",
			Name = "Fungus2_34",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_01.unity",
			Name = "Fungus3_01",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_02.unity",
			Name = "Fungus3_02",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_03.unity",
			Name = "Fungus3_03",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_04.unity",
			Name = "Fungus3_04",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_05.unity",
			Name = "Fungus3_05",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_08.unity",
			Name = "Fungus3_08",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_10.unity",
			Name = "Fungus3_10",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_11.unity",
			Name = "Fungus3_11",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_13.unity",
			Name = "Fungus3_13",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_21.unity",
			Name = "Fungus3_21",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_22.unity",
			Name = "Fungus3_22",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_23.unity",
			Name = "Fungus3_23",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_24.unity",
			Name = "Fungus3_24",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_25.unity",
			Name = "Fungus3_25",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_25b.unity",
			Name = "Fungus3_25b",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_26.unity",
			Name = "Fungus3_26",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_27.unity",
			Name = "Fungus3_27",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_28.unity",
			Name = "Fungus3_28",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_30.unity",
			Name = "Fungus3_30",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_34.unity",
			Name = "Fungus3_34",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_35.unity",
			Name = "Fungus3_35",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_39.unity",
			Name = "Fungus3_39",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_40.unity",
			Name = "Fungus3_40",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_44.unity",
			Name = "Fungus3_44",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_47.unity",
			Name = "Fungus3_47",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_48.unity",
			Name = "Fungus3_48",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_49.unity",
			Name = "Fungus3_49",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_50.unity",
			Name = "Fungus3_50",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_archive.unity",
			Name = "Fungus3_archive",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Fungus/Fungus3_archive_02.unity",
			Name = "Fungus3_archive_02",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Cliffs/Cliffs_01.unity",
			Name = "Cliffs_01",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right4"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Cliffs/Cliffs_02.unity",
			Name = "Cliffs_02",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Cliffs/Cliffs_03.unity",
			Name = "Cliffs_03",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Cliffs/Cliffs_04.unity",
			Name = "Cliffs_04",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Cliffs/Cliffs_05.unity",
			Name = "Cliffs_05",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Cliffs/Cliffs_06.unity",
			Name = "Cliffs_06",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_02.unity",
			Name = "RestingGrounds_02",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_04.unity",
			Name = "RestingGrounds_04",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_05.unity",
			Name = "RestingGrounds_05",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_06.unity",
			Name = "RestingGrounds_06",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_07.unity",
			Name = "RestingGrounds_07",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_08.unity",
			Name = "RestingGrounds_08",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_09.unity",
			Name = "RestingGrounds_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_10.unity",
			Name = "RestingGrounds_10",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "top2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_12.unity",
			Name = "RestingGrounds_12",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_Mansion"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Resting Grounds/RestingGrounds_17.unity",
			Name = "RestingGrounds_17",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_01.unity",
			Name = "Mines_01",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_02.unity",
			Name = "Mines_02",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_03.unity",
			Name = "Mines_03",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_04.unity",
			Name = "Mines_04",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_05.unity",
			Name = "Mines_05",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_06.unity",
			Name = "Mines_06",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_07.unity",
			Name = "Mines_07",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_10.unity",
			Name = "Mines_10",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_11.unity",
			Name = "Mines_11",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_13.unity",
			Name = "Mines_13",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_16.unity",
			Name = "Mines_16",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_17.unity",
			Name = "Mines_17",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_18.unity",
			Name = "Mines_18",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_19.unity",
			Name = "Mines_19",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_20.unity",
			Name = "Mines_20",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_23.unity",
			Name = "Mines_23",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_24.unity",
			Name = "Mines_24",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_25.unity",
			Name = "Mines_25",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_28.unity",
			Name = "Mines_28",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_29.unity",
			Name = "Mines_29",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_30.unity",
			Name = "Mines_30",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_31.unity",
			Name = "Mines_31",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_32.unity",
			Name = "Mines_32",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_33.unity",
			Name = "Mines_33",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_34.unity",
			Name = "Mines_34",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_35.unity",
			Name = "Mines_35",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_36.unity",
			Name = "Mines_36",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Mines/Mines_37.unity",
			Name = "Mines_37",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_01.unity",
			Name = "Deepnest_01",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_01b.unity",
			Name = "Deepnest_01b",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_02.unity",
			Name = "Deepnest_02",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_03.unity",
			Name = "Deepnest_03",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_09.unity",
			Name = "Deepnest_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_10.unity",
			Name = "Deepnest_10",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "door2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right3"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_14.unity",
			Name = "Deepnest_14",
			Transitions = new TransitionItem[7]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "left1 (2)"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1 (1)"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1 (3)"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_16.unity",
			Name = "Deepnest_16",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_17.unity",
			Name = "Deepnest_17",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_26.unity",
			Name = "Deepnest_26",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right1 (1)"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right1 (2)"
				},
				new TransitionItem
				{
					Name = "right1 (3)"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_30.unity",
			Name = "Deepnest_30",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_31.unity",
			Name = "Deepnest_31",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_32.unity",
			Name = "Deepnest_32",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_33.unity",
			Name = "Deepnest_33",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_34.unity",
			Name = "Deepnest_34",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_35.unity",
			Name = "Deepnest_35",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_36.unity",
			Name = "Deepnest_36",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_37.unity",
			Name = "Deepnest_37",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_38.unity",
			Name = "Deepnest_38",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_39.unity",
			Name = "Deepnest_39",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_40.unity",
			Name = "Deepnest_40",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_41.unity",
			Name = "Deepnest_41",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_42.unity",
			Name = "Deepnest_42",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_43.unity",
			Name = "Deepnest_43",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_44.unity",
			Name = "Deepnest_44",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_45_v02.unity",
			Name = "Deepnest_45_v02",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_Spider_Town.unity",
			Name = "Deepnest_Spider_Town",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_spider_small.unity",
			Name = "Room_spider_small",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_01.unity",
			Name = "Deepnest_East_01",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_02.unity",
			Name = "Deepnest_East_02",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_03.unity",
			Name = "Deepnest_East_03",
			Transitions = new TransitionItem[6]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_04.unity",
			Name = "Deepnest_East_04",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_06.unity",
			Name = "Deepnest_East_06",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_07.unity",
			Name = "Deepnest_East_07",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_08.unity",
			Name = "Deepnest_East_08",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_09.unity",
			Name = "Deepnest_East_09",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_10.unity",
			Name = "Deepnest_East_10",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_11.unity",
			Name = "Deepnest_East_11",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_12.unity",
			Name = "Deepnest_East_12",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "door_cutsceneReturn"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_13.unity",
			Name = "Deepnest_East_13",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_14.unity",
			Name = "Deepnest_East_14",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "top2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_15.unity",
			Name = "Deepnest_East_15",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_16.unity",
			Name = "Deepnest_East_16",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_17.unity",
			Name = "Deepnest_East_17",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_18.unity",
			Name = "Deepnest_East_18",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Deepnest/Deepnest_East_Hornet.unity",
			Name = "Deepnest_East_Hornet",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Wyrm.unity",
			Name = "Room_Wyrm",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_01.unity",
			Name = "Abyss_01",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_02.unity",
			Name = "Abyss_02",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_03.unity",
			Name = "Abyss_03",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "door_tram"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "door_tram_arrive"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_03_b.unity",
			Name = "Abyss_03_b",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_tram"
				},
				new TransitionItem
				{
					Name = "door_tram_arrive"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_03_c.unity",
			Name = "Abyss_03_c",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "door_tram"
				},
				new TransitionItem
				{
					Name = "door_tram_arrive"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_04.unity",
			Name = "Abyss_04",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_05.unity",
			Name = "Abyss_05",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_06_Core.unity",
			Name = "Abyss_06_Core",
			Transitions = new TransitionItem[7]
			{
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left1 extra"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_08.unity",
			Name = "Abyss_08",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_09.unity",
			Name = "Abyss_09",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right3"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_10.unity",
			Name = "Abyss_10",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_12.unity",
			Name = "Abyss_12",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_15.unity",
			Name = "Abyss_15",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_16.unity",
			Name = "Abyss_16",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_17.unity",
			Name = "Abyss_17",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_18.unity",
			Name = "Abyss_18",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_19.unity",
			Name = "Abyss_19",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "bot2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_20.unity",
			Name = "Abyss_20",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_21.unity",
			Name = "Abyss_21",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_22.unity",
			Name = "Abyss_22",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_fastTravelExit"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Abyss/Abyss_Lighthouse_room.unity",
			Name = "Abyss_Lighthouse_room",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Queen.unity",
			Name = "Room_Queen",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_01.unity",
			Name = "Waterways_01",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_02.unity",
			Name = "Waterways_02",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "top2"
				},
				new TransitionItem
				{
					Name = "top3"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_03.unity",
			Name = "Waterways_03",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_04.unity",
			Name = "Waterways_04",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_04b.unity",
			Name = "Waterways_04b",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_05.unity",
			Name = "Waterways_05",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_06.unity",
			Name = "Waterways_06",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_07.unity",
			Name = "Waterways_07",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_08.unity",
			Name = "Waterways_08",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_09.unity",
			Name = "Waterways_09",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_12.unity",
			Name = "Waterways_12",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_13.unity",
			Name = "Waterways_13",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_14.unity",
			Name = "Waterways_14",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot2"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Waterways/Waterways_15.unity",
			Name = "Waterways_15",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_01.unity",
			Name = "White_Palace_01",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_02.unity",
			Name = "White_Palace_02",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_03_hub.unity",
			Name = "White_Palace_03_hub",
			Transitions = new TransitionItem[7]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "doorWarp"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_04.unity",
			Name = "White_Palace_04",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_05.unity",
			Name = "White_Palace_05",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_06.unity",
			Name = "White_Palace_06",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_07.unity",
			Name = "White_Palace_07",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_08.unity",
			Name = "White_Palace_08",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_09.unity",
			Name = "White_Palace_09",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_11.unity",
			Name = "White_Palace_11",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "doorWarp"
				},
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "door2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_12.unity",
			Name = "White_Palace_12",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_13.unity",
			Name = "White_Palace_13",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_14.unity",
			Name = "White_Palace_14",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_15.unity",
			Name = "White_Palace_15",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_16.unity",
			Name = "White_Palace_16",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left2"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_17.unity",
			Name = "White_Palace_17",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "bot1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_18.unity",
			Name = "White_Palace_18",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "top1"
				},
				new TransitionItem
				{
					Name = "right1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_19.unity",
			Name = "White_Palace_19",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/White Palace/White_Palace_20.unity",
			Name = "White_Palace_20",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Hive/Hive_01.unity",
			Name = "Hive_01",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Hive/Hive_02.unity",
			Name = "Hive_02",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "left3"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Hive/Hive_03.unity",
			Name = "Hive_03",
			Transitions = new TransitionItem[5]
			{
				new TransitionItem
				{
					Name = "right3"
				},
				new TransitionItem
				{
					Name = "right2"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Hive/Hive_04.unity",
			Name = "Hive_04",
			Transitions = new TransitionItem[3]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Hive/Hive_05.unity",
			Name = "Hive_05",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Grimm/Grimm_Divine.unity",
			Name = "Grimm_Divine",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Grimm/Grimm_Main_Tent.unity",
			Name = "Grimm_Main_Tent",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Grimm/Grimm_Nightmare.unity",
			Name = "Grimm_Nightmare",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Nailcollection.unity",
			Name = "Dream_Nailcollection",
			Transitions = new TransitionItem[4]
			{
				new TransitionItem
				{
					Name = "door_dreamReturn3"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn2"
				},
				new TransitionItem
				{
					Name = "door_dreamReturn4"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_01_False_Knight.unity",
			Name = "Dream_01_False_Knight",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_02_Mage_Lord.unity",
			Name = "Dream_02_Mage_Lord",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_03_Infected_Knight.unity",
			Name = "Dream_03_Infected_Knight",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_04_White_Defender.unity",
			Name = "Dream_04_White_Defender",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Mighty_Zote.unity",
			Name = "Dream_Mighty_Zote",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Guardian_Hegemol.unity",
			Name = "Dream_Guardian_Hegemol",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Guardian_Lurien.unity",
			Name = "Dream_Guardian_Lurien",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Guardian_Monomon.unity",
			Name = "Dream_Guardian_Monomon",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Backer_Shrine.unity",
			Name = "Dream_Backer_Shrine",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "door1"
				},
				new TransitionItem
				{
					Name = "door2"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Room_Believer_Shrine.unity",
			Name = "Dream_Room_Believer_Shrine",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Abyss.unity",
			Name = "Dream_Abyss",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Dream/Dream_Final_Boss.unity",
			Name = "Dream_Final_Boss",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "door1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Final_Boss_Atrium.unity",
			Name = "Room_Final_Boss_Atrium",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Room_Final_Boss_Core.unity",
			Name = "Room_Final_Boss_Core",
			Transitions = new TransitionItem[1]
			{
				new TransitionItem
				{
					Name = "left1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Testing/_test_charms.unity",
			Name = "_test_charms",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "right1"
				},
				new TransitionItem
				{
					Name = "bot1"
				}
			}
		},
		new SceneItem
		{
			Path = "Assets/Scenes/Testing/_test_dream.unity",
			Name = "_test_dream",
			Transitions = new TransitionItem[2]
			{
				new TransitionItem
				{
					Name = "left1"
				},
				new TransitionItem
				{
					Name = "top1"
				}
			}
		}
	};
}
