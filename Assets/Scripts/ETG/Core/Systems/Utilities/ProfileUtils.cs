using System.Runtime.InteropServices;

#nullable disable

    public static class ProfileUtils
    {
        public static int GetMonoCollectionCount() => ProfileUtils.mono_gc_collection_count(0);

        public static uint GetMonoHeapSize() => (uint) ProfileUtils.mono_gc_get_heap_size();

        public static uint GetMonoUsedHeapSize() => (uint) ProfileUtils.mono_gc_get_used_size();

[DllImport("mono")]
        private static extern int mono_gc_collection_count(int generation);

[DllImport("mono")]
        private static extern long mono_gc_get_heap_size();

[DllImport("mono")]
        private static extern long mono_gc_get_used_size();
    }

