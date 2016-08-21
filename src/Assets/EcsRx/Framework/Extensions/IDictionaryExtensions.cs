using System.Collections.Generic;
using EcsRx;

namespace EcsRx.Extensions
{
    public static class IDictionaryExtensions
    {
        public static void AddEntityToGroups(this IDictionary<IGroup, IList<IEntity>> groupAccessors, IEntity entity)
        {
            foreach (var group in groupAccessors.Keys)
            {
                if(entity.MatchesGroup(group))
                { groupAccessors[group].Add(entity); }
            }
        }
    }
}