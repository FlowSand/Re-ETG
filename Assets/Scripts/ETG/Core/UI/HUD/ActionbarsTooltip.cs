using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Actionbar/Tooltip")]
public class ActionbarsTooltip : MonoBehaviour
    {
        private static ActionbarsTooltip _instance;
        private static dfPanel _panel;
        private static dfLabel _name;
        private static dfLabel _info;
        private static Vector2 _cursorOffset;

        public void Start()
        {
            ActionbarsTooltip._instance = this;
            ActionbarsTooltip._panel = this.GetComponent<dfPanel>();
            ActionbarsTooltip._name = ActionbarsTooltip._panel.Find<dfLabel>("lblName");
            ActionbarsTooltip._info = ActionbarsTooltip._panel.Find<dfLabel>("lblInfo");
            ActionbarsTooltip._panel.Hide();
            ActionbarsTooltip._panel.IsInteractive = false;
            ActionbarsTooltip._panel.IsEnabled = false;
        }

        public void Update()
        {
            if (!ActionbarsTooltip._panel.IsVisible)
                return;
            ActionbarsTooltip.setPosition((Vector2) Input.mousePosition);
        }

        public static void Show(SpellDefinition spell)
        {
            if (spell == null)
            {
                ActionbarsTooltip.Hide();
            }
            else
            {
                ActionbarsTooltip._name.Text = spell.Name;
                ActionbarsTooltip._info.Text = spell.Description;
                float num = ActionbarsTooltip._info.RelativePosition.y + ActionbarsTooltip._info.Size.y;
                ActionbarsTooltip._panel.Height = num;
                ActionbarsTooltip._cursorOffset = new Vector2(0.0f, num + 10f);
                ActionbarsTooltip._panel.Show();
                ActionbarsTooltip._panel.BringToFront();
                ActionbarsTooltip._instance.Update();
            }
        }

        public static void Hide()
        {
            ActionbarsTooltip._panel.Hide();
            ActionbarsTooltip._panel.SendToBack();
        }

        private static void setPosition(Vector2 position)
        {
            position = ActionbarsTooltip._panel.GetManager().ScreenToGui(position);
            ActionbarsTooltip._panel.RelativePosition = (Vector3) (position - ActionbarsTooltip._cursorOffset);
        }
    }

