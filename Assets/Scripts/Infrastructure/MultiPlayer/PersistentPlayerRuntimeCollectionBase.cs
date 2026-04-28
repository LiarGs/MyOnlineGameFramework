using UnityEngine;

namespace Infrastructure.MultiPlayer
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(fileName = "PersistentPlayerCollection",
        menuName = "RuntimeCollection/PersistentPlayerCollection")]
    public class PersistentPlayerRuntimeCollectionBase : RuntimeCollectionBase<PersistentPlayer>
    {
        public bool TryGetPlayer(ulong clientID, out PersistentPlayer persistentPlayer)
        {
            foreach (var item in Items)
            {
                if (clientID == item.OwnerClientId)
                {
                    persistentPlayer = item;
                    return true;
                }
            }

            persistentPlayer = null;
            return false;
        }
    }
}