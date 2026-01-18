using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
    public class SerializedAction<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9> : 
        BaseSerializedAction
    {
        public void Invoke(
            TParam1 param1,
            TParam2 param2,
            TParam3 param3,
            TParam4 param4,
            TParam5 param5,
            TParam6 param6,
            TParam7 param7,
            TParam8 param8,
            TParam9 param9)
        {
            this.DoInvoke((object) param1, (object) param2, (object) param3, (object) param4, (object) param5, (object) param6, (object) param7, (object) param8, (object) param9);
        }
    }
}
