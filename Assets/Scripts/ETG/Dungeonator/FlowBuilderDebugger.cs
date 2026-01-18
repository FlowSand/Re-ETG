using System.IO;
using System.Text;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    public class FlowBuilderDebugger
    {
        protected StringBuilder builder;

        public FlowBuilderDebugger() => this.builder = new StringBuilder();

        public void Log(string s) => this.builder.AppendLine(s);

        public void Log(RoomHandler parent, RoomHandler child)
        {
            this.builder.AppendLine($"{parent.area.prototypeRoom.name} built {child.area.prototypeRoom.name}");
        }

        public void LogMonoHeapStatus()
        {
        }

        public void FinalizeLog()
        {
            string str = Application.dataPath + "\\dungeonDebug.txt";
            if (File.Exists(str))
            {
                new FileInfo(str).IsReadOnly = false;
                File.Delete(str);
            }
            StreamWriter streamWriter = new StreamWriter(str);
            streamWriter.WriteLine(this.builder.ToString());
            streamWriter.Close();
        }
    }
}
