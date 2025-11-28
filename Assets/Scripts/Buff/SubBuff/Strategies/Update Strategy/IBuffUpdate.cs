using System.Collections.Generic;

namespace Apis
{
    public interface IBuffUpdate : IObserver<List<SubBuff>>
    {
        public void Update();
    }
}