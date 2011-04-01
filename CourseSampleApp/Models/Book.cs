using System.Collections;
using System.Collections.Generic;

namespace CourseSampleApp.Models
{
	public class Book
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Library Library { get; set; }
		public virtual ICollection<BookLoan> Loans { get; set; }

		private IDictionary attributes = new Hashtable();

		public virtual dynamic Attributes
		{
			get { return new HashtableDynamicObject(attributes); }
		}
	}
}