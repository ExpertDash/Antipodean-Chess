using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Utils {
	///<param name="lists">The lists to be merged</param>
	///<returns>Returns the union of given lists</returns>
	///<remarks>Used to prevent duplicate positions for GetMoveableLocations and GetAttackableLocations in Rules</remarks>
	public static List<T> Merge<T>(params List<T>[] lists) {
		IEnumerable<T> result = new List<T>();
		foreach(List<T> list in lists) result = result.Union(list);

		return result.ToList();
	}
}