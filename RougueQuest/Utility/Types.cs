using System;

namespace RougueQuest.Engine
{
    public static class Types
    {
        public static dynamic DynamicCast(dynamic source, Type dest) => Convert.ChangeType(source, dest);
    }
}
