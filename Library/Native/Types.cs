using System;

namespace Library.Native
{
    public static class Types
    {
        public static dynamic DynamicCast(dynamic source, Type dest) => Convert.ChangeType(source, dest);
    }
}
