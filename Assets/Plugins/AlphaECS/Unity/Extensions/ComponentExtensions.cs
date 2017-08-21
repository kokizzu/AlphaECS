using System.Reflection;
using SimpleJSON;
using UnityEngine;
using UniRx;

namespace AlphaECS
{ 
	public static class ComponentExtensions
	{
		public static JSONObject Serialize(this object component)
		{
			var node = new JSONObject();
			foreach (var property in component.GetType().GetProperties())
			{
				if (property.CanRead && property.CanWrite)
				{
                    if (property.PropertyType == typeof(int) || property.PropertyType.IsEnum)
					{
						node.Add(property.Name, new JSONNumber((int)property.GetValue(component, null)));
						continue;
					}
					if (property.PropertyType == typeof(IntReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as IntReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new IntReactiveProperty (); }					
						node.Add(property.Name, new JSONNumber((int)reactiveProperty.Value));
						continue;
					}
					if (property.PropertyType == typeof(float))
					{
						node.Add(property.Name, new JSONNumber((float)property.GetValue(component, null)));
						continue;
					}
					if (property.PropertyType == typeof(FloatReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as FloatReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new FloatReactiveProperty (); }					
						node.Add(property.Name, new JSONNumber((float)reactiveProperty.Value));
						continue;
					}
					if (property.PropertyType == typeof(bool))
					{
						node.Add(property.Name, new JSONBool((bool)property.GetValue(component, null)));
						continue;
					}
					if (property.PropertyType == typeof(BoolReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as BoolReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new BoolReactiveProperty (); }					
						node.Add(property.Name, new JSONBool((bool)reactiveProperty.Value));
						continue;
					}
					if (property.PropertyType == typeof(string))
					{
						node.Add(property.Name, new JSONString((string)property.GetValue(component, null)));
						continue;
					}
					if (property.PropertyType == typeof(StringReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as StringReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new StringReactiveProperty (); }					
						node.Add(property.Name, new JSONString((string)reactiveProperty.Value));
						continue;
					}
					if (property.PropertyType == typeof(Vector2))
					{
						var jsonObject = ((Vector2)property.GetValue (component, null)).AsJSONObject ();
						node.Add (property.Name, jsonObject);
						continue;
					}
					if (property.PropertyType == typeof(Vector2ReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as Vector2ReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new Vector2ReactiveProperty (); }

						var jsonObject = reactiveProperty.Value.AsJSONObject ();
						node.Add(property.Name, jsonObject);
						continue;
					}
					if (property.PropertyType == typeof(Vector3))
					{
						var jsonObject = ((Vector3)property.GetValue (component, null)).AsJSONObject ();
						node.Add (property.Name, jsonObject);
						continue;
					}
					if (property.PropertyType == typeof(Vector3ReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as Vector3ReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new Vector3ReactiveProperty (); }

						var jsonObject = reactiveProperty.Value.AsJSONObject ();
						node.Add(property.Name, jsonObject);
						continue;
					}
					if (property.PropertyType == typeof(Color))
					{
						var jsonObject = ((Color)property.GetValue(component, null)).AsJSONObject();
						node.Add(property.Name, jsonObject);
						continue;
					}
					if (property.PropertyType == typeof(ColorReactiveProperty))
					{
						var reactiveProperty = property.GetValue(component, null) as ColorReactiveProperty;
						if (reactiveProperty == null)
						{ reactiveProperty = new ColorReactiveProperty (); }

						var jsonObject = reactiveProperty.Value.AsJSONObject ();
						node.Add(property.Name, jsonObject);
						continue;
					}
				}
			}
			return node;
		}

		public static void Deserialize(this object component, JSONNode node)
		{
			foreach (var property in component.GetType().GetProperties())
			{
				ApplyValue(component, node, property);
			}
		}

		private static void ApplyValue(object component, JSONNode node, PropertyInfo property)
		{
			if (property.CanRead && property.CanWrite)
			{
				var propertyData = node[property.Name];
				if (propertyData == null) return;

				if (property.PropertyType == typeof(int) || property.PropertyType.IsEnum)
				{
					property.SetValue(component, propertyData.AsInt, null);
					return;
				}
				if (property.PropertyType == typeof(IntReactiveProperty))
				{
					var reactiveProperty = new IntReactiveProperty(propertyData.AsInt);
					property.SetValue(component, reactiveProperty, null);
					return;
				}
				if (property.PropertyType == typeof(float))
				{
					property.SetValue (component, propertyData.AsFloat, null);
					return;
				}
				if (property.PropertyType == typeof(FloatReactiveProperty))
				{
					var reactiveProperty = new FloatReactiveProperty(propertyData.AsFloat);
					property.SetValue(component, reactiveProperty, null);
					return;
				}
				if (property.PropertyType == typeof(bool))
				{
					property.SetValue(component, propertyData.AsBool, null);
					return;
				}
				if (property.PropertyType == typeof(BoolReactiveProperty))
				{
					var reactiveProperty = new BoolReactiveProperty(propertyData.AsBool);
					property.SetValue(component, reactiveProperty, null);
					return;
				}
				if (property.PropertyType == typeof(string))
				{
					property.SetValue(component, propertyData.Value, null);
					return;
				}
				if (property.PropertyType == typeof(StringReactiveProperty))
				{
					var reactiveProperty = new StringReactiveProperty(propertyData.Value);
					property.SetValue(component, reactiveProperty, null);
					return;
				}
				if (property.PropertyType == typeof(Vector2))
				{
					property.SetValue(component, propertyData.AsVector2(), null);
					return;
				}
				if (property.PropertyType == typeof(Vector2ReactiveProperty))
				{
					var reactiveProperty = new Vector2ReactiveProperty(propertyData.AsVector2());
					property.SetValue(component, reactiveProperty, null);
					return;
				}
				if (property.PropertyType == typeof(Vector3))
				{
					property.SetValue(component, propertyData.AsVector3(), null);
					return;
				}
				if (property.PropertyType == typeof(Vector3ReactiveProperty))
				{
					var reactiveProperty = new Vector3ReactiveProperty(propertyData.AsVector3());
					property.SetValue(component, reactiveProperty, null);
					return;
				}
			}
		}
	}
}
