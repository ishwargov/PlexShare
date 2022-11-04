
internal class ServerSide {

	public Dictionary<objectId, ShapeItem> objIdToObjectMap = new Dictionary<objectId, ShapeItem>>();


	private void AddObjectToServerList(String objectId, ShapeItem newShape)
	{
		objIdToObjectMap.Add(objectId, newShape);
		broadcast();
	}
	private void RemoveObjectFromServerList(String objectId, ShapeItem newShape)
	{
		objIdToObjectMap.Remove(objectId);
		broadcast();
	}
	private void UpdateObjectInServerList(String objectId, ShapeItem newShape)
	{
		if (objIdToObjectMap.ContainsKey(objectId))  
		{  
			objIdToObjectMap[objectId] = newShape;
			broadcast();
		}
	}
	private void broadcast()
	{
		for(item in objIdToObjectMap)
		{

		}
	}

}