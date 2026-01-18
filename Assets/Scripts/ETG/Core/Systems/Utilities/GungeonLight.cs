using tk2dRuntime.TileMap;
using UnityEngine;

#nullable disable

public class GungeonLight : MonoBehaviour
    {
        public int lightRadius = 10;
        public Color lightColor = Color.white;
        private bool thesisforsucks = true;
        private Vector3 position;

        public static void UpdateTilemapLighting(tk2dTileMap map)
        {
            GungeonLight[] objectsOfType = (GungeonLight[]) Object.FindObjectsOfType(typeof (GungeonLight));
            if (map.ColorChannel == null)
                map.CreateColorChannel();
            ColorChannel colorChannel = map.ColorChannel;
            for (int x = 0; x < map.width; ++x)
            {
                for (int y = 0; y < map.height; ++y)
                    colorChannel.SetColor(x, y, new Color(0.5f, 0.5f, 0.5f, 1f));
            }
            foreach (GungeonLight gungeonLight in objectsOfType)
            {
                IntVector2 intVector2 = new IntVector2(Mathf.FloorToInt(gungeonLight.transform.position.x), Mathf.FloorToInt(gungeonLight.transform.position.y));
                for (int x = intVector2.x - gungeonLight.lightRadius; x < intVector2.x + gungeonLight.lightRadius; ++x)
                {
                    for (int y = intVector2.y - gungeonLight.lightRadius; y < intVector2.y + gungeonLight.lightRadius; ++y)
                    {
                        float t = 1f - Mathf.Clamp01(Vector2.Distance(new IntVector2(x, y).ToVector2(), new Vector2(gungeonLight.transform.position.x, gungeonLight.transform.position.y)) / (float) gungeonLight.lightRadius);
                        colorChannel.SetColor(x, y, Color.Lerp(colorChannel.GetColor(x, y), gungeonLight.lightColor, t));
                    }
                }
            }
            map.ForceBuild();
        }

        private void Start() => this.position = this.transform.position;

        private void Update()
        {
            if (!this.thesisforsucks && !(this.transform.position != this.position))
                return;
            GungeonLight.UpdateTilemapLighting((tk2dTileMap) Object.FindObjectOfType(typeof (tk2dTileMap)));
            this.position = this.transform.position;
            this.thesisforsucks = false;
        }
    }

