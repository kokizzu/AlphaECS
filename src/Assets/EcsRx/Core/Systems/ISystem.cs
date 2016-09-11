using System.Collections;

namespace EcsRx
{
	public interface ISystem
	{
		IEventSystem EventSystem { get; set; }
		IPoolManager PoolManager { get; set; }
		void Setup();
		IEnumerator SetupAsync();
	}
}