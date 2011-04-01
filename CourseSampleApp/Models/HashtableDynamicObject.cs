using System;
using System.Collections;
using System.Dynamic;

namespace CourseSampleApp.Models
{
	public class HashtableDynamicObject : DynamicObject
	{
		private readonly IDictionary dictionary;

		public HashtableDynamicObject(IDictionary dictionary)
		{
			this.dictionary = dictionary;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = dictionary[binder.Name];
			return dictionary.Contains(binder.Name);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			dictionary[binder.Name] = value;
			return true;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if (indexes == null)
				throw new ArgumentNullException("indexes");
			if (indexes.Length != 1)
				throw new ArgumentException("Only support a single indexer parameter", "indexes");
			result = dictionary[indexes[0]];
			return dictionary.Contains(indexes[0]);
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{

			if (indexes == null)
				throw new ArgumentNullException("indexes");
			if (indexes.Length != 1)
				throw new ArgumentException("Only support a single indexer parameter", "indexes");
			dictionary[indexes[0]] = value;
			return true;
		}
	}
}