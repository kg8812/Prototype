namespace Default
{
    /// <summary>
    ///     주소와 소속 스코프를 한 번에 선언해두는 값. Load 시점에 Scene/Global을 라이브로 뒤지지 않고,
    ///     이 선언을 그대로 신뢰해 해당 스코프로 직행한다.
    /// </summary>
    public readonly struct AssetAddress
    {
        public string Path { get; }
        public AssetLifetime Lifetime { get; }

        public AssetAddress(string path, AssetLifetime lifetime)
        {
            Path = path;
            Lifetime = lifetime;
        }

        public override string ToString() => Path;
    }
}
