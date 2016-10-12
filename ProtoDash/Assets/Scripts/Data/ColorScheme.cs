using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class ColorScheme : ScriptableObject
	{
		public Color MainBackGround;
		public Color MainForground;
		public Material MainForground_Material;

		public Color MainTitle;
		public Material MainTitle_Material;
		public Color DarkText;
		public Material DarkText_Material;
		public Material OptionIcon_Material;
		public Color LightText;
		public Material LightText_Material;

		public Color Button;
		public Material Button_Material;
		public Color LightButton;
		public Material LightButtond_Material;

		public Color World_UI_Background;
		public Material World_UI_BG_Material;
		public Color HardWorld_UI_Backgroud;
		public Material HarldWorld_UI_BG_Material;
		public Color WorldNormalBorder;
		public Material WorldBorder_Material;
		public Color WorldFinishedBorder;
		public Material WorldFinishedBorder_Material;

		public Color WorldGoldenColor1;
		public Color WorldGoldenColor2;
		public Material WorldGoldenBorder_Material;

		public Color MainCharacterBase;
		public Color MainCharacterDepleted;
		public Material Character_Material;

		public Color Gauge;
		public Material Gauge_Material;
		public Color GaugeDepletion;
		public Material GaugeDepletion_Material;

		public Color EndNode;
		public Material EndNode_Material;

		public Color Lava1;
		public Color Lava2;
		public Material Lava_Material;

		public Color BackgroundComposition;
		public Material Background_Material;

		public Color Transition1;
		public Material Transition_Material;
		public Color Transition2;
		public Material Transition2_Material;

		public void SetColors()
		{
			MainForground_Material.color = MainForground;

			MainTitle_Material.color = MainTitle;
			DarkText_Material.color = DarkText;
			OptionIcon_Material.color = DarkText;
			LightText_Material.color = LightText;

			Button_Material.color = Button;
			LightButtond_Material.color = LightButton;

			World_UI_BG_Material.color = World_UI_Background;
			HarldWorld_UI_BG_Material.color = HardWorld_UI_Backgroud;
			WorldBorder_Material.color = WorldNormalBorder;
			WorldFinishedBorder_Material.color = WorldFinishedBorder;

			//WorldGoldenColor1;
			//WorldGoldenColor2;
			//WorldGoldenBorder_Material;
			Debug.LogWarning("golden border color not set");


			//MainCharacterBase;
			//MainCharacterDepleted;
			//Character_Material;
			Debug.LogWarning("character color not set");

			Gauge_Material.color = Gauge;
			GaugeDepletion_Material.color = GaugeDepletion;

			EndNode_Material.color = EndNode;

			//Lava1;
			//Lava2;
			//Lava_Material;
			Debug.Log("laval color not set");

			Background_Material.color = BackgroundComposition;

			Transition_Material.color = Transition1;
			Transition2_Material.color = Transition2;
		}
	}
}
