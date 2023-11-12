namespace SHOU.Extentions
{
    public static class ObjectExtentions
    {
        public static string GenerateGuid() => Guid.NewGuid().ToString("N");
    }
}
