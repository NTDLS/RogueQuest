using Library.Engine.Types;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Library.Types
{
    public class MetaEffect
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemEffect EffectType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemEffectType ValueType { get; set; }
        public int Value { get; set; }
        /// <summary>
        /// For magical items, this is how long an effect will last before it is removed.
        /// </summary>
        public int? Duration { get; set; }
    }
}
