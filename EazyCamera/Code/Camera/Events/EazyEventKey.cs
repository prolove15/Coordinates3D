
namespace EazyCamera.Events
{
    [System.Serializable]
    public struct EazyEventKey : System.IEquatable<EazyEventKey>
    {
        public string Key { get; }

        public EazyEventKey(string key)
        {
            Key = key;
        }

        public static implicit operator string(EazyEventKey key) => key.Key;
        public override int GetHashCode() => Key.GetHashCode();
        public bool Equals(EazyEventKey other) => Key == other.Key;
        public override bool Equals(object obj) => obj is EazyEventKey key && this.Equals(key);
        public static bool operator ==(EazyEventKey lhs, EazyEventKey rhs) => lhs.Equals(rhs);
        public static bool operator !=(EazyEventKey lhs, EazyEventKey rhs) => !(lhs.Equals(rhs));

        public static readonly EazyEventKey OnEnterFocasableRange = new EazyEventKey("OnEnterFocasableRange");
        public static readonly EazyEventKey OnExitFocasableRange = new EazyEventKey("OnExitFocasableRange");
        public static readonly EazyEventKey OnMangerEnabled = new EazyEventKey("OnManagerEnabled");
        public static readonly EazyEventKey OnManagerDisabled = new EazyEventKey("OnManagerDisabled");
    }
}
