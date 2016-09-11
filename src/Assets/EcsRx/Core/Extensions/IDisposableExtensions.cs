using System;
using System.Collections.Generic;

namespace EcsRx.Extensions
{
    public static class IDisposableExtensions
    {
        public static void DisposeAll(this IEnumerable<IDisposable> disposables)
        {	
//			disposables.ForEachRun(x => x.Dispose());
		}
    }
}