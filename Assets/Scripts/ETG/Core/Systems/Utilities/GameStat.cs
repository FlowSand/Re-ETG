#nullable disable

public class GameStat
    {
        public string statName = string.Empty;
        public float statValue;

        public GameStat(string name, float val)
        {
            this.statName = name;
            this.statValue = val;
        }

        public void Modify(float change) => this.statValue += change;
    }

